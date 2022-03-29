using System.Xml;
using System.Reflection;
using Modding;

namespace ItemChanger.Internal
{
    public static class LanguageStringManager
    {
        private static readonly Dictionary<string, Dictionary<string, string>> LanguageStrings = new();
        private static bool loaded;

        internal static void Load()
        {
            if (!loaded)
            {
                Assembly a = typeof(ItemChangerMod).Assembly;
                Stream xmlStream = a.GetManifestResourceStream("ItemChanger.Resources.language.xml");

                // Load XmlDocument from resource stream
                XmlDocument xml = new();
                xml.Load(xmlStream);
                xmlStream.Dispose();

                XmlNodeList nodes = xml.SelectNodes("Language/entry");
                if (nodes == null)
                {
                    LogWarn("Malformatted language xml, no nodes that match Language/entry");
                    return;
                }

                foreach (XmlNode node in nodes)
                {
                    string sheet = node.Attributes?["sheet"]?.Value;
                    string key = node.Attributes?["key"]?.Value;

                    if (sheet == null || key == null)
                    {
                        LogWarn("Malformatted language xml, missing sheet or key on node");
                        continue;
                    }

                    SetString(sheet, key, node.InnerText.Replace("\\n", "\n"));
                }

                try
                {
                    string localPath = Path.Combine(Path.GetDirectoryName(a.Location), "language.xml");
                    if (File.Exists(localPath))
                    {
                        using (FileStream fs = File.OpenRead(localPath))
                        {
                            xml = new();
                            xml.Load(fs);
                        }
                        nodes = xml.SelectNodes("Language/entry");
                        if (nodes == null)
                        {
                            LogWarn("Malformatted language xml, no nodes that match Language/entry");
                            return;
                        }

                        foreach (XmlNode node in nodes)
                        {
                            string sheet = node.Attributes?["sheet"]?.Value;
                            string key = node.Attributes?["key"]?.Value;

                            if (sheet == null || key == null)
                            {
                                LogWarn("Malformatted language xml, missing sheet or key on node");
                                continue;
                            }

                            SetString(sheet, key, node.InnerText.Replace("\\n", "\n"));
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError($"Error loading local language.xml:\n{e}");
                }
            }
            loaded = true;
        }

        internal static void Hook()
        {
            ModHooks.LanguageGetHook += GetLanguageString;
        }

        internal static void Unhook()
        {
            ModHooks.LanguageGetHook -= GetLanguageString;
        }

        private static void SetString(string sheetName, string key, string text)
        {
            if (string.IsNullOrEmpty(sheetName) || string.IsNullOrEmpty(key) || text == null)
            {
                return;
            }

            if (!LanguageStrings.TryGetValue(sheetName, out Dictionary<string, string> sheet))
            {
                sheet = new Dictionary<string, string>();
                LanguageStrings.Add(sheetName, sheet);
            }

            sheet[key] = text;
        }

        private static void ResetString(string sheetName, string key)
        {
            if (string.IsNullOrEmpty(sheetName) || string.IsNullOrEmpty(key))
            {
                return;
            }

            if (LanguageStrings.TryGetValue(sheetName, out Dictionary<string, string> sheet) && sheet.ContainsKey(key))
            {
                sheet.Remove(key);
            }
        }

        /// <summary>
        /// Returns the string with the given sheet and key from the languge.xml. Does not do any other search, nor does it invoke Language.Language.Get, nor does it format the result.
        /// </summary>
        public static string GetICString(string key, string sheetTitle = "IC")
        {
            if (!LanguageStrings.TryGetValue(sheetTitle, out Dictionary<string, string> sheet) || !sheet.TryGetValue(key, out string value))
            {
                return string.Empty;
            }

            return value;
        }

        // keep this private -- the api hook does weird stuff with GetInternal
        private static string GetLanguageString(string key, string sheetTitle, string orig)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(sheetTitle))
            {
                return string.Empty;
            }

            // we check the dictionary first to allow local overriding of the built-in behaviors below.
            if (LanguageStrings.ContainsKey(sheetTitle) && LanguageStrings[sheetTitle].ContainsKey(key))
            {
                return LanguageStrings[sheetTitle][key];
            }

            if (sheetTitle.StartsWith("Internal"))
            {
                return Language.Language.GetInternal(key, sheetTitle[8..]);
            }

            if (sheetTitle == "Exact")
            {
                return key;
            }

            if (key.StartsWith("ITEMCHANGER_NAME_ESSENCE_"))
            {
                return string.Format(Language.Language.Get("ESSENCE", "Fmt"), key["ITEMCHANGER_NAME_ESSENCE_".Length..]);
            }

            if (key.StartsWith("ITEMCHANGER_NAME_GEO_"))
            {
                return string.Format(Language.Language.Get("GEO", "Fmt"), key["ITEMCHANGER_NAME_GEO_".Length..]);
            }

            if (key == "ITEMCHANGER_POSTVIEW_GRUB")
            {
                return string.Format(Language.Language.Get("GRUB", "Fmt"), PlayerData.instance.GetInt(nameof(PlayerData.grubsCollected)));
            }

            if (key == "ITEMCHANGER_POSTVIEW_GRIMMKIN_FLAME")
            {
                int flames = PlayerData.instance.GetInt(nameof(Modules.GrimmkinFlameManager.cumulativeFlamesCollected));
                if (flames <= 0) flames = PlayerData.instance.GetInt(nameof(PlayerData.flamesCollected));
                return string.Format(Language.Language.Get("FLAME", "Fmt"), flames);
            }

            return orig;
        }
    }
}
