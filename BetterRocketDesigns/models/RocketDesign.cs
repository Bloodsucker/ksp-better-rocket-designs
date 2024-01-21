using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterRocketDesigns
{
    class RocketDesign
    {
        public IConfigNodeAdapter ConfigNode { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyCollection<string> Labels { get; private set; }
        public IReadOnlyDictionary<string, float> Capabilities { get; private set; }
        public string CraftPath { get; private set; }

        public RocketDesign(UnsavedRocketDesign unsavedRocket, string craftPat)
        {
            ConfigNode = unsavedRocket.ConfigNode;
            CraftPath = craftPat;

            UpdatePropertiesFromConfigNode();
        }

        public RocketDesign(IConfigNodeAdapter configNode, string craftPath)
        {
            CraftPath = craftPath;
            ConfigNode = configNode;

            UpdatePropertiesFromConfigNode();
        }

        private void UpdatePropertiesFromConfigNode()
        {
            Name = ConfigNode.GetValue("ship");

            ConfigNode.GetValues("labels", out List<string> labels);
            Labels = new SortedSet<string>(labels);

            ConfigNode.GetValue("capabilities", out Dictionary<string, float> capabilities);
            Capabilities = capabilities;
        }
    }
}
