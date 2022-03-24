using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimmeMqqt.Model
{
    public class MQTTMachineData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public int IdealCyclus {  get; set; }
        public bool Break { get; set; }
        public bool Failure { get; set; }
        public int TotalProduction { get; set; }
        public int TotalGoodProduction { get; set; }
    }
}
