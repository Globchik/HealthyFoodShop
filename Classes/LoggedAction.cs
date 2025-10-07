using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyFood_Shop.Classes
{
    public class LoggedAction
    {
        public string LogText { get; set; } = "";
        public string LogTime { get; set; } = "";

        public LoggedAction(string logText, string logTime)
        {
            LogText = logText;
            LogTime = logTime;
        }
    }
}
