using System;
using System.Collections.Generic;
using System.Linq;
using SeanprCore;
using UnityEngine;
using Object = UnityEngine.Object;
using static Modding.Logger;
using ItemChanger.FsmStateActions;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using HutongGames.PlayMaker;
using RandomizerMod.SceneChanges;
using static ItemChanger.Location;

namespace ItemChanger.Actions
{
    public abstract class RandomizerAction
    {
        public enum ActionType
        {
            GameObject,
            PlayMakerFSM
        }

        private static readonly List<RandomizerAction> Actions = new List<RandomizerAction>();
        public static Dictionary<string, string> AdditiveBoolNames = new Dictionary<string, string>(); // item name, additive bool name
        public static Dictionary<(string, string), string> ShopItemBoolNames = new Dictionary<(string, string), string>(); // (item name, shop name), shop item bool name
        internal static IEnumerable<ILP> ILPs;

        public abstract ActionType Type { get; }

        public static void ClearActions()
        {
            PDHooks.ResetSpecialPDHooks();
            ILPs = new ILP[] { };
            On.GameManager.StartNewGame -= GiveStartItems;
            Actions.Clear();
        }

        internal static void CreateActions(IEnumerable<ILP> ILPs, Default.Shops.DefaultShopItems defaultShopItems)
        {
            ClearActions();
            
            ShopItemBoolNames = new Dictionary<(string, string), string>();
            AdditiveBoolNames = new Dictionary<string, string>();

            int newShinies = 0;

            RandomizerAction.ILPs = ILPs;
            On.GameManager.StartNewGame += GiveStartItems;

            // Loop non-shop items
            foreach (ILP ilp in ILPs)
            {
                if (ilp.location.shop || ilp.location.start) continue;
                string fsmName;
                string objName;

                if (ilp.location.replaceObject)
                {
                    fsmName = "Shiny Control";
                    objName = "Randomizer Shiny " + newShinies++;
                    Actions.Add(new ReplaceObjectWithShiny(ilp.location.sceneName, ilp.location.objectName, objName));
                }

                else if (ilp.location.newShiny)
                {
                    fsmName = "Shiny Control";
                    objName = "New Shiny " + newShinies++;
                    Actions.Add(new CreateNewShiny(ilp.location.sceneName, ilp.location.x, ilp.location.y, objName));
                }

                else if (ilp.location.geoChest && ilp.item.type != Item.ItemType.Geo)
                {
                    fsmName = "Shiny Control";
                    objName = "Randomizer Chest Shiny" + newShinies++;
                    Actions.Add(new AddShinyToChest(ilp.location.sceneName, ilp.location.chestName, ilp.location.chestFsm,
                        objName));
                }
                else
                {
                    fsmName = ilp.location.oldShinyFsm;
                    objName = ilp.location.objectName;
                }

                if (ilp.location.destroyObjects != null)
                {
                    foreach (string destroyName in ilp.location.destroyObjects) Actions.Add(new CreateObjectDestroyer(ilp.location.sceneName, destroyName));
                }

                PDHooks.AddSpecialPDHook(ilp.location.specialPDHook);

                switch (ilp.location.specialFSMEdit)
                {
                    default:
                    case SpecialFSMEdit.None:
                        break;

                    case SpecialFSMEdit.MothwingCloakArena:
                        Actions.Add(new AddSendEventToObject(ilp.location.sceneName, fsmName, objName, "BG OPEN", "Destroy", "Finish"));
                        Actions.Add(new EditFsm(ilp.location.sceneName, "FSM", "Camera Locks Boss",
                            (self) =>
                            {
                                if (!PlayerData.instance.hornet1Defeated) GameObject.Destroy(self);
                            }
                            ));
                        break;

                    case SpecialFSMEdit.DreamNailCutscene:
                        Actions.Add(new ChangeBoolTest("RestingGrounds_04", "Binding Shield Activate", "FSM", "Check",
                        altTest: () => ItemChanger.instance.Settings.CheckObtained(ilp.id)));
                        Actions.Add(new ChangeBoolTest("RestingGrounds_04", "Dreamer Plaque Inspect", "Conversation Control", "End",
                            altTest: () => ItemChanger.instance.Settings.CheckObtained(ilp.id)));
                        Actions.Add(new ChangeBoolTest("RestingGrounds_04", "Dreamer Scene 2", "Control", "Init",
                            altTest: () => ItemChanger.instance.Settings.CheckObtained(ilp.id)));
                        Actions.Add(new ChangeBoolTest("RestingGrounds_04", "PreDreamnail", "FSM", "Check",
                            altTest: () => ItemChanger.instance.Settings.CheckObtained(ilp.id)));
                        Actions.Add(new ChangeBoolTest("RestingGrounds_04", "PostDreamnail", "FSM", "Check",
                            altTest: () => ItemChanger.instance.Settings.CheckObtained(ilp.id)));
                        Actions.Add(new AddFsmActionToObject(ilp.location.sceneName, fsmName, objName, first: false,
                            new RandomizerChangeScene(SceneNames.RestingGrounds_07, "right1"), "Finish"));
                        break;

                    case SpecialFSMEdit.SlyBasement:
                        Actions.Add(new AddFsmActionToObject(ilp.location.sceneName, fsmName, objName, first: true,
                            new RandomizerSetBool(nameof(SaveSettings.gotSlyCharm), true),
                            "Finish"));
                        Actions.Add(new AddFsmActionToObject(ilp.location.sceneName, fsmName, objName, first: false,
                            new RandomizerChangeScene(SceneNames.Town, "door_sly"),
                            "Finish"));
                        break;

                    case SpecialFSMEdit.DesolateDive:
                        // open gates after shiny pickup
                        Actions.Add(new AddSendEventToObject(ilp.location.sceneName, fsmName, objName, "BG OPEN", "Destroy", "Finish"));
                        // prevent spell container self destruct
                        Actions.Add(new RemoveFsmActionsFromObject<IntCompare>(ilp.location.sceneName, "Pickup", "Quake Pickup", "Idle"));
                        // make spell container spawn shiny
                        Actions.Add(new EditFsm(ilp.location.sceneName, "Pickup", "Quake Pickup",
                            (self) =>
                            {
                                self.GetState("Appear").Actions[2] = self.GetState("Instant Activate").Actions[0] = new RandomizerExecuteLambda(
                                    () => 
                                    {
                                        GameObject.Find("Quake Pickup").transform.Find(objName).gameObject.SetActive(true);
                                    });
                            }
                                ));
                        break;

                    case SpecialFSMEdit.TutTablet:
                        Actions.Add(new RemoveFsmActionsFromObject<Trigger2dEvent>(ilp.location.sceneName, "Inspection", "Tut_tablet_top", "Out Of Range"));
                        break;
                        
                    case SpecialFSMEdit.BroodingMawlek:
                        Actions.Add(new EditFsm(ilp.location.sceneName, fsmName, objName,
                                (self) =>
                                {
                                    // mmmm spaghetti
                                    GameObject mawlekShard = self.gameObject;
                                    mawlekShard.transform.SetPositionY(100f);
                                    IEnumerator mawlekDead()
                                    {
                                        yield return new WaitUntil(() => PlayerData.instance.killedMawlek || GameManager.instance.GetSceneNameString() != SceneNames.Crossroads_09);
                                        if (GameManager.instance.sceneName == SceneNames.Crossroads_09)
                                        {
                                            mawlekShard.transform.SetPositionY(10f);
                                            mawlekShard.transform.SetPositionX(61.5f);
                                        }
                                    }
                                    GameManager.instance.StartCoroutine(mawlekDead());
                                }
                            ));
                        break;

                    case SpecialFSMEdit.PaleLurker:
                        Actions.Add(new EditFsm(ilp.location.sceneName, fsmName, objName,
                            (self) =>
                            {
                                // who even knows
                                if (PlayerData.instance.killedPaleLurker)
                                {
                                    Components.ObjectDestroyer.Destroy(ilp.location.sceneName, "Shiny Item Key");
                                }
                                else
                                {
                                    self.gameObject.transform.SetPositionY(200f);
                                    IEnumerator LurkerKilled()
                                    {
                                        yield return new WaitUntil(() => PlayerData.instance.killedPaleLurker || GameManager.instance.sceneName != "GG_Lurker");
                                        yield return new WaitUntil(() => GameObject.Find("Shiny Item Key") is GameObject lurkerKey || GameManager.instance.sceneName != "GG_Lurker");
                                        if (GameManager.instance.sceneName == "GG_Lurker")
                                        {
                                            Object.Destroy(GameObject.Find("Shiny Item Key"));
                                            GameObject lurkerCorpse = Object.FindObjectsOfType<GameObject>().First(obj => obj.name.StartsWith("Corpse Pale Lurker")); // Corpse Pale Lurker(Clone)
                                            self.gameObject.transform.SetPosition2D(lurkerCorpse.transform.position);
                                        }
                                    }
                                    GameManager.instance.StartCoroutine(LurkerKilled());
                                }
                            }
                            ));
                        break;

                    case SpecialFSMEdit.VoidHeart:
                        Actions.Add(new EditFsm(ilp.location.sceneName, "Control", "Dream Enter Abyss",
                            (self) =>
                            {
                                FsmState init = self.GetState("Init");
                                init.RemoveTransitionsTo("Idle");
                                init.AddTransition("FINISHED", "Inactive");
                            }
                            ));
                        break;

                    case SpecialFSMEdit.Colosseum1:
                        Actions.Add(new AddFsmActionToObject(ilp.location.sceneName, "Geo Pool", "Colosseum Manager", first: true,
                            new RandomizerSetBool(nameof(PlayerData.colosseumBronzeCompleted), true, true),
                            "Open Gates"));
                        break;

                    case SpecialFSMEdit.Colosseum2:
                        Actions.Add(new AddFsmActionToObject(ilp.location.sceneName, "Geo Pool", "Colosseum Manager", first: true,
                            new RandomizerSetBool(nameof(PlayerData.colosseumSilverCompleted), true, true),
                            "Open Gates"));
                        break;

                    case SpecialFSMEdit.Stag:
                        Actions.Add(new EditFsm(ilp.location.sceneName, 
                            "Stag Bell", 
                            ilp.location.sceneName == SceneNames.RestingGrounds_09 ? "Station Bell Lever" : "Station Bell",
                            (self) =>
                            {
                                FsmState init = self.GetState("Init");
                                init.RemoveActionsOfType<PlayerDataBoolTest>();
                                init.AddTransition("FINISHED", "Opened");
                            }
                            ));
                        Actions.Add(new EditFsm(ilp.location.sceneName, "Stag Control", "Stag",
                            (self) =>
                            {
                                FsmState open = self.GetState("Open Grate");
                                open.RemoveActionsOfType<SetPlayerDataBool>();
                                open.RemoveActionsOfType<SetBoolValue>();
                                if (!PlayerData.instance.GetBool(self.FsmVariables.GetFsmString("Station Opened Bool").Value))
                                {
                                    self.FsmVariables.GetFsmInt("Station Position Number").Value = 0;
                                    self.GetState("Current Location Check").RemoveActionsOfType<IntCompare>();
                                }
                            }
                            ));
                        break;

                    case SpecialFSMEdit.CollectorGrubs:
                        Actions.Add(new CreateObjectDestroyer(ilp.location.sceneName, "Grubs Folder"));
                        break;

                    case SpecialFSMEdit.WhisperingRoot:
                        Actions.Add(new AddFsmActionToObject(ilp.location.sceneName, fsmName, objName, first: true,
                            new RandomizerExecuteLambda(
                                () =>
                                {
                                    foreach (GameObject g in GameObject.FindGameObjectsWithTag("Dream Orb"))
                                    {
                                        g.transform.parent.gameObject.AddComponent<RandomizerDreamPlantOrb>().Awake();
                                    }
                                }
                                ), "Init"
                            ));
                        break;

                }

                switch (ilp.item.type)
                {
                    default:
                        Actions.Add(new ChangeShinyIntoItem(ilp, ilp.location.sceneName, objName, fsmName));
                        if (!string.IsNullOrEmpty(ilp.location.altObjectName))
                        {
                            Actions.Add(new ChangeShinyIntoItem(ilp, ilp.location.sceneName, ilp.location.altObjectName, fsmName));
                        }
                        break;

                    case Item.ItemType.Geo:
                        if (ilp.location.shinyChest || ilp.location.geoChest)
                        {
                            Actions.Add(new ChangeChestGeo(ilp, ilp.location.sceneName, ilp.location.chestName, ilp.location.chestFsm));
                        }
                        else
                        {
                            Actions.Add(new ChangeShinyIntoGeo(ilp, ilp.location.sceneName, objName, fsmName));

                            if (!string.IsNullOrEmpty(ilp.location.altObjectName))
                            {
                                Actions.Add(new ChangeShinyIntoGeo(ilp, ilp.location.sceneName, ilp.location.altObjectName, fsmName));
                            }
                        }
                        break;
                }

                if (ilp.location.costType != CostType.None)
                {
                    int cost = ilp.location.cost;

                    Actions.Add(new AddYNDialogueToShiny(
                        ilp.location.sceneName,
                        objName,
                        fsmName,
                        ilp.item.nameKey,
                        cost,
                        ilp.location.costType));
                }
            }

            List<ChangeShopContents> shopActions = new List<ChangeShopContents>();
            // initialize with empty actions so that shops get changed if we only remove items
            shopActions.Add(new ChangeShopContents(SceneNames.Room_shop, "Shop Menu", new ShopItemDef[] { }, defaultShopItems));
            shopActions.Add(new ChangeShopContents(SceneNames.Room_mapper, "Shop Menu", new ShopItemDef[] { }, defaultShopItems));
            shopActions.Add(new ChangeShopContents(SceneNames.Room_Charm_Shop, "Shop Menu", new ShopItemDef[] { }, defaultShopItems));
            shopActions.Add(new ChangeShopContents(SceneNames.Fungus2_26, "Shop Menu", new ShopItemDef[] { }, defaultShopItems));

            foreach (ILP ilp in ILPs)
            {
                if (!ilp.location.shop) continue;
                string shopItem = ilp.item.name;

                string boolName = "ItemChanger." + ilp.id;
                
                ShopItemDef newItemDef = new ShopItemDef
                {
                    PlayerDataBoolName = boolName,
                    NameConvo = ilp.item.nameKey,
                    DescConvo = ilp.item.shopDescKey,
                    RequiredPlayerDataBool = ilp.location.requiredPlayerDataBool,
                    RemovalPlayerDataBool = string.Empty,
                    DungDiscount = ilp.location.dungDiscount,
                    NotchCostBool = ilp.item.notchCost,
                    Cost = ilp.item.shopPrice,
                    Sprite = ilp.item.sprite
                };

                if (newItemDef.Cost == 0)
                {
                    newItemDef.Cost = 1;
                    LogWarn($"Found item {shopItem} in shop {ilp.location.name} with no saved cost.");
                }

                if (newItemDef.Cost < 5)
                {
                    newItemDef.DungDiscount = false;
                }

                ChangeShopContents existingShopAction = shopActions.FirstOrDefault(action =>
                    action.SceneName == ilp.location.sceneName &&
                    action.ObjectName == ilp.location.objectName);

                if (existingShopAction == null)
                {
                    shopActions.Add(new ChangeShopContents(ilp.location.sceneName,
                        ilp.location.objectName, new[] { newItemDef }, defaultShopItems));
                }
                else
                {
                    existingShopAction.AddItemDefs(new[] { newItemDef });
                }
            }

            shopActions.ForEach(action => Actions.Add(action));
        }

