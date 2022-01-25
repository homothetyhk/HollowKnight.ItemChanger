using System.Reflection;

namespace ItemChanger.Internal
{
    /// <summary>
    /// Class for managing loading and caching Sprites from png files.
    /// </summary>
    public class SpriteManager
    {
        private readonly Assembly _assembly;
        private readonly Dictionary<string, string> _resourcePaths;
        private readonly Dictionary<string, Sprite> _cachedSprites = new();

        /// <summary>
        /// The SpriteManager with access to embedded ItemChanger pngs.
        /// </summary>
        public static SpriteManager Instance { get; } = new(typeof(SpriteManager).Assembly, "ItemChanger.Resources.");

        /// <summary>
        /// Creates a SpriteManager to lazily load and cache Sprites from the embedded png files in the specified assembly.
        /// <br/>Only filepaths with the matching prefix are considered, and the prefix is removed to determine sprite names (e.g. "ItemChangerMod.Resources." is the prefix for Instance).
        /// </summary>
        public SpriteManager(Assembly a, string resourcePrefix)
        {
            _assembly = a;
            _resourcePaths = a.GetManifestResourceNames()
                .Where(n => n.EndsWith(".png") && n.StartsWith(resourcePrefix))
                .ToDictionary(n => n.Substring(resourcePrefix.Length, n.Length - resourcePrefix.Length - ".png".Length));
        }

        /// <summary>
        /// Fetches the Sprite with the specified name. If it has not yet been loaded, loads it from embedded resources and caches the result.
        /// </summary>
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

        /// <summary>
        /// Loads a sprite from the png file passed as a stream.
        /// </summary>
        public static Sprite Load(Stream data, FilterMode filterMode = FilterMode.Bilinear)
        {
            return Load(ToArray(data), filterMode);
        }

        /// <summary>
        /// Loads a sprite from the png file passed as a byte array.
        /// </summary>
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
