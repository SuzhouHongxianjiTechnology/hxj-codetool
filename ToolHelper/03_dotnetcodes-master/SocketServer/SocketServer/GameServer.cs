using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperSocket.SocketBase.Config;

namespace SocketServer
{
    public class GameServer:AppServer<GameSession>
    {
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }
        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
    }
}