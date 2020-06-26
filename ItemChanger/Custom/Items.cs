using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ItemChanger.Item;

namespace ItemChanger.Custom
{
    public static class Items
    {
        public static Item CreateShopItem(string defaultItemName, int shopPrice)
        {
            if (!XmlManager.Items.TryGetValue(defaultItemName, out Item item))
            {
                ItemChanger.instance.LogError($"Key {defaultItemName} did not correspond to any known item.");
                throw new KeyNotFoundException();
            }
            item.shopPrice = shopPrice;
            return item;
        }

        public static Item AddShopPrice(Item item, int shopPrice)
        {
            item.shopPrice = shopPrice;
            return item;
        }

        public static Item[] CreateAdditiveItemSet(Item[] itemset, string additiveGroupName)
        {
            List<Item> newItems = new List<Item>();
            for (int i=0;i<itemset.Length;i++)
            {
                Item item = itemset[i];
                item.additiveGroup = additiveGroupName;
                item.additiveIndex = i;
                newItems.Add(item);
            }
            return newItems.ToArray();
        }

        public static Item CreateCustomItem(string name, Sprite sprite, string nameKey, string shopDescKey, Action customAction, int shopPrice = 0)
        {
            Item item = new Item();
            item.name = name;

            item.sprite = sprite;
            item.nameKey = nameKey;
            item.shopDescKey = shopDescKey;
            
            item.action = GiveAction.Custom;
            item.customAction = customAction;

            item.shopPrice = shopPrice;

            return item;
        }

        public static Item CreateCustomBigItem(string name, Sprite sprite, string nameKey, string shopDescKey, Sprite bigSprite, string takeKey, string buttonKey, string descOneKey, string descTwoKey, Action customAction, int shopPrice = 0)
        {
            Item item = new Item();
            item.name = name;
            
            item.sprite = sprite;
            item.nameKey = nameKey;
            item.shopDescKey = shopDescKey;

            item.bigSprite = bigSprite;
            item.takeKey = takeKey;
            item.buttonKey = buttonKey;
            item.descOneKey = descOneKey;
            item.descTwoKey = descTwoKey;

            item.action = GiveAction.Custom;
            item.customAction = customAction;

            item.shopPrice = shopPrice;

            return item;
        }
    }
}
