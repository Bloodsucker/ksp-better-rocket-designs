using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal interface IRocketDesignLoader
    {
        List<IConfigNodeAdapter> LoadAllRocketDesigns();
        void SaveRocketDesign(string fileName, IConfigNodeAdapter configNode);
    }
}
