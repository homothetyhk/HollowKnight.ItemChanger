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
    /// <summary>
    /// A location which implements a custom shop on a given NPC.
    /// </summary>
    /// <remarks>
    /// Note that while <see cref="ShopLocation.defaultShopItems"/> can be set, it will not have any 
    /// effect on custom shops because they start empty by default.
    /// </remarks>
    public class CustomShopLocation : ShopLocation
    {
        public IString outOfStockConvo;
        public ISprite figureheadSprite;

        protected override void OnLoad()
        {
            // can't call base.OnLoad for most of these edits since the object name for the shop is
            // unique per NPC

            Events.AddFsmEdit(sceneName, new(objectName, fsmName), EditInteractionFsm);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}", "shop_control"), CustomizeShopControl);

            // all events below perform the normal ShopLocation edits on our custom shop object
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}", "shop_control"), EditShopControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), EditItemListControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), EditConfirmControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), HastenItemListControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Item List", "ui_list_getinput"), HastenUIListGetInput);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), HastenConfirmControl);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list"), HastenUIList);
            Events.AddFsmEdit(sceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list_button_listen"), HastenUIListButtonListen);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(sceneName, new(objectName, fsmName), EditInteractionFsm);
            Events.RemoveFsmEdit(sceneName, new($"/Shop Menu {objectName}", "shop_control"), CustomizeShopControl);

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
        /// Customizes cosmetic features of the shop
        /// </summary>
        private void CustomizeShopControl(PlayMakerFSM fsm)
        {
            // apply interaction edits - only if we're the first one. it is technically possible
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
            noStock.AddTransition("CONVO_FINISH", "Box Down");

            stockCheck.RemoveTransitionsOn("FINISHED");
            stockCheck.AddTransition("FINISHED", "Open Window");
        }
    }
}
