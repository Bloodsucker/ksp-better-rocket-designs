using System;
using System.Collections.Generic;
using System.IO;

namespace BetterRocketDesigns.ksp
{
    internal class RocketDesignLoader : IRocketDesignLoader
    {
        private string saveDirectoryPath;
        private const string ROCKET_DESIGN_SAVE_FOLDER_NAME = "RocketDesigns";

        public RocketDesignLoader(string saveDirectoryPath) 
        {
            this.saveDirectoryPath = saveDirectoryPath;
        }

        public List<RocketDesign> LoadAllRocketDesigns()
        {
            List<RocketDesign> rocketDesigns = new List<RocketDesign>();

            string fullRocketDesignFolderPath = getFullRocketDesignFolderPath();

            if (!Directory.Exists(fullRocketDesignFolderPath)) return rocketDesigns;

            string[] craftFiles = Directory.GetFiles(getFullRocketDesignFolderPath(), "*.craft");

            foreach (string craftFile in craftFiles)
            {
                ConfigNode configNode = ConfigNode.Load(craftFile);
                IConfigNodeAdapter configNodeAdapter = new ConfigNodeAdapter(configNode);

                RocketDesign rocketDesign = new RocketDesign(configNodeAdapter, craftFile);

                rocketDesigns.Add(rocketDesign);
            }

            return rocketDesigns;
        }

        public void SaveRocketDesign(RocketDesign rocketDesign)
        {
            if(!Directory.Exists(getFullRocketDesignFolderPath()))
            {
                Directory.CreateDirectory(getFullRocketDesignFolderPath());
            }

            string filename = rocketDesign.Name;
            IConfigNodeAdapter configNode = rocketDesign.ConfigNode;

            string craftFilePath = Path.Combine(getFullRocketDesignFolderPath(), $"{filename}.craft");

            configNode.Save(craftFilePath);

            rocketDesign.CraftPath = craftFilePath;
        }

        private string getFullRocketDesignFolderPath()
        {
            return Path.Combine(this.saveDirectoryPath, ROCKET_DESIGN_SAVE_FOLDER_NAME);
        }
    }
}
