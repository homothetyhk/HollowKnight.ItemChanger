using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SereCore;
using UnityEngine;
using static ItemChanger.Default.Shops;

namespace ItemChanger.Placements
{
    public class ShopPlacement : AbstractPlacement
    {
        public string objectName;
        public string fsmName;

        public DefaultShopItems defaultShopItems;
        public string requiredPlayerDataBool;
        public bool dungDiscount;

        public override string SceneName => sceneName;
        public string sceneName;

        public List<int> costs = new List<int>();

        public override void OnEnableFsm(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == fsmName && fsm.gameObject.name == objectName)
            {
                ShopMenuStock shop = fsm.gameObject.GetComponent<ShopMenuStock>();
                GameObject itemPrefab = UnityEngine.Object.Instantiate(shop.stock[0]);
                itemPrefab.SetActive(false);

                shop.stock = GetNewStock(shop.stock, itemPrefab);
                if (shop.stockAlt != null)
                {
                    shop.stockAlt = GetNewAltStock(shop.stock, shop.stockAlt, itemPrefab);
                }
            }
        }

        public void AddItemWithCost(AbstractItem item, int cost)
        {
            items.Add(item);
            costs.Add(cost);
        }

        public GameObject[] GetNewStock(GameObject[] oldStock, GameObject shopPrefab)
        {
            List<GameObject> stock = new List<GameObject>(oldStock.Length + items.Count);
            foreach (var item in items)
            {
                GameObject shopItem = UnityEngine.Object.Instantiate(shopPrefab);
                shopItem.SetActive(false);
                ApplyItemDef(shopItem.GetComponent<ShopItemStats>(), item);
                stock.Add(shopItem);
            }
            stock.AddRange(oldStock.Where(g => TestVanillaShopItem(defaultShopItems, SceneName, g.GetComponent<ShopItemStats>())));

            return stock.ToArray();
        }

        public GameObject[] GetNewAltStock(GameObject[] newStock, GameObject[] altStock, GameObject shopPrefab)
        {
            return newStock.Concat(altStock.Where(g => TestVanillaAltShopItem(defaultShopItems, g.GetComponent<ShopItemStats>()))).ToArray();
        }

        public override void OnHook()
        {
            Modding.ModHooks.Instance.GetPlayerBoolHook += ShopGetBoolOverride;
            Modding.ModHooks.Instance.SetPlayerBoolHook += ShopSetBoolOverride;
        }

        public bool ShopGetBoolOverride(string boolName)
        {
            if (boolName.StartsWith(GetShopBoolPrefix())
                && int.TryParse(boolName.Split('.').Last(), out int index)
                && index >= 0 && index < items.Count)
            {
                return items[index].IsObtained();
            }

            return PlayerData.instance.GetBoolInternal(boolName);
        }

        public void ShopSetBoolOverride(string boolName, bool value)
        {
            if (boolName.StartsWith(GetShopBoolPrefix())
                && int.TryParse(boolName.Split('.').Last(), out int index)
                && index >= 0 && index < items.Count && value)
            {
                items[index].Give(this, Container.Shop, FlingType.DirectDeposit, null, MessageType.Corner);
                return;
            }

            PlayerData.instance.SetBoolInternal(boolName, value);
        }

        public string GetShopBoolPrefix() => $"ItemChanger.Locations.{name}";
        public string GetShopBool(AbstractItem item) => $"{GetShopBoolPrefix()}.{items.IndexOf(item)}";
        public bool TryGetShopBoolItem(string shopBool, out AbstractItem item)
        {
            if (shopBool.StartsWith(GetShopBoolPrefix())
                && int.TryParse(shopBool.Split('.').Last(), out int index)
                && index >= 0 && index < items.Count)
            {
                item = items[index];
                return true;
            }

            item = null;
            return false;
        }

        public void ApplyItemDef(ShopItemStats stats, AbstractItem item)
        {
            item.UIDef.GetShopData(out Sprite sprite, out string nameKey, out string descKey);

            // Apply all the stored values
            stats.playerDataBoolName = GetShopBool(item);
            stats.nameConvo = nameKey;
            stats.descConvo = descKey;
            stats.requiredPlayerDataBool = requiredPlayerDataBool;
            stats.removalPlayerDataBool = string.Empty;
            stats.dungDiscount = dungDiscount;
            if (item is Items.CharmItem charm && int.TryParse(charm.fieldName.Split('_').Last(), out int charmNum))
            {
                stats.notchCostBool = $"charmCost_{charmNum}";
            }
            if (item is Items.EquippedCharmItem echarm)
            {
                stats.notchCostBool = $"charmCost_{echarm.charmNum}";
            }
            int index = items.IndexOf(item);
            if (index >= 0 && index < costs.Count)
            {
                stats.cost = costs[index];
            }
            else stats.cost = 1;

            // Need to set all these to make sure the item doesn't break in one of various ways
            stats.priceConvo = string.Empty;
            stats.specialType = 0;
            stats.charmsRequired = 0;
            stats.relic = false;
            stats.relicNumber = 0;
            stats.relicPDInt = string.Empty;

            // Apply the sprite for the UI
            stats.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        }

