using BetterRocketDesigns.utils;
using System.Collections.Generic;

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

        public void SetValue(string key, string value)
        {
            configNode.SetValue(key, value);
        }

        public void AddValue(string key, string value)
        {
            configNode.AddValue(key, value);
        }

        public void SetValue(string key, IReadOnlyDictionary<string, float> value)
        {
            ConfigNodeTools.Store(configNode, key, value);
        }

        public void GetValue(string key, out Dictionary<string, float> value)
        {
            ConfigNodeTools.Load(configNode, key, out value);
        }

        public void GetValues(string name, out List<string> values)
        {
            values = configNode.GetValuesList(name);
        }

        public void SetNode(string name, ConfigNode newNode)
        {
            configNode.SetNode(name, newNode);
        }

        public void SetValue(string key, IReadOnlyList<string> values)
        {
            configNode.RemoveValue(key);

            foreach (string value in values)
            {
                configNode.AddValue("labels", value);
            }
        }
    }
}
