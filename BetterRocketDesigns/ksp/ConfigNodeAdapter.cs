using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterRocketDesigns.ksp
{
    internal class ConfigNodeAdapter : IConfigNodeAdapter
    {
        private ConfigNode configNode;

        public ConfigNodeAdapter(ConfigNode configNode)
        {
            this.configNode = configNode;
        }

        public ConfigNode GetOriginalInstance()
        {
            return configNode;
        }

        public string GetValue(string key)
        {
            return configNode.GetValue(key);
        }

        public bool Save(string fileFullName)
        {
            return configNode.Save(fileFullName);
        }

        public bool SetValue(string key, string value)
        {
            return configNode.SetValue(key, value);
        }
    }
}
