using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal interface IRocketDesignLoader
    {
        List<RocketDesign> LoadAllRocketDesigns();
        void SaveRocketDesign(RocketDesign rocketDesign);
    }
}
