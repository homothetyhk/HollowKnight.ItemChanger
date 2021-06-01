using SereCore;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using UnityEngine;
using static Modding.Logger;

namespace ItemChanger
{
    public static class XmlManager
    {
        internal static Dictionary<string, _Item> Items;
        internal static Dictionary<string, _Location> Locations;


        internal static void Load()
        {
            XmlDocument items;
            XmlDocument locations;
            XmlDocument platforms;
            
            Assembly a = typeof(ItemChanger).Assembly;

            Stream itemStream = a.GetManifestResourceStream("ItemChanger.Resources.items.xml");
            items = new XmlDocument();
            items.Load(itemStream);
            itemStream.Dispose();

            Items = new Dictionary<string, _Item>();
            foreach (XmlNode node in items.SelectNodes("randomizer/item"))
            {
                _Item item = ProcessXmlNodeAsItem(node);
                Items.Add(item.name, item);
            }


            Stream locationStream = a.GetManifestResourceStream("ItemChanger.Resources.locations.xml");
            locations = new XmlDocument();
            locations.Load(locationStream);
            locationStream.Dispose();

            Locations = new Dictionary<string, _Location>();
            foreach (XmlNode node in locations.SelectNodes("randomizer/item"))
            {
                _Location location = ProcessXmlNodeAsLocation(node);
                Locations.Add(location.name, location);
            }

            Stream platformStream = a.GetManifestResourceStream("ItemChanger.Resources.platforms.xml");
            platforms = new XmlDocument();
            platforms.Load(platformStream);
            platformStream.Dispose();

            Platform.Platforms = new Dictionary<string, List<Platform>>();
            foreach (XmlNode node in platforms.SelectNodes("randomizer/plat"))
            {
                if (!Platform.Platforms.ContainsKey(node["sceneName"].InnerText))
                {
                    Platform.Platforms.Add(node["sceneName"].InnerText, new List<Platform>());
                }

                Platform.Platforms[node["sceneName"].InnerText].Add(new Platform
                {
                    sceneName = node["sceneName"].InnerText,
                    x = float.Parse(node["x"].InnerText),
                    y = float.Parse(node["y"].InnerText)
                });
            }

        }



        static Dictionary<string, FieldInfo> itemFields;
        public static _Item ProcessXmlNodeAsItem(XmlNode node)
        {
            if (itemFields == null)
            {
                itemFields = new Dictionary<string, FieldInfo>();
                typeof(_Item).GetFields().ToList().ForEach(f => itemFields.Add(f.Name, f));
            }

            XmlAttribute nameAttr = node.Attributes?["name"];
            if (nameAttr == null)
            {
                LogWarn("Node in items.xml has no name attribute");
                return new _Item();
            }

            // Setting as object prevents boxing in FieldInfo.SetValue calls
            object item = new _Item();
            itemFields["name"].SetValue(item, nameAttr.InnerText);

            foreach (XmlNode fieldNode in node.ChildNodes)
            {
                if (!itemFields.TryGetValue(fieldNode.Name, out FieldInfo field))
                {
                    LogWarn(
                        $"Xml node \"{fieldNode.Name}\" does not map to a field in struct Item");
                    continue;
                }

                if (field.FieldType == typeof(string))
                {
                    field.SetValue(item, fieldNode.InnerText);
                }
                else if (field.FieldType == typeof(bool))
                {
                    if (bool.TryParse(fieldNode.InnerText, out bool xmlBool))
                    {
                        field.SetValue(item, xmlBool);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to bool");
                    }
                }
                else if (field.FieldType == typeof(_Item.ItemType))
                {
                    if (fieldNode.InnerText.TryToEnum(out _Item.ItemType type))
                    {
                        field.SetValue(item, type);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to ItemType");
                    }
                }
                else if (field.FieldType == typeof(_Item.GiveAction))
                {
                    if (fieldNode.InnerText.TryToEnum(out _Item.GiveAction type))
                    {
                        field.SetValue(item, type);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to GiveAction");
                    }
                }
                else if (field.FieldType == typeof(_Item.ItemPool))
                {
                    if (fieldNode.InnerText.TryToEnum(out _Item.ItemPool type))
                    {
                        field.SetValue(item, type);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to ItemPool");
                    }
                }
                else if (field.FieldType == typeof(int))
                {
                    if (int.TryParse(fieldNode.InnerText, out int xmlInt))
                    {
                        field.SetValue(item, xmlInt);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to int");
                    }
                }
                else if (field.FieldType == typeof(float))
                {
                    if (float.TryParse(fieldNode.InnerText, out float xmlFloat))
                    {
                        field.SetValue(item, xmlFloat);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to float");
                    }
                }
                else if (field.FieldType == typeof(Sprite))
                {
                    if (SpriteManager.GetSprite(fieldNode.InnerText) is Sprite sprite)
                    {
                        field.SetValue(item, sprite);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to Sprite");
                    }
                }
                else
                {
                    LogWarn("Unsupported type in Item: " + field.FieldType.Name);
                }
            }

            LogDebug($"Parsed XML for item \"{nameAttr.InnerText}\"");
            return (_Item)item;
        }

