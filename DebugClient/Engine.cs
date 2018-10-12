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

namespace DebugClient
{
    public partial class Engine : Form
    {
        public static Engine Instance;
        private Timer time;
        private List<object> SceneObjects;
        private Hero Hero;

        public Engine()
        {
            InitializeComponent();
            SceneObjects = new List<object>();
            //Heros = new List<Hero>();
            Instance = this;
            this.DoubleBuffered = true;

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
    }
}
