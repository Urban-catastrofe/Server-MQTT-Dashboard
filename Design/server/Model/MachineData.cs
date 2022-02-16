using System;

namespace SimmeMqqt.Model
{
    public class MachineData
    {
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
