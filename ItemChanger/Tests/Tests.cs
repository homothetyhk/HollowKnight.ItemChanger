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
            start.AddItem(equipped_crest);
            dreamnailcutscene.AddItem(megarock);
            dreamnailcutscene.AddItem(grub);
            dreamnailcutscene.AddItem(cyclone);
            dreamnailcutscene.AddItem(focus);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                start, dreamnailcutscene
            });
        }

        public static void SlyBasementTest()
        {
            ItemChanger.instance.SET.CustomSkills.canFocus = false;

            start.AddItem(dash);
            start.AddItem(wk);
            start.AddItem(megarock);

            sly.AddItemWithCost(cyclone, 5);
            sly.AddItemWithCost(dslash, 10);
            sly.AddItemWithCost(gslash, 15);

            slybasement.AddItem(grub);
            slybasement.AddItem(focus);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                start, sly, slybasement
            });
        }

        public static void BroodingMawlekTest()
        {
            ItemChanger.ChangeStartGame(new StartLocation { startSceneName = "Crossroads_09", startX = 20f, startY = 9f });
            start.AddItem(supernail);
            mawlek.AddItem(grub);
            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                start, mawlek
            });
        }

        public static void PaleLurkerTest()
        {
            ItemChanger.ChangeStartGame(new StartLocation { startSceneName = "GG_Lurker", startX = 176.8f, startY = 52.4f });
            start.AddItem(supernail);
            lurker.AddItem(wk);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                start, lurker
            });
        }

        public static void MatoTest()
        {
            mato.AddItem(gslash);
            mato.AddItem(wk);
            mato.AddItem(grub);
            mato.AddItem(megarock);
            mato.AddItem(dslash);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                mato
            });
        }

        public static void VengefulSpiritTest()
        {
            shaman.AddItem(grub);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                shaman
            });
        }

        public static void DescendingDarkTest()
        {
            crystal.AddItem(wk);
            crystal.AddItem(gslash);
            crystal.AddItem(grub);
            crystal.AddItem(megarock);
            crystal.AddItem(dslash);

            Ref.Placements.SavePlacements(new AbstractPlacement[]
            {
                crystal
            });

        }

        public static StartPlacement start = new StartPlacement
        {
            name = "Start",
        };

        public static FsmPlacement crystal = new FsmPlacement
        {
            location = new DescendingDarkLocation
            {
                flingType = FlingType.Everywhere,
                fsmName = "Control",
                objectName = "Crystal Shaman",
                messageType = MessageType.Any,
                sceneName = SceneNames.Mines_35,
            },
            name = "Descending_Dark",
        };

        public static MutablePlacement shaman = new MutablePlacement
        {
            name = "Vengeful_Spirit",
            location = new VengefulSpiritLocation
            {
                sceneName = SceneNames.Crossroads_ShamanTemple,
                flingType = FlingType.Everywhere,
                forceShiny = false,
                objectName = "Vengeful Spirit",
                elevation = 0.5f,
            }
        };

        public static FsmPlacement mato = new FsmPlacement
        {
            name = "Cyclone_Slash",
            location = new NailmasterLocation
            {
                sceneName = SceneNames.Room_nailmaster,
                flingType = FlingType.DirectDeposit,
                fsmName = "Conversation Control",
                objectName = "NM Mato NPC",
                messageType = MessageType.Any,
            },
        };

        public static MutablePlacement lurker = new MutablePlacement
        {
            name = "Simple_Key-Lurker",
            location = new PaleLurkerLocation
            {
                flingType = FlingType.Everywhere,
                forceShiny = true,
                objectName = "Corpse Pale Lurker\\Shiny Item Key",
                sceneName = "GG_Lurker",
            }
        };

        public static MutablePlacement mawlek = new MutablePlacement
        {
            name = "Mask_Shard-Brooding_Mawlek",
            location = new BroodingMawlekLocation
            {
                flingType = FlingType.Everywhere,
                forceShiny = true,
                objectName = "Heart Piece",
                sceneName = "Crossroads_09",
            },
        };


        public static MutablePlacement slybasement = new MutablePlacement
        {
            name = "Nailmaster's_Glory",
            location = new NailmastersGloryLocation
            {
                sceneName = SceneNames.Room_Sly_Storeroom,
                forceShiny = true,
                flingType = FlingType.DirectDeposit,
                objectName = "Sly Basement NPC",
            },
        };

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

        public static AbstractItem supernail = new Items.IntItem
        {
            name = "Super_Nail",
            amount = 500,
            fieldName = nameof(PlayerData.nailDamage),
            UIDef = null,
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

        public static AbstractItem equipped_crest = new Items.EquippedCharmItem
        {
            name = "Defender's_Crest-E",
            charmNum = 10,
            UIDef = new UIDef
            {
                nameKey = "CHARM_NAME_10",
                descKey = "CHARM_DESC_10",
                spriteKey = "Charms.10"
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

        public static AbstractItem gslash = new Items.BoolItem
        {
            name = "Great_Slash",
            fieldName = "hasDashSlash",
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_ART_DASH",
                descKey = "INV_DESC_ART_DASH",
                spriteKey = "ShopIcons.GreatSlash",

                takeKey = "GET_ITEM_INTRO3",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_GSLASH_1",
                descTwoKey = "GET_GSLASH_2",
                bigSpriteKey = "Prompts.GreatSlash",
            }
        };

        public static AbstractItem dslash = new Items.BoolItem
        {
            name = "Dash_Slash",
            fieldName = "hasUpwardSlash",
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_ART_UPPER",
                descKey = "INV_DESC_ART_UPPER",
                spriteKey = "ShopIcons.DashSlash",

                takeKey = "GET_ITEM_INTRO3",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_DSLASH_1",
                descTwoKey = "GET_DSLASH_2",
                bigSpriteKey = "Prompts.DashSlash",
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

        public static ShopPlacement sly = new ShopPlacement
        {
            sceneName = SceneNames.Room_shop,
            objectName = "Shop Menu",
            fsmName = "shop_control",
            defaultShopItems = Default.Shops.DefaultShopItems.None,
            dungDiscount = false,
            name = "Sly",
            requiredPlayerDataBool = string.Empty,
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
