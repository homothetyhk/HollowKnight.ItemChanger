using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using System;
using System.Collections.Generic;

namespace ItemChanger.Locations
{
    /// <summary>
    /// The directions that a shopkeeper in a custom shop will face
    /// </summary>
    public enum FacingDirection
    {
        /// <summary>
        /// Allows the shopkeeper to face in the default direction (either towards the player if
        /// the NPC can turn, or the direction which is forced by the NPC by default)
        /// </summary>
        Auto,
        /// <summary>
        /// Forces the shopkeeper to face left
        /// </summary>
        Left,
        /// <summary>
        /// Forces the shopkeeper to face right
        /// </summary>
        Right,
    }

    /// <summary>
    /// A location which implements a custom shop on a given NPC.
    /// </summary>
    /// <remarks>
    /// Note that while <see cref="ShopLocation.defaultShopItems"/> can be set, it will not have any 
    /// effect on custom shops because they start empty by default.
    /// </remarks>
    public class CustomShopLocation : ShopLocation
    {
        // this is the normal position of the shop
        private const float XPositionShopOnRight = 8.53f;
        // I really did pull out a tape measure for this number
        private const float XPositionShopOnLeft = -1.53f;

        /// <summary>
        /// The optional conversation to have when the shop is out of stock
        /// </summary>
        public IString? outOfStockConvo;
        /// <summary>
        /// The optional sprite to use as the figurehead at the top of the shop
        /// </summary>
        public ISprite? figureheadSprite;
        /// <summary>
        /// The direction for the shopkeeper to face. Defaults to Auto.
        /// </summary>
        public FacingDirection facingDirection = FacingDirection.Auto;

        protected override void OnLoad()
        {
            // can't call base.OnLoad for most of these edits since the object name for the shop is
            // unique per NPC

            Events.AddFsmEdit(UnsafeSceneName, new(objectName, fsmName), EditInteractionFsm);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}", "shop_control"), CustomizeShopControl);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), CustomizeConfirmControl);

