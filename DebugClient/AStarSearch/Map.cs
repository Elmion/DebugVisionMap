using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugClient.Geometry;
using System.Drawing;
using UnityEngine;

namespace DebugClient.AStarSearch
{
    public class Map
    {
        public List<Vertex> Vertexs { get; private set; }
        public List<Edge> Edges { get; private set; }
        public List<Contour> Conturs { get; private set; }

        private List<Vertex> NovigationVertex;
        private List<Node> CurrentNodesMap;

        public Map()
        {
            Conturs = new List<Contour>();
            Vertexs = new List<Vertex>();
            Edges = new List<Edge>();
        }

        public void AddCountur(Contour cnt)
        {
            Conturs.Add(cnt);
            cnt.Points.ForEach(x => {
                if (Vertexs.FindIndex(y => y.X == x.X && y.Y == x.Y) == -1)
                {
                    Vertexs.Add(cnt.Points.FindAll(y => y.X == x.X && y.Y == x.Y).First());
                }
            });
            //Все ребра кроме последнего
            for (int i = 0; i < cnt.Points.Count - 1; i++)
            {
                int pointFirst = Vertexs.FindIndex(y => y.X == cnt.Points[i].X && y.Y == cnt.Points[i].Y);
                int pointSecond = Vertexs.FindIndex(y => y.X == cnt.Points[i + 1].X && y.Y == cnt.Points[i + 1].Y);
                Edges.Add(new Edge(pointFirst, pointSecond));
            }
            //Последнее ребро из конца в начало
            Edges.Add(new Edge(
            Vertexs.FindIndex(y => y.X == cnt.Points[0].X && y.Y == cnt.Points[0].Y),
            Vertexs.FindIndex(y => y.X == cnt.Points[cnt.Points.Count - 1].X && y.Y == cnt.Points[cnt.Points.Count - 1].Y)));
            //Перекраиваем карту навигации
            CurrentNodesMap = InitNavMap();
        }
        public void RemoveCountur(Contour cnt)
        {
            throw new NotImplementedException();
        }
        public List<Node> InitNavMap()
        {
            List<Node> nodes = new List<Node>();
            NovigationVertex = new List<Vertex>();
            //делаем смещение от каждого контура на малую величину.
            //получаем список навигационых точек от каждого контура
            Conturs.ForEach(x => NovigationVertex.AddRange(x.Offset(1.01f)));
            //ищем какие навигационые точки видит каждая новигационая точка
            for (int i = 0; i < NovigationVertex.Count; i++)
            {
                //будующая навигационая нода
                Node node = new Node { Id = i, Point = NovigationVertex[i], Visited = false, Connections = new List<SearchEdge>(), MinCostToStart = null, NearestToStart = null };

                Contour c = GetSightAtPoint(NovigationVertex[i].ToVecto2); // взгляд из точки точки 

                for (int j = 0; j < NovigationVertex.Count; j++)
                {
                    //если нав.точка внутри взгляда другой нав. точки то соеденияем их ( если это не она же)
                    if (c.IsPointInPolygon(NovigationVertex[j]) && i != j)
                    {
                        node.Connections.Add(new SearchEdge
                        {
                            ConnectedNode = j,
                            Cost = new Line(NovigationVertex[i], NovigationVertex[j]).Length,
                            Length = new Line(NovigationVertex[i], NovigationVertex[j]).Length
                        });
                    }   
                }
                nodes.Add(node);
            }
            return nodes;
        }

