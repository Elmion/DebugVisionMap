using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DebugClient
{
    abstract class SceneObject: IUpdate
    {
       public Vector2 Position { get; set; }
       public SceneObject()
        {
        }
        public virtual void Draw(System.Drawing.Graphics g)
        {
        }

        public virtual void UpdateLogic()
        {
        }
    }
}
