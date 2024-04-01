using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLibV100.Ports
{
    public class PortPropertyAttribute : Attribute
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }
}
