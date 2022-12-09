namespace ItemChanger
{
    /// <summary>
    /// Enum used to communicate compatibility with different UIDef types.
    /// </summary>
    [Flags]
    public enum MessageType
    {
        None = 0,
        /// <summary>
        /// A message which shows a sprite and text in the bottom-left corner without taking control.
        /// </summary>
        Corner = 1,
        /// <summary>
        /// A message which takes control and shows a fullscreen popup.
        /// </summary>
        Big = 2,
        /// <summary>
        /// A message which takes control and starts a dialogue prompt.
        /// </summary>
        Lore = 4,
        Any = Corner | Big | Lore,
    }

    /// <summary>
    /// Enum for the current state of an item, to determine whether it has been given and whether it is eligible to be given.
    /// </summary>
    public enum ObtainState
    {
        /// <summary>
        /// The item has never been given, and is eligible to be given.
        /// </summary>
        Unobtained,
        /// <summary>
        /// The item has been given, and is no longer eligible to be given.
        /// </summary>
        Obtained,
        /// <summary>
        /// The item was previously given, but it has been refreshed and is reeligible to be given.
        /// </summary>
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
        Previewed = 1 << 1,
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

    /// <summary>
    /// Enum for controlling respawn behavior of items.
    /// </summary>
    public enum Persistence
    {
        /// <summary>
        /// Indicates the item should not be respawned.
        /// </summary>
        Single,
        /// <summary>
        /// Indicates the item should be respawned when the game resets semipersistent items (on bench, death, and a few world events).
        /// </summary>
        SemiPersistent,
        /// <summary>
        /// Indicates the item should be respawned after any scene load.
        /// </summary>
        Persistent,
    }

    /// <summary>
    /// Unused.
    /// </summary>
    public enum DropType
    {
        Fling,
        StraightDown,
    }

    /// <summary>
    /// Enum for controlling how items (particularly geo) should be flung from a location.
    /// </summary>
    public enum FlingType
    {
        /// <summary>
        /// Any fling behavior is acceptable.
        /// </summary>
        Everywhere,
        /// <summary>
        /// Items should not be flung horizontally.
        /// </summary>
        StraightUp,
        /// <summary>
        /// Items should not be flung at all.
        /// </summary>
        DirectDeposit
    }

    /// <summary>
    /// Enum for controlling how a shiny should be flung when activated.
    /// </summary>
    public enum ShinyFling
    {
        /// <summary>
        /// The shiny should fall straight down.
        /// </summary>
        Down,
        /// <summary>
        /// The shiny should be flung to the left.
        /// </summary>
        Left,
        /// <summary>
        /// The shiny should be flung to the right.
        /// </summary>
        Right,
        /// <summary>
        /// The shiny should be flung to the left or right, randomly.
        /// </summary>
        RandomLR,
        None,
    }

    /// <summary>
    /// Enum which provides additional information for serialization and other tag handling purposes.
    /// </summary>
    [Flags]
    public enum TagHandlingFlags
    {
        None = 0,
        /// <summary>
        /// If set, and an error occurs when deserializing this object as part of a TaggableObject's tags list, an InvalidTag will be created with the data of this object, and deserialization will continue.
        /// </summary>
        AllowDeserializationFailure = 1,
        /// <summary>
        /// If set, indicates to consumers that this tag should be removed if the current IC data is copied into a new profile.
        /// </summary>
        RemoveOnNewProfile = 2,
    }

    /// <summary>
    /// Enum for controlling what should happen when a placement is added, and another placement with the same name already exists in settings.
    /// </summary>
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
        /// <summary>
        /// A duplicate placement will result in an ArgumentException.
        /// </summary>
        Throw
    }

    /// <summary>
    /// Enum for adding special behvaior to the respawn marker tied to a StartDef.
    /// </summary>
    [Flags]
    public enum SpecialStartEffects
    {
        None = 0,
        DelayedWake = 1,
        SlowSoulRefill = 1 | 1 << 1,
        ExtraInvincibility = 1 << 2,

        Default = DelayedWake | ExtraInvincibility,
    }

    /// <summary>
    /// Enum used to refer to the different geo rock prefabs that are supported.
    /// </summary>
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

    /// <summary>
    /// Enum used to refer to the different soul totem prefabs that are supported.
    /// </summary>
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

    /// <summary>
    /// Enum for describing the text alignment and background of a lore tablet.
    /// </summary>
    public enum TextType
    {
        /// <summary>
        /// Left-aligned text with a normal text box background.
        /// </summary>
        LeftLore,         // Some lore tablets (the Lurien tablet) have their text left aligned
        /// <summary>
        /// Centered text with a normal text box background. Describes the majority of lore tablets.
        /// </summary>
        Lore,             // Normal Lore tablet (text is top-centre - applies to most, but not all, of the tablets)
        /// <summary>
        /// Cenetered text with a special lore prompt background. Used by tablets in King's Pass, Howling Cliffs, Abyss, and Black Egg Temple.
        /// </summary>
        MajorLore         // "Major" Lore tablet (bring up the lore background, etc)
    }

    /// <summary>
    /// Flags enum used to specify items that should not be removed from a shop by a ShopLocation.
    /// </summary>
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
        LemmRelics = 262144,
    }

    /// <summary>
    /// Flags enum used to specify a subset of rewards from the Seer.
    /// </summary>
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

    /// <summary>
    /// Flags enum used to specify a subset of rewards from Grubfather.
    /// </summary>
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

    /// <summary>
    /// Enum used to specify an operation for comparing two numbers.
    /// </summary>
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
