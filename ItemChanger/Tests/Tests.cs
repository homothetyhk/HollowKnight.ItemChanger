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
using ItemChanger.Internal;
using Ref = ItemChanger.Internal.Ref;

namespace ItemChanger.Tests
{
    public static class Tests
    {
        static AbstractItem cyclone => Finder.GetItem(ItemNames.Cyclone_Slash);
        static AbstractItem gslash => Finder.GetItem(ItemNames.Great_Slash);
        static AbstractItem dslash => Finder.GetItem(ItemNames.Dash_Slash);
        static AbstractItem dreamnail => Finder.GetItem(ItemNames.Dream_Nail);
        static AbstractItem dreamgate => Finder.GetItem(ItemNames.Dream_Gate);
        static AbstractItem fk => Finder.GetItem(ItemNames.Geo_Chest_False_Knight);
        static AbstractItem wk => Finder.GetItem(ItemNames.Geo_Chest_Watcher_Knights);
        static AbstractItem grubsong => Finder.GetItem(ItemNames.Grubsong);
        static AbstractItem grub => Finder.GetItem(ItemNames.Grub);
        static AbstractItem leftlore => Finder.GetItem(ItemNames.Lore_Tablet_City_Entrance);
        static AbstractItem lore => Finder.GetItem(ItemNames.Lore_Tablet_Fungal_Wastes_Hidden);
        static AbstractItem majorlore => Finder.GetItem(ItemNames.Lore_Tablet_Kings_Pass_Focus);
        static AbstractItem megarock => Finder.GetItem(ItemNames.Geo_Rock_Outskirts420);
        static AbstractItem shopkey => Finder.GetItem(ItemNames.Shopkeepers_Key);
        static AbstractItem simple => Finder.GetItem(ItemNames.Simple_Key);
        static AbstractItem supernail => new Items.IntItem
        {
            name = "Super_Nail",
            amount = 500,
            fieldName = nameof(PlayerData.nailDamage),
            UIDef = null,
        };
        static AbstractItem dive => Finder.GetItem(ItemNames.Desolate_Dive);
        static AbstractItem dash => Finder.GetItem(ItemNames.Mothwing_Cloak);
        static AbstractItem focus => Finder.GetItem(ItemNames.Focus);
        static AbstractItem leftdash => Finder.GetItem(ItemNames.Left_Mothwing_Cloak);


        private static AbstractPlacement AddTestingItems(this AbstractPlacement placement)
        {
            placement.AddItem(cyclone);
            placement.AddItem(wk);
            placement.AddItem(grubsong);
            placement.AddItem(leftlore);
            placement.AddItem(grub);
            placement.AddItem(majorlore);
            placement.AddItem(megarock);
            return placement;
        }

        static AbstractPlacement ex_sou = new DualPlacement
        {
            Test = new PDBool { boolName = nameof(PlayerData.slugEncounterComplete) },
            falseLocation = new ExistingContainerLocation
            {
                containerType = "Shiny",
                fsmName = "Shiny Control",
                objectName = "Shiny Item",
                sceneName = SceneNames.Fungus1_Slug,
                flingType = FlingType.StraightUp
            },
            trueLocation = new ExistingContainerLocation
            {
                containerType = "Shiny",
                fsmName = "Shiny Control",
                objectName = "Shiny Item Return",
                sceneName = SceneNames.Fungus1_Slug,
                flingType = FlingType.StraightUp
            },
        };

        public static void ShapeOfUnnTest()
        {
            Ref.QuickSave(ex_sou.AddTestingItems());
        }

        static AbstractPlacement ex_joni = new AutoPlacement
        {
            location = new ExistingContainerLocation
            {
                name = "Joni",
                containerType = "Shiny",
                fsmName = "Shiny Control",
                objectName = "Shiny Item Stand",
                sceneName = "Cliffs_05",
            }
        };

        static AbstractPlacement ex_worldsense = new AutoPlacement
        {
            location = new ExistingContainerLocation
            {
                containerType = Container.Tablet,
                fsmName = "Inspection",
                objectName = "Tut_tablet_top",
                sceneName = SceneNames.Room_Final_Boss_Atrium,
            }
        };

        public static void ExistingLocationTest()
        {
            AddTestingItems(ex_joni);
            AddTestingItems(ex_worldsense);

            Ref.QuickSave(ex_joni, ex_worldsense);
        }

        public static void VanillaShopTest()
        {
            PlayerData.instance.geo = 100000;
            Ref.QuickSave(VanillaShops.GetVanillaShops());
        }

        public static void FailedChampionTest()
        {
            PlayerData.instance.falseKnightDefeated = true;
            PlayerData.instance.falseKnightDreamDefeated = true;

            AddTestingItems(fcessence);
            Ref.QuickSave(fcessence);
        }

