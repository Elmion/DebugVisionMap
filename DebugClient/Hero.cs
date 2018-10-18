using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DebugClient
{
    class Hero : SceneObject, IMove
    {
        public int Speed { get; set; }

        private const int GRADIUS = 20;
        private Vector2 NewPosition;

        public Hero() : base()
        {
            Position = new Vector2(3, 3);
            NewPosition = Position;
        }

        public override void UpdateLogic()
        {
            if (Position != NewPosition)
            {
                Position = Vector2.MoveTowards(Position, NewPosition, Speed);
            }   
        }
        public override void Draw(System.Drawing.Graphics g)
        {
            g.FillPie(Brushes.Blue, new Rectangle((int)Position.x - GRADIUS/2, (int)Position.y - GRADIUS/2, GRADIUS, GRADIUS), 0, 360);
        }

        public void Move(Vector2 Position)
        {
            this.Position = Position;
            NewPosition = Position;
        }
        public void MoveTo(Vector2 toPosition)
        {
            NewPosition = toPosition;
        }

    }
}
