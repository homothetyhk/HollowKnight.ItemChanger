using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.UIDefs;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Placements;
using SereCore;

namespace ItemChanger.Tests
{
    public static class Tests
    {
        public static void YNShinyTest()
        {
            ItemChanger.ChangeStartGame(new StartLocation { startSceneName = SceneNames.Grimm_Main_Tent, startX = 70f, startY = 7f });
            multigrimmchild.AddItemWithCost(grubsong, Cost.NewGeoCost(100));
            multigrimmchild.AddItem(wk);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                multigrimmchild
            });
        }

        public static void CostChestTest()
        {
            ItemChanger.ChangeStartGame(new StartLocation { startSceneName = SceneNames.Grimm_Main_Tent, startX = 70f, startY = 7f });

            chestgrimmchild.AddItem(wk);
            chestgrimmchild.AddItemWithCost(grub, Cost.NewGeoCost(100));
            chestgrimmchild.AddItemWithCost(grubsong, Cost.NewGrubCost(1));
            chestgrimmchild.AddItemWithCost(cyclone, new PDBoolCost { fieldName = "gotCharm_3", uiText = "Requires Grubsong" });

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                chestgrimmchild
            });
        }

        public static void CustomSkillTest()
        {
            ItemChanger.instance.SET.CustomSkills.canFocus = false;

            hornet.AddItem(focus);
            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                hornet
            });
        }

        public static void DreamNailCutsceneTest()
        {
            ItemChanger.instance.SET.CustomSkills.canFocus = false;
            dreamnailcutscene.AddItem(megarock);
            dreamnailcutscene.AddItem(grub);
            dreamnailcutscene.AddItem(cyclone);
            dreamnailcutscene.AddItem(focus);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                dreamnailcutscene
            });
        }


        public static MutablePlacement dreamnailcutscene = new MutablePlacement
        {
            name = "Dream_Nail",
            location = new DreamNailLocation
            {
                sceneName = SceneNames.Dream_Nailcollection,
                forceShiny = true,
                flingType = FlingType.DirectDeposit,
                objectName = "Moth NPC",
            },
        };

        public static MutablePlacement hornet = new MutablePlacement
        {
            name = "Mothwing_Cloak",
            location = new MothwingCloakLocation()
        };

        public static AbstractItem dash = new Items.MultiBoolItem
        {
            name = "Mothwing_Cloak",
            fieldNames = new string[] { "canDash", "hasDash" },
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_DASH",
                descKey = "INV_DESC_DASH",
                spriteKey = "ShopIcons.Dash",
                bigSpriteKey = "Prompts.Dash",
                descOneKey = "GET_DASH_1",
                descTwoKey = "GET_DASH_2",
                buttonKey = "RANDOMIZER_EMPTY",
                takeKey = "GET_ITEM_INTRO1"
            }
        };

        public static AbstractItem leftdash = new Items.CustomSkillItem
        {
            name = "Left_Mothwing_Cloak",
            boolName = "canDashLeft",
            UIDef = new BigUIDef
            {
                nameKey = "RANDOMIZER_NAME_LEFT_CLOAK",
                descKey = "RANDOMIZER_SHOP_DESC_LEFT_CLOAK",
                spriteKey = "ShopIcons.Dash",
                bigSpriteKey = "Prompts.DashReflected",
                descOneKey = "RANDOMIZER_DESC_LEFT_CLOAK",
                descTwoKey = "RANDOMIZER_DESC_LEFT_CLOAK_2",
                buttonKey = "RANDOMIZER_EMPTY",
                takeKey = "GET_ITEM_INTRO1"
            }
        };

        public static AbstractItem focus = new Items.CustomSkillItem
        {
            name = "Focus",
            boolName = "canFocus",
            UIDef = new BigUIDef
            {
                nameKey = "RANDOMIZER_NAME_FOCUS",
                descKey = "RANDOMIZER_SHOP_DESC_FOCUS",
                spriteKey = "ShopIcons.Focus",
                descOneKey = "RANDOMIZER_DESC_FOCUS",
                descTwoKey = "RANDOMIZER_EMPTY",
                bigSpriteKey = "Prompts.Focus",
                buttonKey = "RANDOMIZER_EMPTY",
                takeKey = "GET_ITEM_INTRO1",
            }
        };

        public static AbstractItem grubsong = new Items.CharmItem
        {
            name = "Grubsong",
            fieldName = "gotCharm_3",
            UIDef = new UIDef
            {
                nameKey = "CHARM_NAME_3",
                descKey = "CHARM_DESC_3",
                spriteKey = "Charms.3"
            }
        };

        public static AbstractItem cyclone = new Items.BoolItem
        {
            name = "Cyclone_Slash",
            fieldName = "hasCyclone",
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_ART_CYCLONE",
                descKey = "INV_DESC_ART_CYCLONE",
                spriteKey = "ShopIcons.CycloneSlash",

                takeKey = "GET_ITEM_INTRO3",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_CYCLONE_1",
                descTwoKey = "GET_CYCLONE_2",
                bigSpriteKey = "Prompts.CycloneSlash",
            }
        };

        public static AbstractItem fk = new SpawnGeoItem
        {
            name = "False_Knight_Chest",
            amount = 200,
            UIDef = new UIDef
            {
                descKey = "RANDOMIZER_NAME_GEO_200",
                nameKey = "RANDOMIZER_NAME_GEO_200",
                spriteKey = "ShopIcons.Geo",
            }
        };

        public static AbstractItem wk = new SpawnGeoItem
        {
            name = "Watcher_Knight_Chest",
            amount = 655,
            UIDef = new UIDef
            {
                descKey = "RANDOMIZER_NAME_GEO_655",
                nameKey = "RANDOMIZER_NAME_GEO_655",
                spriteKey = "ShopIcons.Geo",
            }
        };

        public static AbstractItem megarock = new GeoRockItem
        {
            name = "420",
            amount = 420,
            geoRockSubtype = GeoRockSubtype.Outskirts420,
            UIDef = new UIDef
            {
                descKey = "RANDOMIZER_NAME_GEO_420",
                nameKey = "RANDOMIZER_NAME_GEO_420",
                spriteKey = "ShopIcons.Geo",
            }
        };

        public static AbstractItem grub = new GrubItem
        {
            name = "Grub",
            UIDef = GrubUIDef.Def
        };

        public static AbstractItem simple = new IntItem
        {
            name = "Simple_Key",
            fieldName = nameof(PlayerData.simpleKeys),
            amount = 1,
            UIDef = new UIDef
            {
                nameKey = "INV_NAME_SIMPLEKEY",
                descKey = "INV_DESC_SIMPLEKEY",
                spriteKey = "ShopIcons.SimpleKey",
            }
        };

        public static ObjectLocation joni_loc = new ObjectLocation
        {
            sceneName = SceneNames.Cliffs_05,
            forceShiny = false,
            elevation = 0.1f,
            flingType = FlingType.Everywhere,
            objectName = "Shiny Item Stand"
        };

        public static AbstractPlacement joni = new MutablePlacement
        {
            location = joni_loc,
            name = "Joni's_Blessing",
        };

        public static AbstractPlacement fury = new ExistingChestPlacement
        {
            sceneName = "Tutorial_01",
            chestFsm = "Chest Control",
            chestName = "Chest",
            name = "Fury_of_the_Fallen",
        };

        public static ShopPlacement salubra = new ShopPlacement
        {
            sceneName = SceneNames.Room_Charm_Shop,
            objectName = "Shop Menu",
            fsmName = "shop_control",
            defaultShopItems = Default.Shops.DefaultShopItems.None,
            dungDiscount = false,
            name = "Salubra",
            requiredPlayerDataBool = string.Empty,
        };

        public static CoordinateLocation grimmchild_loc = new CoordinateLocation
        {
            sceneName = SceneNames.Grimm_Main_Tent,
            x = 75,
            y = 7,
            elevation = 0.6f,
            flingType = FlingType.Everywhere,
            forceShiny = false,
        };

        public static CoordinateLocation near_grimmchild_loc = new CoordinateLocation
        {
            sceneName = SceneNames.Grimm_Main_Tent,
            x = 70,
            y = 7,
            elevation = 0.5f,
            flingType = FlingType.Everywhere,
            forceShiny = false,
        };


        public static CostChestPlacement chestgrimmchild = new CostChestPlacement
        {
            chestLocation = grimmchild_loc,
            tabletLocation = near_grimmchild_loc,
            name = "Grimmchild",
        };

        public static YNShinyPlacement multigrimmchild = new YNShinyPlacement
        {
            location = grimmchild_loc,
            name = "Grimmchild",
        };
    }
}
