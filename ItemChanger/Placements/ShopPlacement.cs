using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Extensions;
using UnityEngine;
using ItemChanger.Locations;
using ItemChanger.Util;
using ItemChanger.Components;
using Newtonsoft.Json;

namespace ItemChanger.Placements
{
    public interface IShopPlacement
    {
        GameObject[] GetNewStock(GameObject[] oldStock, GameObject prefab);
        GameObject[] GetNewAltStock(GameObject[] newStock, GameObject[] altStock, GameObject prefab);
    }

    public class ShopPlacement : AbstractPlacement, IShopPlacement
    {
        public ShopLocation location;
        public override AbstractLocation Location => location;
        public override string MainContainerType => "Shop";

        public string requiredPlayerDataBool
        { 
            get => location.requiredPlayerDataBool; 
            set => location.requiredPlayerDataBool = value; 
        }
        public bool dungDiscount
        {
            get => location.dungDiscount;
            set => location.dungDiscount = value;
        }
        public DefaultShopItems defaultShopItems
        {
            get => location.defaultShopItems;
            set => location.defaultShopItems = value;
        }

        public void AddItemWithCost(AbstractItem item, Cost cost)
        {
            CostTag tag = item.GetTag<CostTag>() ?? item.AddTag<CostTag>();
            tag.Cost = cost;
            Items.Add(item);
        }

        public void AddItemWithCost(AbstractItem item, int geoCost)
        {
            AddItemWithCost(item, Cost.NewGeoCost(geoCost));
        }

        public GameObject[] GetNewStock(GameObject[] oldStock, GameObject shopPrefab)
        {
            List<GameObject> stock = new List<GameObject>(oldStock.Length + Items.Count());
            foreach (var item in Items)
            {
                GameObject shopItem = UnityEngine.Object.Instantiate(shopPrefab);
                shopItem.SetActive(false);
                ApplyItemDef(shopItem.GetComponent<ShopItemStats>(), item, item.GetTag<CostTag>()?.Cost);
                stock.Add(shopItem);
            }

            stock.AddRange(oldStock.Where(g => KeepOldItem(g.GetComponent<ShopItemStats>())));

            return stock.ToArray();
        }

        public GameObject[] GetNewAltStock(GameObject[] newStock, GameObject[] altStock, GameObject shopPrefab)
        {
            return newStock.Union(altStock.Where(g => KeepOldItem(g.GetComponent<ShopItemStats>()))).ToArray();
        }

        public void ApplyItemDef(ShopItemStats stats, AbstractItem item, Cost cost)
        {
            foreach (var m in stats.gameObject.GetComponents<ModShopItemStats>()) GameObject.Destroy(m); // Probably not necessary

            var mod = stats.gameObject.AddComponent<ModShopItemStats>();
            mod.item = item;
            mod.Cost = cost;

            // Apply all the stored values
            stats.playerDataBoolName = string.Empty;
            stats.nameConvo = string.Empty;
            stats.descConvo = string.Empty;
            stats.requiredPlayerDataBool = requiredPlayerDataBool;
            stats.removalPlayerDataBool = string.Empty;
            stats.dungDiscount = dungDiscount;
            stats.notchCostBool = string.Empty;
            stats.SetCost(cost?.GetDisplayGeo() ?? 0);

            // Need to set all these to make sure the item doesn't break in one of various ways
            stats.priceConvo = string.Empty;
            stats.specialType = 0;
            stats.charmsRequired = 0;
            stats.relic = false;
            stats.relicNumber = 0;
            stats.relicPDInt = string.Empty;

            // Apply the sprite for the UI
            stats.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = item.GetResolvedUIDef(this).GetSprite();
        }

        public bool KeepOldItem(ShopItemStats stats)
        {
            DefaultShopItems? itemType = ShopUtil.GetVanillaShopItemType(SceneName, stats);
            if (itemType == null) return true; // unrecognized items are kept by default
            return (itemType & defaultShopItems) == itemType;
        }
    }
}
