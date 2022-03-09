using Microsoft.AspNetCore.Mvc;
using SimmeMqqt.EntityFramework;
using SimmeMqqt.Model;
using SimmeMqqt.Pages;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using SimmeMqqt.Hubs;
using System.Timers;
using System;

namespace SimmeMqqt.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        public readonly IHubContext<DashboardHub> _hubContext;
        public Timer aTimer;
        public DashboardController(IHubContext<DashboardHub> hubContext)
        {
            _hubContext = hubContext;
            SetTimer();
        }
        private void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new Timer(500);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += UpdateDashboard;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public void UpdateDashboard(Object source, ElapsedEventArgs e)
        {
            SetRealtimeData();
        }


        public void SetRealtimeData()
        {
            using (var context = new MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .FirstOrDefault();
                int TotalFailureProcent;
                if (query.Failure == true)
                {
                    TotalFailureProcent = 100;
                }
                else
                {
                    TotalFailureProcent = 0;
                }

                var Beschikbaarheid = TotalFailureProcent;
                var Prestaties = (double)query.TotalProduction / (double)query.IdealCyclus;
                var Kwaliteit = (double)query.TotalGoodProduction / (double)query.TotalProduction;
                var OEE = (double)Beschikbaarheid * (double)Prestaties * (double)Kwaliteit;

                _hubContext.Clients.All.SendAsync("RealtimeData", Beschikbaarheid, Prestaties, Kwaliteit, OEE);
            }
        }


    }
}
