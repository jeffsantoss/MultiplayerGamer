using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence
{
    public class GamerRepository
    {
        public List<Player> Players { get; set; }

        public GamerRepository(List<Player> Players)
        {
            this.Players = Players;
        }

        public Player GetPlayer(Player player){
            return this.Players.SingleOrDefault(a => a.Equals(player));
        }

        public void Add(Player player)
        {
            this.Players.Add(player);
        }

        

    }
}
