using Microsoft.EntityFrameworkCore;
using SimmeMqqt.Model;
using Pomelo;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace SimmeMqqt.EntityFramework
{
    public class MachineData : DbContext
    {
        public MachineData() : base()
        {

        }
        public DbSet<MQTTMachineData> MachineDatas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var ConnectionString = "server=gcmsi.nl;port=3306;database=simmebuiting_DatabaseServerMqtt;User=simmebuiting_DatabaseServerMqtt;Password=Geheim04;";
            var ServerVersion = MySqlServerVersion.AutoDetect(ConnectionString);
            optionsBuilder.UseMySql(ConnectionString, ServerVersion);
        }
    }
}

