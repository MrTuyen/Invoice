using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SPA_Invoice.Common
{
    public class SignlRConf:Hub
    {
        public override Task OnConnected()
        {
            var name = Context.QueryString["username"];
            if(!string.IsNullOrEmpty(name))
                Groups.Add(Context.ConnectionId, name);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var name = Context.QueryString["username"];
            if (!string.IsNullOrEmpty(name))
                Groups.Remove(Context.ConnectionId, name);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var name = Context.QueryString["username"];
            if (!string.IsNullOrEmpty(name))
                Groups.Add(Context.ConnectionId, name);

            return base.OnReconnected();
        }

        public void BroadcastMessageToAll(string message)
        {

            Clients.All.newMessageReceived(message);
        }

        public void JoinAGroup(string group)
        {
            Groups.Add(Context.ConnectionId, group);
        }

        public void RemoveFromAGroup(string group)
        {
            Groups.Remove(Context.ConnectionId, group);
        }

        #region Invoice
        // Lắng nghe sự kiện từ InvoiceController
        public void BroadcastToGroup(string message, string group)
        {
            Clients.Group(group).newMessageReceived(message);
        }

        // Lắng nghe sự kiện từ HomeController
        public void BroadcastToGroupSignInvoice(string message, string group)
        {
            Clients.Group(group).newMessageReceivedSignInvoice(message);
        }
        #endregion

        #region Customer
        // Lắng nghe sự kiện từ CustomerController
        public void BroadcastToGroupCustomer(string message, string group)
        {
            Clients.Group(group).newMessageReceivedCustomer(message);
        }
        #endregion

        #region Product
        // Lắng nghe sự kiện từ ProductController
        public void BroadcastToGroupProduct(string message, string group)
        {
            Clients.Group(group).newMessageReceivedProduct(message);
        }
        #endregion
    }
}