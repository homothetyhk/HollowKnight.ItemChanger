using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
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
}