        public static bool TestVanillaShopItem(DefaultShopItems shopItems, string shopScene, ShopItemStats stats)
        {
            switch (shopScene)
            {
                case SceneNames.Room_mapper:
                    switch (stats.specialType)
                    {
                        // Map marker
                        case 17 when (shopItems & DefaultShopItems.IseldaMapMarkers) == DefaultShopItems.IseldaMapMarkers:
                        // Map pin
                        case 16 when (shopItems & DefaultShopItems.IseldaMapPins) == DefaultShopItems.IseldaMapPins:
                        // Quill
                        case 0 when stats.playerDataBoolName == nameof(PlayerData.hasQuill) && (shopItems & DefaultShopItems.IseldaQuill) == DefaultShopItems.IseldaQuill:
                        // Map
                        case 9 when (shopItems & DefaultShopItems.IseldaMaps) == DefaultShopItems.IseldaMaps:
                            return true;
                        default:
                            return false;
                    }

                case SceneNames.Room_shop:
                    switch (stats.specialType)
                    {
                        // sly mask shards
                        case 1 when (shopItems & DefaultShopItems.SlyMaskShards) == DefaultShopItems.SlyMaskShards:
                        // sly charms
                        case 2 when (shopItems & DefaultShopItems.SlyCharms) == DefaultShopItems.SlyCharms:
                        // sly vessel fragments
                        case 3 when (shopItems & DefaultShopItems.SlyVesselFragments) == DefaultShopItems.SlyVesselFragments:
                        // sly simple key
                        case 10 when (shopItems & DefaultShopItems.SlySimpleKey) == DefaultShopItems.SlySimpleKey:
                        // sly rancid egg
                        case 11 when (shopItems & DefaultShopItems.SlyRancidEgg) == DefaultShopItems.SlyRancidEgg:
                        // sly lantern
                        case 0 when stats.playerDataBoolName == nameof(PlayerData.hasLantern) && (shopItems & DefaultShopItems.SlyLantern) == DefaultShopItems.SlyLantern:
                            return true;
                        default:
                            return false;
                    }

                case SceneNames.Room_Charm_Shop:
                    switch (stats.specialType)
                    {
                        case 2 when (shopItems & DefaultShopItems.SalubraCharms) == DefaultShopItems.SalubraCharms:
                        case 8 when (shopItems & DefaultShopItems.SalubraNotches) == DefaultShopItems.SalubraNotches:
                            return true;
                        default:
                            return false;
                    }

                case SceneNames.Fungus2_26:
                    switch (stats.specialType)
                    {
                        // fragile charms
                        case 2 when (shopItems & DefaultShopItems.LegEaterCharms) == DefaultShopItems.LegEaterCharms:
                        // fragile repair
                        case 12 when (shopItems & DefaultShopItems.LegEaterRepair) == DefaultShopItems.LegEaterRepair:
                        case 13 when (shopItems & DefaultShopItems.LegEaterRepair) == DefaultShopItems.LegEaterRepair:
                        case 14 when (shopItems & DefaultShopItems.LegEaterRepair) == DefaultShopItems.LegEaterRepair:
                            return true;
                        default:
                            return false;
                    }

                default:
                    return false;
            }
        }
        public static bool TestVanillaAltShopItem(DefaultShopItems shopItems, ShopItemStats stats)
        {
            switch (stats.specialType)
            {
                // sly mask shards
                case 1 when (shopItems & DefaultShopItems.SlyMaskShards) == DefaultShopItems.SlyMaskShards:
                // sly charms
                case 2 when stats.requiredPlayerDataBool != nameof(PlayerData.gaveSlykey) && (shopItems & DefaultShopItems.SlyCharms) == DefaultShopItems.SlyCharms:
                // sly key charms
                case 2 when stats.requiredPlayerDataBool == nameof(PlayerData.gaveSlykey) && (shopItems & DefaultShopItems.SlyKeyCharms) == DefaultShopItems.SlyKeyCharms:
                // sly vessel fragments
                case 3 when (shopItems & DefaultShopItems.SlyVesselFragments) == DefaultShopItems.SlyVesselFragments:
                // sly simple key
                case 10 when (shopItems & DefaultShopItems.SlySimpleKey) == DefaultShopItems.SlySimpleKey:
                // sly rancid egg
                case 11 when (shopItems & DefaultShopItems.SlyRancidEgg) == DefaultShopItems.SlyRancidEgg:
                // sly lantern
                case 0 when stats.playerDataBoolName == nameof(PlayerData.hasLantern) && (shopItems & DefaultShopItems.SlyLantern) == DefaultShopItems.SlyLantern:
                // sly key elegant key
                case 0 when stats.playerDataBoolName == nameof(PlayerData.hasWhiteKey) && (shopItems & DefaultShopItems.SlyKeyElegantKey) == DefaultShopItems.SlyKeyElegantKey:
                    return true;
                default:
                    return false;
            }
        }



        // TODO: Implement ShopLocation

    }
}
