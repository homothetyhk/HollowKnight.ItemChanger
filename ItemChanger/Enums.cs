using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public enum ObtainState
    {
        Unobtained,
        Obtained,
        Refreshed
    }

    public enum Persistence
    {
        Single,
        SemiPersistent,
        Persistent,
    }

    public enum DropType
    {
        Fling,
        StraightDown,
    }

    public enum FlingType
    {
        Everywhere,
        StraightUp,
        DirectDeposit
    }

    public enum Container
    {
        Shiny,
        Chest,
        GrubJar,
        GeoRock,
        Shop,
        NPCDialogue,
        Tablet,
        Unknown,
    }


    public enum CostType
    {
        None = 0,
        Geo,
        Essence,
        Simple,
        Grub,
        Wraiths,
        Dreamnail,
        WhisperingRoot,
        Spore,
        Flame,
    }

    public enum ItemType
    {
        Big,
        Charm,
        Trinket,
        Shop,
        Spell,
        Geo,
        Soul,
        Lifeblood,
        Flame,
        Lore
    }

    public enum GeoRockSubtype
    {
        Default,
        Abyss,
        City,
        Deepnest,
        Fung01,
        Fung02,
        Grave01,
        Grave02,
        GreenPath01,
        GreenPath02,
        Hive,
        Mine,
        Outskirts,
        Outskirts420
    }

    public enum TextType
    {
        LeftLore,         // Some lore tablets (the Lurien tablet) have their text left aligned
        Lore,             // Normal Lore tablet (text is top-centre - applies to most, but not all, of the tablets)
        MajorLore         // "Major" Lore tablet (bring up the lore background, etc)
    }

    [Flags]
    public enum DefaultShopItems
    {
        None = 0,
        LegEaterCharms = 1,
        SalubraCharms = 2,
        SalubraNotches = 4,
        IseldaCharms = 8,
        IseldaMaps = 16,
        IseldaMapPins = 32,
        IseldaMapMarkers = 64,
        IseldaQuill = 128,
        SlyCharms = 256,
        SlyMaskShards = 512,
        SlyVesselFragments = 1024,
        SlyLantern = 2048,
        SlySimpleKey = 4096,
        SlyRancidEgg = 8192,
        SlyKeyCharms = 16384,
        SlyKeyElegantKey = 32768,
        LegEaterRepair = 65536
    }
}
