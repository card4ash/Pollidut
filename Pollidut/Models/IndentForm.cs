using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pollidut.Models
{
    public class IndentForm
    {
        public int FormId { get; set; }
        public int PollidutId { get; set; }
        public int SupervisorId { get; set; }
        public int DistributionHouseId { get; set; }
        public string FormCode { get; set; }
        public string OrderDate { get; set; }
        public string DeliveryDate { get; set; }
        public double TotalPrice { get; set; }
        public double DTotalPrice { get; set; }
    }
    public class RowItem
    {
        public int RowId { get; set; }
        public int ProductSKUId { get; set; }
        public int FormId { get; set; }
        public double UnitPrice { get; set; }
        public double DUnitPrice { get; set; }
        public int JCOffer { get; set; }
        public int DJCOffer { get; set; }
        public int IndentCartonQty { get; set; }
        public int IndentPieceQty { get; set; }
        public int DeliveryCartonQty { get; set; }
        public int DeliveryPieceQty { get; set; }
        public double TotalPrice { get; set; }
        public double DDAmount { get; set; }
        public double NetPrice { get; set; }
        public double DTotalPrice { get; set; }
        public double DDDAmount { get; set; }
        public double DNetPrice { get; set; }
    }
}