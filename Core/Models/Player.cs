using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public int Coin { get; set; }
        public Label Label { get; set; }
        public Timer Timer { get; set; }

        public Player()
        {
            this.Y = 50;
            this.X = 50;
        }

        public override bool Equals(object obj)
        {
            return ((Player)obj).Login.Equals(this.Login);
        }

    }
}
