using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace ItemChanger.Internal
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
            if (_sprites == null)
            {
                Modding.Logger.LogError($"GetSprite called before SpriteManager.Setup");
                return null;
            }

            if (!_sprites.TryGetValue(spriteName, out Sprite sprite))
            {
                Modding.Logger.LogError($"Sprite at {spriteName} was not loaded.");
                return null;
            }

            return sprite;
        }
    }
}