        static AbstractPlacement fcessence = new AutoPlacement
        {
            location = new BossEssenceLocation
            {
                sceneName = "Crossroads_10",
                objName = "Ghost False Knight NPC",
                fsmName = "Conversation Control",
            }
        };
        public static void FailedChampionEssenceTest()
        {

        }

        public static void SlyNMGTest()
        {
            PlayerData.instance.hasUpwardSlash = true;
            PlayerData.instance.hasDashSlash = true;
            PlayerData.instance.hasCyclone = true;
            PlayerData.instance.gotSlyCharm = true;
            PlayerData.instance.hasAllNailArts = true;
            PlayerData.instance.hasNailArt = true;

            slynmg.AddItem(cyclone);
            slynmg.AddItem(wk);
            slynmg.AddItem(grubsong);
            slynmg.AddItem(leftlore);
            slynmg.AddItem(megarock);
            slynmg.AddItem(majorlore);

            Ref.QuickSave(slynmg);
        }

        static AbstractPlacement slynmg = new AutoPlacement
        {
            location = new NailmastersGloryLocation
            {
                flingType = FlingType.DirectDeposit,
                sceneName = SceneNames.Room_Sly_Storeroom,
            }
        };

        static AbstractPlacement quirreltablet = new AutoPlacement
        {
            location = new ExistingContainerLocation
            {
                name = LocationNames.Lore_Tablet_City_Entrance,
                fsmName = "inspect_region",
                objectName = "Inspect Region",
                containerType = Container.Tablet,
                sceneName = "Ruins1_02",
            }
        };

        public static void NormalLoreTabletAltTest()
        {
            quirreltablet.AddItem(cyclone);
            quirreltablet.AddItem(wk);
            quirreltablet.AddItem(grubsong);
            quirreltablet.AddItem(leftlore);
            quirreltablet.AddItem(megarock);
            quirreltablet.AddItem(majorlore);

            Ref.QuickSave(quirreltablet);
        }

        static AbstractPlacement worldsense = new AutoPlacement
        {
            location = new ExistingContainerLocation
            {
                name = "World_Sense",
                fsmName = "Inspection",
                objectName = "Tut_tablet_top",
                containerType = Container.Tablet,
                sceneName = SceneNames.Room_Final_Boss_Atrium
            }
        };

        public static void WorldSenseTest()
        {
            worldsense.AddItem(cyclone);
            worldsense.AddItem(wk);
            worldsense.AddItem(grubsong);
            worldsense.AddItem(leftlore); 
            worldsense.AddItem(megarock);
            worldsense.AddItem(majorlore);

            Ref.QuickSave(worldsense);
        }

        static MutablePlacement colo1 = new MutablePlacement
        {
            location = new ColosseumLocation
            {
                name = "Charm_Notch-Colosseum",
                sceneName = SceneNames.Room_Colosseum_Bronze,
                fsmName = "Geo Pool",
                fsmParent = "Colosseum Manager",
                fsmVariable = "Shiny Obj",
                objectName = "Shiny Item"
            },
        };

        public static void ColosseumTest()
        {
            start.AddItem(supernail);
            colo1.AddItem(grub);

            Ref.QuickSave(start, colo1);
        }

        public static void CollectorTest()
        {
            var p1 = Finder.GetLocation(LocationNames.Grub_Collector_1).Wrap();
            var p2 = Finder.GetLocation(LocationNames.Grub_Collector_2).Wrap();
            var p3 = Finder.GetLocation(LocationNames.Grub_Collector_3).Wrap();
            var cm = Finder.GetLocation(LocationNames.Collectors_Map).Wrap();

            p1.AddItem(grub);
            p2.AddItem(grubsong);
            p3.AddItem(megarock);
            cm.AddItem(cyclone);

            Ref.QuickSave(p1, p2, p3, cm);

        }


        static AbstractPlacement rgroot = new WhisperingRootLocation
        {
            sceneName = SceneNames.RestingGrounds_05,
            flingType = FlingType.DirectDeposit,
            name = "Whispering_Root-Resting_Grounds",
        }.Wrap();

        public static void WhisperingRootTest()
        {
            megarock.AddTag<Tags.PersistentItemTag>().Persistence = Persistence.Persistent;
            grubsong.AddTag<Tags.PersistentItemTag>().Persistence = Persistence.SemiPersistent;
            rgroot.AddItem(megarock);
            rgroot.AddItem(grubsong);

            Ref.QuickSave(rgroot);
        }

        public static void ItemChainTest()
        {
            salubra.AddItem(Finder.GetItem("Queen_Fragment"));
            salubra.AddItem(Finder.GetItem("King_Fragment"));
            salubra.AddItem(Finder.GetItem("Kingsoul"));
            salubra.AddItem(Finder.GetItem("Void_Heart"));

            Ref.QuickSave(salubra);
        }

