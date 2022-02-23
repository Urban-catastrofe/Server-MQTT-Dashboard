using Microsoft.EntityFrameworkCore;
using SimmeMqqt.Model;

namespace SimmeMqqt.EntityFramework
{
    public class MachineData : DbContext
    {
        public DbSet<MQTTMachineData> MachineDatas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDatabase;Trusted_Connection=True;");
        }
    }
}

