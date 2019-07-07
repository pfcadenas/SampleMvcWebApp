using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceLayer.PostServices;

namespace SampleWebApp.Models
{
    /// <summary>
    /// This class is for return to the view a json object with necessary data for datatable
    /// </summary>
    public class TablePartitioningResponseViewModels
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public IEnumerable<object> aaData { get; set; }
        public IEnumerable<object> aaData1 { get; set; }
        public string content { get; set; }
    }
}