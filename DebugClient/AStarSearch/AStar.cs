using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriangleNet;
using TriangleNet.Geometry;

namespace DebugClient.AStarSearch
{

    class AStarPath
    {
        private List<Edge> edges;

        public AStarPath(Mesh mesh)
        {
            edges = new List<Edge>(mesh.Edges);

        }
        //public void BuildCostsPath()
        //{
        //    NodeVisits = 0;
        //    Start.MinCostToStart = 0;
        //    var prioQueue = new List<Node>();
        //    prioQueue.Add(Start);
        //    do
        //    {
        //        prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
        //        var node = prioQueue.First();
        //        prioQueue.Remove(node);
        //        NodeVisits++;
        //        foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
        //        {
        //            var childNode = cnn.ConnectedNode;
        //            if (childNode.Visited)
        //                continue;
        //            if (childNode.MinCostToStart == null ||
        //                node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
        //            {
        //                childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
        //                childNode.NearestToStart = node;
        //                if (!prioQueue.Contains(childNode))
        //                    prioQueue.Add(childNode);
        //            }
        //        }
        //        node.Visited = true;
        //        if (node == End)
        //            return;
        //    } while (prioQueue.Any());
        //}

    }
    public class Searchmap
    {

        public int NodeVisits { get; private set; }
        public double ShortestPathLength { get; set; }
        public double ShortestPathCost { get; private set; }


        public List<Node> Nodes { get; set; } = new List<Node>();

        public Node StartNode { get; set; }

        public Node EndNode { get; set; }

        public List<Node> ShortestPath { get; set; } = new List<Node>();


        public void AstarSearch(List<Node> geericList)
        {
           // NodeVisits = 0;
            StartNode.MinCostToStart = 0;
            var prioQueue = new List<Node>();
            prioQueue.Add(StartNode);
            do
            {
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                //NodeVisits++;
                foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
                {
                    var childNode = geericList[cnn.ConnectedNode];
                    if (childNode.Visited)
                        continue;
                    if (childNode.MinCostToStart == null ||
                        node.MinCostToStart + cnn.Cost < childNode.MinCostToStart)
                    {
                        childNode.MinCostToStart = node.MinCostToStart + cnn.Cost;
                        childNode.NearestToStart = node;
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }
                node.Visited = true;
                if (node == EndNode)
                    return;
            } while (prioQueue.Any());
        }

        public List<Node> GetShortestPathAstart(List<Node> genericList)
        {
            foreach (var node in Nodes)
                node.StraightLineDistanceToEnd = node.StraightLineDistanceTo(StartNode);;
            AstarSearch(genericList);
            var shortestPath = new List<Node>();
            shortestPath.Add(EndNode);
            BuildShortestPath(shortestPath, EndNode, genericList);
            shortestPath.Reverse();
            return shortestPath;
        }

        private void BuildShortestPath(List<Node> list, Node node, List<Node> genericList)
        {
            if (node.NearestToStart == null)
                return;
            list.Add(node.NearestToStart);
            ShortestPathLength += node.Connections.Single(x => genericList[x.ConnectedNode] == node.NearestToStart).Length;
            ShortestPathCost += node.Connections.Single(x => genericList[x.ConnectedNode] == node.NearestToStart).Cost;
            BuildShortestPath(list, node.NearestToStart, genericList);
        }


    }


    public class Node 
    {
        public int Id { get; set; }
       // public string Name { get; set; }
        public Vertex Point { get; set; }
        public List<SearchEdge> Connections { get; set; } = new List<SearchEdge>();

        public double? MinCostToStart { get; set; }
        public Node NearestToStart { get; set; }
        public bool Visited { get; set; }
        public double StraightLineDistanceToEnd { get; set; }

        public static List<Node> CreateListNodes(ref Mesh map)
        {
            List<Node> nodes = new List<Node>();
            Vertex[] verx = map.Vertices.ToArray();
            for (int i = 0; i < verx.Length; i++)
            {
                Node node = new Node { Id = i,  Point = verx[i], Visited = false, Connections = new List<SearchEdge>(), MinCostToStart = null, NearestToStart = null };
                List<Edge> list = map.Edges.Where(x => x.P0 == i || x.P1 == i).ToList();

                foreach (Edge item in list)
                {
                    node.Connections.Add(new SearchEdge {
                                     ConnectedNode = item.P0 == node.Id? item.P1: item.P0,
                                     Cost = Math.Sqrt(Math.Pow(verx[item.P0].X - verx[item.P1].X, 2) + Math.Pow(verx[item.P0].Y - verx[item.P1].Y, 2)),
                                     Length = Math.Sqrt(Math.Pow(verx[item.P0].X - verx[item.P1].X, 2) + Math.Pow(verx[item.P0].Y - verx[item.P1].Y, 2))
                                    });
                }
                nodes.Add(node);
            }
            return nodes;
        }
        internal void ConnectClosestNodes(ref Mesh map)
        {
        }
        //internal void ConnectClosestNodes(List<Node> nodes, int branching, Random rnd, bool randomWeight)
        //{
        //    var connections = new List<SearchEdge>();
        //    foreach (var node in nodes)
        //    {
        //        //if (node.Id == this.Id)
        //        //    continue;

        //        // var dist = Math.Sqrt(Math.Pow(Point.X - node.Point.X, 2) + Math.Pow(Point.Y - node.Point.Y, 2));
        //        var dist = Math.Pow(Point.X - node.Point.X, 2) + Math.Pow(Point.Y - node.Point.Y, 2);
        //        connections.Add(new SearchEdge
        //        {
        //            ConnectedNode = node,
        //            Length = dist,
        //            Cost = randomWeight ? rnd.NextDouble() : dist,
        //        });
        //    }
        //    connections = connections.OrderBy(x => x.Length).ToList();
        //    var count = 0;
        //    foreach (var cnn in connections)
        //    {
        //        //Connect three closes nodes that are not connected.
        //        if (!Connections.Any(c => c.ConnectedNode == cnn.ConnectedNode))
        //            Connections.Add(cnn);
        //        count++;

        //        //Make it a two way connection if not already connected
        //        if (!cnn.ConnectedNode.Connections.Any(cc => cc.ConnectedNode == this))
        //        {
        //            var backConnection = new SearchEdge { ConnectedNode = this, Length = cnn.Length };
        //            cnn.ConnectedNode.Connections.Add(backConnection);
        //        }
        //        if (count == branching)
        //            return;
        //    }
        //}

        public double StraightLineDistanceTo(Node end)
        {
            // return Math.Sqrt(Math.Pow(Point.X - end.Point.X, 2) + Math.Pow(Point.Y - end.Point.Y, 2));
            return Math.Sqrt(Math.Pow(Point.X - end.Point.X, 2) + Math.Pow(Point.Y - end.Point.Y, 2));
        }

        internal bool ToCloseToAny(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                // var d = Math.Sqrt(Math.Pow(Point.X - node.Point.X, 2) + Math.Pow(Point.Y - node.Point.Y, 2));
                var d = Math.Pow(Point.X - node.Point.X, 2) + Math.Pow(Point.Y - node.Point.Y, 2);
                if (d < 0.01)
                    return true;
            }
            return false;
        }
    }

    public class SearchEdge
    {
        public double Length { get; set; }
        public double Cost { get; set; }
        public int ConnectedNode { get; set; }
    }
}
