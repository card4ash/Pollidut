namespace Pollidut.Utils
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;

    [HubName("totalMemoAmountHub")]
    public class TotalMemoAmountHub : Hub
    {
        public void Send(string amount)
        {
            Clients.All.increaseTotalMemoAmount(amount);
        }
    }

}