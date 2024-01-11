using System.Collections.Generic;
using System.Linq;

namespace BetterRocketDesigns.utils
{
    class ConfigNodeTools
    {
        public static void Store(ConfigNode configNode, string name, IReadOnlyDictionary<string, float> value)
        {
            string dicKeys = ConfigNode.WriteStringArray(value.Keys.ToArray());
            string dicValues = ConfigNode.WriteStringArray(value.Values.Select(x => x.ToString()).ToArray());

            configNode.SetValue($"{name}_keys", dicKeys, true);
            configNode.SetValue($"{name}_values", dicValues, true);
        }

        public static void Load(ConfigNode configNode, string name, out Dictionary<string, float> dictionary)
        {
            dictionary = new Dictionary<string, float>();
            Dictionary<string, float>  tmpDictionary = new Dictionary<string, float>();

            if (!configNode.HasValue($"{name}_keys") || !configNode.HasValue($"{name}_values"))
            {
                return;
            }

            string[] keysArray = configNode.GetValue($"{name}_keys").Split(',');
            string[] valuesArray = configNode.GetValue($"{name}_values").Split(',');

            if (keysArray.Length != valuesArray.Length)
            {
                return;
            }

            for (int i = 0; i < keysArray.Length; i++)
            {
                if (!float.TryParse(valuesArray[i], out float value))
                {
                    return;
                }

                tmpDictionary[keysArray[i]] = value;
            }

            dictionary = tmpDictionary;
        }
    }
}
