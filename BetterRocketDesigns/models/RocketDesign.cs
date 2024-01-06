using BetterRocketDesigns.utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BetterRocketDesigns
{
    internal class RocketDesign
    {
        private IConfigNodeAdapter _configNode;
        private string _name;
        private List<string> _labels;
        private Dictionary<string, float> _capabilities;

        public RocketDesign(IConfigNodeAdapter configNode) {
            ThumbnailImage = Tools.MakeTexture(32, 32, Color.magenta);
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

        public IReadOnlyList<string> Labels
        {
            get { return _labels; }
            set
            {
                _labels = new List<string>(value);
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

        private Texture2D _thumbnailImage;

        public Texture2D ThumbnailImage
        {
            get { return _thumbnailImage; }
            set { _thumbnailImage = value; }
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
            _name = _configNode.GetValue("ship");
            _configNode.GetValues("labels", out _labels);
            _configNode.GetValue("capabilities", out _capabilities);
        }
    }
}
