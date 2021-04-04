namespace Pollidut.Utils
{
    using System.Collections.Generic;

    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    [HubName("consumersCountHub")]
    public class ConsumersCountHub : Hub
    {
        public void Send()
        {
            Clients.All.increaseConsumerQuantity();
            // Call the increaseConsumerQuantity method to update clients.
            //var context = GlobalHost.ConnectionManager.GetHubContext<UserActivityHub>();
            //context.Clients.All.increaseConsumerQuantity();
        }
    }
}