namespace BetterRocketDesigns
{
    internal interface IConfigNodeAdapter
    {
        string GetValue(string key);
        bool SetValue(string key, string value);
        bool Save(string fileFullName);
    }
}
