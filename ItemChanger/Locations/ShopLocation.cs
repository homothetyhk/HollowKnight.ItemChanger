using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Placements;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;
using SereCore;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

namespace ItemChanger.Locations
{
    public class ShopLocation : AbstractLocation
    {
        public string objectName;
        public string fsmName;

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            if (fsm.FsmName == fsmName && fsm.gameObject.name == objectName)
            {
                Transform = fsm.transform;
            }

            switch (fsm.FsmName)
            {
                case "shop_control":
                    {
                        ShopMenuStock shop = fsm.gameObject.GetComponent<ShopMenuStock>();
                        GameObject itemPrefab = UnityEngine.Object.Instantiate(shop.stock[0]);
                        itemPrefab.SetActive(false);

                        shop.stock = (Placement as IShopPlacement).GetNewStock(shop.stock, itemPrefab);
                        if (shop.stockAlt != null)
                        {
                            shop.stockAlt = (Placement as IShopPlacement).GetNewAltStock(shop.stock, shop.stockAlt, itemPrefab);
                        }
                    }
                    break;

                case "Item List Control":
                    {
                        FsmState init = fsm.GetState("Init");
                        if (init.GetActionsOfType<Lambda>().Any()) return; // Fsm has already been edited
                        FsmState getDetailsInit = fsm.GetState("Get Details Init");
                        FsmState getDetails = fsm.GetState("Get Details");
                        FsmState charmsRequiredInit = fsm.GetState("Charms Required? Init");
                        FsmState charmsRequired = fsm.GetState("Charms Required?");
                        FsmState notchDisplayInit = fsm.GetState("Notch Display Init");
                        FsmState notchDisplay = fsm.GetState("Notch Display?");
                        FsmState checkCanBuy = fsm.GetState("Check Can Buy");
                        FsmState activateConfirm = fsm.GetState("Activate confirm");
                        FsmState activateUI = fsm.GetState("Activate UI");
                        
                        var textSetters = getDetails.GetActionsOfType<SetTextMeshProText>();

                        void SetName()
                        {
                            int index = fsm.FsmVariables.FindFsmInt("Current Item").Value;
                            GameObject shopItem = fsm.gameObject.GetComponent<ShopMenuStock>().stockInv[index];
                            var mod = shopItem.GetComponent<ModShopItemStats>();
                            string name;
                            if (mod && mod.item != null)
                            {
                                name = mod.item.GetResolvedUIDef(Placement).GetPreviewName();
                            }
                            else
                            {
                                name = Language.Language.Get(shopItem.GetComponent<ShopItemStats>().GetNameConvo(), "UI");
                            }

                            fsm.FsmVariables.FindFsmGameObject("Item name").Value.GetComponent<TextMeshPro>().text = name;
                        }

                        void ResetSprites()
                        {
                            foreach (GameObject shopItem in fsm.gameObject.GetComponent<ShopMenuStock>().stockInv)
                            {
                                var mod = shopItem.GetComponent<ModShopItemStats>();
                                if (!mod || mod.item == null) continue;
                                shopItem.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = mod.item.GetResolvedUIDef(Placement).GetSprite();
                            }                            
                        }

                        void SetDesc()
                        {
                            int index = fsm.FsmVariables.FindFsmInt("Current Item").Value;
                            GameObject shopItem = fsm.gameObject.GetComponent<ShopMenuStock>().stockInv[index];
                            var mod = shopItem.GetComponent<ModShopItemStats>();
                            string desc;
                            if (mod && mod.item != null)
                            {
                                desc = mod.item.GetResolvedUIDef(Placement).GetShopDesc();
                                if (mod.Cost != null && !(mod.Cost is GeoCost))
                                {
                                    desc += $"\n\n<#888888>{mod.Cost.GetCostText()}";
                                }
                            }
                            else
                            {
                                desc = Language.Language.Get(shopItem.GetComponent<ShopItemStats>().GetDescConvo(), "UI").Replace("<br>", "\n");
                            }
                            
                            fsm.FsmVariables.FindFsmGameObject("Item desc").Value.GetComponent<TextMeshPro>()
                                .text = desc;
                        }

                        void GetNotchCost()
                        {
                            int index = fsm.FsmVariables.FindFsmInt("Current Item").Value;
                            GameObject shopItem = fsm.gameObject.GetComponent<ShopMenuStock>().stockInv[index];
                            var mod = shopItem.GetComponent<ModShopItemStats>();
                            var stats = shopItem.GetComponent<ShopItemStats>();
                            int notchCost = 0;
                            if (mod && mod.item is AbstractItem item)
                            {
                                if (item.GetTag<ShopNotchCostTag>() is ShopNotchCostTag notchCostTag)
                                {
                                    notchCost = notchCostTag.GetNotchCost(item);
                                }
                                else if (item is Items.CharmItem charm)
                                {
                                    notchCost = PlayerData.instance.GetInt($"charmCost_{charm.charmNum}");
                                }
                                else if (item is Items.EquippedCharmItem echarm)
                                {
                                    notchCost = PlayerData.instance.GetInt($"charmCost_{echarm.charmNum}");
                                }
                            }
                            else
                            {
                                notchCost = stats.GetNotchCost();
                            }
                            
                            fsm.FsmVariables.FindFsmInt("Notch Cost").Value = notchCost;
                        }

                        bool CanBuy()
                        {
                            int index = fsm.FsmVariables.FindFsmInt("Current Item").Value;
                            GameObject shopItem = fsm.gameObject.GetComponent<ShopMenuStock>().stockInv[index];
                            var mod = shopItem.GetComponent<ModShopItemStats>();
                            if (mod)
                            {
                                Cost cost = mod.Cost;
                                return cost == null || cost.Paid|| cost.CanPay();
                            }
                            else
                            {
                                var stats = shopItem.GetComponent<ShopItemStats>();
                                return stats.cost <= PlayerData.instance.GetInt(nameof(PlayerData.geo));
                            }
                        }

                        void SetConfirmName()
                        {
                            int index = fsm.FsmVariables.FindFsmInt("Current Item").Value;
                            GameObject shopItem = fsm.gameObject.GetComponent<ShopMenuStock>().stockInv[index];
                            var mod = shopItem.GetComponent<ModShopItemStats>();
                            string name;
                            if (mod && mod.item != null)
                            {
                                name = mod.item.UIDef.GetPreviewName();
                            }
                            else
                            {
                                name = Language.Language.Get(shopItem.GetComponent<ShopItemStats>().GetNameConvo(), "UI");
                            }

                            fsm.FsmVariables.FindFsmGameObject("Confirm").Value.transform.Find("Item name").GetComponent<TextMeshPro>()
                                .text = name;
                        }

                        void AddIntToConfirm()
                        {
                            GameObject uiList = fsm.FsmVariables.FindFsmGameObject("UI List").Value;
                            PlayMakerFSM confirmControl = uiList.LocateFSM("Confirm Control");
                            FsmInt itemIndex = confirmControl.FsmVariables.FindFsmInt("Item Index");
                            if (itemIndex == null)
                            {
                                int length = confirmControl.FsmVariables.IntVariables.Length;
                                FsmInt[] fsmInts = new FsmInt[length + 1];
                                confirmControl.FsmVariables.IntVariables.CopyTo(fsmInts, 0);
                                itemIndex = fsmInts[length] = new FsmInt
                                {
                                    Name = "Item Index",
                                };
                                confirmControl.FsmVariables.IntVariables = fsmInts;
                            }
                            itemIndex.Value = fsm.FsmVariables.FindFsmInt("Current Item").Value;
                        }

                        Lambda resetSprites = new Lambda(ResetSprites);
                        Lambda setName = new Lambda(SetName);
                        Lambda setSprite = new Lambda(ResetSprites);
                        Lambda setDesc = new Lambda(SetDesc);
                        Lambda getNotchCost = new Lambda(GetNotchCost);
                        BoolTestMod canBuy = new BoolTestMod(CanBuy, checkCanBuy.GetActionOfType<BoolTest>());
                        Lambda setConfirmName = new Lambda(SetConfirmName);
                        Lambda addIntToConfirm = new Lambda(AddIntToConfirm);

                        init.AddAction(resetSprites);
                        getDetailsInit.Actions = new[] { setName, setSprite };
                        getDetails.Actions = new[] { setName };
                        charmsRequiredInit.Actions = new[] { setDesc };
                        charmsRequired.Actions = new[] { setDesc };
                        notchDisplayInit.AddFirstAction(getNotchCost);
                        notchDisplay.AddFirstAction(getNotchCost);
                        checkCanBuy.Actions = new[] { canBuy };
                        activateConfirm.Actions = new[]
                        {
                            // Find Children
                            activateConfirm.Actions[0],
                            activateConfirm.Actions[1],
                            activateConfirm.Actions[2],
                            // 3-4 Set Confirm Name -- replace
                            setConfirmName,
                            // 5-6 Set Confirm Cost
                            activateConfirm.Actions[5],
                            activateConfirm.Actions[6],
                            // 7-10 Set and adjust sprite
                            activateConfirm.Actions[7],
                            activateConfirm.Actions[8],
                            activateConfirm.Actions[9],
                            activateConfirm.Actions[10],
                            // 11 Set relic number
                            activateConfirm.Actions[11],
                            // 12-15 Activate and send events
                            activateConfirm.Actions[12],
                            activateConfirm.Actions[13],
                            activateConfirm.Actions[14],
                            activateConfirm.Actions[15],
                        };
                        activateUI.AddAction(addIntToConfirm);
                    }
                    break;
                case "Confirm Control":
                    {
                        FsmState deductSet = fsm.GetState("Deduct Geo and set PD");
                        if (deductSet.GetActionsOfType<Lambda>().Any()) return; // Fsm has already been edited

                        void Give()
                        {
                            int index = fsm.FsmVariables.FindFsmInt("Item Index").Value;
                            GameObject shopItem = fsm.transform.parent.parent.Find("Item List").GetComponent<ShopMenuStock>().stockInv[index];
                            var mod = shopItem.GetComponent<ModShopItemStats>();

                            if (mod)
                            {
                                mod.item.Give(Placement, new GiveInfo
                                {
                                    Container = Container.Shop,
                                    FlingType = this.flingType,
                                    MessageType = MessageType.Corner,
                                });
                            }
                            else
                            {
                                string boolName = shopItem.GetComponent<ShopItemStats>().GetPlayerDataBoolName();
                                PlayerData.instance.SetBool(boolName, true);
                            }
                        }

                        void Pay()
                        {
                            int index = fsm.FsmVariables.FindFsmInt("Item Index").Value;
                            GameObject shopItem = fsm.transform.parent.parent.Find("Item List").GetComponent<ShopMenuStock>().stockInv[index];
                            var mod = shopItem.GetComponent<ModShopItemStats>();
                            var stats = shopItem.GetComponent<ShopItemStats>();

                            if (mod)
                            {
                                Cost cost = mod.Cost;
                                if (cost is null || cost.Paid) return;
                                cost.Pay();
                            }
                            else
                            {
                                if (stats.cost > 0)
                                {
                                    HeroController.instance.TakeGeo(stats.cost);
                                }
                            }
                        }

                        Lambda give = new Lambda(Give);
                        Lambda pay = new Lambda(Pay);

                        deductSet.Actions = new[] { give, pay };
                    }
                    break;
            }

