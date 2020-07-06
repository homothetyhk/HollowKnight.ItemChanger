using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;

namespace ItemChanger
{
    public struct Item
    {
        public string name;

        public string additiveGroup;
        public int additiveIndex;

        public string boolName;
        public string altBoolName;
        public string intName;

        public int geo;
        public int essence;
        public int shopPrice;

        
        public int charmNum;
        public string equipBoolName;
        public string notchCost;

        public Sprite sprite;
        public string nameKey;
        public string shopDescKey;

        public Sprite bigSprite;
        public string takeKey;
        public string buttonKey;
        public string descOneKey;
        public string descTwoKey;
        

        public ItemType type;
        public GiveAction action;
        public ItemPool pool;
        public Action customAction;

        public override string ToString()
        {
            return name;
        }

        public Item(string defaultItemName)
        {
            if (!XmlManager.Items.TryGetValue(defaultItemName, out Item val))
            {
                Modding.Logger.LogError($"No default item found corresponding to {defaultItemName}");
                throw new KeyNotFoundException();
            }
            this = val;
        }
        public enum ItemType
        {
            Generic = 0,
            Big,
            Geo
        }

        public enum GiveAction
        {
            Bool = 0,
            Int,
            Charm,
            EquippedCharm,
            SpawnGeo,
            AddGeo,

            Map,
            Grub,
            Essence,
            Stag,
            DirtmouthStag,

            MaskShard,
            VesselFragment,
            WanderersJournal,
            HallownestSeal,
            KingsIdol,
            ArcaneEgg,

            Dreamer,
            Kingsoul,

            Grimmchild2,

            SettingsBool,
            Custom,
            None
        }

        public enum ItemPool
        {
            None,
            Dreamer,
            Skill,
            Charm,
            Key,
            Mask,
            Vessel,
            Ore,
            Notch,
            Geo,
            Egg,
            Relic,
            Map,
            Stag,
            Grub,
            Root,

            Dupe,
            Cursed,
            Custom,
        }
    }

    
}
