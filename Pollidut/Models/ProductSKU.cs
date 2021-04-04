using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pollidut.Models
{
    public class ProductSKU
    {
        public int ProductSKUId { get; set; }
        public string ProductSKUName { get; set; }
        public double UnitPrice { get; set; }
        public int PiecePerCarton { get; set; }
        public int JCOffer { get; set; }
        public int JCOfferType { get; set; }
        public int OutOf { get; set; }
        public int OutOfType { get; set; }
        public double DDAmount { get; set; }

    }
}