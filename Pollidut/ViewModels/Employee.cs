using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pollidut.ViewModels
{
    public class FamilyDetail
    {
        public int id { get; set; }
        public string memberName { get; set; }
        public string relation { get; set; }
        public int age { get; set; }
        public string occupation { get; set; }
    }
    public class DistributionModel
    {
        public int Id { get; set; }
    }
    public class WorkDetail
    {
        public int id { get; set; }
        public string areaname { get; set; }
        public string unionname { get; set; }
        public string thananame { get; set; }
    }
}