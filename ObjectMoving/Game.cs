using Core.Models;
using Core.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Threading;

namespace ObjectMoving
{   
    public partial class Game : Form
    {
        GamerRepository _repository;
        Player _myselfuser;
        TcpClient _client;
        NetworkStream _stream;
        Thread _thread;

        public Game(TcpClient client,List<Player> players)
        {
            InitializeComponent();

            _myselfuser = players[0];
            _client = client;
            _repository = new GamerRepository(players.Where(c => c != this._myselfuser).ToList());

            _stream = _client.GetStream();

            _thread = new Thread(ResponseThread);
            _thread.Start();

            PopulateForm();
        }

        public void PopulateForm()
        {
            _repository.Players.ForEach(p => CreateFormsToPlayer(p));
        }

        public void ResponseThread()
        {
            while (true)
            {
                var response = _stream.Response(_client);

                if (response == null)
                    continue;

                var obj = response.Object as Dictionary<string, object>;

                var player = new Player
                {
                    Coins = (int)obj["Coins"],
                    D = (Position)obj["D"],
                    Id = (int)obj["Id"],
                    Login = (string)obj["Login"],
                    X = (int)obj["X"],
                    Y = (int)obj["Y"]
                };

                if (response.Cod == MessageType.PlayerMessage)
                {
                    if (!_repository.Players.Any(p => p.Equals(player)))
                    {
                        CreateFormsToPlayer(player);
                        _repository.Players.Add(player);

                        Console.WriteLine($"User {player.Login} join in game at {DateTime.Now.ToString("dd/MMM/yyyy - HH:mm")}");
                    }
                    else
                    {
                        var playerfind = _repository.GetPlayer(player);

                        playerfind.D = player.D;
                    }
                }
                else if (response.Cod == MessageType.LogoutMessage)
                {
                    var playerfind = _repository.GetPlayer(player);

                    if (playerfind.Label.InvokeRequired)
                        playerfind.Label.BeginInvoke((MethodInvoker) delegate {
                            playerfind.Label.Text = "";                    
                            });

                    _repository.Players.Remove(player);

                    Console.WriteLine($"User {player.Login} left the game at {DateTime.Now.ToString("dd/MMM/yyyy - HH:mm")}");
                }

            }
        }

        private void FormView_Paint(object sender, PaintEventArgs e)
        {     
            foreach (var player in _repository.Players)
            {
                if (!Controls.Contains(player.Label))
                {
                    Controls.Add(player.Label);
                }

               player.Label.Location = new Point(player.X, player.Y);

               e.Graphics.DrawImage(new Bitmap("mushroom.png"), player.X, player.Y, 64, 64);
            }

            label1.Location = new Point(_myselfuser.X, _myselfuser.Y);

            label1.Text = _myselfuser.Login;

            e.Graphics.DrawImage(new Bitmap("mushroom.png"), _myselfuser.X, _myselfuser.Y, 64, 64);
        }


        private void tmrMoving_Tick(object sender, EventArgs e)
        {
            _myselfuser.Move();

            _repository.Players.ForEach(p => p.Move());

            Invalidate();
        }
        

        private void FormView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                _myselfuser.D = Position.Left;
            }
            else if (e.KeyCode == Keys.Right)
            {
                _myselfuser.D = Position.Right;
            }
            else if (e.KeyCode == Keys.Up)
            {
                _myselfuser.D = Position.Up;
            }
            else if (e.KeyCode == Keys.Down)
            {
                _myselfuser.D = Position.Down;
            }

            _stream.Send(new Communication(MessageType.PlayerMessage, _myselfuser).ToJson());
        }

        private void GameClosing(object sender, FormClosingEventArgs e)
        {
            _stream.Send(new Communication(MessageType.LogoutMessage, _myselfuser).ToJson());

            _client.Close();

            Environment.Exit(0);
        }

        private void CreateFormsToPlayer(Player player)
        {
            player.Timer = new System.Windows.Forms.Timer(this.components);

            player.Label = new Label()
            {
                AutoSize = true,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                ForeColor = System.Drawing.Color.Red,
                Size = new Size(85, 18),
                Text = player.Login
            };
        }
    }
}
