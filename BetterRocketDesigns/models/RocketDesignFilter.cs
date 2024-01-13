using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterRocketDesigns
{
    class RocketDesignFilter
    {
        public string TextFilter { get; set; } = "";
        public HashSet<string> FilterLabels { get; set; } = new HashSet<string>();
        public HashSet<string> FilterCapabilities { get; set; } = new HashSet<string>();
    }
}
