using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace lawzand
{
    public class ChatHub : Hub
    {
        public static int _userCount = 0;
     

        public void Send(string name, string message)
        {
            Clients.All.addNewMessageToPage(name, message);
        }

        public void Send(int count)
        {
            // Call the addNewMessageToPage method to update clients.
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.updateUsersOnlineCount(_userCount);
        }
        public override Task OnConnected()
        {
            _userCount++;
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.online(_userCount);
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            _userCount++;
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.online(_userCount);
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _userCount--;
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.online(_userCount);
            return base.OnDisconnected(stopCalled);
        }
        

    }
}