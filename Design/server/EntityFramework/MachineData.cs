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
            var ConnectionString = "server=sql11.freesqldatabase.com;port=3306;database=sql11480795;User=sql11480795;Password=sXWQ4MEyxW;";
            var ServerVersion = MySqlServerVersion.AutoDetect(ConnectionString);
            optionsBuilder.UseMySql(ConnectionString, ServerVersion);
        }
    }
}

