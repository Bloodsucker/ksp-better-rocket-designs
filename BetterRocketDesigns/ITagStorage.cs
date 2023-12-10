namespace BetterRocketDesigns
{
    internal interface ITagStorage
    {
        void SaveTags(RocketDesign rocketDesign);
        void LoadTags(RocketDesign rocketDesign);
    }
}
