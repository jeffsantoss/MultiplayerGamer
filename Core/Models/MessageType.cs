using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public enum MessageType
    {
        RegisterMessage = 0,
        LoginMessage = 1,
        PlayerMessage = 2,
        AllUserMessage = 3,
        LogoutMessage = 4,
        SyncMessage = 5
    }
}
