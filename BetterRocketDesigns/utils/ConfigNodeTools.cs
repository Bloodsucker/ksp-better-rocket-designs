using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterRocketDesigns.utils
{
    class ConfigNodeTools
    {
        public static void Store(ConfigNode configNode, string name, IReadOnlyDictionary<string, float> value)
        {
            ConfigNode dicConfigNode = new ConfigNode();
            ConfigNode dicConfigNodeValues = new ConfigNode();
            foreach (var kvp in value)
            {
                dicConfigNode.AddValue("keys", kvp.Key);
                dicConfigNodeValues.AddValue(kvp.Key, kvp.Value);
            }

            dicConfigNode.SetNode("kvp", dicConfigNodeValues, true);
            configNode.SetNode(name, dicConfigNode, true);
        }

        public static void Load(ConfigNode configNode, string name, out Dictionary<string, float> value)
        {
            value = new Dictionary<string, float>();

            ConfigNode dicConfigNode = configNode.GetNode(name);
            if (dicConfigNode == null)
            {
                return;
            }

            List<string> dicKeys = dicConfigNode.GetValuesList("keys");
            ConfigNode kvpConfigNode = dicConfigNode.GetNode("kvp");
            if(kvpConfigNode == null)
            {
                Debug.Log("Incorrect serialized ConfigNode Dictionary structure (kvp)");
            }

            foreach(var dicKey in dicKeys)
            {
                string valueStr = kvpConfigNode.GetValue(dicKey);
                float dicValue = float.Parse(valueStr);
                value.Add(dicKey, dicValue);
            }
        }
    }
}
