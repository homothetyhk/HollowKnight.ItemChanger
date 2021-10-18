using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    [Flags]
    public enum MessageType
    {
        None = 0,
        Corner = 1,
        Big = 2,
        Lore = 4,
        Any = Corner | Big | Lore,
    }

    public enum ObtainState
    {
        Unobtained,
        Obtained,
        Refreshed
    }

    [Flags]
    public enum VisitState
    {
        None = 0,
        ObtainedAnyItem = 1 << 0,
        /// <summary>
        /// Applies to shops, placements with preview dialogues, and placements with hint boxes.
        /// </summary>
        Previewed = 1 << 1, // TODO: add to shops, add to preview dialogues
        /// <summary>
        /// Corresponds to opening a container: e.g. opening a chest, breaking a grub jar or geo rock, etc.
        /// </summary>
        Opened = 1 << 2,
        /// <summary>
        /// Applies to enemy drop items.
        /// </summary>
        Dropped = 1 << 3,
        /// <summary>
        /// Applies to placements offered by NPCs (Cornifer, Nailmasters). Usually set to indicate that the NPC is no longer required to make the offer when items respawn.
        /// </summary>
        Accepted = 1 << 4,
        /// <summary>
        /// Defined on a per-placement basis.
        /// </summary>
        Special = 1 << 31,
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

    public enum PlacementConflictResolution
    {
        /// <summary>
        /// Keep new placement, discard old placement, and append items of old placement to new placement.
        /// </summary>
        MergeKeepingNew,
        /// <summary>
        /// Keep old placement, discard new placement, and append items of new placement to old placement.
        /// </summary>
        MergeKeepingOld,
        /// <summary>
        /// Keep new placement, discard old placement
        /// </summary>
        Replace,
        /// <summary>
        /// Keep old placement, discard new placement
        /// </summary>
        Ignore,
        Throw
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

    public enum SoulTotemSubtype
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        Palace,
        PathOfPain
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
        LegEaterRepair = 65536,
        SalubraBlessing = 131072,
    }

    [Flags]
    public enum SeerRewards
    {
        None = 0,
        dreamReward1 = 1 << 0,
        HallownestSeal = dreamReward1,
        dreamReward2 = 1 << 1,
        GladeDoor = dreamReward2,
        dreamReward3 = 1 << 2,
        PaleOre = dreamReward3,
        dreamReward4 = 1 << 3,
        DreamWielder = dreamReward4,
        dreamReward5 = 1 << 4,
        VesselFragment = dreamReward5,
        dreamReward5b = 1 << 5,
        DreamGate = dreamReward5b,
        dreamReward6 = 1 << 6,
        ArcaneEgg = dreamReward6,
        dreamReward7 = 1 << 7,
        MaskShard = dreamReward7,
        dreamReward8 = 1 << 8,
        AwokenDreamNail = dreamReward8,
        dreamReward9 = 1 << 9,
        Ascension = dreamReward9,
        All = (1 << 10) - 1,
        // Warning - enum representation chosen by ToString is not specified by c#
    }

    [Flags]
    public enum GrubfatherRewards : long
    {
        None = 0,
        grubReward1 = 1L << 0,
        grubReward2 = 1L << 1,
        grubReward3 = 1L << 2,
        grubReward4 = 1L << 3,
        grubReward5 = 1L << 4,
        MaskShard = grubReward5,
        grubReward6 = 1L << 5,
        grubReward7 = 1L << 6,
        grubReward8 = 1L << 7,
        grubReward9 = 1L << 8,
        grubReward10 = 1L << 9,
        Grubsong = grubReward10,
        grubReward11 = 1L << 10,
        grubReward12 = 1L << 11,
        grubReward13 = 1L << 12,
        grubReward14 = 1L << 13,
        grubReward15 = 1L << 14,
        grubReward16 = 1L << 15,
        RancidEgg = grubReward16,
        grubReward17 = 1L << 16,
        grubReward18 = 1L << 17,
        grubReward19 = 1L << 18,
        grubReward20 = 1L << 19,
        grubReward21 = 1L << 20,
        grubReward22 = 1L << 21,
        grubReward23 = 1L << 22,
        HallownestSeal = grubReward23,
        grubReward24 = 1L << 23,
        grubReward25 = 1L << 24,
        grubReward26 = 1L << 25,
        grubReward27 = 1L << 26,
        grubReward28 = 1L << 27,
        grubReward29 = 1L << 28,
        grubReward30 = 1L << 29,
        grubReward31 = 1L << 30,
        PaleOre = grubReward31,
        grubReward32 = 1L << 31,
        grubReward33 = 1L << 32,
        grubReward34 = 1L << 33,
        grubReward35 = 1L << 34,
        grubReward36 = 1L << 35,
        grubReward37 = 1L << 36,
        grubReward38 = 1L << 37,
        KingsIdol = grubReward38,
        grubReward39 = 1L << 38,
        grubReward40 = 1L << 39,
        grubReward41 = 1L << 40,
        grubReward42 = 1L << 41,
        grubReward43 = 1L << 42,
        grubReward44 = 1L << 43,
        grubReward45 = 1L << 44,
        grubReward46 = 1L << 45,
        GrubberflysElegy = grubReward46,
        All = (1L << 46) - 1,
        AllNonGeo = MaskShard | Grubsong | RancidEgg | HallownestSeal | PaleOre | KingsIdol | GrubberflysElegy,
    }


    public enum ComparisonOperator
    {
        Eq,
        Neq,
        Lt,
        Le,
        Gt,
        Ge
    }
}
