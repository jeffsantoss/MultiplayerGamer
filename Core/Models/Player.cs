using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Core.Models
{
    public enum Position
    {
        Left, Right, Up, Down,Invalid
    }


    public class Player
    {

        public int Id { get; set; }
        public string Login { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Position D { get; set; }
        public int Coins { get; set; }
        public Label Label { get; set; }
        public System.Windows.Forms.Timer Timer { get; set; }

        public Player()
        {
            this.Y = 50;
            this.X = 50;
        }

        public override bool Equals(object obj)
        {
            return ((Player)obj).Login.Equals(this.Login);
        }

        /*
        public void StartMove()
        {
            Thread thread = new Thread(PlayerMove);
            thread.Start();
        }
        
        public void PlayerMove()
        {
            while (this.D < Position.Invalid)
            {
                Move();
            }
        }
        */

        public void Move()
        {
            if (this.D == Position.Right && this.X < 620)
            {
                this.X += 10;
            }
            else if (this.D == Position.Left && this.X > 5)
            {
                this.X -= 10;
            }
            else if (this.D == Position.Up && this.Y > 10)
            {
                this.Y -= 10;
            }
            else if (this.D == Position.Down && this.Y < 520)
            {
                this.Y += 10;
            }
        }
    }
}
