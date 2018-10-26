// -----------------------------------------------------------------------
// <copyright file="Contour.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using UnityEngine;
using System.Linq;

namespace DebugClient.Geometry
{
    public class Contour
    {
        int marker;
        bool convex;

        /// <summary>
        /// Gets or sets the list of points making up the contour.
        /// </summary>
        public List<Vertex> Points { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contour" /> class.
        /// </summary>
        /// <param name="points">The points that make up the contour.</param>
        public Contour(IEnumerable<Vertex> points)
            : this(points, 0, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Contour" /> class.
        /// </summary>
        /// <param name="points">The points that make up the contour.</param>
        /// <param name="marker">Contour marker.</param>
        public Contour(IEnumerable<Vertex> points, int marker)
            : this(points, marker, false)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Contour" /> class.
        /// </summary>
        /// <param name="points">The points that make up the contour.</param>
        /// <param name="marker">Contour marker.</param>
        /// <param name="convex">The hole is convex.</param>
        public Contour(IEnumerable<Vertex> points, int marker, bool convex)
        {
            AddPoints(points);

            this.marker = marker;
            this.convex = convex;
        }

        private void AddPoints(IEnumerable<Vertex> points)
        {
            this.Points = new List<Vertex>(points);

            int count = Points.Count - 1;

            // Check if first vertex equals last vertex.
            if (Points[0] == Points[count])
            {
                Points.RemoveAt(count);
            }
        }
        #region Helper methods
        /// <summary>
        /// Return true if the given point is inside the polygon, or false if it is not.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="poly">The polygon (list of contour points).</param>
        /// <returns></returns>
        /// <remarks>
        /// WARNING: If the point is exactly on the edge of the polygon, then the function
        /// may return true or false.
        /// 
        /// See http://alienryderflex.com/polygon/
        /// </remarks>
        public  bool IsPointInPolygon(Vertex point)
        {
            return IsPointInPolygon(point, Points);
        }
        private static bool IsPointInPolygon(Vertex point, List<Vertex> poly)
        {
            bool inside = false;

            double x = point.X;
            double y = point.Y;

            int count = poly.Count;

            for (int i = 0, j = count - 1; i < count; i++)
            {
                if (((poly[i].Y < y && poly[j].Y >= y) || (poly[j].Y < y && poly[i].Y >= y))
                    && (poly[i].X <= x || poly[j].X <= x))
                {
                    inside ^= (poly[i].X + (y - poly[i].Y) / (poly[j].Y - poly[i].Y) * (poly[j].X - poly[i].X) < x);
                }

                j = i;
            }

            return inside;
        }
        public  List<Vertex> Offset(float offset)
        {
            List<Vertex> enlarged_points = new List<Vertex>();

            int num_points = Points.Count;
            for (int j = 0; j < num_points; j++)
            {
                // Find the new location for point j.
                // Find the points before and after j.
                int i = (j - 1);
                if (i < 0) i += num_points;
                int k = (j + 1) % num_points;

                // Move the points by the offset.
                Vector2 v1 = new Vector2(Points[j].ToPointF.X - Points[i].ToPointF.X, Points[j].ToPointF.Y - Points[i].ToPointF.Y);
                v1.Normalize();
                v1 *= offset;
                Vector2 n1 = new Vector2(-v1.y, v1.x);

                Vertex pij1 = new Vertex((float)(Points[i].ToPointF.X + n1.x), (float)(Points[i].ToPointF.Y + n1.y));
                Vertex pij2 = new Vertex((float)(Points[j].ToPointF.X + n1.x), (float)(Points[j].ToPointF.Y + n1.y));

                Vector2 v2 = new Vector2(Points[k].ToPointF.X - Points[j].ToPointF.X, Points[k].ToPointF.Y - Points[j].ToPointF.Y);
                v2.Normalize();
                v2 *= offset;
                Vector2 n2 = new Vector2(-v2.y, v2.x);

                Vertex pjk1 = new Vertex(
                    (float)(Points[j].X + n2.x),
                    (float)(Points[j].Y + n2.y));
                Vertex pjk2 = new Vertex(
                    (float)(Points[k].X + n2.x),
                    (float)(Points[k].Y + n2.y));

                // See where the shifted lines ij and jk intersect.
                bool lines_intersect, segments_intersect;
                Vertex poi, close1, close2;
                FindIntersection(pij1, pij2, pjk1, pjk2,
                    out lines_intersect, out segments_intersect,
                    out poi, out close1, out close2);
                enlarged_points.Add(poi);
            }
            return enlarged_points;
        }
        public List<Vertex> OffsetAtPoint(float offset)
        {
            List<Vertex> out_points = new List<Vertex>();

            int num_points = Points.Count;

            for (int j = 2, i = 1, k = 0; j < num_points; j++,i++,k++)
            {
                Vector2 v1 = new Line(Points[k], Points[i]).Vector2;
                Vector2 v2 = new Line(Points[i], Points[j]).Vector2;
                double angle  = (Math.PI/180) * (Vector2.Angle(v1, v2) / 2);
                out_points.Add(new Vertex(Points[i].X + offset * Math.Cos(angle), Points[i].Y + offset * Math.Sin(angle)));
            }
            return out_points;
        }

        private static void FindIntersection(Vertex p1, Vertex p2, Vertex p3, Vertex p4,
                  out bool lines_intersect, out bool segments_intersect, out Vertex intersection, out Vertex close_p1, out Vertex close_p2)
        {
            // Get the segments' parameters.
            double dx12 = p2.X - p1.X;
            double dy12 = p2.Y - p1.Y;
            double dx34 = p4.X - p3.X;
            double dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            double denominator = (dy12 * dx34 - dx12 * dy34);

            double t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                    / denominator;
            if (double.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new Vertex(double.NaN, double.NaN);
                close_p1 =     new Vertex(double.NaN, double.NaN);
                close_p2 =     new Vertex(double.NaN, double.NaN);
                return;
            }
            lines_intersect = true;

            double t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Vertex(p1.X + dx12 * t1, p1.Y + dy12 * t1);

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
            close_p1 = new Vertex(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new Vertex(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }
        #endregion
    }
}
