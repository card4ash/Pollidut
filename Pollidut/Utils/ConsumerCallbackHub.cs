namespace Pollidut.Utils
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    [HubName("consumerCallbackHub")]
    public class ConsumerCallbackHub : Hub
    {
        public void Send(int consumerid)
        {
            Clients.All.ConsumerCallbacked(consumerid);
        }
    }
}