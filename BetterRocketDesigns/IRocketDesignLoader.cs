using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal interface IRocketDesignLoader<T> where T : IConfigNodeAdapter
    {
        List<T> LoadAllRocketDesigns();
        void SaveRocketDesign(string fileName, T configNode);
    }
}
