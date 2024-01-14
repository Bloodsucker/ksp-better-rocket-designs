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

        public List<IConfigNodeAdapter> LoadAllRocketDesigns()
        {
            List<IConfigNodeAdapter> craftList = new List<IConfigNodeAdapter>();

            string fullRocketDesignFolderPath = getFullRocketDesignFolderPath();

            if (!Directory.Exists(fullRocketDesignFolderPath)) return craftList;

            string[] craftFiles = Directory.GetFiles(getFullRocketDesignFolderPath(), "*.craft");

            foreach (string craftFile in craftFiles)
            {
                ConfigNode configNode = ConfigNode.Load(craftFile);

                craftList.Add(new ConfigNodeAdapter(configNode));
            }

            return craftList;
        }

        public void SaveRocketDesign(string fileName, IConfigNodeAdapter configNode)
        {
            if(!Directory.Exists(getFullRocketDesignFolderPath()))
            {
                Directory.CreateDirectory(getFullRocketDesignFolderPath());
            }

            string craftFilePath = Path.Combine(getFullRocketDesignFolderPath(), $"{fileName}.craft");

            configNode.Save(craftFilePath);
        }

        private string getFullRocketDesignFolderPath()
        {
            return Path.Combine(this.saveDirectoryPath, ROCKET_DESIGN_SAVE_FOLDER_NAME);
        }
    }
}
