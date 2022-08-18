using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSocket4Net;

namespace WebAppClient
{
    public  class SocketClient
    {

        public WebSocket websocket { get; set; }


        public SocketClient()
        {
            string url = "127.0.0.1:2017";
            //string authKey = AuthUrl.CreateAuthKey(url);
            // url = url + "/" + authKey;
            websocket = new WebSocket(url);
            websocket.Opened += websocket_Opened;
            websocket.Closed += websocket_Closed;
            websocket.MessageReceived += websocket_MessageReceived;
            websocket.Open();
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {

        }

        private void websocket_Closed(object sender, EventArgs e)
        {

        }

        private void websocket_Opened(object sender, EventArgs e)
        {

        }
 

    }
}