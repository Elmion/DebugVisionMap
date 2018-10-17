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

    class AStarPath
    {
        private List<Edge> edges;

        public AStarPath(Map map)
        {
           // edges = new List<Edge>(mesh.Edges);

        }
    }
    public class SearchPath
    {

        public int NodeVisits { get; private set; }
        public double ShortestPathLength { get; set; }
        public double ShortestPathCost { get; private set; }


        public List<Node> Nodes { get; set; } = new List<Node>();

        public Node StartNode { get; set; }

        public Node EndNode { get; set; }

        public List<Node> ShortestPath { get; set; } = new List<Node>();

        public SearchPath()
        {


        }
/*
        public void SearchPath(Node start, Node end)  List<Node> geericList
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
        }*/
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


        public static List<Node> CreateListNodes(ref Map map)
        {

            var d = GetEnlargedPolygon(map.Vertexs.Select(x => x.ToPointF).ToList(), 1.01f);
            var con = map.GetSightAtPoint(new Vector2(d[11].X, d[11].Y));

            List<Node> nodes = new List<Node>();
            Vertex[] verx = map.Vertexs.ToArray();
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

        public static List<PointF> GetEnlargedPolygon(
    List<PointF> old_points, float offset)
        {
            List<PointF> enlarged_points = new List<PointF>();
            int num_points = old_points.Count;
            for (int j = 0; j < num_points; j++)
            {
                // Find the new location for point j.
                // Find the points before and after j.
                int i = (j - 1);
                if (i < 0) i += num_points;
                int k = (j + 1) % num_points;

                // Move the points by the offset.
                Vector2 v1 = new Vector2(old_points[j].X - old_points[i].X,old_points[j].Y - old_points[i].Y);
                v1.Normalize();
                v1 *= offset;
                Vector2 n1 = new Vector2(-v1.y, v1.x);

                PointF pij1 = new PointF((float)(old_points[i].X + n1.x),(float)(old_points[i].Y + n1.y));
                PointF pij2 = new PointF((float)(old_points[j].X + n1.x),(float)(old_points[j].Y + n1.y));

                Vector2 v2 = new Vector2(old_points[k].X - old_points[j].X,old_points[k].Y - old_points[j].Y);
                v2.Normalize();
                v2 *= offset;
                Vector2 n2 = new Vector2(-v2.y, v2.x);

                PointF pjk1 = new PointF(
                    (float)(old_points[j].X + n2.x),
                    (float)(old_points[j].Y + n2.y));
                PointF pjk2 = new PointF(
                    (float)(old_points[k].X + n2.x),
                    (float)(old_points[k].Y + n2.y));

                // See where the shifted lines ij and jk intersect.
                bool lines_intersect, segments_intersect;
                PointF poi, close1, close2;
                FindIntersection(pij1, pij2, pjk1, pjk2,
                    out lines_intersect, out segments_intersect,
                    out poi, out close1, out close2);
                enlarged_points.Add(poi);
            }

            return enlarged_points;
        }
        private static void FindIntersection(PointF p1, PointF p2, PointF p3, PointF p4,
                                    out bool lines_intersect, out bool segments_intersect, out PointF intersection, out PointF close_p1, out PointF close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new PointF(float.NaN, float.NaN);
                close_p1 = new PointF(float.NaN, float.NaN);
                close_p2 = new PointF(float.NaN, float.NaN);
                return;
            }
            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }
        /* public static List<Node> CreateListNodes(ref Segment[] map)
         {
             List<Node> nodes = new List<Node>();
             Vertex[] verx = map.Vertices.ToArray();
             for (int i = 0; i < verx.Length; i++)
             {
                 Node node = new Node { Id = i, Point = verx[i], Visited = false, Connections = new List<SearchEdge>(), MinCostToStart = null, NearestToStart = null };
                 List<Edge> list = map.Edges.Where(x => x.P0 == i || x.P1 == i).ToList();

                 foreach (Edge item in list)
                 {
                     node.Connections.Add(new SearchEdge
                     {
                         ConnectedNode = item.P0 == node.Id ? item.P1 : item.P0,
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

         public double StraightLineDistanceTo(Node end)
         {
             // return Math.Sqrt(Math.Pow(Point.X - end.Point.X, 2) + Math.Pow(Point.Y - end.Point.Y, 2));
             return Math.Sqrt(Math.Pow(Point.X - end.Point.X, 2) + Math.Pow(Point.Y - end.Point.Y, 2));
         }
         private static bool IsPointInPolygon(Point point, List<Vertex> poly)
         {
             bool inside = false;

             double x = point.x;
             double y = point.y;

             int count = poly.Count;

             for (int i = 0, j = count - 1; i < count; i++)
             {
                 if (((poly[i].y < y && poly[j].y >= y) || (poly[j].y < y && poly[i].y >= y))
                     && (poly[i].x <= x || poly[j].x <= x))
                 {
                     inside ^= (poly[i].x + (y - poly[i].y) / (poly[j].y - poly[i].y) * (poly[j].x - poly[i].x) < x);
                 }

                 j = i;
             }

             return inside;
         }*/
    }

    public class SearchEdge
    {
        public double Length { get; set; }
        public double Cost { get; set; }
        public int ConnectedNode { get; set; }
    }
}
