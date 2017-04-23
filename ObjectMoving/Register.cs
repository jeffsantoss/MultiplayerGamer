using Core.Models;
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


namespace ObjectMoving
{
    public partial class Register : Form
    {
        TcpClient _client;
        ApplicationUser _user;
        
        public Register(TcpClient client)
        {
            InitializeComponent();
            _client = client;
        }

        // only for tests.

        private void button1_Click(object sender, EventArgs e)
        {
            _user = new ApplicationUser()
            {
                Email = this.textBox3.Text,
                Name = this.textBox4.Text,
                Password =  this.textBox2.Text,
                Login = this.textBox1.Text
            };

            var json = new Communication(MessageType.RegisterMessage, _user).ToJson();
                    
            //MessageBox.Show($"Send object {json} to server: {_client.Client.RemoteEndPoint}!");

            var stream = _client.GetStream();

            stream.Send(json);
            
            var jsonResponse = stream.Response(_client);

            //stream.Close();

            var User = jsonResponse.Object as Dictionary<string, object>;

            if (!User["Id"].Equals(0 as object))
            {
                MessageBox.Show($"Welcome {User["Name"]} your register Sucesss! Make your credence now..");
                this.HideAndShow(new Login(_client));
                return;
            }
            
            MessageBox.Show($"Login { User["Name"] } exists! ):");
            this.ClearFields(_user);
            return;
        }

     
    }
}
