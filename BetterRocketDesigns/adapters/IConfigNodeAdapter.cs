using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal interface IConfigNodeAdapter
    {
        bool Save(string fileFullName);

        string GetValue(string key);
        void GetValue(string key, out Dictionary<string, float> value);
        void GetValues(string name, out List<string> values);

        void SetValue(string key, string value);
        void SetValue(string key, IReadOnlyList<string> value);
        void SetValue(string key, IReadOnlyDictionary<string, float> value);

        void SetNode(string name, ConfigNode newNode);
        void AddValue(string key, string value);
    }
}
