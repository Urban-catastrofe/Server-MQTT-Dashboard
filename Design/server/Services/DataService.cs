using SimmeMqqt.EntityFramework;
using SimmeMqqt.Model;
using System.Net.Http;

namespace SimmeMqqt.Services
{
    public class DataService : IDataService
    {
        private readonly HttpClient _httpClient;
        public DataService(HttpClient http)
        {
            _httpClient = http;
        }

        public void UpdateDashboard(MQTTMachineData EFMachineData)
        {
            using (var context = new MachineData())
            {
                var Data = EFMachineData;

                context.MachineDatas.Add(Data);
                context.SaveChanges();
            }
            //DashboardService dashboardService = new DashboardService();
            //dashboardService.SetRealtimeData();

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

                Realtime realtime = new Realtime();
                realtime.SetDataRealtime(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
            }
        }
    }
}
