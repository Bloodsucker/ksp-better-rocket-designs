using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal class RocketDesignManager
    {
        private List<RocketDesign> _cachedRocketDesigns;
        private IRocketDesignLoader _rocketDesignLoader;

        public RocketDesignManager(IRocketDesignLoader rocketDesignLoader)
        {
            this._rocketDesignLoader = rocketDesignLoader;
            _cachedRocketDesigns = new List<RocketDesign>();
        }

        public void LoadAllRocketDesigns()
        {
            List<RocketDesign> cachedRocketDesigns = new List<RocketDesign>();

            List<IConfigNodeAdapter> configNodes = _rocketDesignLoader.LoadAllRocketDesigns();

            foreach (var configNode in configNodes)
            {
                RocketDesign rocketDesign = new RocketDesign(configNode);

                cachedRocketDesigns.Add(rocketDesign);
            }

            this._cachedRocketDesigns = cachedRocketDesigns;
        }

        public List<RocketDesign> GetCachedRocketDesigns()
        {
            return _cachedRocketDesigns;
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
        public List<RocketDesign> Filter(string filterCriteria)
        {
            List<RocketDesign> filteredRocketDesigns = new List<RocketDesign>();

            foreach(RocketDesign rocketDesign in _cachedRocketDesigns)
            {
                if(rocketDesign.Name.Contains(filterCriteria))
                {
                    filteredRocketDesigns.Add(rocketDesign);
                }
            }

            return filteredRocketDesigns;
        }
    }
}
