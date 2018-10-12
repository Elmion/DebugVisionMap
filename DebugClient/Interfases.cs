using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DebugClient
{
    interface IMove
    {
        int Speed { get; set; }
        void Move(Vector2 Position);
        void MoveTo(Vector2 toPosition);
    }
    interface IUpdate
    {
        void Draw(System.Drawing.Graphics g);
        void UpdateLogic();
    }
}
