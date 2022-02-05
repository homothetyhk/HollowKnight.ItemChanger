using System.Reflection;
using TMPro;
using ItemChanger.Components;

namespace ItemChanger.Util
{
    public static class ShopUtil
    {
        private static readonly FieldInfo spawnedStockField = typeof(ShopMenuStock).GetField("spawnedStock", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo spawnStockMethod = typeof(ShopMenuStock).GetMethod("SpawnStock", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo itemCostField = typeof(ShopItemStats).GetField("itemCost", BindingFlags.NonPublic | BindingFlags.Instance);

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
                Cost cost = mod.cost;
                if (cost != null)
                {
                    if (self.dungDiscount && PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_10)))
                    {
                        cost.DiscountRate = 0.8f;
                    }
                    else
                    {
                        cost.DiscountRate = 1.0f;
                    }
                }

                if (cost == null || cost.Paid || cost.CanPay())
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

                int geo;
                if (!mod.placement.HasTag<Tags.DisableCostPreviewTag>() && !mod.item.HasTag<Tags.DisableCostPreviewTag>()
                && cost is not null && !cost.Paid)
                {
                    geo = cost.GetDisplayGeo();
                }
                else
                {
                    geo = 0;
                }
                self.SetCost(geo);
                ((GameObject)itemCostField.GetValue(self)).GetComponent<TextMeshPro>().text = geo.ToString();
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
            foreach (var g in self.stockInv
                .Select(go => go?.GetComponent<ModShopItemStats>())
                .Where(m => m != null)
                .GroupBy(m => m.placement))
            {
                if (g.Key is Placements.ShopPlacement shop)
                {
                    shop.OnPreviewBatch(g.Select(m => (m.GetRecordText(), m.item)));
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
            return shopScene switch
            {
                SceneNames.Room_mapper => stats.specialType switch
                {
                    17 => DefaultShopItems.IseldaMapMarkers,
                    16 => DefaultShopItems.IseldaMapPins,
                    0 when stats.playerDataBoolName == nameof(PlayerData.hasQuill) => DefaultShopItems.IseldaQuill,
                    9 => DefaultShopItems.IseldaMaps,
                    2 => DefaultShopItems.IseldaCharms,
                    _ => null,
                },
                SceneNames.Room_shop => stats.specialType switch
                {
                    1 => DefaultShopItems.SlyMaskShards,
                    2 when stats.requiredPlayerDataBool != nameof(PlayerData.gaveSlykey) => DefaultShopItems.SlyCharms,
                    2 when stats.requiredPlayerDataBool == nameof(PlayerData.gaveSlykey) => DefaultShopItems.SlyKeyCharms,
                    3 => DefaultShopItems.SlyVesselFragments,
                    10 => DefaultShopItems.SlySimpleKey,
                    11 => DefaultShopItems.SlyRancidEgg,
                    0 when stats.playerDataBoolName == nameof(PlayerData.hasLantern) => DefaultShopItems.SlyLantern,
                    0 when stats.playerDataBoolName == nameof(PlayerData.hasWhiteKey) => DefaultShopItems.SlyKeyElegantKey,
                    _ => null,
                },
                SceneNames.Room_Charm_Shop => stats.specialType switch
                {
                    2 => DefaultShopItems.SalubraCharms,
                    8 => DefaultShopItems.SalubraNotches,
                    15 => DefaultShopItems.SalubraBlessing,
                    _ => null,
                },
                SceneNames.Fungus2_26 => stats.specialType switch
                {
                    2 => DefaultShopItems.LegEaterCharms,
                    12 or 13 or 14 => DefaultShopItems.LegEaterRepair,
                    _ => null,
                },
                _ => null,
            };
        }

    }
}
