﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
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
        private HubConnection hubConnection;
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
            SetDailyData();
            SetMaandelijksData();
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
                int Totaltrueorfalse;
                int TotalBreakTime;
                if (query.Failure == true)
                {
                    TotalFailureProcent = 0;
                    Totaltrueorfalse = 1;
                }
                else
                {
                    TotalFailureProcent = 1;
                    Totaltrueorfalse = 0;
                }
                if (query.Break == true)
                {
                    TotalBreakTime = 1;
                }
                else
                {
                    TotalBreakTime = 0;
                }

                float Beschikbaarheid = TotalFailureProcent;
                float Prestaties = ((float)query.TotalProduction / (float)query.IdealCyclus);
                float Kwaliteit = ((float)query.TotalGoodProduction / (float)query.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);
                if (query.Failure == true)
                {
                    Kwaliteit = 1;
                    Prestaties = 0;
                }

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;

                if (Beschikbaarheid != Beschikbaarheid)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties != Prestaties)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit != Kwaliteit)
                {
                    Kwaliteit = 0;
                }
                if (OEE != OEE)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("RealtimeData", Beschikbaarheid, Prestaties, Kwaliteit, OEE, TotalBreakTime, Totaltrueorfalse);
            }
        }
        public void SetUurlijkData()
        {
            using (var context = new EntityFramework.MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .Where(p => p.Timestamp >= DateTime.UtcNow.AddHours(-1))
                                   .ToList();

                int QueryBreak = query.Where(c => c.Break == true).Count();
                var Data = query;
                query.RemoveAll(x => x.Break == true);

                var Machinedatas = new MQTTMachineData();

                Machinedatas.TotalProduction = query.Sum(p => p.TotalProduction);
                Machinedatas.TotalGoodProduction = query.Sum(p => p.TotalGoodProduction);
                var GoodData = query.Where(s => s.Failure == false);
                Machinedatas.IdealCyclus = GoodData.Sum(p => p.IdealCyclus);
                int FailureTrue = query.Where(c => c.Failure == true).Count();
                int FailureFalse = query.Where(c => c.Failure == true || c.Failure == false).Count();

                float Beschikbaarheid = (float)FailureTrue / (float)FailureFalse;
                Beschikbaarheid = 1 - Beschikbaarheid;
                if (FailureTrue == 0)
                {
                    Beschikbaarheid = 1;
                }
                float Prestaties = ((float)Machinedatas.TotalProduction / (float)Machinedatas.IdealCyclus);
                float Kwaliteit = ((float)Machinedatas.TotalGoodProduction / (float)Machinedatas.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;
                var PauzeCurrent = 0;
                var FailureCurrent = 0;
                var id = 0;
                var Bool = true;
                try
                {
                    while (Bool == true)
                    {
                        var Condition = query[id];

                        if (Condition.Failure == false)
                        {
                            FailureCurrent = id;
                            Bool = false;
                        }
                        else
                        {
                            id++;
                        }
                    }

                    id = 0;
                    Bool = true;

                    while (Bool == true)
                    {
                        var Condition = Data[id];

                        if (Condition.Break == false)
                        {
                            PauzeCurrent = id;
                            Bool = false;
                            if(id >= 5)
                            {

                            } else if (id != 0)
                            {

                            }
                        }
                        else
                        {
                            id++;
                        }
                    }
                }
                catch (Exception e)
                {

                }

                if (Beschikbaarheid != Beschikbaarheid)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties != Prestaties)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit != Kwaliteit)
                {
                    Kwaliteit = 0;
                }
                if (OEE != OEE)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("UurlijkData", Beschikbaarheid, Prestaties, Kwaliteit, OEE, QueryBreak, FailureTrue, PauzeCurrent, FailureCurrent);
            }
        }
        public void SetDailyData()
        {
            using (var context = new EntityFramework.MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .Where(p => p.Timestamp >= DateTime.UtcNow.AddDays(-1))
                                   .ToList();
                int QueryBreak = query.Where(c => c.Break == true).Count();
                query.RemoveAll(x => x.Break == true);

                var Machinedatas = new MQTTMachineData();

                Machinedatas.TotalProduction = query.Sum(p => p.TotalProduction);
                Machinedatas.TotalGoodProduction = query.Sum(p => p.TotalGoodProduction);
                var GoodData = query.Where(s => s.Failure == false);
                Machinedatas.IdealCyclus = GoodData.Sum(p => p.IdealCyclus);
                int FailureTrue = query.Where(c => c.Failure == true).Count();
                int FailureFalse = query.Where(c => c.Failure == true || c.Failure == false).Count();

                float Beschikbaarheid = (float)FailureTrue / (float)FailureFalse;
                Beschikbaarheid = 1 - Beschikbaarheid;
                if (FailureTrue == 0)
                {
                    Beschikbaarheid = 1;
                }

                float Prestaties = ((float)Machinedatas.TotalProduction / (float)Machinedatas.IdealCyclus);
                float Kwaliteit = ((float)Machinedatas.TotalGoodProduction / (float)Machinedatas.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;

                if (Beschikbaarheid != Beschikbaarheid)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties != Prestaties)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit != Kwaliteit)
                {
                    Kwaliteit = 0;
                }
                if (OEE != OEE)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("DailyData", Beschikbaarheid, Prestaties, Kwaliteit, OEE, QueryBreak, FailureTrue, Machinedatas);
            }
        }

        public void SetMaandelijksData()
        {
            using (var context = new EntityFramework.MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .Where(p => p.Timestamp >= DateTime.UtcNow.AddMonths(-1))
                                   .ToList();
                int QueryBreak = query.Where(c => c.Break == true).Count();
                query.RemoveAll(x => x.Break == true);

                var Machinedatas = new MQTTMachineData();

                Machinedatas.TotalProduction = query.Sum(p => p.TotalProduction);
                Machinedatas.TotalGoodProduction = query.Sum(p => p.TotalGoodProduction);
                var GoodData = query.Where(s => s.Failure == false);
                Machinedatas.IdealCyclus = GoodData.Sum(p => p.IdealCyclus);
                int FailureTrue = query.Where(c => c.Failure == true).Count();
                int FailureFalse = query.Where(c => c.Failure == true || c.Failure == false).Count();

                float Beschikbaarheid = (float)FailureTrue / (float)FailureFalse;
                Beschikbaarheid = 1 - Beschikbaarheid;
                if (FailureTrue == 0)
                {
                    Beschikbaarheid = 1;
                }

                float Prestaties = ((float)Machinedatas.TotalProduction / (float)Machinedatas.IdealCyclus);
                float Kwaliteit = ((float)Machinedatas.TotalGoodProduction / (float)Machinedatas.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;

                if (Beschikbaarheid != Beschikbaarheid)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties != Prestaties)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit != Kwaliteit)
                {
                    Kwaliteit = 0;
                }
                if (OEE != OEE)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("MaandelijksData", Beschikbaarheid, Prestaties, Kwaliteit, OEE, QueryBreak, FailureTrue, Machinedatas);
            }
        }

        public void SetForeverData()
        {
            using (var context = new EntityFramework.MachineData())
            {
                var query = context.MachineDatas
                                   .OrderByDescending(p => p.Id)
                                   .ToList();
                int QueryBreak = query.Where(c => c.Break == true).Count();
                query.RemoveAll(x => x.Break == true);

                var Machinedatas = new MQTTMachineData();

                Machinedatas.TotalProduction = query.Sum(p => p.TotalProduction);
                Machinedatas.TotalGoodProduction = query.Sum(p => p.TotalGoodProduction);
                var GoodData = query.Where(s => s.Failure == false);
                Machinedatas.IdealCyclus = GoodData.Sum(p => p.IdealCyclus);
                int FailureTrue = query.Where(c => c.Failure == true).Count();
                int FailureTotal = query.Where(c => c.Failure == true || c.Failure == false).Count();

                float Beschikbaarheid = (float)FailureTrue / (float)FailureTotal;
                Beschikbaarheid = 1 - Beschikbaarheid;
                if (FailureTrue == 0)
                {
                    Beschikbaarheid = 1;
                }

                var FailureKort = 0;
                var FailureLang = 0;
                var FailureKortMin = 0;
                var FailureLangMin = 0;
                var StoringTrue = 0;
                var id = 0;
                var Bool = true;

                while (Bool == true)
                {
                    try
                    {
                        var Condition = query[id];
                        if (Condition.Failure == true)
                        {
                            id++;
                            StoringTrue++;
                        }
                        else if (Condition.Failure == false)
                        {
                            if (StoringTrue >= 5)
                            {
                                FailureLang++;
                                FailureLangMin = FailureLangMin + StoringTrue;
                                StoringTrue = 0;
                            }
                            else if (StoringTrue != 0)
                            {
                                FailureKort++;
                                FailureKortMin = FailureKortMin + StoringTrue;
                                StoringTrue = 0;
                            }

                            id++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Bool = false;
                    }
                }


                float Prestaties = ((float)Machinedatas.TotalProduction / (float)Machinedatas.IdealCyclus);
                float Kwaliteit = ((float)Machinedatas.TotalGoodProduction / (float)Machinedatas.TotalProduction);
                float OEE = ((float)Beschikbaarheid * (float)Prestaties * (float)Kwaliteit);

                //DashboardHub.SendRealtimeData(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
                Beschikbaarheid = Beschikbaarheid * 100;
                Prestaties = Prestaties * 100;
                Kwaliteit = Kwaliteit * 100;
                OEE = OEE * 100;

                if (Beschikbaarheid != Beschikbaarheid)
                {
                    Beschikbaarheid = 0;
                }
                if (Prestaties != Prestaties)
                {
                    Prestaties = 0;
                }
                if (Kwaliteit != Kwaliteit)
                {
                    Kwaliteit = 0;
                }
                if (OEE != OEE)
                {
                    OEE = 0;
                }
                _hubContext.Clients.All.SendAsync("foreverData", Beschikbaarheid, Prestaties, Kwaliteit, OEE, QueryBreak, FailureTrue, Machinedatas);
                _hubContext.Clients.All.SendAsync("foreverData1", FailureLang, FailureKort, FailureLangMin, FailureKortMin);
            }
            //public async Task GetData()
            //{
            //    hubConnection = new HubConnectionBuilder()
            //        .WithUrl(NavigationManager.ToAbsoluteUri("/dashboardHub"))
            //        .WithAutomaticReconnect()
            //        .Build();

            //    hubConnection.On<DateTime>("DateGekozen", (Tijd) =>
            //    {

            //    });
            //    await hubConnection.StartAsync();
            //}
        }
    }
}


