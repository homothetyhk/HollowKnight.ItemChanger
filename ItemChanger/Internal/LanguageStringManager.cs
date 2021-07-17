using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using System.Reflection;
using Language;
using static Modding.Logger;
using Modding;

namespace ItemChanger.Internal
{
    internal static class LanguageStringManager
    {
        private static readonly Dictionary<string, Dictionary<string, string>> LanguageStrings =
            new Dictionary<string, Dictionary<string, string>>();
        private static bool loaded;

        internal static void Load()
        {
            if (!loaded)
            {
                Assembly a = typeof(ItemChanger).Assembly;
                Stream xmlStream = a.GetManifestResourceStream("ItemChanger.Resources.language.xml");

                // Load XmlDocument from resource stream
                XmlDocument xml = new XmlDocument();
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
            }
            loaded = true;
            ModHooks.Instance.LanguageGetHook += GetLanguageString;
        }
        internal static void Unhook()
        {
            ModHooks.Instance.LanguageGetHook -= GetLanguageString;
        }

        public static void SetString(string sheetName, string key, string text)
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

        public static void ResetString(string sheetName, string key)
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

        // keep this private -- the api hook does weird stuff with GetInternal
        private static string GetLanguageString(string key, string sheetTitle)
        {
            if (key.StartsWith("RANDOMIZER_NAME_ESSENCE_"))
            {
                return key.Split('_').Last() + " Essence";
            }

            if (key.StartsWith("RANDOMIZER_NAME_GEO_"))
            {
                return key.Split('_').Last() + " Geo";
            }

            // TODO: Decide what to do for shops
            if (key.StartsWith("RANDOMIZER_NAME_GRUB"))
            {
                return $"A grub! ({PlayerData.instance.grubsCollected}/46)";
            }

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(sheetTitle))
            {
                return string.Empty;
            }

            if (LanguageStrings.ContainsKey(sheetTitle) && LanguageStrings[sheetTitle].ContainsKey(key))
            {
                return LanguageStrings[sheetTitle][key];
            }

            return Language.Language.GetInternal(key, sheetTitle);
        }
    }
}
