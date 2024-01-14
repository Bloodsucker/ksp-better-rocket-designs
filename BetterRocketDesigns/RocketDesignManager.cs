using System.Collections.Generic;
using System.Linq;

namespace BetterRocketDesigns
{
    internal class RocketDesignManager
    {
        private List<RocketDesign> _cachedRocketDesigns;
        private IRocketDesignLoader _rocketDesignLoader;

        private SortedSet<string> _labels = new SortedSet<string>();
        private SortedSet<string> _capabilities = new SortedSet<string>();

        public RocketDesignManager(IRocketDesignLoader rocketDesignLoader)
        {
            this._rocketDesignLoader = rocketDesignLoader;
            _cachedRocketDesigns = new List<RocketDesign>();
        }

        public void LoadAllRocketDesigns()
        {
            _labels = new SortedSet<string>();
            _capabilities = new SortedSet<string>();
            _cachedRocketDesigns = new List<RocketDesign>();

            List<IConfigNodeAdapter> configNodes = _rocketDesignLoader.LoadAllRocketDesigns();

            foreach (var configNode in configNodes)
            {
                RocketDesign rocketDesign = new RocketDesign(configNode);

                _cachedRocketDesigns.Add(rocketDesign);
            }

            ReloadCachedRocketDesignMetadata();
        }

        private void ReloadCachedRocketDesignMetadata()
        {
            _labels = new SortedSet<string>(_cachedRocketDesigns.SelectMany(rd => rd.Labels).Distinct());
            _capabilities = new SortedSet<string>(_cachedRocketDesigns.SelectMany(rd => rd.Capabilities.Keys).Distinct());
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

            ReloadCachedRocketDesignMetadata();

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
