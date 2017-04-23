using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Communication
    {
        public MessageType Cod { get; set; }
        public object Object { get; set; }

        public Communication(MessageType cod, object obj)
        {
            this.Cod = cod;
            this.Object = obj;
        }

        public Communication()
        {
        }
    }
}
