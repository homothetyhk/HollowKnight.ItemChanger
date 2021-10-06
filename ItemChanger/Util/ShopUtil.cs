using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using TMPro;
using ItemChanger.Extensions;
using ItemChanger.Components;

namespace ItemChanger.Util
{
    public static class ShopUtil
    {
        private static FieldInfo spawnedStockField = typeof(ShopMenuStock).GetField("spawnedStock", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo spawnStockMethod = typeof(ShopMenuStock).GetMethod("SpawnStock", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void HookShops()
        {
            On.ShopMenuStock.BuildItemList += BuildItemList;
            On.ShopMenuStock.StockLeft += StockLeft;
            On.ShopItemStats.OnEnable += OnEnable;
        }

        private static void OnEnable(On.ShopItemStats.orig_OnEnable orig, ShopItemStats self)
        {
            orig(self);
            var mod = self.gameObject.GetComponent<ModShopItemStats>();
            if (mod)
            {
                if (mod.cost == null || mod.cost.Paid|| mod.cost.CanPay())
                {
                    self.transform.Find("Geo Sprite").gameObject.GetComponent<SpriteRenderer>().color = self.activeColour;
                    self.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().color = self.activeColour;
                    self.transform.Find("Item cost").gameObject.GetComponent<TextMeshPro>().color = self.activeColour;
                }
                else
                {
                    self.transform.Find("Geo Sprite").gameObject.GetComponent<SpriteRenderer>().color = self.inactiveColour;
                    self.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().color = self.inactiveColour;
                    self.transform.Find("Item cost").gameObject.GetComponent<TextMeshPro>().color = self.inactiveColour;
                }
            }
        }

        private static bool StockLeft(On.ShopMenuStock.orig_StockLeft orig, ShopMenuStock self)
        {
            return self.stock.Any(g => ShopMenuItemAppears(g));
        }

        public static void UnhookShops()
        {
            On.ShopMenuStock.BuildItemList -= BuildItemList;
            On.ShopMenuStock.StockLeft -= StockLeft;
            On.ShopItemStats.OnEnable -= OnEnable;
        }

        public static Dictionary<GameObject, GameObject> GetSpawnedStock(this ShopMenuStock stock)
        {
            var dict = spawnedStockField.GetValue(stock);
            if (dict == null)
            {
                spawnStockMethod.Invoke(stock, new object[0]);
                spawnedStockField.GetValue(stock);
            }

            return (Dictionary<GameObject, GameObject>)spawnedStockField.GetValue(stock);
        }

        private static void BuildItemList(On.ShopMenuStock.orig_BuildItemList orig, ShopMenuStock self)
        {
            Dictionary<GameObject, GameObject> spawnedStock = self.GetSpawnedStock();

            self.itemCount = -1;
            float num = 0f;
            self.stockInv = new GameObject[self.stock.Length];
            for (int i = 0; i < self.stock.Length; i++)
            {
                /*
                var stats = self.stock[i].GetComponent<ShopItemStats>();
                ItemChanger.instance.Log($"Item {i}");
                ItemChanger.instance.Log(stats.playerDataBoolName);
                ItemChanger.instance.Log(stats.requiredPlayerDataBool);
                ItemChanger.instance.Log(stats.removalPlayerDataBool);
                ItemChanger.instance.Log(stats.nameConvo);
                ItemChanger.instance.Log(stats.descConvo);
                ItemChanger.instance.Log(stats.specialType);
                */

                if (ShopMenuItemAppears(self.stock[i]))
                {
                    self.itemCount++;
                    if (!spawnedStock.TryGetValue(self.stock[i], out GameObject gameObject))
                    {
                        gameObject = GameObject.Instantiate(self.stock[i]);
                        gameObject.SetActive(false);
                        spawnedStock.Add(self.stock[i], gameObject);
                    }
                    if (gameObject.GetComponent<ModShopItemStats>() is ModShopItemStats mod && mod.item == null)
                    {
                        // Instantiate can't copy custom classes
                        var origMod = self.stock[i].GetComponent<ModShopItemStats>();
                        mod.item = origMod.item;
                        mod.cost = origMod.cost;
                        mod.placement = origMod.placement;
                    }
                    gameObject.transform.SetParent(self.transform, false);
                    gameObject.transform.localPosition = new Vector3(0f, num, 0f);
                    gameObject.GetComponent<ShopItemStats>().itemNumber = self.itemCount;
                    self.stockInv[self.itemCount] = gameObject;
                    num += self.yDistance;
                    gameObject.SetActive(true);
                }
            }
        }

        private static bool ShopMenuItemAppears(GameObject shopItem)
        {
            ShopItemStats stats = shopItem.GetComponent<ShopItemStats>();
            ModShopItemStats mod = shopItem.GetComponent<ModShopItemStats>();

            string requiredPD = stats.requiredPlayerDataBool;
            string removalPD = stats.removalPlayerDataBool;
            string PDBool = stats.playerDataBoolName;

            bool remove = PDTest(removalPD) ?? false;
            if (mod != null)
            {
                foreach (var t in mod.item.GetTags<Tags.IShopRemovalTag>())
                {
                    remove |= t.Remove;
                }
            }

            bool meetsRequirement = PDTest(requiredPD) ?? true;
            if (mod != null)
            {
                foreach (var t in mod.item.GetTags<Tags.IShopRequirementTag>())
                {
                    meetsRequirement &= t.MeetsRequirement;
                }
            }

            bool obtained = mod?.item?.IsObtained() ?? PDTest(PDBool) ?? false;

            return !obtained && !remove && meetsRequirement;

            bool? PDTest(string boolName)
            {
                switch (boolName)
                {
                    case null:
                    case "Null":
                    case "":
                        return null;
                    default:
                        return PlayerData.instance.GetBool(boolName);
                }
            }
        }

        public static DefaultShopItems? GetVanillaShopItemType(string shopScene, ShopItemStats stats)
        {
            switch (shopScene)
            {
                case SceneNames.Room_mapper:
                    switch (stats.specialType)
                    {
                        // Map marker
                        case 17:
                            return DefaultShopItems.IseldaMapMarkers;
                        // Map pin
                        case 16:
                            return DefaultShopItems.IseldaMapPins;
                        // Quill
                        case 0 when stats.playerDataBoolName == nameof(PlayerData.hasQuill):
                            return DefaultShopItems.IseldaQuill;
                        // Map
                        case 9:
                            return DefaultShopItems.IseldaMaps;
                        default:
                            return null;
                    }
                case SceneNames.Room_shop:
                    switch (stats.specialType)
                    {
                        // sly mask shards
                        case 1:
                            return DefaultShopItems.SlyMaskShards;
                        // sly charms
                        case 2 when stats.requiredPlayerDataBool != nameof(PlayerData.gaveSlykey):
                            return DefaultShopItems.SlyCharms;
                        // sly key charms
                        case 2 when stats.requiredPlayerDataBool == nameof(PlayerData.gaveSlykey):
                            return DefaultShopItems.SlyKeyCharms;
                        // sly vessel fragments
                        case 3:
                            return DefaultShopItems.SlyVesselFragments;
                        // sly simple key
                        case 10:
                            return DefaultShopItems.SlySimpleKey;
                        // sly rancid egg
                        case 11:
                            return DefaultShopItems.SlyRancidEgg;
                        // sly lantern
                        case 0 when stats.playerDataBoolName == nameof(PlayerData.hasLantern):
                            return DefaultShopItems.SlyLantern;
                        // sly key elegant key
                        case 0 when stats.playerDataBoolName == nameof(PlayerData.hasWhiteKey):
                            return DefaultShopItems.SlyKeyElegantKey;
                        default:
                            return null;
                    }
                case SceneNames.Room_Charm_Shop:
                    switch (stats.specialType)
                    {
                        case 2:
                            return DefaultShopItems.SalubraCharms;
                        case 8:
                            return DefaultShopItems.SalubraNotches;
                        case 15:
                            return DefaultShopItems.SalubraBlessing;
                        default:
                            return null;
                    }
                case SceneNames.Fungus2_26:
                    switch (stats.specialType)
                    {
                        // fragile charms
                        case 2:
                            return DefaultShopItems.LegEaterCharms;
                        // fragile repair
                        case 12:
                        case 13:
                        case 14:
                            return DefaultShopItems.LegEaterRepair;
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

    }
}
