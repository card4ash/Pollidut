using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pollidut.Models.SignalRModels
{
    public class ReportedPosition
    {
        public Int32 Id { get; set; } //Employee Id
        public Decimal Lat { get; set; }
        public Decimal Lon { get; set; }

    }
}