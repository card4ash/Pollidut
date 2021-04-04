namespace Pollidut.Utils
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    [HubName("consumerObservationHub")]
    public class ConsumerObservationHub : Hub
    {
        public void Send(int consumerid)
        {
            Clients.All.ConsumerObserved(consumerid);
        }
    }
}