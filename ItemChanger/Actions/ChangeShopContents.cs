using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;
using static Modding.Logger;
using System;
using Object = UnityEngine.Object;
using static ItemChanger.Default.Shops;
using SereCore;

namespace ItemChanger.Actions
{
    public struct ShopItemDef
    {
        // Values from ShopItemStats
        public string PlayerDataBoolName;
        public string NameConvo;
        public string DescConvo;
        public string RequiredPlayerDataBool;
        public string RemovalPlayerDataBool;
        public bool DungDiscount;
        public string NotchCostBool;
        public int Cost;

        // Sprite
        public Sprite Sprite;
    }




    public class ChangeShopContents : RandomizerAction, ISerializationCallbackReceiver
    {
        // Variable for serialization hack
        private List<string> _itemDefStrings;
        private ShopItemDef[] _items;

        // Variables that actually get used

        public ChangeShopContents(string sceneName, string objectName, ShopItemDef[] items, DefaultShopItems defaultShopItems)
        {
            SceneName = sceneName;
            ObjectName = objectName;
            _items = items;
            _defaultShopItems = defaultShopItems;
        }

        public override ActionType Type => ActionType.GameObject;

        public string SceneName { get; }

        public string ObjectName { get; }

        private readonly Default.Shops.DefaultShopItems _defaultShopItems;

        public void OnBeforeSerialize()
        {
            _itemDefStrings = new List<string>();
            foreach (ShopItemDef item in _items)
            {
                _itemDefStrings.Add(JsonUtility.ToJson(item));
            }
        }

        public void OnAfterDeserialize()
        {
            List<ShopItemDef> itemDefList = new List<ShopItemDef>();

            foreach (string item in _itemDefStrings)
            {
                itemDefList.Add(JsonUtility.FromJson<ShopItemDef>(item));
            }

            _items = itemDefList.ToArray();
        }

        public void AddItemDefs(ShopItemDef[] newItems)
        {
            if (_items == null)
            {
                _items = newItems;
                return;
            }

            if (newItems == null)
            {
                return;
            }

            ShopItemDef[] combined = new ShopItemDef[_items.Length + newItems.Length];
            _items.CopyTo(combined, 0);
            newItems.CopyTo(combined, _items.Length);
            _items = combined;
        }