        /*
        public static void WriteItemXml()
        {
            XmlWriter xw = XmlWriter.Create(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\Team Cherry\Hollow Knight\items.xml"
                , new XmlWriterSettings { Indent = true });
            xw.WriteStartElement("randomizer");
            FieldInfo[] fields = typeof(Item).GetFields();
            List<string> fieldOrder = new List<string>
            {
                "type",
                "action",

                "boolName",
                "altBoolName",
                "intName",

                "geo",
                "essence",
                "shopPrice",

                "uiName",
                "shopDesc",
                "sprite",

                "buttonKey",
                "bigTakeText",
                "bigDescOne",
                "bigDescTwo",
                "bigSprite",

            };
            fields = fields.Where(f => fieldOrder.Contains(f.Name)).OrderBy(f => fieldOrder.IndexOf(f.Name)).ToArray();

            foreach (Item item in Items.Values)
            {
                xw.WriteStartElement("item");
                xw.WriteAttributeString("name", item.name);
                foreach (FieldInfo field in fields)
                {
                    if (field.Name == "name") { }

                    else if (field.FieldType == typeof(string) && (string)field.GetValue(item) != null)
                    {
                        xw.WriteElementString(field.Name, (string)field.GetValue(item));
                    }
                    else if (field.FieldType == typeof(int) && (int)field.GetValue(item) != 0)
                    {
                        xw.WriteElementString(field.Name, field.GetValue(item).ToString());
                    }
                    else if (field.FieldType == typeof(Item.GiveAction) && item.action != Item.GiveAction.Bool)
                    {
                        xw.WriteElementString(field.Name, item.action.ToString());
                    }
                    else if (field.FieldType == typeof(Item.ItemType) && item.type != Item.ItemType.Generic)
                    {
                        xw.WriteElementString(field.Name, item.type.ToString());
                    }
                    else if (field.FieldType == typeof(Sprite) && field.GetValue(item) != null)
                    {
                        xw.WriteElementString(field.Name, SpriteManager._sprites.First(kvp => kvp.Value == (Sprite)field.GetValue(item)).Key);
                    }
                }
                xw.WriteEndElement();
            }
            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();
        }
        */

        static Dictionary<string, FieldInfo> locFields;
        public static _Location ProcessXmlNodeAsLocation(XmlNode node)
        {
            if (locFields == null)
            {
                locFields = new Dictionary<string, FieldInfo>();
                typeof(_Location).GetFields().ToList().ForEach(f => locFields.Add(f.Name, f));
            }

            XmlAttribute nameAttr = node.Attributes?["name"];
            if (nameAttr == null)
            {
                LogWarn("Node in locations.xml has no name attribute");
                return new _Location();
            }

            // Setting as object prevents boxing in FieldInfo.SetValue calls
            object location = new _Location();
            locFields["name"].SetValue(location, nameAttr.InnerText);

            foreach (XmlNode fieldNode in node.ChildNodes)
            {
                if (!locFields.TryGetValue(fieldNode.Name, out FieldInfo field))
                {
                    LogWarn(
                        $"Xml node \"{fieldNode.Name}\" does not map to a field in struct Location");
                    continue;
                }

                if (field.FieldType == typeof(string))
                {
                    field.SetValue(location, fieldNode.InnerText);
                }
                else if (field.FieldType == typeof(string[]))
                {
                    field.SetValue(location, fieldNode.InnerText.Split(','));
                }
                else if (field.FieldType == typeof(bool))
                {
                    if (bool.TryParse(fieldNode.InnerText, out bool xmlBool))
                    {
                        field.SetValue(location, xmlBool);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to bool");
                    }
                }
                else if (field.FieldType == typeof(CostType))
                {
                    if (fieldNode.InnerText.TryToEnum(out CostType type))
                    {
                        field.SetValue(location, type);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to CostType");
                    }
                }
                else if (field.FieldType == typeof(_Location.SpecialFSMEdit))
                {
                    if (fieldNode.InnerText.TryToEnum(out _Location.SpecialFSMEdit type))
                    {
                        field.SetValue(location, type);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to CostType");
                    }
                }
                else if (field.FieldType == typeof(_Location.SpecialPDHook))
                {
                    if (fieldNode.InnerText.TryToEnum(out _Location.SpecialPDHook type))
                    {
                        field.SetValue(location, type);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to CostType");
                    }
                }
                else if (field.FieldType == typeof(_Location.LocationPool))
                {
                    if (fieldNode.InnerText.TryToEnum(out _Location.LocationPool type))
                    {
                        field.SetValue(location, type);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to LocationPool");
                    }
                }
                else if (field.FieldType == typeof(int))
                {
                    if (int.TryParse(fieldNode.InnerText, out int xmlInt))
                    {
                        field.SetValue(location, xmlInt);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to int");
                    }
                }
                else if (field.FieldType == typeof(float))
                {
                    if (float.TryParse(fieldNode.InnerText, out float xmlFloat))
                    {
                        field.SetValue(location, xmlFloat);
                    }
                    else
                    {
                        LogWarn($"Could not parse \"{fieldNode.InnerText}\" to float");
                    }
                }
                else
                {
                    LogWarn("Unsupported type in Location: " + field.FieldType.Name);
                }
            }

            LogDebug($"Parsed XML for item \"{nameAttr.InnerText}\"");
            return (_Location)location;
        }

    }
}
