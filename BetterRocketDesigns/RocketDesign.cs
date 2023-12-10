using BetterRocketDesigns.utils;
using System.Collections.Generic;
using UnityEngine;

namespace BetterRocketDesigns
{
    internal class RocketDesign
    {
        public RocketDesign() {
            ThumbnailImage = Tools.MakeTexture(32, 32, Color.magenta);
        }

        public string Name { get; set; }

        public Texture2D ThumbnailImage {  get ; set; }

        public Dictionary<string, Dictionary<string, float>> tags { get; set; }

        public IConfigNodeAdapter ConfigNode { get; set; }
    }
}
