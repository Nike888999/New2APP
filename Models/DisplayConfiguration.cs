using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAPP.Models
{
    public class DisplayConfiguration
    {
        public string Name { get; set; }
        public int Columns { get; set; }
        public int Sensors { get; set; }

        public override string ToString ( ) => Name;
       
    }
}
