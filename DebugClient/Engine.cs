using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using DebugClient.AStarSearch;
using System.Collections;
using DebugClient.Geometry;

namespace DebugClient
{
    public partial class Engine : Form
    {
        public static Engine Instance;
        private Timer time;
        private List<object> SceneObjects;
        private Hero Hero;
        private Vertex start;
        private Vertex end;
        Map m;

        public Engine()
        {
            InitializeComponent();
            SceneObjects = new List<object>();
            Instance = this;
            this.DoubleBuffered = true;

            m = new Map();
            m.AddCountur(new Contour(new List<Vertex> { new Vertex(0, 0),
                                                        new Vertex(640, 0),
                                                        new Vertex(640, 360),
                                                        new Vertex(0, 360) }));

            m.AddCountur(new Contour(new List<Vertex> {new Vertex(100, 150),
                                                       new Vertex(120, 50) ,
                                                       new Vertex(200, 80) ,
                                                       new Vertex(140, 210) }));

            m.AddCountur(new Contour(new List<Vertex> {new Vertex(100, 200), 
                                                       new Vertex(120, 250),
                                                       new Vertex(60, 300) 
                                                       }));
            m.AddCountur(new Contour(new List<Vertex> { new Vertex(200, 260),
                                                        new Vertex(220, 150),
                                                        new Vertex(300, 200),
                                                        new Vertex(350, 320)
                                                       }));
            m.AddCountur(new Contour(new List<Vertex> {new Vertex(340, 60),
                                                       new Vertex(360, 40),
                                                       new Vertex(370, 70) 
                                                       }));
            m.AddCountur(new Contour(new List<Vertex> { new Vertex(450, 190),
                                                        new Vertex(560, 170),
                                                        new Vertex(540, 270),
                                                        new Vertex(430, 290) 
                                                       }));
            m.AddCountur(new Contour(new List<Vertex> { new Vertex(400, 95),
                                                        new Vertex(580, 50),
                                                        new Vertex(480, 150),
                                                        new Vertex(400, 95)
                                                       }));

            var dffff = Node.CreateListNodes(ref m);

            time = new Timer();
            time.Interval = 1;
            time.Tick += Time_Tick;

            Hero = CreateSceneObject<Hero>();
            Hero.Speed = 1;
          
            time.Start();

        }
        public T CreateSceneObject<T>(params object[] parm)
        {
            object obj =  typeof(T).GetConstructor(new Type[] {}).Invoke(parm);
            SceneObjects.Add(obj);
            return (T)obj;
        }
        private void Time_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
            foreach (SceneObject item in SceneObjects)
            {
                item.UpdateLogic();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {



            System.Drawing.Graphics g = e.Graphics;



            var d = Node.GetEnlargedPolygon(m.Vertexs.Select(x => x.ToPointF).ToList(), 1.01f);
            var con = m.GetSightAtPoint(new Vector2(d[11].X, d[11].Y));
            List<double> ff = new List<double>(); 
            for (int i = 0; i < con.Points.Count-1; i++)
            {
                ff.Add(new Line(con.Points[i], con.Points[i + 1]).Length);
            } 

            //Уровень
            Line[] lines = m.GetLines();
            for (var i = 0; i < lines.Length; i++)
            {
                g.DrawLine(Pens.Black, lines[i].p0.ToPoint, lines[i].p1.ToPoint);
            }
            //Видимость
            // g.FillPolygon(Brushes.Salmon, m.GetSightAtPoint(Hero.Position).Points.Select(x => x.ToPoint).ToArray());
            g.FillPolygon(Brushes.Salmon, con.Points.Select(x => x.ToPoint).ToArray());
            //Другие объекты
            foreach (SceneObject item in SceneObjects)
            {
                item.Draw(g);
            }
        }
        private void Engine_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Hero.MoveTo(new Vector2(e.X, e.Y));
            }
        }
        private void UpdateStartPoint(Vertex start)
        {
            this.start = start;
        }
        private void UpdateEndPoint(Vertex end)
        {
            this.end = end;
        }
        //private void UpdateMesh(Vertex start,Vertex end)
        //{
        //    input = new BoxWithHole().Generate(start, end);
        //    renderManager.Set(input);
        //    Triangulate();

        //    var r = Node.CreateListNodes(ref mesh);
        //    Searchmap a = new Searchmap { Nodes = r, StartNode = r[0], EndNode = r[1] };

        //    var e = a.GetShortestPathAstart(r);
        //    var points = e.Select(x => new TriangleNet.Geometry.Point(x.Point.X, x.Point.Y));
        //    List<Edge> edges = new List<Edge>();
        //    for (int i = 0; i < points.Count() - 1; i++)
        //    {
        //        edges.Add(new Edge(i, i + 1));
        //    }

        //    renderManager.Set(points.ToArray(), edges, false, true);
        //}
    }
}
