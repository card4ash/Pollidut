namespace Pollidut.Utils
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    [HubName("consumerVerificationCountHub")]
    public class ConsumerVerificationCountHub : Hub
    {
        public void Send()
        {
            Clients.All.increaseConsumerVerificationQuantity();
        }
    }

}