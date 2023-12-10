using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterRocketDesigns
{
    internal class GameLabelsManager
    {
        private Dictionary<string, List<string>> labels = new Dictionary<string, List<string>>();

        public GameLabelsManager()
        {
            labels.Add("LV", new List<string> { "LEO East", "LEO Polar" });
            labels.Add("LOI Stage", new List<string> { "LOI" });
        }

        public Dictionary<string, List<string>> getLabels()
        {
            return labels;
        }
    }
}
