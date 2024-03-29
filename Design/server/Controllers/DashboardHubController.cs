﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SimmeMqqt.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Timers;


namespace SimmeMqqt.Controllers
{
    public class DashboardHubController : Controller
    {
        public readonly IHubContext<DashboardHub> _hubContext;
        public Timer aTimer;
        public DashboardHubController(IHubContext<DashboardHub> hubContext)
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
