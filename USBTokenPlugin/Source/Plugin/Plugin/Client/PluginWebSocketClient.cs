using System.Text;
using WebSockets.Client;
using WebSockets.Common;

namespace WebSocketsCmd.Client
{
    class PluginWebSocketClient : WebSocketClient
    {
        public PluginWebSocketClient(bool noDelay, IWebSocketLogger logger) : base(noDelay, logger)
        {
            
        }
       // @override
        public void Send(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            Send(WebSocketOpCode.TextFrame, buffer);
        }
    }
}