        public static void LoreTest()
        {
            crystal.AddItem(majorlore);
            crystal.AddItem(lore);
            crystal.AddItem(leftlore);

            Ref.QuickSave(crystal);
        }

        public static void FinderTest()
        {
            AbstractPlacement placement = Finder.GetLocation(LocationNames.Vengeful_Spirit).Wrap();
            placement.AddItem(Finder.GetItem(ItemNames.Grubsong));

            Ref.Settings.SavePlacements(new[]
            {
                placement
            });
        }

        public static void GreyMournerTest()
        {
            PlayerData.instance.xunFlowerGiven = true;

            greymourner.AddItem(grub);

            Ref.QuickSave(greymourner);
        }

        public static void ShadeCloakTest()
        {
            PlayerData.instance.hasDash = PlayerData.instance.canDash = PlayerData.instance.canShadowDash = PlayerData.instance.hasShadowDash = true;

            scloak.AddItem(cyclone);
            scloak.AddItem(megarock);
            scloak.AddItem(grubsong);
            scloak.AddItem(gslash);

            Ref.QuickSave(scloak);
        }

        public static void AbyssShriekTest()
        {
            PlayerData.instance.screamLevel = 2;
            shriek.AddItem(cyclone);
            shriek.AddItem(megarock);
            shriek.AddItem(grubsong);
            shriek.AddItem(gslash);

            Ref.QuickSave(shriek);
        }

        public static void VoidHeartTest()
        {
            //for (int i = 0; i < 3; i++) start.AddItem(new WhiteFragmentItem());
            start.AddItem(dreamnail);

            voidheart.AddItem(megarock);
            voidheart.AddItem(cyclone);
            voidheart.AddItem(gslash);
            voidheart.AddItem(grubsong);

            Ref.QuickSave(start, voidheart);
        }

        static MutablePlacement herrah = new MutablePlacement
        {
            location = new DreamerLocation
            {
                sceneName = SceneNames.Dream_Guardian_Hegemol,
                objectName = "Dreamer NPC",
                previousScene = SceneNames.Deepnest_Spider_Town,
                flingType = FlingType.DirectDeposit,
                forceShiny = true,
            }
        };

        static MutablePlacement monomon = new MutablePlacement
        {
            location = new DreamerLocation
            {
                sceneName = SceneNames.Dream_Guardian_Monomon,
                objectName = "Dreamer NPC",
                previousScene = SceneNames.Fungus3_archive_02,
                flingType = FlingType.DirectDeposit,
                forceShiny = true,
            }
        };

        static MutablePlacement lurien = new MutablePlacement
        {
            location = new DreamerLocation
            {
                sceneName = SceneNames.Dream_Guardian_Lurien,
                objectName = "Dreamer NPC",
                previousScene = SceneNames.Ruins2_Watcher_Room,
                flingType = FlingType.DirectDeposit,
                forceShiny = true,
            }
        };

        public static void DreamerTest()
        {
            start.AddItem(dreamnail);
            start.AddItem(new BoolItem { fieldName = nameof(PlayerData.lurienDefeated) });
            start.AddItem(new BoolItem { fieldName = nameof(PlayerData.monomonDefeated) });
            start.AddItem(new BoolItem { fieldName = nameof(PlayerData.hegemolDefeated) });

            var lurien = Finder.GetLocation(LocationNames.Lurien).Wrap();
            var monomon = Finder.GetLocation(LocationNames.Monomon).Wrap();
            var herrah = Finder.GetLocation(LocationNames.Herrah).Wrap();

            lurien.AddItem(grubsong);
            monomon.AddItem(cyclone);
            herrah.AddItem(megarock);

            Ref.QuickSave(start, lurien, monomon, herrah);
        }

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

        static MutablePlacement greymourner = Finder.GetLocation(LocationNames.Mask_Shard_Grey_Mourner).Wrap() as MutablePlacement;

        static AutoPlacement grimmkinGP = new AutoPlacement
        {
            location = new GrimmkinLocation
            {
                grimmkinLevel = 1,
                name = "Grimmkin Flame-Greenpath",
                flingType = FlingType.Everywhere,
                sceneName = "Fungus1_10",
            },
        };

        static DualPlacement gruzmother = new DualPlacement
        {
            Test = new SDBool { id = "Battle Scene", sceneName = SceneNames.Crossroads_04 },
            trueLocation = new CoordinateLocation
            {
                name = "Gruz_Mother",
                sceneName = SceneNames.Crossroads_04,
                x = 92.5f,
                y = 15.5f,
            },
            falseLocation = new GruzMotherDropLocation
            {
                name = "Gruz_Mother",
                sceneName = SceneNames.Crossroads_04,
                removeGeo = true,
            },
        };

