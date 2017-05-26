using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Core.Models
{
    public enum Position
    {
        Left, Right, Up, Down
    }


    public class Player
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public Position D { get; set; }
        public Label Label { get; set; }
        public System.Windows.Forms.Timer Timer { get; set; }
        public bool IsLeader { get; set; }

        public Player()
        {
            this.Y = 50;
            this.X = 50;
        }

        public override bool Equals(object obj)
        {
            return ((Player)obj).Login.Equals(this.Login);
        }

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

        public bool Colidiu(Player player)
        {
            var myself = new Rectangle(this.X, this.Y, 64, 64);

            var enemy = new Rectangle(player.X, player.Y, 64, 64);

            return myself.IntersectsWith(enemy);
        }

        public static Player Criar(int id, string login,int x, int y, Position pos, bool leader)
        {
            var player = new Player
            {
                D = pos,
                Id = id,
                IsLeader = leader,
                Login = login,
                X = x,
                Y = y
            };

            if (x < 0)
            {
                player.X = 0;
            }
            else if (x > 620)
            {
                player.X = 620;
            }
            if (y < 0)
            {
                player.Y = 0;
            }
            else if (y > 520)
            {
                player.Y = 520;
            }

            return player;
        }
}
}