        private void PricingPath(Node Start, Node End , ref List<Node> NodeMap) 
        {
            Start.MinCostToStart = 0;
            var prioQueue = new List<Node>();
            prioQueue.Add(Start);
            do
            {
                prioQueue = prioQueue.OrderBy(x => x.MinCostToStart + x.StraightLineDistanceToEnd).ToList();
                var node = prioQueue.First();
                prioQueue.Remove(node);
                foreach (var cnn in node.Connections.OrderBy(x => x.Cost))
                {
                    var childNode = NodeMap[cnn.ConnectedNode];
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
                if (node == End)
                    return;
            } while (prioQueue.Any());
        }
        private List<Node> SearchPath(List<Node> NodeMap)
        {
            //Так стартовая и конечная точки поставлены в предыдущем методе
            Node StartNode = NodeMap[NodeMap.Count - 2]; //
            Node EndNode = NodeMap[NodeMap.Count - 1]; //

            foreach (var node in NodeMap)
                     node.StraightLineDistanceToEnd = node.StraightLineDistanceTo(StartNode); ;
            PricingPath(StartNode, EndNode, ref NodeMap);
            var shortestPath = new List<Node>();
            shortestPath.Add(EndNode);
            BuildShortestPath(shortestPath, EndNode, NodeMap);
            shortestPath.Reverse();
            return shortestPath;
        }
        private void BuildShortestPath(List<Node> list, Node node, List<Node> genericList)
        {
            if (node.NearestToStart == null)
                return;
            list.Add(node.NearestToStart);
            //ShortestPathLength += node.Connections.Single(x => genericList[x.ConnectedNode] == node.NearestToStart).Length;
            //ShortestPathCost += node.Connections.Single(x => genericList[x.ConnectedNode] == node.NearestToStart).Cost;
            BuildShortestPath(list, node.NearestToStart, genericList);
        }

        public List<Node> GetNavigationForPoints(Vertex start, Vertex end)
        {
            //немного дублировный код но ради 2 точек перстраивать всё не есть гуд
            List<Vertex> VertexMap = new List<Vertex>(NovigationVertex);
            VertexMap.Add(start);
            VertexMap.Add(end);

            int startIndex = VertexMap.Count - 2;
            int endIndex = VertexMap.Count - 1;

            //будующая навигационая нода
            Node nodeStart = new Node { Id = startIndex, Point = VertexMap[startIndex], Visited = false, Connections = new List<SearchEdge>(), MinCostToStart = null, NearestToStart = null };
            Node nodeEnd = new Node { Id = endIndex, Point = VertexMap[endIndex], Visited = false, Connections = new List<SearchEdge>(), MinCostToStart = null, NearestToStart = null };

            Contour cStart = GetSightAtPoint(VertexMap[startIndex].ToVecto2); // взгляд из точки точки 
            Contour cEnd = GetSightAtPoint(VertexMap[endIndex].ToVecto2); // взгляд из точки точки 

            for (int j = 0; j < VertexMap.Count; j++)
            {
                //если нав.точка внутри взгляда другой нав. точки то соеденияем их ( если это не она же)
                if (cEnd.IsPointInPolygon(VertexMap[j]) && endIndex != j)
                {
                    nodeEnd.Connections.Add(new SearchEdge
                    {
                        ConnectedNode = j,
                        Cost = new Line(VertexMap[endIndex], VertexMap[j]).Length,
                        Length = new Line(VertexMap[endIndex], VertexMap[j]).Length
                    });
                }
                if (cStart.IsPointInPolygon(VertexMap[j]) && startIndex != j)
                {
                    nodeStart.Connections.Add(new SearchEdge
                    {
                        ConnectedNode = j,
                        Cost = new Line(VertexMap[startIndex], VertexMap[j]).Length,
                        Length = new Line(VertexMap[startIndex], VertexMap[j]).Length
                    });
                }
            }
            List<Node> NodeOUT = new List<Node>(CurrentNodesMap);
            NodeOUT.Add(nodeStart);
            NodeOUT.Add(nodeEnd);
            return SearchPath(NodeOUT);
        }
        public Line[]  GetLines()
        {
            return Edges.Select(x => new Line(Vertexs[x.P0], Vertexs[x.P1])).Cast<Line>().ToArray();
        }
        public Contour GetSightAtPoint(Vector2 p)
        {
            Line[] lines = GetLines();
            List<double> uniqueAngles = new List<double>();

            for (var j = 0; j < Vertexs.Count; j++)
            {
                var uniquePoint = Vertexs[j];
                var angle = Math.Atan2(uniquePoint.Y - (double)p.y, uniquePoint.X - (double)(p.x));
                //uniquePoint.angle = angle;
                uniqueAngles.AddRange(new double[] {angle - 0.0000001, angle, angle + 0.0000001 });
            }
            // RAYS IN ALL DIRECTIONS
            Dictionary<Vertex,double> intersects = new Dictionary<Vertex, double>();

            for (var j = 0; j<uniqueAngles.Count; j++)
            {
                var angle = uniqueAngles[j];

                // Calculate dx & dy from angle
                var dx = Math.Cos(angle);
                var dy = Math.Sin(angle);

                // Ray from center of screen to mouse
                var ray = new Line( new Vertex(p.x, p.y),  new Vertex((double)p.x + dx, (double)p.y + dy) );

                // Find CLOSEST intersection
                Vertex closestIntersect = null;
                double Tbuff = double.NaN, T1 = double.NaN;
                for (var i = 0; i< lines.Length; i++)
                {
                    Vertex intersect = ray.getIntersectionWithRay(lines[i],out T1);
                    if (intersect == null) continue;
                    if (closestIntersect == null || T1 < Tbuff)
                    {
                        Tbuff = T1;
                        closestIntersect = intersect;
                    }
                }
                // Intersect angle
                if (closestIntersect == null) continue;
                // Add to list of intersects
                intersects.Add(closestIntersect,angle);
            }
            // Sort intersects by angle
            var sortlist = intersects.ToList();
            sortlist.Sort((x, y) => x.Value.CompareTo(y.Value));

            return new Contour(sortlist.Select(x => x.Key));
        }
    }
    public class SearchEdge
    {
        public double Length { get; set; }
        public double Cost { get; set; }
        public int ConnectedNode { get; set; }
    }
    public class Node
    {
        public int Id { get; set; }
        public Vertex Point { get; set; }
        public List<SearchEdge> Connections { get; set; } = new List<SearchEdge>();
        public double? MinCostToStart { get; set; }
        public Node NearestToStart { get; set; }
        public bool Visited { get; set; }
        public double StraightLineDistanceToEnd { get; set; }

        public double StraightLineDistanceTo(Node end)
        {
            return Math.Sqrt(Math.Pow(Point.X - end.Point.X, 2) + Math.Pow(Point.Y - end.Point.Y, 2));
        }
    }
}
