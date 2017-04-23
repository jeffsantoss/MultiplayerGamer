using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Game
    {
        List<Player> Players { get; set; }

        public Game()
        {
            Players = new List<Player>();
        }
    }
}
