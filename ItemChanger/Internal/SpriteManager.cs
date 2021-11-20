using System.Reflection;

namespace ItemChanger.Internal
{
    public class SpriteManager
    {
        private readonly Assembly _assembly;
        private readonly Dictionary<string, string> _resourcePaths;
        private readonly Dictionary<string, Sprite> _cachedSprites = new();

        public static SpriteManager Instance { get; } = new(typeof(SpriteManager).Assembly, "ItemChanger.Resources.");

        public SpriteManager(Assembly a, string resourcePrefix)
        {
            _assembly = a;
            _resourcePaths = a.GetManifestResourceNames()
                .Where(n => n.EndsWith(".png") && n.StartsWith(resourcePrefix))
                .ToDictionary(n => n.Substring(resourcePrefix.Length, n.Length - resourcePrefix.Length - ".png".Length));
        }

        public Sprite GetSprite(string name)
        {
            if (_cachedSprites.TryGetValue(name, out Sprite sprite)) return sprite;
            else if (_resourcePaths.TryGetValue(name, out string path))
            {
                using Stream s = _assembly.GetManifestResourceStream(path);
                return _cachedSprites[name] = Load(s);
            }
            else throw new ArgumentException($"{name} did not correspond to an embedded image file.");
        }

        public static Sprite Load(Stream data, FilterMode filterMode = FilterMode.Bilinear)
        {
            return Load(ToArray(data), filterMode);
        }

        public static Sprite Load(byte[] data, FilterMode filterMode)
        {
            Texture2D tex = new(1, 1, TextureFormat.RGBA32, false);
            tex.LoadImage(data, markNonReadable: true);
            tex.filterMode = filterMode;
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
