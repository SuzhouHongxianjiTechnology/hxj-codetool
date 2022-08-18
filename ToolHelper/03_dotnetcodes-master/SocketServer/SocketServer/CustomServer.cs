using SuperSocket.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperSocket.SocketBase;
using SuperSocket.WebSocket.Protocol;
using SuperSocket.SocketBase.Config;

namespace SocketServer
{
    public class CustomServer:WebSocketServer<CustomSession>
    {
     
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        } 
        
    }
}