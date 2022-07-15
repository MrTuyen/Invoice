using Microsoft.ServiceModel.WebSockets;
using Microsoft.Web.WebSockets;
using SPA_Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Invoice.API.Controllers
{
    [RoutePrefix("ws")]
    public class WebSocketController : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                var noteHandler = new SignSocketHandler();
                HttpContext.Current.AcceptWebSocketRequest(noteHandler);
            }

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        internal class SignSocketHandler : WebSocketHandler
        {
            private static WebSocketCollection connections = new WebSocketCollection();
            public override void OnClose()
            {
                connections.Remove(this);
            }

            public override void OnError()
            {
                connections.Remove(this);
            }

            public override void OnOpen()
            {
                connections.Add(this);
            }

            public override void OnMessage(byte[] message)
            {
            }

            public override void OnMessage(string message)
            {
                foreach(var con in connections)
                {
                    con.Send(message);
                }
            }
        }
    }
}