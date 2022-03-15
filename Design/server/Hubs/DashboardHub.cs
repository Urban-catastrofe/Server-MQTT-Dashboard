using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SimmeMqqt.Hubs
{
    public class DashboardHub : Hub
    {
        public void SendRealtimeData(double Beschikbaarheid, double Prestaties, double Kwaliteit, double OEE)
        {
            Clients.All.SendAsync("RealtimeData", Beschikbaarheid, Prestaties, Kwaliteit, OEE);
        }
    }
}
