﻿using System;
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

           // var d = GetEnlargedPolygon(map., 1.01f);
           // var con = map.GetSightAtPoint(new Vector2(d[11].X, d[11].Y));

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