            // all events below perform the normal ShopLocation edits on our custom shop object
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}", "shop_control"), EditShopControl);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), EditItemListControl);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), EditConfirmControl);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), HastenItemListControl);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Item List", "ui_list_getinput"), HastenUIListGetInput);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), HastenConfirmControl);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list"), HastenUIList);
            Events.AddFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list_button_listen"), HastenUIListButtonListen);
        }

        protected override void OnUnload()
        {
            Events.RemoveFsmEdit(UnsafeSceneName, new(objectName, fsmName), EditInteractionFsm);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}", "shop_control"), CustomizeShopControl);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), CustomizeConfirmControl);

            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}", "shop_control"), EditShopControl);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), EditItemListControl);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), EditConfirmControl);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Item List", "Item List Control"), HastenItemListControl);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Item List", "ui_list_getinput"), HastenUIListGetInput);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "Confirm Control"), HastenConfirmControl);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list"), HastenUIList);
            Events.RemoveFsmEdit(UnsafeSceneName, new($"/Shop Menu {objectName}/Confirm/UI List", "ui_list_button_listen"), HastenUIListButtonListen);
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

            if (facingDirection == FacingDirection.Left)
            {
                fsm.FsmVariables.GetFsmBool("Hero Always Left").Value = true;
                fsm.FsmVariables.GetFsmBool("Hero Always Right").Value = false;
            }
            else if (facingDirection == FacingDirection.Right)
            {
                fsm.FsmVariables.GetFsmBool("Hero Always Left").Value = false;
                fsm.FsmVariables.GetFsmBool("Hero Always Right").Value = true;
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

            shopUp.SetActions(
                new Lambda(() =>
                {
                    GameObject hero = fsm.FsmVariables.GetFsmGameObject("Hero Obj").Value;
                    GameObject self = fsm.gameObject;
                    Vector3 shopPosition = shopObject.transform.position;
                    if (hero.transform.position.x < self.transform.position.x)
                    {
                        // Player is on the left, shop is on the right
                        shopObject.transform.position = new Vector3(XPositionShopOnRight, shopPosition.y, shopPosition.z);
                    }
                    if (hero.transform.position.x > self.transform.position.x)
                    {
                        // Player is on the right, shop is on the left
                        shopObject.transform.position = new Vector3(XPositionShopOnLeft, shopPosition.y, shopPosition.z);
                    }
                    // otherwise the player is in the exact position of the npc somehow, and hasn't been moved,
                    // so just defer to wherever the shop last was
                }),
                new Lambda(() => fsm.Fsm.BroadcastEventToGameObject(shopObject, "SHOP UP", false))
            );

            shopUp.AddTransition("HERO DAMAGED", resetShop);
            shopUp.AddTransition("SHOP CLOSED", "Convo End");

            resetShop.SetActions(
                new Lambda(() => fsm.Fsm.BroadcastEventToGameObject(shopObject, "CLOSE SHOP WINDOW", false))
            );
            resetShop.AddTransition("FINISHED", "Convo End");
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
            FsmState stockCheck = fsm.GetState("Stock?");
            FsmState boxUp = fsm.GetState("Box Up");
            FsmState checkRelics = fsm.GetState("Check Relics");

            setFigurehead.AddLastAction(new Lambda(() => {
                GameObject figurehead = fsm.FsmVariables.FindFsmGameObject("Current Figurehead").Value;
                if (figureheadSprite != null)
                {
                    UObject.DestroyImmediate(figurehead.GetComponent<InvAnimateUpAndDown>());
                    UObject.DestroyImmediate(figurehead.GetComponent<tk2dSprite>());
                    UObject.DestroyImmediate(figurehead.GetComponent<tk2dSpriteAnimator>());
                    UObject.DestroyImmediate(figurehead.GetComponent<MeshRenderer>());
                    UObject.DestroyImmediate(figurehead.GetComponent<MeshFilter>());

                    SpriteRenderer renderer = figurehead.AddComponent<SpriteRenderer>();
                    renderer.sprite = figureheadSprite.GetValue();
                    renderer.enabled = false;
                    FadeGroup group = renderer.transform.parent.parent.GetComponent<FadeGroup>();
                    SpriteRenderer[] spriteRenderers = new SpriteRenderer[group.spriteRenderers.Length + 1];
                    spriteRenderers[0] = renderer;
                    group.spriteRenderers.CopyTo(spriteRenderers, 1);
                    group.spriteRenderers = spriteRenderers;
                }
                else
                {
                    figurehead.SetActive(false);
                }
            }));

            stockCheck.RemoveTransitionsOn("FINISHED");
            stockCheck.AddTransition("FINISHED", "Open Window");

            boxUp.AddFirstAction(new DelegateBoolTest(() => outOfStockConvo == null, "SKIP CONVO", null));
            boxUp.RemoveTransitionsOn("FINISHED");
            boxUp.AddTransition("FINISHED", noStock);
            boxUp.AddTransition("SKIP CONVO", "End");

            noStock.AddFirstAction(new Lambda(() =>
            {
                DialogueCenter.StartConversation(outOfStockConvo!.GetValue());
            }));
            noStock.AddTransition("CONVO_FINISH", "Box Down");
        }

        private void CustomizeConfirmControl(PlayMakerFSM fsm)
        {
            FsmState reset = fsm.GetState("Reset");
            if (reset.GetActionsOfType<Lambda>().Any())
            {
                return;
            }

            reset.ReplaceAction(new Lambda(() =>
            {
                GameObject shopWindow = fsm.FsmVariables.FindFsmGameObject("Shop Window").Value;
                fsm.Fsm.BroadcastEventToGameObject(shopWindow, "RESET SHOP WINDOW", false);
            }), 1);
        }
    }
}
