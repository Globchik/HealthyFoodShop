using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyFood_Shop.Classes
{
    public class ID_Value_Pair
    {
        public long? ID { get; set; }
        public string Name { get; set; }

        public ID_Value_Pair(in long nid, in string nname)
        {
            ID = nid;
            Name = nname;
        }

        public ID_Value_Pair()
        {
            ID = null;
            Name = "";
        }
    }
}
