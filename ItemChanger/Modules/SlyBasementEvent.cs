using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which sets the Sly Basement event to occur when the player has 3 nail arts, independently of Nailmaster's Glory. Makes room in the shop for Sly and the basement entrance.
    /// </summary>
    [DefaultModule]
    public class SlyBasementEvent : Module
    {
        /// <summary>
        /// If evaluates true, Sly's basement will no longer be available.
        /// <br/>If null, defaults to PD.gotSlyCharm (which is always false when the standard NMG location is used, and in vanilla is set in Dirtmouth when NMG is in inventory).
        /// <br/>Default test is true iff "Nailmaster's_Glory" placement exists and is cleared, or no such placement exists and PD.gotSlyCharm is true.
        /// </summary>
        public IBool Closed = new PlacementAllObtainedBool(placementName: LocationNames.Nailmasters_Glory, missingPlacementTest: new PDBool(nameof(PlayerData.gotSlyCharm)));

        private const float closedOffset = -1.5f;
        private const float openOffset = 0.6f;

        public override void Initialize()
        {
            Modding.ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
            Events.AddFsmEdit(SceneNames.Room_shop, new("Basement Closed", "Control"), EditBasementClosed);
            Events.AddFsmEdit(SceneNames.Room_shop, new("Basement Open", "Control"), EditBasementOpen);
            Events.AddFsmEdit(SceneNames.Room_shop, new("Shop Region", "Shop Region"), EditShopMoveToX);
            Events.AddSceneChangeEdit(SceneNames.Room_shop, EditScene);
        }

        public override void Unload()
        {
            Modding.ModHooks.GetPlayerBoolHook -= GetPlayerBoolHook;
            Events.RemoveFsmEdit(SceneNames.Room_shop, new("Basement Closed", "Control"), EditBasementClosed);
            Events.RemoveFsmEdit(SceneNames.Room_shop, new("Basement Open", "Control"), EditBasementOpen);
            Events.RemoveFsmEdit(SceneNames.Room_shop, new("Shop Region", "Shop Region"), EditShopMoveToX);
            Events.RemoveSceneChangeEdit(SceneNames.Room_shop, EditScene);
        }

        private void EditScene(Scene scene)
        {
            scene.FindGameObject("_Scenery/Shop Counter").transform.Translate(new Vector2(closedOffset, 0f));
        }

        public bool ShouldOpenBasement()
        {
            return PlayerData.instance.GetBool(nameof(PlayerData.hasAllNailArts)) && !(Closed?.Value ?? PlayerData.instance.GetBool(nameof(PlayerData.gotSlyCharm)));
        }

        private void EditBasementOpen(PlayMakerFSM fsm)
        {
            fsm.GetState("Check").Actions = new FsmStateAction[]
            {
                new DelegateBoolTest(ShouldOpenBasement, null, "CLOSED"),
            };
            fsm.transform.Translate(new Vector2(openOffset, 0));
        }

        private void EditBasementClosed(PlayMakerFSM fsm)
        {
            fsm.GetState("Check").ClearTransitions();
            fsm.transform.Translate(new Vector2(closedOffset, 0f));
        }

        private void EditShopMoveToX(PlayMakerFSM fsm)
        {
            fsm.FsmVariables.FindFsmFloat("Move To X").Value += closedOffset;
        }

        private bool GetPlayerBoolHook(string name, bool orig) => name switch
        {
            nameof(PlayerData.hasAllNailArts) => orig || (PlayerData.instance.GetBool(nameof(PlayerData.hasCyclone))
                                                 && PlayerData.instance.GetBool(nameof(PlayerData.hasDashSlash))
                                                 && PlayerData.instance.GetBool(nameof(PlayerData.hasUpwardSlash))),
            nameof(PlayerData.hasNailArt) => orig || (PlayerData.instance.GetBool(nameof(PlayerData.hasCyclone))
                                             || PlayerData.instance.GetBool(nameof(PlayerData.hasDashSlash))
                                             || PlayerData.instance.GetBool(nameof(PlayerData.hasUpwardSlash))),
            _ => orig,
        };
    }
}
