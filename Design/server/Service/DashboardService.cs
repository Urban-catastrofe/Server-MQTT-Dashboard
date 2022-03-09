using SimmeMqqt.EntityFramework;
using SimmeMqqt.Model;
using SimmeMqqt.Pages;
using System.Linq;

namespace SimmeMqqt.Service
{
    public class DashboardService
    {
        public static void UpdateDashboard(MQTTMachineData EFMachineData)
        {
            using (var context = new MachineData())
            {
                var Data = EFMachineData;

                context.MachineDatas.Add(Data);
                context.SaveChanges();
            }
            DashboardService dashboardService = new DashboardService();
            dashboardService.SetRealtimeData();
        }

        public void SetRealtimeData()
        {
            using (var context = new MachineData())
            {
                var query = context.MachineDatas
                                   .LastOrDefault();
                int TotalFailureProcent;
                if(query.Failure == true)
                {
                    TotalFailureProcent = 100;
                }
                else
                {
                    TotalFailureProcent = 0;
                }

                var Beschikbaarheid = TotalFailureProcent;
                var Prestaties = query.TotalProduction / query.IdealCyclus;
                var Kwaliteit = query.TotalGoodProduction / query.TotalProduction;
                var OEE = Beschikbaarheid * Prestaties * Kwaliteit;

                Realtime realtime = new Realtime();
                realtime.SetDataRealtime(Beschikbaarheid, Prestaties, Kwaliteit, OEE);
            }          
        }
    }
}