            Hasten(fsm); // brrr
        }

        public void Hasten(PlayMakerFSM fsm)
        {
            switch (fsm.FsmName)
            {
                case "Item List Control":
                    {
                        FsmState menuDown = fsm.GetState("Menu Down");
                        FsmState blankName = fsm.GetState("Blank Name and Desc");
                        FsmState activateConfirm = fsm.GetState("Activate confirm");

                        void ReduceFadeOutTime()
                        {
                            var fade = fsm.FsmVariables.FindFsmGameObject("Parent").Value.GetComponent<FadeGroup>();
                            fade.fadeOutTimeFast = fade.fadeOutTime = 0.01f;
                        }
                        menuDown.AddFirstAction(new Lambda(ReduceFadeOutTime));
                        menuDown.GetActionOfType<Wait>().time = 0.01f;
                        foreach (var a in menuDown.GetActionsOfType<SendEventByName>())
                        {
                            if (a.sendEvent.Value == "DOWN")
                            {
                                ItemChanger.instance.Log("Changing event");
                                a.sendEvent.Value = "DOWN INSTANT";
                            }
                        }

                        void ReduceFadeInTime()
                        {
                            var fade = fsm.FsmVariables.FindFsmGameObject("Confirm").Value.GetComponent<FadeGroup>();
                            fade.fadeInTime = 0.01f;
                        }

                        blankName.AddAction(new Lambda(ReduceFadeInTime));
                        activateConfirm.GetActionOfType<Wait>().time = 0.01f;
                    }
                    break;

                case "Confirm Control":
                    {
                        FsmState particles = fsm.GetState("Particles");
                        particles.GetActionOfType<Wait>().time = 0.2f;
                        FsmState bob = fsm.GetState("Bob");
                        bob.Actions = new[] { bob.Actions[0], bob.Actions[1] };
                        bob.Transitions[0].ToState = "Reset";

                        FsmState thankFade = fsm.GetState("Thank Fade");
                        thankFade.GetActionOfType<SendEventByName>().sendEvent.Value = "DOWN INSTANT";
                        thankFade.GetActionOfType<Wait>().time = 0.01f;
                    }
                    break;

                case "ui_list_getinput" when fsm.gameObject.name == "Item List":
                    {
                        FsmState confirm = fsm.GetState("Confirm");
                        FsmState cancel = fsm.GetState("Cancel");
                        confirm.GetActionOfType<Wait>().time = 0.01f;
                        cancel.GetActionOfType<Wait>().time = 0.01f;

                        FsmState stillUp = fsm.GetState("Still Up?");
                        FsmState stillLeft = fsm.GetState("Still Left?");
                        FsmState stillRight = fsm.GetState("Still Right?");
                        FsmState stillDown = fsm.GetState("Still Down?");
                        stillUp.GetActionOfType<Wait>().time = 0.15f;
                        stillLeft.GetActionOfType<Wait>().time = 0.15f;
                        stillRight.GetActionOfType<Wait>().time = 0.15f;
                        stillDown.GetActionOfType<Wait>().time = 0.15f;

                        FsmState repeatUp = fsm.GetState("Repeat Up");
                        FsmState repeatLeft = fsm.GetState("Repeat Left");
                        FsmState repeatRight = fsm.GetState("Repeat Right");
                        FsmState repeatDown = fsm.GetState("Repeat Down");
                        repeatUp.GetActionOfType<Wait>().time = 0.1f;
                        repeatLeft.GetActionOfType<Wait>().time = 0.1f;
                        repeatRight.GetActionOfType<Wait>().time = 0.1f;
                        repeatDown.GetActionOfType<Wait>().time = 0.1f;
                    }
                    break;
                case "ui_list" when fsm.gameObject.name == "UI List":
                    {
                        FsmState selectionMade = fsm.GetState("Selection Made");
                        FsmState selectionMadeCancel = fsm.GetState("Selection Made Cancel");
                        selectionMade.GetActionOfType<Wait>().time = 0.01f;
                        selectionMadeCancel.GetActionOfType<Wait>().time = 0.01f;
                    }
                    break;
                case "ui_list_button_listen" when fsm.gameObject.name == "UI List":
                    {
                        FsmState selectPressed = fsm.GetState("Select Pressed");
                        FsmState cancelPressed = fsm.GetState("Cancel Pressed");

                        selectPressed.GetActionOfType<Wait>().time = 0.1f;
                        cancelPressed.GetActionOfType<Wait>().time = 0.1f;
                    }
                    break;
            }
        }

        public override AbstractPlacement Wrap()
        {
            return new ShopPlacement
            {
                location = this,
                defaultShopItems = DefaultShopItems.None,
                dungDiscount = sceneName == SceneNames.Fungus2_26,
                requiredPlayerDataBool = name == "Sly_(Key)" ? nameof(PlayerData.gaveSlykey) : string.Empty,
            };
        }
    }
}
