using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DebugClient.Geometry;
using System.Drawing;

namespace DebugClient.AStarSearch
{
    public class Map
    {
        public List<Vertex> Vertexs { get; private set; }
        public List<Edge> Edges { get; private set; }

        List<Contour> Conturs;

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
            Edge newEdge = new Edge(
            Vertexs.FindIndex(y => y.X == cnt.Points[0].X && y.Y == cnt.Points[0].Y),
            Vertexs.FindIndex(y => y.X == cnt.Points[cnt.Points.Count - 1].X && y.Y == cnt.Points[cnt.Points.Count - 1].Y));
        }
        public void RemoveCountur(Contour cnt)
        {
            throw new NotImplementedException();
        }
        public Line[] GetLines()
        {
            return Edges.Select(x => new Line(Vertexs[x.P0], Vertexs[x.P1])).Cast<Line>().ToArray();
        }
        public Contour GetSightAtPoint(Point p)
        {
            Line[] lines = GetLines();
            List<double> uniqueAngles = new List<double>();

            for (var j = 0; j < Vertexs.Count; j++)
            {
                var uniquePoint = Vertexs[j];
                var angle = Math.Atan2(uniquePoint.Y - (double)p.Y, uniquePoint.X - (double)(p.X));
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
                var ray = new Line( new Vertex(p.X, p.Y),  new Vertex((double)p.X + dx, (double)p.Y + dy) );

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
}
