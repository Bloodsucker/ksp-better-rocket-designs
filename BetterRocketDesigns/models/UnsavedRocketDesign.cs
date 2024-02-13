using BetterRocketDesigns.utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterRocketDesigns
{
    internal class UnsavedRocketDesign
    {
        private IConfigNodeAdapter _configNode;
        private string _name;
        private SortedSet<string> _labels;
        private Dictionary<string, float> _capabilities;

        public UnsavedRocketDesign(IConfigNodeAdapter configNode)
        {
            _configNode = configNode;

            UpdatePropertiesFromConfigNode();
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                UpdateConfigNode();
            }
        }

        public IReadOnlyCollection<string> Labels
        {
            get { return _labels; }
            set
            {
                _labels = new SortedSet<string>(value);
                UpdateConfigNode();
            }
        }

        public IReadOnlyDictionary<string, float> Capabilities
        {
            get { return _capabilities; }
            set
            {
                _capabilities = value.ToDictionary(pair => pair.Key, pair => pair.Value);
                UpdateConfigNode();
            }
        }

        public IConfigNodeAdapter ConfigNode {
            get
            {
                return _configNode;
            }

            set
            {
                _configNode = value;
                UpdatePropertiesFromConfigNode();
            }
        }
        private void UpdateConfigNode()
        {
            _configNode.SetValue("ship", _name);
            _configNode.SetValue("labels", _labels);
            _configNode.SetValue("capabilities", _capabilities);
        }

        private void UpdatePropertiesFromConfigNode()
        {
            _name = ConfigNode.GetValue("ship");

            ConfigNode.GetValues("labels", out List<string> labels);
            _labels = new SortedSet<string>(labels);

            ConfigNode.GetValue("capabilities", out Dictionary<string, float> capabilities);
            _capabilities = capabilities;
        }
    }
}
