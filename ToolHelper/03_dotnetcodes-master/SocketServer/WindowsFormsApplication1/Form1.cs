/*
 源码己托管:http://git.oschina.net/kuiyu/dotnetcodes
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebSocket4Net;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        WebSocket websocket;
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        void websocket_Opened(object sender, EventArgs e)
        {

            websocket.Send("winform 客户端上线"); //向服务端发消息

        }
        void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {

            this.label4.Invoke(new EventHandler(ShowMessage), e.Message);

        }

       

        void websocket_Closed(object sender, EventArgs e)
        {



        }



       



        void ShowMessage(object sender, EventArgs e)
        {

           this.label4.Text = sender.ToString();

        }

       

        //连接服务器
        private void button2_Click(object sender, EventArgs e)
        {
            string ip = "ws://"+ this.textBox3.Text;
            
            

            websocket = new WebSocket(ip);

            websocket.Opened += websocket_Opened;

            websocket.Closed += websocket_Closed;

            websocket.MessageReceived += websocket_MessageReceived;

            

            websocket.Open();
        }

        //发送消息
        private void button3_Click(object sender, EventArgs e)
        {
            websocket.Send(this.textBox4.Text);
        }
    }
}
