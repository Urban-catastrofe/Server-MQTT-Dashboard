using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimmeMqqt.EntityFramework;
using SimmeMqqt.Model;
using SimmeMqqt.Pages;
using System.Linq;

namespace SimmeMqqt.Controllers
{
    [Route("api/DashboardController")]
    [ApiController]
    public class DashboardController : Controller
    {
        [HttpPost]
        // POST: DashboardController/UpdateRealtime/5
        public ActionResult UpdateRealtime(MQTTMachineData Data)
        {
            UpdateDashboard(Data);
            return Ok();

        }

        
    }
}
