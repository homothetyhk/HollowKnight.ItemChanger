using HutongGames.PlayMaker.Actions;
using ItemChanger.Components;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using ItemChanger.Placements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;

namespace ItemChanger.Locations
{
    public class CustomShopLocation : AbstractLocation
    {
        public string objectName;
        public string fsmName;

        /// <inheritdoc/>
        public string requiredPlayerDataBool = string.Empty;
        public bool dungDiscount;

        public IString outOfStockConvo;
        public ISprite figureheadSprite;
        public Type primaryCostType;
        public ISprite costSprite;

        protected override void OnLoad()
        {
            Events.AddFsmEdit(sceneName, new(objectName, fsmName), EditInteractionFsm);

            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}", "shop_control"), EditShopControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), EditItemListControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), EditConfirmControl);

            // brrr
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), HastenItemListControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "ui_list_getinput"), HastenUIListGetInput);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), HastenConfirmControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list"), HastenUIList);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list_button_listen"), HastenUIListButtonListen);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, fsmName), EditInteractionFsm);

            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}", "shop_control"), EditShopControl);
            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), EditItemListControl);
            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), EditConfirmControl);

            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), HastenItemListControl);
            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "ui_list_getinput"), HastenUIListGetInput);
            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), HastenConfirmControl);
            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list"), HastenUIList);
            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list_button_listen"), HastenUIListButtonListen);
        }

        /// <summary>
        /// Instantiates a shop menu for the owning GameObject and injects shop events into
        /// the owning FSM
        /// </summary>
        private void EditInteractionFsm(PlayMakerFSM fsm)
        {
            FsmState shopUp = fsm.GetState("Shop Up");
            if (shopUp != null)
            {
                return; // already edited
            }

            GameObject shopObject = ObjectCache.ShopMenu;
            shopObject.name = $"Shop Menu {objectName}";
            ShopMenuStock stock = shopObject.GetComponent<ShopMenuStock>();
            stock.stock = new GameObject[0];
            stock.stockAlt = null;
            shopObject.SetActive(true);

            shopUp = fsm.AddState("Shop Up");
            FsmState resetShop = fsm.AddState("Reset Shop");
            FsmState convoStart = fsm.GetState("Convo Start");

            convoStart.RemoveTransitionsOn("CONVO END");
            convoStart.AddTransition("CONVO END", shopUp);

            shopUp.AddFirstAction(new Lambda(() => {
                fsm.Fsm.BroadcastEventToGameObject(shopObject, $"SHOP UP {objectName}", false);
            }));

            shopUp.AddTransition("HERO DAMAGED", resetShop);
            shopUp.AddTransition("SHOP CLOSED", "Convo End");

            resetShop.AddFirstAction(new Lambda(() => {
                fsm.Fsm.BroadcastEventToGameObject(shopObject, "CLOSE SHOP WINDOW", false);
            }));
        }

        /// <summary>
        /// Change how the shop stock is constructed and customizes cosmetic features of the shop
        /// </summary>
        private void EditShopControl(PlayMakerFSM fsm)
        {
            // add stock to the shop - this happens unconditionally, to support multiple shop locations and placements in the same GO
            ShopMenuStock shop = fsm.gameObject.GetComponent<ShopMenuStock>();
            GameObject itemPrefab = ObjectCache.ShopItem;
            if (costSprite != null)
            {
                itemPrefab.transform.Find("Geo Sprite")
                    .GetComponent<SpriteRenderer>().sprite = costSprite.Value;
            }

            shop.stock = (Placement as IShopPlacement).GetNewStock(shop.stock, itemPrefab);

            // now apply functional edits - only if we're the first one. it is technically possible
            // that shops sharing an object may have conflicting cosmetics here but that's fine because
            // they're cosmetic. assume they'll just get along well
            FsmState noStock = fsm.GetState("No Stock Convo");
            if (noStock != null)
            {
                return;
            }
            noStock = fsm.AddState("No Stock Convo");
            FsmState setFigurehead = fsm.GetState("Set Figurehead");
            FsmState idle = fsm.GetState("Idle");
            FsmState stockCheck = fsm.GetState("Stock?");
            FsmState boxUp = fsm.GetState("Box Up");
            FsmState checkRelics = fsm.GetState("Check Relics");

            setFigurehead.AddLastAction(new Lambda(() => {
                GameObject figurehead = fsm.FsmVariables.FindFsmGameObject("Current Figurehead").Value;
                if (figureheadSprite != null)
                {
                    figurehead.GetComponent<SpriteRenderer>().sprite = figureheadSprite.Value;
                }
                else
                {
                    figurehead.SetActive(false);
                }
            }));

            idle.RemoveTransitionsOn("SHOP UP");
            idle.AddTransition($"SHOP UP {objectName}", stockCheck);

            noStock.AddFirstAction(new Lambda(() =>
            {
                DialogueCenter.StartConversation(outOfStockConvo.Value);
            }));
            boxUp.RemoveTransitionsOn("FINISHED");
            boxUp.AddTransition("FINISHED", noStock);
            noStock.AddTransition("FINISHED", "Box Down");

            stockCheck.RemoveTransitionsOn("FINISHED");
            stockCheck.AddTransition("FINISHED", "Open Window");

            checkRelics.SetActions(
                // 0-3 initialize confirm UI
                checkRelics.Actions[0],
                checkRelics.Actions[1],
                checkRelics.Actions[2],
                checkRelics.Actions[3],
                new Lambda(() => fsm.SendEvent("NOT RELIC DEALER"))
            );
        }

        /// <summary>
        /// Change how the shop stock is presented.
        /// </summary>
        private void EditItemListControl(PlayMakerFSM fsm)
        {
            FsmState init = fsm.GetState("Init");

            bool hasBeenEdited = init.GetActionsOfType<Lambda>().Any(); // for cases like sly, sly key, only one placement needs to edit the shop functionality
            if (hasBeenEdited)
            {
                return;
            }

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
                string name = mod.GetPreviewName();

                fsm.FsmVariables.FindFsmGameObject("Item name").Value.GetComponent<TextMeshPro>().text = name;
            }

            void ResetSprites()
            {
                foreach (GameObject shopItem in fsm.gameObject.GetComponent<ShopMenuStock>().stockInv)
                {
                    var mod = shopItem.GetComponent<ModShopItemStats>();
                    if (!mod || mod.item == null) continue;
                    shopItem.transform.Find("Item Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = mod.GetSprite();
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
                    desc = mod.GetShopDesc();
                    if (mod.cost is not null && !mod.cost.Paid)
                    {
                        string costText = mod.GetShopCostText();
                        if (!string.IsNullOrEmpty(costText))
                        {
                            desc += $"\n\n<#888888>{costText}";
                        }
                    }
                }
                else
                {
                    int charmsRequired = shopItem.GetComponent<ShopItemStats>().GetCharmsRequired();
                    if (charmsRequired > 0)
                    {
                        charmsRequired -= PlayerData.instance.GetInt(nameof(PlayerData.charmsOwned));
                    }
                    if (charmsRequired > 0)
                    {
                        desc = string.Concat(Language.Language.Get(shopItem.GetComponent<ShopItemStats>().GetDescConvo() + "_NE", "UI").Replace("<br>", "\n"), " ", charmsRequired.ToString(), " ",
                            Language.Language.Get("CHARMS_REMAINING", "UI"));
                    }
                    else
                    {
                        desc = Language.Language.Get(shopItem.GetComponent<ShopItemStats>().GetDescConvo(), "UI").Replace("<br>", "\n");
                    }
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
                if (mod && !mod.IsSecretItem() && mod.item is AbstractItem item)
                {
                    if (item.GetTag<IShopNotchCostTag>() is IShopNotchCostTag notchCostTag)
                    {
                        notchCost = notchCostTag.GetNotchCost(item);
                    }
                    else if (item is Items.CharmItem charm)
                    {
                        notchCost = PlayerData.instance.GetInt($"charmCost_{charm.charmNum}");
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
                    Cost cost = mod.cost;
                    return cost == null || cost.Paid || cost.CanPay();
                }
                else
                {
                    return fsm.gameObject.GetComponent<ShopMenuStock>().CanBuy(index);
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
                    name = mod.GetPreviewName();
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
                PlayMakerFSM confirmControl = uiList.LocateMyFSM("Confirm Control");
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
            DelegateBoolTest canBuy = new DelegateBoolTest(CanBuy, checkCanBuy.GetFirstActionOfType<BoolTest>());
            Lambda setConfirmName = new Lambda(SetConfirmName);
            Lambda addIntToConfirm = new Lambda(AddIntToConfirm);

            init.AddLastAction(resetSprites);
            getDetailsInit.SetActions(
                setName,
                setSprite,
                // 7-8 Activate detail pane
                getDetailsInit.Actions[7],
                getDetailsInit.Actions[8]);
            getDetails.SetActions(setName);
            charmsRequiredInit.SetActions(setDesc);
            charmsRequired.SetActions(setDesc);
            notchDisplayInit.AddFirstAction(getNotchCost);
            notchDisplay.AddFirstAction(getNotchCost);
            checkCanBuy.SetActions(canBuy);
            activateConfirm.SetActions(
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
                activateConfirm.Actions[15]
            );
            activateUI.AddLastAction(addIntToConfirm);
        }

        /// <summary>
        /// Change the effects of purchasing a shop item.
        /// </summary>
        private void EditConfirmControl(PlayMakerFSM fsm)
        {
            FsmState yes = fsm.GetState("Yes");
            FsmState deductSet = fsm.GetState("Deduct Geo and set PD");

            yes.RemoveTransitionsOn("FINISHED");
            yes.AddTransition("FINISHED", deductSet);

            void Give()
            {
                int index = fsm.FsmVariables.FindFsmInt("Item Index").Value;
                GameObject shopItem = fsm.transform.parent.parent.Find("Item List").GetComponent<ShopMenuStock>().stockInv[index];
                var mod = shopItem.GetComponent<ModShopItemStats>();

                mod.item.Give(mod.placement, new GiveInfo
                {
                    Container = mod.placement.MainContainerType,
                    FlingType = this.flingType,
                    MessageType = MessageType.Corner,
                    Transform = GameObject.Find(objectName)?.transform,
                });
            }

            void Pay()
            {
                int index = fsm.FsmVariables.FindFsmInt("Item Index").Value;
                GameObject shopItem = fsm.transform.parent.parent.Find("Item List").GetComponent<ShopMenuStock>().stockInv[index];
                var mod = shopItem.GetComponent<ModShopItemStats>();
                var stats = shopItem.GetComponent<ShopItemStats>();

                Cost cost = mod.cost;
                if (cost is null || cost.Paid) return;
                cost.Pay();
            }

            Lambda give = new(Give);
            Lambda pay = new(Pay);

            deductSet.SetActions(give, pay);
        }

        private void HastenItemListControl(PlayMakerFSM fsm)
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
            menuDown.GetFirstActionOfType<Wait>().time = 0.01f;
            foreach (var a in menuDown.GetActionsOfType<SendEventByName>())
            {
                if (a.sendEvent.Value == "DOWN")
                {
                    a.sendEvent.Value = "DOWN INSTANT";
                }
            }

            void ReduceFadeInTime()
            {
                var fade = fsm.FsmVariables.FindFsmGameObject("Confirm").Value.GetComponent<FadeGroup>();
                fade.fadeInTime = 0.01f;
            }

            blankName.AddLastAction(new Lambda(ReduceFadeInTime));
            activateConfirm.GetFirstActionOfType<Wait>().time = 0.01f;
        }

        private void HastenConfirmControl(PlayMakerFSM fsm)
        {
            FsmState particles = fsm.GetState("Particles");
            particles.GetFirstActionOfType<Wait>().time = 0.2f;
            FsmState bob = fsm.GetState("Bob");
            bob.SetActions(bob.Actions[0], bob.Actions[1]);
            FsmState specialType = fsm.GetState("Special Type?");
            bob.Transitions[0].SetToState(specialType);

            //FsmState thankFade = fsm.GetState("Thank Fade");
            //thankFade.GetFirstActionOfType<SendEventByName>().sendEvent.Value = "DOWN INSTANT";
            //thankFade.GetFirstActionOfType<Wait>().time = 0.01f;
        }

        private void HastenUIList(PlayMakerFSM fsm)
        {
            FsmState selectionMade = fsm.GetState("Selection Made");
            FsmState selectionMadeCancel = fsm.GetState("Selection Made Cancel");
            selectionMade.GetFirstActionOfType<Wait>().time = 0.01f;
            selectionMadeCancel.GetFirstActionOfType<Wait>().time = 0.01f;
        }

        private void HastenUIListGetInput(PlayMakerFSM fsm)
        {
            FsmState confirm = fsm.GetState("Confirm");
            FsmState cancel = fsm.GetState("Cancel");
            confirm.GetFirstActionOfType<Wait>().time = 0.01f;
            cancel.GetFirstActionOfType<Wait>().time = 0.01f;

            FsmState stillUp = fsm.GetState("Still Up?");
            FsmState stillLeft = fsm.GetState("Still Left?");
            FsmState stillRight = fsm.GetState("Still Right?");
            FsmState stillDown = fsm.GetState("Still Down?");
            stillUp.GetFirstActionOfType<Wait>().time = 0.15f;
            stillLeft.GetFirstActionOfType<Wait>().time = 0.15f;
            stillRight.GetFirstActionOfType<Wait>().time = 0.15f;
            stillDown.GetFirstActionOfType<Wait>().time = 0.15f;

            FsmState repeatUp = fsm.GetState("Repeat Up");
            FsmState repeatLeft = fsm.GetState("Repeat Left");
            FsmState repeatRight = fsm.GetState("Repeat Right");
            FsmState repeatDown = fsm.GetState("Repeat Down");
            repeatUp.GetFirstActionOfType<Wait>().time = 0.1f;
            repeatLeft.GetFirstActionOfType<Wait>().time = 0.1f;
            repeatRight.GetFirstActionOfType<Wait>().time = 0.1f;
            repeatDown.GetFirstActionOfType<Wait>().time = 0.1f;
        }

        private void HastenUIListButtonListen(PlayMakerFSM fsm)
        {
            FsmState selectPressed = fsm.GetState("Select Pressed");
            FsmState cancelPressed = fsm.GetState("Cancel Pressed");

            selectPressed.GetFirstActionOfType<Wait>().time = 0.1f;
            cancelPressed.GetFirstActionOfType<Wait>().time = 0.1f;
        }

        public override AbstractPlacement Wrap()
        {
            return new CustomShopPlacement(name)
            {
                Location = this
            };
        }
    }
}
