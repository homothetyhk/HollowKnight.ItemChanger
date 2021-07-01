using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.UIDefs;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Placements;
using SereCore;
using ItemChanger.Locations.SpecialLocations;

namespace ItemChanger.Tests
{
    public static class Tests
    {
        public static void FinderTest()
        {
            AbstractPlacement placement = Finder.GetLocation(LocationNames.Vengeful_Spirit).Wrap();
            placement.AddItem(Finder.GetItem(ItemNames.Grubsong));

            Ref.Settings.SavePlacements(new[]
            {
                placement
            });
        }

        public static AbstractItem dreamnail = new BoolItem
        {
            name = "Dream_Nail",
            fieldName = nameof(PlayerData.hasDreamNail),
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_DREAMNAIL_A",
                shopDescKey = "INV_DESC_DREAMNAIL_A",
                spriteKey = "ShopIcons.Dreamnail",

                bigSpriteKey = "Prompts.Dreamnail",
                takeKey = "GET_ITEM_INTRO5",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_DREAMNAIL_1",
                descTwoKey = "GET_DREAMNAIL_2",
            },
            tags = new List<Tag>
            {
                new AdditiveGroupTag{AdditiveGroup = "Dream_Nail"},
            }
        };

        public static AbstractItem dreamgate = new BoolItem
        {
            name = "Dream_Gate",
            fieldName = nameof(PlayerData.hasDreamGate),
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_DREAMGATE",
                shopDescKey = "INV_DESC_DREAMGATE",
                spriteKey = "ShopIcons.Dreamnail",

                bigSpriteKey = "Prompts.Dream Gate",
                takeKey = "GET_ITEM_INTRO5",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_DREAMGATE_1",
                descTwoKey = "GET_DREAMGATE_2",
            },
            tags = new List<Tag>
            {
                new AdditiveGroupTag{AdditiveGroup = "Dream_Nail"},
            }
        };



        public static void AdditiveTest()
        {
            salubra.AddItem(dreamnail);
            salubra.AddItem(dreamgate);

            Ref.Settings.AdditiveGroups = new Dictionary<string, AbstractItem[]>
            {
                { "Dream_Nail", new[]{dreamnail, dreamgate} }
            };

            Ref.Settings.SavePlacements(new[]
            {
                salubra
            });
        }

        public static void SalubraTest()
        {
            salubra.AddItemWithCost(grubsong, Cost.NewGeoCost(100));
            salubra.AddItemWithCost(cyclone, new PDBoolCost
            {
                fieldName = nameof(PlayerData.gotCharm_3),
                uiText = "Requires Grubsong",
            });

            Ref.Settings.SavePlacements(new[]
            {
                salubra
            });
        }

        public static void SlyTest()
        {
            sly_key.defaultShopItems = sly.defaultShopItems = DefaultShopItems.SlyRancidEgg | DefaultShopItems.SlyKeyElegantKey;
            sly.AddItemWithCost(wk, null);
            sly.AddItemWithCost(grub, 100);
            sly.AddItemWithCost(shopkey, Cost.NewGrubCost(1));
            sly.AddItem(dive);
            sly_key.AddItem(cyclone);

            Ref.Settings.SavePlacements(new[]
            {
                sly, sly_key
            });
        }

        public static void JoniTest()
        {
            joni.AddItem(grub);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                joni
            });
        }

        public static void BaldurTest()
        {
            baldur.AddItem(grub);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                baldur
            });
        }

        public static void YNShinyTest()
        {
            ItemChanger.ChangeStartGame(new StartDef { startSceneName = SceneNames.Grimm_Main_Tent, startX = 70f, startY = 7f });
            multigrimmchild.AddItemWithCost(grubsong, Cost.NewGeoCost(100));
            multigrimmchild.AddItem(wk);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                multigrimmchild
            });
        }

        public static void CostChestTest()
        {
            ItemChanger.ChangeStartGame(new StartDef { startSceneName = SceneNames.Grimm_Main_Tent, startX = 70f, startY = 7f });

            chestgrimmchild.AddItem(wk);
            chestgrimmchild.AddItem(grub, Cost.NewGeoCost(100));
            chestgrimmchild.AddItem(grubsong, Cost.NewGrubCost(1));
            chestgrimmchild.AddItem(cyclone, new PDBoolCost { fieldName = "gotCharm_3", uiText = "Requires Grubsong" });

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                chestgrimmchild
            });
        }

        public static void CustomSkillTest()
        {
            ItemChanger.instance.SET.CustomSkills.canFocus = false;

            hornet.AddItem(focus);
            Ref.Settings.SavePlacements(new AbstractPlacement[]
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

            Ref.Settings.SavePlacements(new AbstractPlacement[]
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

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, sly, slybasement
            });
        }

        public static void BroodingMawlekTest()
        {
            ItemChanger.ChangeStartGame(new StartDef { startSceneName = "Crossroads_09", startX = 20f, startY = 9f });
            start.AddItem(supernail);
            mawlek.AddItem(grub);
            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, mawlek
            });
        }

        public static void PaleLurkerTest()
        {
            ItemChanger.ChangeStartGame(new StartDef { startSceneName = "GG_Lurker", startX = 176.8f, startY = 52.4f });
            start.AddItem(supernail);
            lurker.AddItem(grubsong);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
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

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                mato
            });
        }

        public static void VengefulSpiritTest()
        {
            shaman.AddItem(grubsong);
            shaman.AddItem(megarock);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
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

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                crystal
            });
        }

        public static void ShadeSoulTest()
        {
            shadesoul.AddItem(wk);
            shadesoul.AddItem(gslash);
            shadesoul.AddItem(grub);
            shadesoul.AddItem(megarock);
            shadesoul.AddItem(dslash);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                shadesoul
            });
        }

        public static void DesolateDiveTest()
        {
            start.AddItem(dive);
            dive_p.AddItem(grub);
            dive_p.AddItem(megarock);
            dive_p.AddItem(cyclone);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, dive_p
            });
        }

        public static void CliffsCorniferTest()
        {
            start.AddItem(new MultiBoolItem
            {
                fieldNames = new[]
                {
                    //nameof(PlayerData.corn_cliffsEncountered),
                    //nameof(PlayerData.corn_cliffsLeft),
                    nameof(PlayerData.mapCliffs),
                    nameof(PlayerData.visitedCliffs)
                }
            });

            cliffsmap.AddItem(grub);
            cliffsmap.AddItem(dive);
            cliffsmap.AddItem(cyclone);
            cliffsmap.AddItem(wk);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, cliffsmap
            });
        }

        public static void BrummFlameTest()
        {
            start.AddItem(new EquippedCharmItem { charmNum = 40 });
            start.AddItem(new IntItem
            {
                amount = 3,
                fieldName = nameof(PlayerData.grimmChildLevel),
            });

            brumm.AddItem(grub);
            brumm.AddItem(dive);
            brumm.AddItem(wk);
            brumm.AddItem(cyclone);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, brumm
            });
        }

        public static void GruzMotherTest()
        {
            start.AddItem(supernail);
            gruzmother.AddItem(grub);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, gruzmother
            });
        }

        public static void VengeflyKingTest()
        {
            start.AddItem(supernail);
            vengeflyking_alt.AddItem(grub);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, vengeflyking_alt
            });
        }

        public static void SoulWarriorTest()
        {
            start.AddItem(supernail);
            soulwarrior.AddItem(grub);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, soulwarrior
            });
        }

        public static void GorgeousHuskTest()
        {
            start.AddItem(supernail);
            ghusk.AddItem(grubsong);
            ghusk.AddItem(megarock);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, ghusk
            });
        }

        public static void CrystalGuardianTest()
        {
            start.AddItem(supernail);
            crystalguardian.AddItem(dive);
            crystalguardian.AddItem(grub);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, crystalguardian
            });
        }

        public static void GrimmkinTest()
        {
            start.AddItem(supernail);
            start.AddItem(new Grimmchild1Item { });
            start.AddItem(new BoolItem { fieldName = nameof(PlayerData.equippedCharm_40) });
            grimmkinGP.AddItem(grub);
            grimmkinGP.AddItem(cyclone);

            Ref.Settings.SavePlacements(new AbstractPlacement[]
            {
                start, grimmkinGP
            });
        }

        static StartPlacement start = new StartPlacement
        {
            location = new StartLocation
            {
                name = "Start",
            }
        };

        static MutablePlacement fountain = new MutablePlacement
        {
            location = new BasinFountainLocation
            {
                objectName = "Vessel Fragment",
                fsmParent = "Wishing_Well_anims",
                fsmName = "Fountain Control",
                fsmVariable = "Vessel Fragment",
                flingType = FlingType.Everywhere,
                sceneName = SceneNames.Abyss_04
            }
        };

        static FsmPlacement grimmkinGP = new FsmPlacement
        {
            location = new GrimmkinLocation
            {
                grimmkinLevel = 1,
                name = "Grimmkin Flame-Greenpath",
                flingType = FlingType.Everywhere,
                sceneName = "Fungus1_10",
            },
        };

        static MutablePlacement gruzmother = new MutablePlacement
        {
            location = new SDDualLocation
            {
                name = "Gruz_Mother",
                sceneName = SceneNames.Crossroads_04,
                pbdID = "Battle Scene",
                trueLocation = new CoordinateLocation
                {
                    x = 92.5f,
                    y = 15.5f,
                },
                falseLocation = new GruzMotherDropLocation
                {
                },
            }
        };

        static MutablePlacement vengeflyking = new MutablePlacement
        {
            location = new PDDualLocation
            {
                name = "Vengefly_King",
                sceneName = SceneNames.Fungus1_20_v02,
                pdBool = nameof(PlayerData.zoteRescuedBuzzer),
                trueLocation = new CoordinateLocation
                {
                    x = 45f,
                    y = 13.5f,
                },
                falseLocation = new EnemyLocation
                {
                    objectName = "Giant Buzzer",
                    removeGeo = true,
                }
            }
        };

        static MutablePlacement vengeflyking_alt = new MutablePlacement
        {
            location = new PDDualLocation
            {
                name = "Vengefly_King",
                sceneName = SceneNames.Fungus1_20_v02,
                pdBool = nameof(PlayerData.zoteRescuedBuzzer),
                trueLocation = new CoordinateLocation
                {
                    x = 45f,
                    y = 13.5f,
                },
                falseLocation = new EnemyLocationAlt
                {
                    enemyObj = "Giant Buzzer",
                    enemyFsm = "Big Buzzer",
                    removeGeo = true,
                }
            }
        };

        static MutablePlacement soulwarrior = new MutablePlacement
        {
            location = new SDDualLocation
            {
                name = "Soul_Warrior",
                sceneName = SceneNames.Ruins1_23,
                pbdID = "Battle Scene v2",
                trueLocation = new CoordinateLocation
                {
                    x = 30f,
                    y = 75f,
                },
                falseLocation = new EnemyLocation
                {
                    objectName = "Mage Knight",
                    removeGeo = true,
                }
            }
        };

        static MutablePlacement crystalguardian = new MutablePlacement
        {
            location = new PDDualLocation
            {
                name = "Crystal_Guardian",
                sceneName = SceneNames.Mines_18,
                pdBool = nameof(PlayerData.defeatedMegaBeamMiner),
                trueLocation = new CoordinateLocation
                {
                    x = 34f,
                    y = 11.5f,
                },
                falseLocation = new EnemyLocationAlt
                {
                    enemyFsm = "Beam Miner",
                    enemyObj = "Mega Zombie Beam Miner (1)",
                    removeGeo = true,
                }
            },
        };

        /*
        static MutablePlacement crystalguardian = new MutablePlacement
        {
            location = new BossLocation
            {
                name = "Crystal_Guardian",
                sceneName = SceneNames.Mines_18,
                flingType = FlingType.Everywhere,
                removeGeo = true,
                bossFsm = "Beam Miner",
                bossObj = "Mega Zombie Beam Miner (1)",
                pdBool = nameof(PlayerData.defeatedMegaBeamMiner),
                fallbackLocation = new CoordinateLocation
                {
                    x = 34f,
                    y = 11.5f,
                    sceneName = SceneNames.Mines_18,
                    flingType = FlingType.Everywhere,
                }
            },
        };
        */

        static MutablePlacement ghusk = new MutablePlacement
        {
            location = new EnemyLocation
            {
                name = "Gorgeous_Husk",
                sceneName = SceneNames.Ruins_House_02,
                objectName = "Gorgeous Husk",
                flingType = FlingType.Everywhere,
                removeGeo = true,
            },
        };

        public static FsmPlacement brumm = new FsmPlacement
        {
            location = new BrummFlameLocation
            {
                name = "Grimmkin_Flame-Brumm",
                flingType = FlingType.Everywhere,
                sceneName = SceneNames.Room_spider_small,
            },
        };

        static MutablePlacement dive_p = new MutablePlacement
        {
            location = new DesolateDiveLocation
            {
                name = "Desolate_Dive",
                flingType = FlingType.Everywhere,
                objectName = "Quake Item",
                sceneName = "Ruins1_24",
                fsmParent = "Quake Pickup",
                fsmName = "Pickup",
                fsmVariable = "Quake Item",
            }
        };

        static FsmPlacement crystal = new FsmPlacement
        {
            location = new DescendingDarkLocation
            {
                name = "Descending_Dark",
                flingType = FlingType.Everywhere,
                fsmName = "Control",
                objectName = "Crystal Shaman",
                sceneName = SceneNames.Mines_35,
            },
        };

        static FsmPlacement shadesoul = new FsmPlacement
        {
            location = new ShadeSoulLocation
            {
                name = "Shade_Soul",
                sceneName = "Ruins1_31b",
                flingType = FlingType.Everywhere,
            }
        };

        static MutablePlacement shaman = new MutablePlacement
        {
            location = new VengefulSpiritLocation
            {
                name = "Vengeful_Spirit",
                sceneName = SceneNames.Crossroads_ShamanTemple,
                flingType = FlingType.Everywhere,
                objectName = "Vengeful Spirit",
                fsmParent = "Shaman Meeting",
                fsmName = "Conversation Control",
                fsmVariable = "Vengeful Spirit",
            },
        };

        static FsmPlacement cliffsmap = new FsmPlacement
        {
            location = new CorniferLocation
            {
                name = "Howling_Cliffs_Map",
                sceneName = "Cliffs_01",
                objectName = "Cornifer",
                flingType = FlingType.Everywhere,
            },
            
        };

        static FsmPlacement mato = new FsmPlacement
        {
            location = new NailmasterLocation
            {
                name = "Cyclone_Slash",
                sceneName = SceneNames.Room_nailmaster,
                flingType = FlingType.DirectDeposit,
                fsmName = "Conversation Control",
                objectName = "NM Mato NPC",
            },
        };

        static MutablePlacement lurker = new MutablePlacement
        {
            location = new PDDualLocation
            {
                name = "Simple_Key-Lurker",
                sceneName = "GG_Lurker",
                pdBool = nameof(PlayerData.killedPaleLurker),
                trueLocation = new ObjectLocation
                {
                    name = "Simple_Key-Lurker",
                    sceneName = "GG_Lurker",
                    objectName = "Corpse Pale Lurker\\Shiny Item Key",
                },
                falseLocation = new PaleLurkerDropLocation
                {
                    name = "Simple_Key-Lurker",
                    sceneName = "GG_Lurker",
                    objectName = "Lurker Control\\Pale Lurker",
                },
            },
        };

        static MutablePlacement mawlek = new MutablePlacement
        {
            location = new BroodingMawlekLocation
            {
                name = "Mask_Shard-Brooding_Mawlek",
                flingType = FlingType.Everywhere,
                forceShiny = true,
                objectName = "Heart Piece",
                sceneName = "Crossroads_09",
                fsmParent = "Battle Scene",
                fsmName = "Battle Control",
                fsmVariable = "Heart Piece"
            },
        };

        static MutablePlacement slybasement = new MutablePlacement
        {
            location = new NailmastersGloryObjectLocation
            {
                name = "Nailmaster's_Glory",
                sceneName = SceneNames.Room_Sly_Storeroom,
                forceShiny = true,
                flingType = FlingType.DirectDeposit,
                objectName = "Sly Basement NPC",
            },
        };

        static MutablePlacement dreamnailcutscene = new MutablePlacement
        {
            location = new DreamNailLocation
            {
                name = "Dream_Nail",
                sceneName = SceneNames.Dream_Nailcollection,
                forceShiny = true,
                flingType = FlingType.DirectDeposit,
                objectName = "Moth NPC",
            },
        };

        static MutablePlacement hornet = new MutablePlacement
        {
            location = new MothwingCloakLocation
            {
                name = "Mothwing_Cloak",
            }
        };

        static AbstractItem supernail = new Items.IntItem
        {
            name = "Super_Nail",
            amount = 500,
            fieldName = nameof(PlayerData.nailDamage),
            UIDef = null,
        };

        static AbstractItem dive = new IntItem
        {
            fieldName = nameof(PlayerData.quakeLevel),
            amount = 1,
            name = "Desolate_Dive",
            UIDef = new BigUIDef
            {
                takeKey = "GET_ITEM_INTRO3",
                nameKey = "INV_NAME_SPELL_QUAKE1",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_QUAKE_1",
                descTwoKey = "GET_QUAKE_2",
                shopDescKey = "INV_DESC_SPELL_QUAKE1",
                bigSpriteKey = "Prompts.Quake1",
                spriteKey = "ShopIcons.Quake1"
            }
        };

        static AbstractItem dash = new Items.MultiBoolItem
        {
            name = "Mothwing_Cloak",
            fieldNames = new string[] { "canDash", "hasDash" },
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_DASH",
                shopDescKey = "INV_DESC_DASH",
                spriteKey = "ShopIcons.Dash",
                bigSpriteKey = "Prompts.Dash",
                descOneKey = "GET_DASH_1",
                descTwoKey = "GET_DASH_2",
                buttonKey = "RANDOMIZER_EMPTY",
                takeKey = "GET_ITEM_INTRO1"
            }
        };

        static AbstractItem leftdash = new Items.CustomSkillItem
        {
            name = "Left_Mothwing_Cloak",
            boolName = "canDashLeft",
            UIDef = new BigUIDef
            {
                nameKey = "RANDOMIZER_NAME_LEFT_CLOAK",
                shopDescKey = "RANDOMIZER_SHOP_DESC_LEFT_CLOAK",
                spriteKey = "ShopIcons.Dash",
                bigSpriteKey = "Prompts.DashReflected",
                descOneKey = "RANDOMIZER_DESC_LEFT_CLOAK",
                descTwoKey = "RANDOMIZER_DESC_LEFT_CLOAK_2",
                buttonKey = "RANDOMIZER_EMPTY",
                takeKey = "GET_ITEM_INTRO1"
            }
        };

        static AbstractItem focus = new Items.CustomSkillItem
        {
            name = "Focus",
            boolName = "canFocus",
            UIDef = new BigUIDef
            {
                nameKey = "RANDOMIZER_NAME_FOCUS",
                shopDescKey = "RANDOMIZER_SHOP_DESC_FOCUS",
                spriteKey = "ShopIcons.Focus",
                descOneKey = "RANDOMIZER_DESC_FOCUS",
                descTwoKey = "RANDOMIZER_EMPTY",
                bigSpriteKey = "Prompts.Focus",
                buttonKey = "RANDOMIZER_EMPTY",
                takeKey = "GET_ITEM_INTRO1",
            }
        };

        static AbstractItem grubsong = new Items.CharmItem
        {
            name = "Grubsong",
            charmNum = 3,
            UIDef = new UIDef
            {
                nameKey = "CHARM_NAME_3",
                shopDescKey = "CHARM_DESC_3",
                spriteKey = "Charms.3"
            }
        };

        static AbstractItem equipped_crest = new Items.EquippedCharmItem
        {
            name = "Defender's_Crest-E",
            charmNum = 10,
            UIDef = new UIDef
            {
                nameKey = "CHARM_NAME_10",
                shopDescKey = "CHARM_DESC_10",
                spriteKey = "Charms.10"
            }
        };

        static AbstractItem cyclone = new Items.BoolItem
        {
            name = "Cyclone_Slash",
            fieldName = "hasCyclone",
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_ART_CYCLONE",
                shopDescKey = "INV_DESC_ART_CYCLONE",
                spriteKey = "ShopIcons.CycloneSlash",

                takeKey = "GET_ITEM_INTRO3",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_CYCLONE_1",
                descTwoKey = "GET_CYCLONE_2",
                bigSpriteKey = "Prompts.CycloneSlash",
            }
        };

        static AbstractItem gslash = new Items.BoolItem
        {
            name = "Great_Slash",
            fieldName = "hasDashSlash",
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_ART_DASH",
                shopDescKey = "INV_DESC_ART_DASH",
                spriteKey = "ShopIcons.GreatSlash",

                takeKey = "GET_ITEM_INTRO3",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_GSLASH_1",
                descTwoKey = "GET_GSLASH_2",
                bigSpriteKey = "Prompts.GreatSlash",
            }
        };

        static AbstractItem dslash = new Items.BoolItem
        {
            name = "Dash_Slash",
            fieldName = "hasUpwardSlash",
            UIDef = new BigUIDef
            {
                nameKey = "INV_NAME_ART_UPPER",
                shopDescKey = "INV_DESC_ART_UPPER",
                spriteKey = "ShopIcons.DashSlash",

                takeKey = "GET_ITEM_INTRO3",
                buttonKey = "RANDOMIZER_BUTTON_DESC",
                descOneKey = "GET_DSLASH_1",
                descTwoKey = "GET_DSLASH_2",
                bigSpriteKey = "Prompts.DashSlash",
            }
        };

        static AbstractItem fk = new SpawnGeoItem
        {
            name = "False_Knight_Chest",
            amount = 200,
            UIDef = new UIDef
            {
                shopDescKey = "RANDOMIZER_NAME_GEO_200",
                nameKey = "RANDOMIZER_NAME_GEO_200",
                spriteKey = "ShopIcons.Geo",
            }
        };

        static AbstractItem wk = new SpawnGeoItem
        {
            name = "Watcher_Knight_Chest",
            amount = 655,
            UIDef = new UIDef
            {
                shopDescKey = "RANDOMIZER_NAME_GEO_655",
                nameKey = "RANDOMIZER_NAME_GEO_655",
                spriteKey = "ShopIcons.Geo",
            }
        };

        static AbstractItem megarock = new GeoRockItem
        {
            name = "420",
            amount = 420,
            geoRockSubtype = GeoRockSubtype.Outskirts420,
            UIDef = new UIDef
            {
                shopDescKey = "RANDOMIZER_NAME_GEO_420",
                nameKey = "RANDOMIZER_NAME_GEO_420",
                spriteKey = "ShopIcons.Geo",
            }
        };

        static AbstractItem grub = new GrubItem
        {
            name = "Grub",
            UIDef = GrubUIDef.Def
        };

        static AbstractItem shopkey = new BoolItem
        {
            name = "Shopkeeper's_Key",
            fieldName = nameof(PlayerData.hasSlykey),
            UIDef = new UIDef
            {
                nameKey = "INV_NAME_STOREKEY",
                shopDescKey = "INV_DESC_STOREKEY",
                spriteKey = "ShopIcons.ShopkeepersKey",
            }
        };

        static AbstractItem simple = new IntItem
        {
            name = "Simple_Key",
            fieldName = nameof(PlayerData.simpleKeys),
            amount = 1,
            UIDef = new UIDef
            {
                nameKey = "INV_NAME_SIMPLEKEY",
                shopDescKey = "INV_DESC_SIMPLEKEY",
                spriteKey = "ShopIcons.SimpleKey",
            }
        };

        static ObjectLocation joni_loc = new ObjectLocation
        {
            name = "Joni's_Blessing",
            sceneName = SceneNames.Cliffs_05,
            forceShiny = false,
            elevation = 0.1f,
            flingType = FlingType.Everywhere,
            objectName = "Shiny Item Stand"
        };

        static AbstractPlacement joni = new MutablePlacement
        {
            location = joni_loc,
        };

        static AbstractPlacement baldur = new MutablePlacement
        {
            location = new ObjectLocation
            {
                name = "Baldur_Shell",
                elevation = -1.3f,
                flingType = FlingType.Everywhere,
                objectName = "Shiny Item",
                sceneName = SceneNames.Fungus1_28,
            },
        };

        static AbstractPlacement fury = new ExistingChestPlacement
        {
            location = new ExistingChestLocation
            {
                sceneName = "Tutorial_01",
                chestFsm = "Chest Control",
                chestName = "Chest",
                //objectName = "_Props\\Chest",
                name = "Fury_of_the_Fallen",
            }
        };

        static ShopPlacement sly = new ShopPlacement
        {
            location = new ShopLocation
            {
                name = "Sly",
                sceneName = SceneNames.Room_shop,
                flingType = FlingType.DirectDeposit,
            },
            objectName = "Shop Menu",
            fsmName = "shop_control",
            defaultShopItems = DefaultShopItems.None,
            dungDiscount = false,
            requiredPlayerDataBool = string.Empty,
        };

        static ShopPlacement sly_key = new ShopPlacement
        {
            location = new ShopLocation
            {
                name = "Sly_(Key)",
                sceneName = SceneNames.Room_shop,
                flingType = FlingType.DirectDeposit,
            },
            objectName = "Shop Menu",
            fsmName = "shop_control",
            defaultShopItems = DefaultShopItems.None,
            dungDiscount = false,
            requiredPlayerDataBool = nameof(PlayerData.gaveSlykey),
        };

        static ShopPlacement salubra = new ShopPlacement
        {
            location = new ShopLocation
            {
                name = "Salubra",
                sceneName = SceneNames.Room_Charm_Shop,
                flingType = FlingType.DirectDeposit,
            },
            objectName = "Shop Menu",
            fsmName = "shop_control",
            defaultShopItems = DefaultShopItems.None,
            dungDiscount = false,
            requiredPlayerDataBool = string.Empty,
        };

        /*
        static ShopPlacement sly = new ShopPlacement
        {
            sceneName = SceneNames.Room_shop,
            objectName = "Shop Menu",
            fsmName = "shop_control",
            defaultShopItems = DefaultShopItems.None,
            dungDiscount = false,
            name = "Sly",
            requiredPlayerDataBool = string.Empty,
        };

        static ShopPlacement salubra = new ShopPlacement
        {
            sceneName = SceneNames.Room_Charm_Shop,
            objectName = "Shop Menu",
            fsmName = "shop_control",
            defaultShopItems = DefaultShopItems.None,
            dungDiscount = false,
            name = "Salubra",
            requiredPlayerDataBool = string.Empty,
        };
        */

        static CoordinateLocation grimmchild_loc = new CoordinateLocation
        {
            name = "Grimmchild",
            sceneName = SceneNames.Grimm_Main_Tent,
            x = 75,
            y = 7,
            elevation = 0.6f,
            flingType = FlingType.Everywhere,
            forceShiny = false,
        };

        static CoordinateLocation near_grimmchild_loc = new CoordinateLocation
        {
            sceneName = SceneNames.Grimm_Main_Tent,
            x = 70,
            y = 7,
            elevation = 0.5f,
            flingType = FlingType.Everywhere,
            forceShiny = false,
        };


        static CostChestPlacement chestgrimmchild = new CostChestPlacement
        {
            chestLocation = grimmchild_loc,
            tabletLocation = near_grimmchild_loc,
            
        };

        static YNShinyPlacement multigrimmchild = new YNShinyPlacement
        {
            location = grimmchild_loc,
        };
    }
}
