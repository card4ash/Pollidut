namespace Pollidut.Utils
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
    using Pollidut.Models.SignalRModels;
    
    [HubName("consumerHub")]
    public class ConsumerHub : Hub
    {
        public void Send(Consumer consumer)
        {
            Clients.All.addConsumer(consumer);
        }
    }
}