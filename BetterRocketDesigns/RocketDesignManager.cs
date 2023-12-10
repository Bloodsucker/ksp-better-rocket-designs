using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal class RocketDesignManager
    {
        private List<RocketDesign> cachedRocketDesigns;
        private IRocketDesignLoader rocketDesignLoader;
        /*private ITagStorage tagStorage;*/

        public RocketDesignManager(IRocketDesignLoader rocketDesignLoader)
        {
            this.rocketDesignLoader = rocketDesignLoader;
            /*this.tagStorage = tagStorage;*/
            cachedRocketDesigns = new List<RocketDesign>();
        }

        public void LoadAllRocketDesigns()
        {
            List<IConfigNodeAdapter> configNodes = rocketDesignLoader.LoadAllRocketDesigns();

            foreach (var configNode in configNodes)
            {
                RocketDesign rocketDesign = new RocketDesign
                {
                    Name = configNode.GetValue("ship"),
                    ConfigNode = configNode,
                };

                /*tagStorage.LoadTags(rocketDesign);*/

                cachedRocketDesigns.Add(rocketDesign);
            }
        }

        public List<RocketDesign> GetCachedRocketDesigns()
        {
            return cachedRocketDesigns;
        }

        public RocketDesign SaveOrReplaceAsRocketDesign(RocketDesign rocketDesign)
        {
            rocketDesignLoader.SaveRocketDesign(rocketDesign.Name, rocketDesign.ConfigNode);

            int index = cachedRocketDesigns.FindIndex(rd => rd.Name == rocketDesign.Name);
            if (index != -1)
            {
                cachedRocketDesigns.RemoveAt(index);
            }

            cachedRocketDesigns.Add(rocketDesign);

            return rocketDesign;
        }
        public List<RocketDesign> Filter(string filterCriteria)
        {
            List<RocketDesign> filteredRocketDesigns = new List<RocketDesign>();

            foreach(RocketDesign rocketDesign in cachedRocketDesigns)
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
