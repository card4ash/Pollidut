namespace Pollidut.Utils
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Hubs;
    using Pollidut.Models.SignalRModels;

    [HubName("positionReportingHub")]
    public class PositionReportingHub : Hub
    {
        public void Send(PositionReportingHub positionreport)
        {
            Clients.All.ReportPosition(positionreport);
        }
    }
}