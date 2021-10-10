using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

namespace ItemChanger.Internal
{
    internal static class SpriteManager
    {
        private static readonly Dictionary<string, Sprite> _sprites = new();
        private static readonly Dictionary<string, string> _resourceNames = typeof(SpriteManager).Assembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".png"))
            .ToDictionary(n => n.Substring("ItemChanger.Resources.".Length, n.Length - "ItemChanger.Resources.".Length - ".png".Length));

        public static Sprite GetSprite(string name)
        {
            if (_sprites.TryGetValue(name, out Sprite sprite)) return sprite;
            else if (_resourceNames.TryGetValue(name, out string path))
            {
                using Stream s = typeof(SpriteManager).Assembly.GetManifestResourceStream(path);
                return _sprites[name] = FromStream(s);
            }
            else throw new ArgumentException($"{name} did not correspond to an embedded image file.");
        }

        private static Sprite FromStream(Stream s)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            byte[] buffer = ToArray(s);
            tex.LoadImage(buffer, markNonReadable: true);
            tex.filterMode = FilterMode.Point;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

        private static byte[] ToArray(Stream s)
        {
            using MemoryStream ms = new();
            s.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
