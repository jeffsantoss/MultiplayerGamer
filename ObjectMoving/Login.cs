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
using System.IO;
using System.Drawing.Imaging;

namespace ObjectMoving
{
    public partial class Login : Form
    {
        TcpClient _client;
        Random _rand;
        string code;

        public Login(TcpClient client)
        {
            _client = client;
            _rand = new Random();
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

            //if (this.captchaText.Text != code.ToString())
            //{
            //    MessageBox.Show("Captcha incorreto!");
            //    this.ClearFields(user);
            //    return;
            //}

            var stream = _client.GetStream();

            stream.Send(new Communication(MessageType.LoginMessage, user).ToJson());

            var jsonPlayers = stream.RecieveJson(_client);
            
            if (jsonPlayers.Cod == MessageType.AllUserMessage)
            {           
                stream.Send(new Communication(MessageType.LoginMessage, "Timer").ToJson());

                var ObjectTimer = stream.ReceiveObject(_client);

                var players = new List<Player>();

                foreach (var item in jsonPlayers.Object as object[])
                {
                    var obj = item as Dictionary<string, object>;

                    var player = Player.Criar((int)obj["Id"], (string)obj["Login"], (int)obj["X"], (int)obj["Y"], (Position)obj["D"], (bool)obj["IsLeader"]);

                    players.Add(player);
                }

                MessageBox.Show("Login OK");

                this.HideAndShow(new Game(_client, players, int.Parse(ObjectTimer as string)));

                return;
            }

            MessageBox.Show("Algo deu errado, tente novamente.");

            this.ClearFields(user);

            return;
        }

        // register
        private void button2_Click(object sender, EventArgs e)
        {
            this.HideAndShow(new Register(_client));
        }

        //captcha
        private void Login_Load(object sender, EventArgs e)
        {
            //CreateImage();
        }

        //private void CreateImage()
        //{
        //    string code = GetRandomText();

        //    Bitmap bitmap = new Bitmap(200, 50, PixelFormat.Format32bppArgb);
        //    Graphics g = Graphics.FromImage(bitmap);
        //    Pen pen = new Pen(Color.Yellow);
        //    Rectangle rect = new Rectangle(0, 0, 200, 50);

        //    SolidBrush b = new SolidBrush(Color.Black);
        //    SolidBrush White = new SolidBrush(Color.White);

        //    int counter = 0;

        //    g.DrawRectangle(pen, rect);
        //    g.FillRectangle(b, rect);

        //    for (int i = 0; i < code.Length; i++)
        //    {
        //        g.DrawString(code[i].ToString(), new Font("Georgia", 10 + _rand.Next(14, 18)), White, new PointF(10 + counter, 10));
        //        counter += 20;
        //    }

        //    DrawRandomLines(g);
        //    var diretorio = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

        //    if (File.Exists(diretorio))
        //    {

        //        try
        //        {
        //            File.Delete(diretorio);
        //            bitmap.Save(diretorio);

        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }

        //    }
        //    else
        //    {
        //        bitmap.Save(diretorio);

        //    }

        //    g.Dispose();
        //    bitmap.Dispose();
        //    pictureBox1.Image = Image.FromFile(diretorio);

        //}
        //private void DrawRandomLines(Graphics g)
        //{
        //    SolidBrush green = new SolidBrush(Color.Green);
        //    //For Creating Lines on The Captcha
        //    for (int i = 0; i < 20; i++)
        //    {
        //        g.DrawLines(new Pen(green, 2), GetRandomPoints());
        //    }

        //}
        //private Point[] GetRandomPoints()
        //{
        //    Point[] points = { new Point(_rand.Next(10, 150), _rand.Next(10, 150)), new Point(_rand.Next(10, 100), _rand.Next(10, 100)) };
        //    return points;
        //}

        //private string GetRandomText()
        //{
        //    StringBuilder randomText = new StringBuilder();

        //    if (String.IsNullOrEmpty(code))
        //    {
        //        string alphabets = "abcdefghijklmnopqrstuvwxyz1234567890";

        //        Random r = new Random();
        //        for (int j = 0; j <= 5; j++)
        //        {

        //            randomText.Append(alphabets[r.Next(alphabets.Length)]);
        //        }

        //        code = randomText.ToString();
        //    }

        //    return code;
        //}
    }
}