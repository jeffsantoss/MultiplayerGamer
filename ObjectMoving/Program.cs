using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ObjectMoving
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var client = new TcpClient("172.17.26.72" , 5556);

           var _stream = client.GetStream();

           _stream.Response(client);

           _stream.Send(new Communication(MessageType.SyncMessage,"Sync").ToJson());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login(client));
        }
    }
}
