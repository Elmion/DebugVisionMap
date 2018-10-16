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
using TriangleNet.Geometry;
using TriangleNet;
using TriangleNet.Rendering;
using TriangleNet.Meshing;
using DebugClient.AStarSearch;
using System.Collections;

namespace DebugClient
{
    public partial class Engine : Form
    {
        public static Engine Instance;
        private Timer time;
        private List<object> SceneObjects;
        private Hero Hero;


        RenderManager renderManager;
        private TriangleNet.Mesh mesh;
        private IPolygon input;

        private Vertex start, end;

        public Engine()
        {
            InitializeComponent();
            SceneObjects = new List<object>();
            //Heros = new List<Hero>();
            Instance = this;
            this.DoubleBuffered = true;

            new TriangleNet.Geometry.Rectangle(10,10,10,10).
            //renderManager = new RenderManager();
            //IRenderControl control = new TriangleNet.Rendering.GDI.RenderControl();

            //InitializeRenderControl((Control)control);

            //renderManager.Initialize(control);

            //((TriangleNet.Rendering.GDI.RenderControl)control).OnRenderLeftClick += UpdateStartPoint;
            //((TriangleNet.Rendering.GDI.RenderControl)control).OnRenderRightClick += UpdateEndPoint;

            //this.start = new Vertex(-25, -25);
            //this.end = new Vertex(0, 35);
            //UpdateMesh(this.start, this.end);



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
            UpdateGraphic(e.Graphics);
        }
        private void UpdateGraphic(System.Drawing.Graphics g)
        {
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
            UpdateMesh(this.start, this.end);
        }
        private void UpdateEndPoint(Vertex end)
        {
            this.end = end;
            UpdateMesh(this.start, this.end);
        }
        private void UpdateMesh(Vertex start,Vertex end)
        {
            input = new BoxWithHole().Generate(start, end);
            renderManager.Set(input);
            Triangulate();

            var r = Node.CreateListNodes(ref mesh);
            Searchmap a = new Searchmap { Nodes = r, StartNode = r[0], EndNode = r[1] };

            var e = a.GetShortestPathAstart(r);
            var points = e.Select(x => new TriangleNet.Geometry.Point(x.Point.X, x.Point.Y));
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < points.Count() - 1; i++)
            {
                edges.Add(new Edge(i, i + 1));
            }

            renderManager.Set(points.ToArray(), edges, false, true);
        }
        private void InitializeRenderControl(Control control)
        {
            this.SuspendLayout();
            this.Controls.Add(control);

            var size = this.ClientRectangle;

            // Initialize control
            control.BackColor = System.Drawing.Color.Black;
            control.Dock = DockStyle.Fill;
            control.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            control.Location = new System.Drawing.Point(0, 0);
            control.Name = "renderControl1";
            control.Size = new Size(size.Width, size.Height);
            control.TabIndex = 0;
            control.Text = "renderControl1";

            this.ResumeLayout();
        }
        private void Triangulate()
        {
            if (input == null) return;

            var options = new ConstraintOptions();
            var quality = new QualityOptions();

            mesh = (TriangleNet.Mesh)input.Triangulate(options, quality);
            
            renderManager.Set(mesh, true);
        }
    }
    public class BoxWithHole : BaseGenerator
    {
        public BoxWithHole()
        {
            name = "Box with Hole";
            description = "";
            parameter = 3;

            descriptions[0] = "Points on box sides:";
            descriptions[1] = "Points on hole:";
            descriptions[2] = "Radius:";

            ranges[0] = new int[] { 5, 50 };
            ranges[1] = new int[] { 10, 200 };
            ranges[2] = new int[] { 5, 20 };
        }

        public override IPolygon Generate(Vertex start, Vertex end)
        {
            var input = new Polygon();

            input.Add(start);
            input.Add(end);
            input.Add(new Contour(CreateRectangle(new TriangleNet.Geometry.Rectangle(-20, -20, 40, 40), 1)), true);
            input.Add(new Contour(CreateRectangle(new TriangleNet.Geometry.Rectangle(-15, 30, 10, 10), 1)), true);
            input.Add(new Contour(CreateRectangle(new TriangleNet.Geometry.Rectangle(-50, -50, 100, 100), 1)), false);
            return input;
        }

    }
   

}
