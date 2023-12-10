using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal class RocketDesignManager
    {
        private List<RocketDesign> cachedRocketDesigns;
        private IRocketDesignLoader<IConfigNodeAdapter> rocketDesignLoader;
        /*private ITagStorage tagStorage;*/

        public RocketDesignManager(IRocketDesignLoader<IConfigNodeAdapter> rocketDesignLoader)
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

        public RocketDesign SaveOrReplaceAsRocketDesign(IConfigNodeAdapter configNode)
        {
            RocketDesign newRocketDesign = new RocketDesign
            {
                Name = configNode.GetValue("ship"),
                ConfigNode = configNode
            };

            /*tagStorage.LoadTags(newRocketDesign);*/

            rocketDesignLoader.SaveRocketDesign(newRocketDesign.Name, newRocketDesign.ConfigNode);

            int index = cachedRocketDesigns.FindIndex(rd => rd.Name == newRocketDesign.Name);
            if (index != -1)
            {
                cachedRocketDesigns.RemoveAt(index);
            }

            cachedRocketDesigns.Add(newRocketDesign);

            return newRocketDesign;
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
