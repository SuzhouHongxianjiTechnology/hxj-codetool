using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket.SubProtocol;

namespace SocketServer
{
    public class CustomSession:WebSocketSession<CustomSession>
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        protected override void OnSessionStarted()
        {
            var name = Path;

            if(!string.IsNullOrEmpty(name))
            {
                name = Path.TrimStart('/');
            } 
            if(string.IsNullOrEmpty(name))                
            base.Close();
            base.OnSessionStarted();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }
        protected override void HandleException(Exception e)
        {
            base.HandleException(e);
        }

        protected override void HandleUnknownCommand(SubRequestInfo requestInfo)
        {
            base.HandleUnknownCommand(requestInfo);
        }
    }

   
}