        static DualPlacement vengeflyking = new DualPlacement
        {
            Test = new PDBool { boolName = nameof(PlayerData.zoteRescuedBuzzer)},
            trueLocation = new CoordinateLocation
            {
                name = LocationNames.Boss_Geo_Vengefly_King,
                sceneName = SceneNames.Fungus1_20_v02,
                x = 45f,
                y = 13.5f,
            },
            falseLocation = new EnemyLocation
            {
                name = LocationNames.Boss_Geo_Vengefly_King,
                sceneName = SceneNames.Fungus1_20_v02,
                objectName = "Giant Buzzer",
                removeGeo = true,
            }
        };

        static DualPlacement vengeflyking_alt = new DualPlacement
        {
            Test = new PDBool { boolName = nameof(PlayerData.zoteRescuedBuzzer) },
            trueLocation = new CoordinateLocation
            {
                name = LocationNames.Boss_Geo_Vengefly_King,
                sceneName = SceneNames.Fungus1_20_v02,
                x = 45f,
                y = 13.5f,
            },
            falseLocation = new EnemyFsmLocation
            {
                name = LocationNames.Boss_Geo_Vengefly_King,
                sceneName = SceneNames.Fungus1_20_v02,
                enemyObj = "Giant Buzzer",
                enemyFsm = "Big Buzzer",
                removeGeo = true,
            }
        };

        static DualPlacement soulwarrior = new DualPlacement
        {
            Test = new SDBool { id = "Battle Scene v2", sceneName = SceneNames.Ruins1_23 },
            trueLocation = new CoordinateLocation
            {
                name = LocationNames.Boss_Geo_Sanctum_Soul_Warrior,
                sceneName = SceneNames.Ruins1_23,
                x = 30f,
                y = 75f,
            },
            falseLocation = new EnemyLocation
            {
                name = LocationNames.Boss_Geo_Sanctum_Soul_Warrior,
                sceneName = SceneNames.Ruins1_23,
                objectName = "Mage Knight",
                removeGeo = true,
            }
        };

        static AbstractPlacement crystalguardian = new DualPlacement
        {
            Test = new PDBool { boolName = nameof(PlayerData.defeatedMegaBeamMiner) },
            trueLocation = new CoordinateLocation
            {
                name = LocationNames.Boss_Geo_Crystal_Guardian,
                sceneName = SceneNames.Mines_18,
                x = 34f,
                y = 11.5f,
            },
            falseLocation = new EnemyFsmLocation
            {
                name = LocationNames.Boss_Geo_Crystal_Guardian,
                sceneName = SceneNames.Mines_18,
                enemyFsm = "Beam Miner",
                enemyObj = "Mega Zombie Beam Miner (1)",
                removeGeo = true,
            }
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

        static AutoPlacement scloak = new AutoPlacement
        {
            location = new ShadeCloakLocation
            {
                name = "Shade_Cloak",
                flingType = FlingType.Everywhere,
                sceneName = SceneNames.Abyss_10
            }
        };

        static AutoPlacement shriek = new AutoPlacement
        {
            location = new AbyssShriekLocation
            {
                name = "Abyss_Shriek",
                flingType = FlingType.Everywhere,
                sceneName = SceneNames.Abyss_12,
            },
        };

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

        public static AutoPlacement brumm = new AutoPlacement
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

        static AutoPlacement crystal = new AutoPlacement
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

        static AutoPlacement shadesoul = new AutoPlacement
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

        static AutoPlacement cliffsmap = new AutoPlacement
        {
            location = new CorniferLocation
            {
                name = "Howling_Cliffs_Map",
                sceneName = "Cliffs_01",
                objectName = "Cornifer",
                flingType = FlingType.Everywhere,
            },
            
        };

        static AutoPlacement mato = new AutoPlacement
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

        static AbstractPlacement lurker = new DualPlacement
        {
            Test = new PDBool { boolName = nameof(PlayerData.killedPaleLurker)},
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

        static AbstractItem equipped_crest => new Items.EquippedCharmItem
        {
            name = "Defender's_Crest-E",
            charmNum = 10,
            UIDef = Finder.GetItem(ItemNames.Defenders_Crest).UIDef.Clone()
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

        static AbstractPlacement voidheart = new AutoPlacement
        {
            location = new VoidHeartLocation
            {
                name = "Void_Heart",
                flingType = FlingType.DirectDeposit,
                sceneName = SceneNames.Dream_Abyss,
            }
        };

        static AbstractPlacement fury = new AutoPlacement
        {
            location = new ExistingContainerLocation
            {
                sceneName = "Tutorial_01",
                fsmName = "Chest Control",
                objectName = "Chest",
                containerType = Container.Chest,
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
            defaultShopItems = DefaultShopItems.None,
            dungDiscount = false,
            requiredPlayerDataBool = string.Empty,
        };

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
