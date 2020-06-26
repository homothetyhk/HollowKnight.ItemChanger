using SeanprCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace ItemChanger
{
    internal static class SpriteManager
    {
        public static Dictionary<string, Sprite> _sprites;

        public static void Setup()
        {
            _sprites = ResourceHelper.GetSprites("ItemChanger.Resources.");
        }

        public static Sprite GetSprite(string spriteName)
        {
            if(!_sprites.TryGetValue(spriteName, out Sprite sprite))
            {
                ItemChanger.instance.LogError($"Sprite at {spriteName} was not loaded.");
                return null;
            }
            return sprite;
        }
    }
}
