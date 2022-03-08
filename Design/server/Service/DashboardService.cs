using SimmeMqqt.EntityFramework;
using SimmeMqqt.Model;

namespace SimmeMqqt.Service
{
    public class DashboardService
    {
        public static void UpdateDashboard(MQTTMachineData EFMachineData)
        {
            using (var ctx = new MachineData())
            {
                var Data = EFMachineData;

                ctx.MachineDatas.Add(Data);
                ctx.SaveChanges();
            }

        }
    }
}
