using System.Collections.Generic;

namespace BetterRocketDesigns
{
    internal interface IRocketDesignLoader
    {
        List<RocketDesign> LoadAllRocketDesigns();
        RocketDesign SaveRocketDesign(UnsavedRocketDesign rocketDesign);
    }
}
