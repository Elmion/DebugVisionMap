// -----------------------------------------------------------------------
// <copyright file="Vertex.cs" company="">
// Original Triangle code by Jonathan Richard Shewchuk, http://www.cs.cmu.edu/~quake/triangle.html
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Drawing;
using UnityEngine;

namespace DebugClient.Geometry
{
   

    /// <summary>
    /// The vertex data structure.
    /// </summary>
    public class Vertex 
    {
        public double X;
        public double Y;

        public Point ToPoint {
            get { return new Point((int)X, (int)Y); } }
        public Vector2 ToVecto2
        {
            get { return new Vector2((int)X, (int)Y); }
        }
        public PointF ToPointF
        {
            get { return new PointF((float)X, (float)Y); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex" /> class.
        /// </summary>
        public Vertex()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Vertex" /> class.
        /// </summary>
        /// <param name="x">The x coordinate of the vertex.</param>
        /// <param name="y">The y coordinate of the vertex.</param>
        public Vertex(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Vertex(float x, float y)
        {
            X = x;
            Y = y;
        }


    }
}