        public override void Process(string scene, Object changeObj)
        {
            if (scene != SceneName)
            {
                return;
            }

            foreach (GameObject shopObj in Object.FindObjectsOfType<GameObject>())
            {
                if (shopObj.name != ObjectName) continue;

                ShopMenuStock shop = shopObj.GetComponent<ShopMenuStock>();
                GameObject itemPrefab = Object.Instantiate(shop.stock[0]);
                itemPrefab.SetActive(false);

                // Remove all charm type items from the store
                List<GameObject> newStock = new List<GameObject>();

                foreach (ShopItemDef itemDef in _items)
                {
                    // Create a new shop item for this item def
                    GameObject newItemObj = Object.Instantiate(itemPrefab);
                    newItemObj.SetActive(false);

                    // Apply all the stored values
                    ShopItemStats stats = newItemObj.GetComponent<ShopItemStats>();
                    stats.playerDataBoolName = itemDef.PlayerDataBoolName;
                    stats.nameConvo = itemDef.NameConvo;
                    stats.descConvo = itemDef.DescConvo;
                    stats.requiredPlayerDataBool = itemDef.RequiredPlayerDataBool;
                    stats.removalPlayerDataBool = itemDef.RemovalPlayerDataBool;
                    stats.dungDiscount = itemDef.DungDiscount;
                    stats.notchCostBool = itemDef.NotchCostBool;
                    stats.cost = itemDef.Cost;

                    // Need to set all these to make sure the item doesn't break in one of various ways
                    stats.priceConvo = string.Empty;
                    stats.specialType = 0;
                    stats.charmsRequired = 0;
                    stats.relic = false;
                    stats.relicNumber = 0;
                    stats.relicPDInt = string.Empty;

                    // Apply the sprite for the UI
                    stats.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = itemDef.Sprite;

                    newStock.Add(newItemObj);
                }

                foreach (GameObject g in shop.stock)
                {
                    ShopItemStats stats = g.GetComponent<ShopItemStats>();
                    switch (SceneName)
                    {
                        case SceneNames.Room_mapper:
                            switch (stats.specialType)
                            {
                                // Map marker
                                case 17 when (_defaultShopItems & DefaultShopItems.IseldaMapMarkers) == DefaultShopItems.IseldaMapMarkers:
                                // Map pin
                                case 16 when (_defaultShopItems & DefaultShopItems.IseldaMapPins) == DefaultShopItems.IseldaMapPins:
                                // Quill
                                case 0 when stats.playerDataBoolName == nameof(PlayerData.hasQuill) && ((_defaultShopItems & DefaultShopItems.IseldaQuill) == DefaultShopItems.IseldaQuill):
                                // Map
                                case 9 when (_defaultShopItems & DefaultShopItems.IseldaMaps) == DefaultShopItems.IseldaMaps:
                                    newStock.Add(g);
                                    continue;
                                default:
                                    continue;
                            }

                        case SceneNames.Room_shop:
                            switch (stats.specialType)
                            {
                                // sly mask shards
                                case 1 when (_defaultShopItems & DefaultShopItems.SlyMaskShards) == DefaultShopItems.SlyMaskShards:
                                // sly charms
                                case 2 when (_defaultShopItems & DefaultShopItems.SlyCharms) == DefaultShopItems.SlyCharms:
                                // sly vessel fragments
                                case 3 when (_defaultShopItems & DefaultShopItems.SlyVesselFragments) == DefaultShopItems.SlyVesselFragments:
                                // sly simple key
                                case 10 when (_defaultShopItems & DefaultShopItems.SlySimpleKey) == DefaultShopItems.SlySimpleKey:
                                // sly rancid egg
                                case 11 when (_defaultShopItems & DefaultShopItems.SlyRancidEgg) == DefaultShopItems.SlyRancidEgg:
                                // sly lantern
                                case 0 when stats.playerDataBoolName == nameof(PlayerData.hasLantern) && (_defaultShopItems & DefaultShopItems.SlyLantern) == DefaultShopItems.SlyLantern:
                                    newStock.Add(g);
                                    continue;
                                default:
                                    continue;
                            }

                        case SceneNames.Room_Charm_Shop:
                            switch (stats.specialType)
                            {
                                case 2 when (_defaultShopItems & DefaultShopItems.SalubraCharms) == DefaultShopItems.SalubraCharms:
                                case 8 when (_defaultShopItems & DefaultShopItems.SalubraNotches) == DefaultShopItems.SalubraNotches:
                                    newStock.Add(g);
                                    continue;
                                default:
                                    continue;
                            }

                        case SceneNames.Fungus2_26:
                            switch (stats.specialType)
                            {
                                // fragile charms
                                case 2 when (_defaultShopItems & DefaultShopItems.LegEaterCharms) == DefaultShopItems.LegEaterCharms:
                                // fragile repair
                                case 12 when (_defaultShopItems & DefaultShopItems.LegEaterRepair) == DefaultShopItems.LegEaterRepair:
                                case 13 when (_defaultShopItems & DefaultShopItems.LegEaterRepair) == DefaultShopItems.LegEaterRepair:
                                case 14 when (_defaultShopItems & DefaultShopItems.LegEaterRepair) == DefaultShopItems.LegEaterRepair:
                                    newStock.Add(g);
                                    continue;
                                default:
                                    continue;
                            }

                        default:
                            continue;
                    }
                }

                shop.stock = newStock.ToArray();

                // Update alt stock; Sly only
                if (shop.stockAlt != null)
                {
                    // Save unchanged list for potential alt stock
                    List<GameObject> altStock = new List<GameObject>();
                    altStock.AddRange(newStock);

                    foreach (GameObject g in shop.stockAlt)
                    {
                        ShopItemStats stats = g.GetComponent<ShopItemStats>();
                        switch (stats.specialType)
                        {
                            // sly mask shards
                            case 1 when (_defaultShopItems & DefaultShopItems.SlyMaskShards) == DefaultShopItems.SlyMaskShards:
                            // sly charms
                            case 2 when stats.requiredPlayerDataBool != nameof(PlayerData.gaveSlykey) && (_defaultShopItems & DefaultShopItems.SlyCharms) == DefaultShopItems.SlyCharms:
                            // sly key charms
                            case 2 when stats.requiredPlayerDataBool == nameof(PlayerData.gaveSlykey) && (_defaultShopItems & DefaultShopItems.SlyKeyCharms) == DefaultShopItems.SlyKeyCharms:
                            // sly vessel fragments
                            case 3 when (_defaultShopItems & DefaultShopItems.SlyVesselFragments) == DefaultShopItems.SlyVesselFragments:
                            // sly simple key
                            case 10 when (_defaultShopItems & DefaultShopItems.SlySimpleKey) == DefaultShopItems.SlySimpleKey:
                            // sly rancid egg
                            case 11 when (_defaultShopItems & DefaultShopItems.SlyRancidEgg) == DefaultShopItems.SlyRancidEgg:
                            // sly lantern
                            case 0 when stats.playerDataBoolName == nameof(PlayerData.hasLantern) && (_defaultShopItems & DefaultShopItems.SlyLantern) == DefaultShopItems.SlyLantern:
                            // sly key elegant key
                            case 0 when stats.playerDataBoolName == nameof(PlayerData.hasWhiteKey) && (_defaultShopItems & DefaultShopItems.SlyKeyElegantKey) == DefaultShopItems.SlyKeyElegantKey:
                                newStock.Add(g);
                                continue;
                            default:
                                continue;
                        }
                    }

                    shop.stockAlt = altStock.ToArray();
                }
            }
        }
    }
}
