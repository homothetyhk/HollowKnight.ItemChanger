using System.Reflection;
using System.Text;
using UnityEngine.Audio;

namespace ItemChanger.Internal
{
    /// <summary>
    /// Class for managing loading and caching of AudioClips from wav files.
    /// </summary>
    public class SoundManager
    {
        private readonly Assembly _assembly;
        private readonly Dictionary<string, string> _resourcePaths;
        private readonly Dictionary<string, AudioClip> _cachedClips = new();

        /// <summary>
        /// The SoundManager with access to embedded ItemChanger wavs.
        /// </summary>
        public static SoundManager Instance { get; } = new(typeof(SoundManager).Assembly, "ItemChanger.Resources.Audio.");

        /// <summary>
        /// Creates a SoundManager to lazily load and cache AudioClips from the embedded wav files in the specified assembly.
        /// <br/>Only filepaths with the matching prefix are considered, and the prefix is removed to determine clip names (e.g. "ItemChanger.Resources.Audio." is the prefix for Instance).
        /// </summary>
        public SoundManager(Assembly a, string resourcePrefix)
        {
            _assembly = a;
            _resourcePaths = a.GetManifestResourceNames()
                .Where(n => n.EndsWith(".wav") && n.StartsWith(resourcePrefix))
                .ToDictionary(n => n.Substring(resourcePrefix.Length, n.Length - resourcePrefix.Length - ".wav".Length));
        }

        /// <summary>
        /// Fetches the AudioClip with the specified name. If it has not yet been loaded, loads it from embedded resources and caches the result.
        /// </summary>
        public AudioClip GetAudioClip(string name)
        {
            if (_cachedClips.TryGetValue(name, out AudioClip clip)) return clip;
            else if (_resourcePaths.TryGetValue(name, out string path))
            {
                using Stream s = _assembly.GetManifestResourceStream(path);
                return _cachedClips[name] = FromStream(s, name);
            }
            else throw new ArgumentException($"{name} does not correspond to an embedded audio file.");
        }

        /// <summary>
        /// Equivalent to the static PlayClipAtPoint, composed with GetAudioClip.
        /// </summary>
        public void PlayClipAtPoint(string name, Vector3 pos)
        {
            PlayClipAtPoint(GetAudioClip(name), pos);
        }

        /// <summary>
        /// Creates an AudioSource component at the specified point with generally applicable settings, and plays the clip from that source.
        /// </summary>
        public static void PlayClipAtPoint(AudioClip clip, Vector3 pos)
        {
            try
            {
                GameObject parent = new("Temp Audio Source " + pos);
                parent.transform.position = pos;
                AudioSource source = parent.AddComponent<AudioSource>();
                source.spatialBlend = 1;
                source.minDistance = 39;
                source.maxDistance = 50;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.playOnAwake = false;
                source.outputAudioMixerGroup = actors;
                source.PlayOneShot(clip);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        /// <summary>
        /// Save the uncompressed 16-bit audio clip as a .WAV file.
        /// </summary>
        public static void SaveAs(AudioClip clip, string fileName)
        {
            using FileStream fs = File.Create(Path.Combine(Path.GetDirectoryName(typeof(SoundManager).Assembly.Location), fileName));
            using BinaryWriter bw = new(fs, Encoding.ASCII);

            const short bitsPerSample = 16;
            const short bytesPerSample = bitsPerSample / 8;

            float[] data = new float[clip.samples * clip.channels];
            clip.GetData(data, 0);
            int dataLength = data.Length * bytesPerSample; // each float will become a short = 2 bytes
            int totalLength = dataLength + 36; // header length is 36 (first 8 bytes are excluded)

            // Begin Header
            foreach (char c in "RIFF") bw.Write(c);
            bw.Write(totalLength); // length of the rest of the file
            foreach (char c in "WAVE") bw.Write(c);
            foreach (char c in "fmt ") bw.Write(c);
            bw.Write(16); // length of format data following this number
            bw.Write((short)1); // pcm format (noncompressed)
            bw.Write((short)clip.channels); // number of channels
            bw.Write(clip.frequency); // sample rate
            bw.Write(clip.frequency * clip.channels * bytesPerSample); // byte rate
            bw.Write((short)(clip.channels * bytesPerSample)); // block align
            bw.Write(bitsPerSample); // bits per sample
            foreach (char c in "data") bw.Write(c);
            bw.Write(dataLength);
            // End Header

            float scale = short.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                bw.Write(Convert.ToInt16(data[i] * scale)); // Convert rounds instead of truncates
            }
        }

        /// <summary>
        /// Convert the stream representing a 16-bit uncompressed .WAV file to an AudioClip.
        /// </summary>
        public static AudioClip FromStream(Stream s, string name)
        {
            using BinaryReader br = new(s, Encoding.ASCII);

            // Begin Header
            string riff = new(br.ReadChars(4)); // "RIFF"
            int length = br.ReadInt32(); // file size in bytes
            string wave = new(br.ReadChars(4)); // "WAVE"
            string fmt = new(br.ReadChars(4)); // "fmt "
            int formatLength = br.ReadInt32();
            short formatType = br.ReadInt16();
            short channels = br.ReadInt16();
            int sampleRate = br.ReadInt32();
            int byteRate = br.ReadInt32();
            short blockAlign = br.ReadInt16();
            short bitsPerSample = br.ReadInt16();
            string dataHeader = new(br.ReadChars(4)); // "data"
            int dataLength = br.ReadInt32();
            // End Header

            float[] data = new float[dataLength / 2];
            float scale = short.MaxValue;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = br.ReadInt16() / scale;
            }

            AudioClip clip = AudioClip.Create(name, data.Length / channels, channels, sampleRate, false);
            clip.SetData(data, 0);

            return clip;
        }

        /// <summary>
        /// The Actors AudioMixerGroup, from global resources. Used as the mixer for SoundManager.PlayClipAtPoint.
        /// </summary>
        public static readonly AudioMixerGroup actors = Resources.FindObjectsOfTypeAll<AudioMixerGroup>().First(amg => amg.name.StartsWith("Actors"));
    }
}
