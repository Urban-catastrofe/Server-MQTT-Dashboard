using Microsoft.AspNetCore.SignalR;
using SimmeMqqt.Controllers;
using SimmeMqqt.Hubs;
using SimmeMqqt.Model;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace SimmeMqqt.Services
{
    public class DashboardService
    {
        public readonly IHubContext<DashboardHub> _hubContext;
        public Timer aTimer;
        public DashboardService(IHubContext<DashboardHub> hubContext)
        {
            Console.WriteLine("helphub");
            _hubContext = hubContext;
            SetTimer();
        }

        public async Task SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += UpdateDashboard;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            aTimer.Start();
        }

        public void UpdateDashboard(Object source, ElapsedEventArgs e)
        {
            SetRealtimeData();
            SetUurlijkData();
            SetForeverData();
        }


        public void SetRealtimeData()
        {
            using (var context = new EntityFramework.MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .FirstOrDefault();
                int TotalFailureProcent;
                if (query.Failure == true)
                {
                    TotalFailureProcent = 0;
                }
                else
                {
                    TotalFailureProcent = 1;
                }

                float Beschikbaarheid = TotalFailureProcent;
                float Prestaties = ((float)query.TotalProduction / (float)query.IdealCyclus);
                float Kwaliteit = ((float)query.TotalGoodProduction / (float)query.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);
                if(query.Failure == true)
                {
                    Kwaliteit = 1;
                    Prestaties = 0;
                }

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;

                if(Beschikbaarheid == float.NaN)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties == float.NaN)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit == float.NaN)
                {
                    Kwaliteit = 0;
                }
                if (OEE == float.NaN)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("RealtimeData", Beschikbaarheid, Prestaties, Kwaliteit, OEE);
            }
        }

        public void SetUurlijkData()
        {
            using (var context = new EntityFramework.MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .Where(p => p.Timestamp > DateTime.UtcNow.AddHours(-1))
                                   .ToList();
                int TotalFailureProcent;

                var Machinedatas = new MQTTMachineData();

                Machinedatas.TotalProduction = query.Sum(p => p.TotalProduction);
                Machinedatas.TotalGoodProduction = query.Sum(p => p.TotalGoodProduction);
                Machinedatas.IdealCyclus = query.Sum(p => p.IdealCyclus);

                int FailureTrue = query.Where(c => c.Failure == true).Count();
                int FailureFalse = query.Where(c => c.Failure == false).Count();

                float Beschikbaarheid = (float)FailureTrue / (float)FailureFalse;
                if (FailureTrue == 0)
                {
                    Beschikbaarheid = 1;
                }else if (FailureFalse == 0)
                {
                    Beschikbaarheid = 0;
                }

                float Prestaties = ((float)Machinedatas.TotalProduction / (float)Machinedatas.IdealCyclus);
                float Kwaliteit = ((float)Machinedatas.TotalGoodProduction / (float)Machinedatas.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;

                if (Beschikbaarheid == float.NaN)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties == float.NaN)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit == float.NaN)
                {
                    Kwaliteit = 0;
                }
                if (OEE == float.NaN)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("UurlijkData", Beschikbaarheid, Prestaties, Kwaliteit, OEE);
            }

        }
        public void SetForeverData()
        {
            using (var context = new EntityFramework.MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .Where(p => p.Timestamp > DateTime.UtcNow.AddHours(-1))
                                   .ToList();
                int TotalFailureProcent;

                var Machinedatas = new MQTTMachineData();

                Machinedatas.TotalProduction = query.Sum(p => p.TotalProduction);
                Machinedatas.TotalGoodProduction = query.Sum(p => p.TotalGoodProduction);
                Machinedatas.IdealCyclus = query.Sum(p => p.IdealCyclus);

                int FailureTrue = query.Where(c => c.Failure == true).Count();
                int FailureFalse = query.Where(c => c.Failure == false).Count();

                float Beschikbaarheid = (float)FailureTrue / (float)FailureFalse;
                if (FailureTrue == 0)
                {
                    Beschikbaarheid = 1;
                }
                else if (FailureFalse == 0)
                {
                    Beschikbaarheid = 0;
                }

                float Prestaties = ((float)Machinedatas.TotalProduction / (float)Machinedatas.IdealCyclus);
                float Kwaliteit = ((float)Machinedatas.TotalGoodProduction / (float)Machinedatas.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;

                if (Beschikbaarheid == float.NaN)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties == float.NaN)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit == float.NaN)
                {
                    Kwaliteit = 0;
                }
                if (OEE == float.NaN)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("UurlijkData", Beschikbaarheid, Prestaties, Kwaliteit, OEE);
            }
        }
    }
}


