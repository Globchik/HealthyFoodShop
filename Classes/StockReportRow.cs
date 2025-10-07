using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyFood_Shop.Classes
{
    public class StockReportRow
    {
        public string ItemName { get; set; } = String.Empty;
        public string ExpiryDate { get; set; } = String.Empty;
        public string Amount { get; set; } = String.Empty;
    }
}
