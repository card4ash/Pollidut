using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pollidut.ViewModels
{
    //printing notes
    public class ComplainViewModel
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string User { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public int Zindex { get; set; }
    }

}