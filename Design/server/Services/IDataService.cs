using SimmeMqqt.Model;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimmeMqqt.Services
{
    public interface IDataService
    {
        Task<ICollection<MQTTMachineData>> GetDataUurlijk();
        Task<MQTTMachineData> GetDataRealtime();
    }
}
