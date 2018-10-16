using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestSigh
{
    public partial class Form1 : Form
    {
        Segment[] Map;
        Point[] points;
        bool updateCanvas = false;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Map = InitMap();
            
            
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
             points = CalcSightPoligon(e.Location, Map);
             updateCanvas = true;
             Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (updateCanvas)
            {
                for (var i = 0; i < Map.Length; i++)
                {
                    var seg = Map[i];
                    e.Graphics.DrawLine(Pens.Black, new Point(seg.a.GetPoint.X, seg.a.GetPoint.Y), new Point(seg.b.GetPoint.X, seg.b.GetPoint.Y));
                }
               
                e.Graphics.FillPolygon(Brushes.Salmon , points);
                updateCanvas = false;
            }
        }
        private Segment[] InitMap()
        {
            return new Segment[] {

                // Border
                new Segment { a = new PointSight(0, 0), b = new PointSight(640, 0) },
            new Segment { a = new PointSight(640, 0), b = new PointSight(640, 360) },
            new Segment { a = new PointSight(640, 360), b = new PointSight(0, 360) },
            new Segment { a = new PointSight(0, 360), b = new PointSight(0, 0) },

            // Polygon #1

            new Segment { a = new PointSight(100, 150), b = new PointSight(120, 50) },
            new Segment { a = new PointSight(120, 50), b = new PointSight(200, 80) },
            new Segment { a = new PointSight(200, 80), b = new PointSight(140, 210) },
            new Segment { a = new PointSight(140, 210), b = new PointSight(100, 150) },


            // Polygon #2
            new Segment { a = new PointSight(100, 200), b = new PointSight(120, 250) },
            new Segment { a = new PointSight(120, 250), b = new PointSight(60, 300) },
            new Segment { a = new PointSight(60, 300), b = new PointSight(100, 200) },

                // Polygon #3
            new Segment { a = new PointSight(200, 260), b = new PointSight(220, 150) },
            new Segment { a = new PointSight(220, 150), b = new PointSight(300, 200) },
            new Segment { a = new PointSight(300, 200), b = new PointSight(350, 320) },
            new Segment { a = new PointSight(350, 320), b = new PointSight(200, 260) },

                // Polygon #4
            new Segment { a = new PointSight(340, 60), b = new PointSight(360, 40) },
            new Segment { a = new PointSight(360, 40), b = new PointSight(370, 70) },
            new Segment { a = new PointSight(370, 70), b = new PointSight(340, 60) },

            // Polygon #5
            new Segment { a = new PointSight(450, 190), b = new PointSight(560, 170) },
            new Segment { a = new PointSight(560, 170), b = new PointSight(540, 270) },
            new Segment { a = new PointSight(540, 270), b = new PointSight(430, 290) },
            new Segment { a = new PointSight(430, 290), b = new PointSight(450, 190) },

                // Polygon #6
            new Segment { a = new PointSight(400, 95), b = new PointSight(580, 50) },
            new Segment { a = new PointSight(580, 50), b = new PointSight(480, 150) },
            new Segment { a = new PointSight(480, 150), b = new PointSight(400, 95) },
            new Segment { a = new PointSight(430, 290), b = new PointSight(450, 190) }
            };
        }
        private PointSight getIntersection(Segment ray,Segment segment)
        {
            // RAY in parametric: Point + Delta*T1
            var r_px = ray.a.X;
            var r_py = ray.a.Y;
            var r_dx = ray.b.X - ray.a.X;
            var r_dy = ray.b.Y - ray.a.Y;

            // SEGMENT in parametric: Point + Delta*T2
            var s_px = segment.a.X;
            var s_py = segment.a.Y;
            var s_dx = segment.b.X - segment.a.X;
            var s_dy = segment.b.Y - segment.a.Y;

            // Are they parallel? If so, no intersect
            var r_mag = Math.Sqrt(r_dx * r_dx + r_dy * r_dy);
            var s_mag = Math.Sqrt(s_dx * s_dx + s_dy * s_dy);
            if (r_dx / r_mag == s_dx / s_mag && r_dy / r_mag == s_dy / s_mag)
            {
                // Unit vectors are the same.
                return null;
            }

            // SOLVE FOR T1 & T2
            // r_px+r_dx*T1 = s_px+s_dx*T2 && r_py+r_dy*T1 = s_py+s_dy*T2
            // ==> T1 = (s_px+s_dx*T2-r_px)/r_dx = (s_py+s_dy*T2-r_py)/r_dy
            // ==> s_px*r_dy + s_dx*T2*r_dy - r_px*r_dy = s_py*r_dx + s_dy*T2*r_dx - r_py*r_dx
            // ==> T2 = (r_dx*(s_py-r_py) + r_dy*(r_px-s_px))/(s_dx*r_dy - s_dy*r_dx)
            double T2 = (r_dx * (s_py - r_py) + r_dy * (r_px - s_px)) / (s_dx * r_dy - s_dy * r_dx);
            double T1 = (s_px + s_dx * T2 - r_px) / r_dx;
            // Must be within parametic whatevers for RAY/SEGMENT
            if (Double.IsNaN(T2) || Double.IsNaN(T1)) return null;
            if (T1 < 0) return null;
            if (T2 < 0 || T2 > 1) return null;

            // Return the POINT OF INTERSECTION
            return new PointSight ( 
                 (r_px + r_dx * T1),
                 (r_py + r_dy * T1),
		         T1
            );

        }
        public Point[] CalcSightPoligon(Point position, Segment[] map)
        {


            List<PointSight> points = map.Select(x => x.a).ToList();
            points.AddRange(map.Select(x => x.b).ToList());

            List<PointSight> uniquePoints = new List<PointSight>();
            points.ForEach(x => { if (uniquePoints.FindIndex(y => y.X == x.X && y.Y == x.Y) == -1)
            {
                uniquePoints.Add(points.FindAll(y => y.X == x.X && y.Y == x.Y).First());
            }
            });

            List<double> uniqueAngles = new List<double>();
            for (var j = 0; j < uniquePoints.Count; j++)
            {
                var uniquePoint = uniquePoints[j];
                var angle = Math.Atan2(uniquePoint.Y -(double)position.Y, uniquePoint.X - (double)(position.X));
                uniquePoint.angle = angle;
                uniqueAngles.AddRange(new double[] {angle - 0.0000001, angle, angle + 0.0000001 });
            }

            // RAYS IN ALL DIRECTIONS
            List<PointSight> intersects = new List<PointSight>();
            for (var j = 0; j < uniqueAngles.Count; j++)
            {
                var angle = uniqueAngles[j];

                // Calculate dx & dy from angle
                var dx = Math.Cos(angle);
                var dy = Math.Sin(angle);

                // Ray from center of screen to mouse
                var ray = new Segment { a = new PointSight(position.X, position.Y), b = new PointSight( (double)position.X + dx, (double)position.Y + dy) };

                // Find CLOSEST intersection
                PointSight closestIntersect = null;
                for (var i = 0; i < map.Length; i++)
                {
                    var intersect = getIntersection(ray, map[i]);
                    if (intersect == null) continue;
                    if (closestIntersect == null || intersect.param < closestIntersect.param)
                    {
                        closestIntersect = intersect;
                    }
                }
                // Intersect angle
                if (closestIntersect == null) continue;
                closestIntersect.angle = angle;

                // Add to list of intersects
                intersects.Add(closestIntersect);
            }

            // Sort intersects by angle
            intersects.Sort( (p, p1) => {
                double TEMP = p.angle - p1.angle;
                if (TEMP > 0) return 1;
                if (TEMP < 0) return -1;
                return 0;
                    });


            return intersects.Select(x => x.GetPoint).ToArray();
        }
    }
    public class Segment
    {
        public PointSight a;
        public PointSight b;
    }
    public class PointSight
    {
        public double angle;
        public double param;
        public Point GetPoint { get { return new Point((int)X, (int)Y); } }

        public double X;
        public double Y;

        public PointSight(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public PointSight(double X, double Y, double param)
        {
            this.X = X;
            this.Y = Y;
            this.param = param;
        }
    }
}
