using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBISX_POS
{
    
   public class ReportSetting
    {
        public required SalesReport SalesReport { get; set; }
    }

    public class SalesReport
    {
        public required  string DailySalesReport { get; set; }
    }
}
