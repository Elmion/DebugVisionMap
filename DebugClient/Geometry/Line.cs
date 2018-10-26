using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace DebugClient.Geometry
{
    public class Line
    {
        public Vertex p0;
        public Vertex p1;
        public double Length { get { return Math.Sqrt(Math.Pow(p0.X - p1.X, 2) + Math.Pow(p0.Y - p1.Y, 2)); }
        }
        public Line(Vertex p0, Vertex p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public Vertex getIntersectionWithRay(Line segment, out double howClose)
        {
            howClose = double.NaN;
            // RAY in parametric: Point + Delta*T1
            var r_px = this.p0.X;
            var r_py = this.p0.Y;
            var r_dx = this.p1.X - this.p0.X;
            var r_dy = this.p1.Y - this.p0.Y;

            // SEGMENT in parametric: Point + Delta*T2
            var s_px = segment.p0.X;
            var s_py = segment.p0.Y;
            var s_dx = segment.p1.X - segment.p0.X;
            var s_dy = segment.p1.Y - segment.p0.Y;

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

            howClose = T1;
            // Return the POINT OF INTERSECTION
            return new Vertex(
                 (r_px + r_dx * T1),
                 (r_py + r_dy * T1)
            );
        }

        public Vector2 Vector2
        {
            get
            {
                return new Vector2((float)(p1.X - p0.X), (float)(p1.Y - p0.Y));
            }

        }
        public double A
        {
            get
            {
                return p1.Y - p0.Y;
            }
        }
        public double B
        {
            get
            {
                return p1.X - p0.X;
            }
        }
        public double C
        {
            get
            {
                return p0.X*p1.Y - p1.X*p0.Y;
            }
        }


        public float Dot(Line line)
        {
            Vector2 a = new Vector2((float)(p1.X - p0.X), (float)(p1.Y - p0.Y));
            Vector2 b = new Vector2((float)(line.p1.X - line.p0.X),(float)(line.p1.Y - line.p0.Y));
            return Vector2.Dot(a, b);
        }
       

    }
}
