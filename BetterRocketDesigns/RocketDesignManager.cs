using System.Collections.Generic;
using System.Linq;

namespace BetterRocketDesigns
{
    internal class RocketDesignManager
    {
        private List<RocketDesign> _cachedRocketDesigns;
        private IRocketDesignLoader _rocketDesignLoader;

        private HashSet<string> _labels = new HashSet<string>();
        private HashSet<string> _capabilities = new HashSet<string>();

        public RocketDesignManager(IRocketDesignLoader rocketDesignLoader)
        {
            this._rocketDesignLoader = rocketDesignLoader;
            _cachedRocketDesigns = new List<RocketDesign>();
        }

        public void LoadAllRocketDesigns()
        {
            _labels = new HashSet<string>();
            _capabilities = new HashSet<string>();
            _cachedRocketDesigns = new List<RocketDesign>();

            List<IConfigNodeAdapter> configNodes = _rocketDesignLoader.LoadAllRocketDesigns();

            foreach (var configNode in configNodes)
            {
                RocketDesign rocketDesign = new RocketDesign(configNode);

                foreach(var label in rocketDesign.Labels)
                {
                    if (!_labels.Contains(label))
                    {
                        _labels.Add(label);
                    }
                }

                foreach(var capabilityKvp in rocketDesign.Capabilities)
                {
                    if (!_capabilities.Contains(capabilityKvp.Key))
                    {
                        _capabilities.Add(capabilityKvp.Key);

                    }
                }

                _cachedRocketDesigns.Add(rocketDesign);
            }
        }

        public List<RocketDesign> GetCachedRocketDesigns()
        {
            return _cachedRocketDesigns;
        }

        public IReadOnlyCollection<string> getCachedLabels()
        {
            return _labels;
        }

        public IReadOnlyCollection<string> getCachedCapabilities()
        {
            return _capabilities;
        }

        public RocketDesign SaveOrReplaceAsRocketDesign(RocketDesign rocketDesign)
        {
            _rocketDesignLoader.SaveRocketDesign(rocketDesign.Name, rocketDesign.ConfigNode);

            int index = _cachedRocketDesigns.FindIndex(rd => rd.Name == rocketDesign.Name);
            if (index != -1)
            {
                _cachedRocketDesigns.RemoveAt(index);
            }

            _cachedRocketDesigns.Add(rocketDesign);

            return rocketDesign;
        }

        public List<RocketDesign> Filter(RocketDesignFilter filter)
        {
            List<RocketDesign> filteredRocketDesigns = new List<RocketDesign>();

            foreach (RocketDesign rocketDesign in _cachedRocketDesigns)
            {
                if (filter.TextFilter.Length > 0 && !rocketDesign.Name.Contains(filter.TextFilter))
                {
                    continue;
                }

                if (filter.FilterLabels.Count > 0 && !filter.FilterLabels.Intersect(rocketDesign.Labels).Any())
                {
                    continue;
                }

                if (filter.FilterCapabilities.Count > 0 && !filter.FilterCapabilities.Intersect(rocketDesign.Capabilities.Keys).Any())
                {
                    continue;
                }

                filteredRocketDesigns.Add(rocketDesign);
            }

            return filteredRocketDesigns;
        }
    }
}
