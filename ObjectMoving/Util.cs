using Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ObjectMoving
{
    public static class Util
    {
        // Forms
        public static void HideAndShow(this Form formToHide, Form formToShow) {
            formToHide.Hide();
            formToShow.Show();
        }

        public static void ClearFields(this Form form, object obj)
        {
            foreach (var item in form.Controls)
            {
                if (item is TextBox)
                {
                    (item as TextBox).Text = "";
                }
            }

            obj = null;
        }

        // JSON
        public static string ToJson(this object Object)
        {
            return new JavaScriptSerializer().Serialize(Object);
        }

        // Stream to send/recieve Json's
        public static void Send(this NetworkStream stream,string Json)
        {
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(Json);

            stream.Write(data, 0, data.Length);
        }

        public static Communication Response(this NetworkStream stream, TcpClient client)
        {
            Byte[] data = new Byte[client.ReceiveBufferSize];

            Int32 bytes = stream.Read(data, 0, data.Length);

            var response = System.Text.Encoding.UTF8.GetString(data, 0, bytes);

            var communication = new Communication();

            try
            {
                communication = new JavaScriptSerializer().Deserialize<Communication>(response);
            }
            catch {
                return null;
            }

            return communication;
        }
        
        // in test.
        public static Communication ResponseAsyc(this NetworkStream stream, TcpClient client)
        {
            Byte[] data = new Byte[client.ReceiveBufferSize];
            

            var response = "";

            void MyCallback(System.IAsyncResult ar)
            {
                // End pending asynchronous request.
                if (ar.IsCompleted)
                    stream.EndRead(ar);

                // Write result
                Debug.WriteLine(System.Text.Encoding.Default.GetString(data));

                response = System.Text.Encoding.Default.GetString(data);
            }

            stream.BeginRead(data, 0, data.Length, new System.AsyncCallback(MyCallback), null);

            return new JavaScriptSerializer().DeserializeObject(response) as Communication;
        }



    }
}
