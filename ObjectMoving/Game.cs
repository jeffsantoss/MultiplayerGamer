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
        Thread _threadtime;
        int _secondsGame;

        public Game(TcpClient client, List<Player> players, int secondsByServer)
        {
            InitializeComponent();

            _secondsGame = secondsByServer;
            _client = client;
            _stream = _client.GetStream();

            _myselfuser = players[0];
            _repository = new GamerRepository(players.Where(c => c != this._myselfuser).ToList());

            _thread = new Thread(ResponseThread);
            _thread.Start();

            _threadtime = new Thread(CountTime);
            _threadtime.Start();
            
            PopulateForm();
            
            Text = $"Trabalho Sistemas Distribuídos: Player {_myselfuser.Login}";
        }

        private void CountTime()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                _secondsGame--;

                if (this.labelTime.InvokeRequired)
                    this.labelTime.BeginInvoke((MethodInvoker)delegate
                    {
                        this.labelTime.Text = $"Tempo: {_secondsGame}";
                    });

                if (_secondsGame <= 0)
                {
                    MessageBox.Show("Tempo do servidor esgotado");
                    _stream.Send(new Communication(MessageType.LogoutMessage, _myselfuser).ToJson());
                    _client.Close();
                    Environment.Exit(0);
                }
            }
        }

        public void PopulateForm()
        {
            _repository.Players.ForEach(p => CreateFormsToPlayer(p));
        }

        public void ResponseThread()
        {
            while (true)
            {
                var replyMessage = _stream.RecieveJson(_client);

                if (replyMessage == null)
                    continue;

                var obj = replyMessage.Object as Dictionary<string, object>;

                var player = Player.Criar((int)obj["Id"], (string)obj["Login"], 
                                          (int)obj["X"], (int)obj["Y"], 
                                          (Position)obj["D"], (bool)obj["IsLeader"]);
                       
                if (replyMessage.Cod == MessageType.PlayerMessage)
                {
                    if (player.Equals(_myselfuser))
                    {
                        _myselfuser.IsLeader = true;

                        MessageBox.Show("Você se tornou o lider");
                    }
                    else
                    {
                        if (!_repository.Players.Any(p => p.Equals(player)))
                        {
                            CreateFormsToPlayer(player);
                            _repository.Players.Add(player);


                            if (player.Label.InvokeRequired)
                                player.Label.BeginInvoke((MethodInvoker)delegate
                                {
                                    this.TextAll.Text = $"Player: {player.Login} entrou no jogo";
                                });
                        }
                        else
                        {
                            var playerfind = _repository.GetPlayer(player);

                            playerfind.D = player.D;
                        }
                    }
                }
                else if (replyMessage.Cod == MessageType.LogoutMessage)
                {
                    var playerfind = _repository.GetPlayer(player);

                    if (playerfind != null)
                    {
                        if (playerfind.Label.InvokeRequired)
                            playerfind.Label.BeginInvoke((MethodInvoker)delegate
                            {
                                playerfind.Label.Text = "";
                            });

                        _repository.Players.Remove(player);

                        if (TextAll.InvokeRequired)
                            TextAll.BeginInvoke((MethodInvoker)delegate
                            {
                                this.TextAll.Text = $"Player: {player.Login.ToUpper()} saiu do jogo";
                            });
                    }

                }

            }
        }


        private void FormView_Paint(object sender, PaintEventArgs e)
        {
            Player playerdead = null;

            var lider = _repository.Players.FirstOrDefault(p => p.IsLeader);

            this.TextLider.Text = lider != null ? $"Lider: {lider.Login} " : "Lider: Você";

            for (int i = _repository.Players.Count - 1; i >= 0; i--)
            {
                var player = _repository.Players.ElementAt(i);

                if (!Controls.Contains(player.Label))
                {
                    Controls.Add(player.Label);
                }
                if (_myselfuser.Colidiu(player) && _myselfuser.IsLeader)
                {
                    this.TextAll.Text = $"Você derrotou o player: {player.Login}";
                    playerdead = player;
                    break;
                }
                else if (_myselfuser.Colidiu(player) && player.IsLeader)
                {
                    playerdead = player;                 
                    break;
                }

                player.Label.Location = new Point(player.X, player.Y);
                e.Graphics.DrawImage(new Bitmap("mushroom.png"), player.X, player.Y, 64, 64);
            }

            if (playerdead != null)
            {
                var playerfind = _repository.GetPlayer(playerdead);
                
                if (playerdead.IsLeader)
                {
                    _stream.Send(new Communication(MessageType.LogoutMessage, _myselfuser).ToJson());
                    _client.Close();                    
                    Environment.Exit(0);
                }
                else
                {
                    _repository.Players.Remove(playerfind);
                    playerdead.Label.Text = "";
                    playerdead.Label = null;
                    playerdead.Timer = null;
                    _stream.Send(new Communication(MessageType.LogoutMessage, playerfind).ToJson());
                }
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
                _stream.Send(new Communication(MessageType.PlayerMessage, _myselfuser).ToJson());
            }
            else if (e.KeyCode == Keys.Right)
            {
                _myselfuser.D = Position.Right;
                _stream.Send(new Communication(MessageType.PlayerMessage, _myselfuser).ToJson());
            }
            else if (e.KeyCode == Keys.Up)
            {
                _myselfuser.D = Position.Up;
                _stream.Send(new Communication(MessageType.PlayerMessage, _myselfuser).ToJson());
            }
            else if (e.KeyCode == Keys.Down)
            {
                _myselfuser.D = Position.Down;
                _stream.Send(new Communication(MessageType.PlayerMessage, _myselfuser).ToJson());
            }
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
            player.Timer.Interval = 5000;
            player.Label = new Label()
            {
                AutoSize = true,
                Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                ForeColor = System.Drawing.Color.Red,
                Size = new Size(85, 18),
                Text = player.Login
            };
        }

        private void Game_Load(object sender, EventArgs e)
        {

        }
    }
}
