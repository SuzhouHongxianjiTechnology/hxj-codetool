using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperSocket.SocketBase.Protocol;

namespace SocketServer
{
    public class GameSession:AppSession<GameSession>
    {
        public long UserId { get; set; }

        public long  ClientId { get; set; }

        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }

        protected override void HandleException(Exception e)
        {
            base.HandleException(e);
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }
    }
}