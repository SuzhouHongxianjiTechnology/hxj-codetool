using SuperSocket.WebSocket.SubProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocketServer
{
    public class Add:SubCommandBase<CustomSession>
    {
        public override void ExecuteCommand(CustomSession session, SubRequestInfo requestInfo)
        {
             var paramArray= requestInfo.Body.Split('#');
        }
    }
}