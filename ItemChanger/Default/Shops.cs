using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Default
{
    public static class Shops
    {
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

        public const DefaultShopItems Basic = Completionist | DefaultShopItems.SalubraNotches | DefaultShopItems.SlyMaskShards | DefaultShopItems.SlyVesselFragments;

        public const DefaultShopItems Completionist = JunkPit | DefaultShopItems.SlyRancidEgg;

        public const DefaultShopItems JunkPit = MiniSuperJunkPit;

        public const DefaultShopItems MiniSuperJunkPit =
            DefaultShopItems.IseldaMaps | DefaultShopItems.IseldaQuill | SuperJunkPit;

        public const DefaultShopItems SuperJunkPit = DefaultShopItems.IseldaMapMarkers | DefaultShopItems.IseldaMapPins | DefaultShopItems.LegEaterRepair;

        public const DefaultShopItems None = DefaultShopItems.None;

        public const DefaultShopItems All = Basic |
            DefaultShopItems.IseldaCharms | DefaultShopItems.SlyCharms | DefaultShopItems.LegEaterCharms | DefaultShopItems.SlyKeyCharms | DefaultShopItems.SalubraCharms |
            DefaultShopItems.SlySimpleKey | DefaultShopItems.SlyKeyElegantKey | DefaultShopItems.SlyLantern;
    }
}