        private static void GiveStartItems(On.GameManager.orig_StartNewGame orig, GameManager self, bool permadeathMode, bool bossRushMode)
        {
            orig(self, permadeathMode, bossRushMode);
            foreach(ILP ilp in ILPs)
            {
                if (ilp.location.start) GiveItemActions.GiveItem(ilp);
            }
        }

        public static void Hook()
        {
            UnHook();

            On.PlayMakerFSM.OnEnable += ProcessFSM;
        }

        public static void UnHook()
        {
            On.PlayMakerFSM.OnEnable -= ProcessFSM;
        }

        public static void ProcessFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM fsm)
        {
            orig(fsm);

            string scene = fsm.gameObject.scene.name;
            foreach (RandomizerAction action in Actions)
            {
                if (action.Type != ActionType.PlayMakerFSM)
                {
                    continue;
                }

                try
                {
                    action.Process(scene, fsm);
                }
                catch (Exception e)
                {
                    LogError(
                        $"Error processing action of type {action.GetType()}:\n{JsonUtility.ToJson(action)}\n{e}");
                }
            }
        }

        public static void EditShinies()
        {
            string scene = Ref.GM.GetSceneNameString();

            foreach (RandomizerAction action in Actions)
            {
                if (action.Type != ActionType.GameObject)
                {
                    continue;
                }

                try
                {
                    action.Process(scene, null);
                }
                catch (Exception e)
                {
                    LogError(
                        $"Error processing action of type {action.GetType()}:\n{JsonUtility.ToJson(action)}\n{e}");
                }
            }
        }

        public abstract void Process(string scene, Object changeObj);
    }
}
