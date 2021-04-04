using System;

namespace Pollidut.Models.SignalRModels
{
    public class Consumer
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }
        public String Mobile { get; set; }
        public Decimal Lat { get; set; }
        public Decimal Lon { get; set; }
    }
}