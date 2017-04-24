using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using Core.Models;

namespace ObjectMoving
{
    public partial class Login : Form
    {
        TcpClient _client;

        public Login(TcpClient client)
        {
            this._client = client;
            InitializeComponent();
        }

        // start
        private void button1_Click(object sender, EventArgs e)
        {

            var user = new ApplicationUser()
            {
                Login = this.textBox1.Text,
                Password = this.textBox2.Text
            };


            var json = new Communication(MessageType.LoginMessage, user).ToJson();

            var stream = _client.GetStream();

            stream.Send(json);

            var jsonresponse = stream.Response(_client);
           
            if (jsonresponse.Cod == MessageType.AllUserMessage)
            {
                var players = new List<Player>();

                foreach (var item in jsonresponse.Object as object[])
                {
                    var obj = item as Dictionary<string, object>;

                    players.Add(
                    new Player
                    {
                        Coins = (int) obj["Coins"],
                        D = (Position) obj["D"],
                        Id = (int) obj["Id"],
                        Login = (string) obj["Login"],
                        X = (int) obj["X"],
                        Y = (int) obj["Y"]
                    }
                    );
                 }

                MessageBox.Show("Login Sucesss!");

                this.HideAndShow(new Game(_client, players));

                return;
            }

            MessageBox.Show("Somenthing went wrong. Try again.");

            this.ClearFields(user);

            return;
        }

        // register
        private void button2_Click(object sender, EventArgs e)
        {
            this.HideAndShow(new Register(_client));
        }


    }
}