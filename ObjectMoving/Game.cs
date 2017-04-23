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

namespace ObjectMoving
{
    public partial class Game : Form
    {

        List<Tuple<Player, Timer, Label>> _forms;
        GamerRepository _players;
        Player _myselfuser;
        TcpClient _client;
        NetworkStream _stream;
        //List<System.Threading.Thread> ThreadPlayers;
        System.Threading.Thread _thread;

        public Game(TcpClient client,List<Player> players)
        {
            InitializeComponent();

            this._myselfuser = players[0];
            this._client = client;
            this._players = new GamerRepository(players.Where(c => c != this._myselfuser).ToList());
            _stream = _client.GetStream();
            _thread = new System.Threading.Thread(ResponseThread);
            _thread.Start();

            PopulateForm();
        }

        public void PopulateForm()
        {
            _forms = new List<Tuple<Player, Timer, Label>>();

            foreach (var player in _players.Players)
            {
                var timer = new System.Windows.Forms.Timer(this.components);

                var label = new System.Windows.Forms.Label()
                {
                    AutoSize = true,
                    Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                    Size = new System.Drawing.Size(37, 16)
                };

                _forms.Add(new Tuple<Player, Timer, Label>(player, timer, label));

                this.Controls.Add(label);
            }

        }

        public void ResponseThread()
        {
            while (true)
            {
                var response = _stream.Response(_client);

                var obj = response.Object as Dictionary<string, object>;

                var player = new Player
                {
                    Coin = (int)obj["Coins"],
                    D = (Position)obj["D"],
                    Id = (int)obj["Id"],
                    Login = (string)obj["Login"],
                    X = (int)obj["X"],
                    Y = (int)obj["Y"]
                };

                if (response.Cod == MessageType.PlayerMessage)
                {
                    if (!_players.Players.Contains(player))
                    {
                        var timer = new System.Windows.Forms.Timer(this.components);

                        var label = new System.Windows.Forms.Label()
                        {
                            AutoSize = true,
                            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                            Size = new System.Drawing.Size(37, 16)
                        };

                        try
                        {
                            this.Controls.Add(label);
                            c _forms.Add(new Tuple<Player, Timer, Label>(player, timer, label));
                        }
                        catch
                        {
                        }
                        
                        _players.Players.Add(player);
                    }

                    var playerfind = _players.GetPlayer(player);
  
                    playerfind.D = player.D;
                }
                else if (response.Cod == MessageType.LogoutMessage)
                {
                    var tupletoremove = _forms.Find(a => a.Item1.Equals(player));

                    _forms.Remove(tupletoremove);

                    _players.Players.Remove(player);
                }

            }
        }

        private void FormView_Paint(object sender, PaintEventArgs e)
        {
            foreach (var player in _players.Players)
            {
                var tuple = _forms.SingleOrDefault(a => a.Item1.Equals(player));

                tuple.Item3.Text = player.Login;

                tuple.Item3.Location = new Point(player.X, player.Y);

                e.Graphics.DrawImage(new Bitmap("mushroom.png"), player.X, player.Y, 64, 64);
            }

            // myself.

            label1.Location = new Point(_myselfuser.X, _myselfuser.Y);

            label1.Text = _myselfuser.Login;

            e.Graphics.DrawImage(new Bitmap("mushroom.png"), _myselfuser.X, _myselfuser.Y, 64, 64);
        }

        private void tmrMoving_Tick(object sender, EventArgs e)
        {
            PlayerMoving(_myselfuser);

            this._players.Players.ForEach(p => PlayerMoving(p));

            Invalidate();
        }

        private void PlayerMoving(Player player)
        {
            if (player.X > 620)
            {
                player.X -= 10;
            }
            else if (player.X < 5)
            {
                player.X += 10;
            }

            if (player.Y > 520)
            {
                player.Y -= 10;
            }
            else if (player.Y < 10)
            {
                player.Y += 10;
            }

            if (player.D == Position.Right)
            {
                player.X += 10;
            }
            else if (player.D == Position.Left)
            {
                player.X -= 10;
            }
            else if (player.D == Position.Up)
            {
                player.Y -= 10;
            }
            else if (player.D == Position.Down)
            {
                player.Y += 10;
            }
        }

        private void FormView_KeyDown(object sender, KeyEventArgs e)
        {
            Moving(e);
        }

        private void Moving(KeyEventArgs e)
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



    }
}
