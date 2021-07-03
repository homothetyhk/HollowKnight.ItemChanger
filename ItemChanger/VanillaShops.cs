using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Placements;
using ItemChanger.Tags;
using SereCore;

namespace ItemChanger
{
    static class VanillaShops
    {
        public static void Print()
        {
            AbstractItem mask1 = Finder.GetItem(ItemNames.Mask_Shard);
            mask1.AddTag<CostTag>().Cost = Cost.NewGeoCost(150);
            mask1.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.slyShellFrag1);

            AbstractItem mask2 = Finder.GetItem(ItemNames.Mask_Shard);
            mask2.AddTag<CostTag>().Cost = Cost.NewGeoCost(500);
            mask2.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.slyShellFrag2);
            mask2.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.slyShellFrag1);

            AbstractItem mask3 = Finder.GetItem(ItemNames.Mask_Shard);
            mask3.AddTag<CostTag>().Cost = Cost.NewGeoCost(800);
            mask3.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.slyShellFrag3);
            mask3.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.slyShellFrag2);
            mask3.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.gaveSlykey);

            AbstractItem mask4 = Finder.GetItem(ItemNames.Mask_Shard);
            mask4.AddTag<CostTag>().Cost = Cost.NewGeoCost(1500);
            mask4.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.slyShellFrag4);
            mask4.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.slyShellFrag3);
            mask4.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.gaveSlykey);

            AbstractItem vessel1 = Finder.GetItem(ItemNames.Vessel_Fragment);
            vessel1.AddTag<CostTag>().Cost = Cost.NewGeoCost(550);
            vessel1.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.slyVesselFrag1);

            AbstractItem vessel2 = Finder.GetItem(ItemNames.Vessel_Fragment);
            vessel2.AddTag<CostTag>().Cost = Cost.NewGeoCost(900);
            vessel2.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.slyVesselFrag1);
            vessel2.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.gaveSlykey);

            AbstractItem elegant_key = Finder.GetItem(ItemNames.Elegant_Key);
            elegant_key.AddTag<CostTag>().Cost = Cost.NewGeoCost(800);
            elegant_key.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.gaveSlykey);

            AbstractItem lantern = Finder.GetItem(ItemNames.Lumafly_Lantern);
            lantern.AddTag<CostTag>().Cost = Cost.NewGeoCost(1800);

            AbstractItem gathering_swarm = Finder.GetItem(ItemNames.Gathering_Swarm);
            gathering_swarm.AddTag<CostTag>().Cost = Cost.NewGeoCost(300);

            AbstractItem stalwart_shell = Finder.GetItem(ItemNames.Stalwart_Shell);
            stalwart_shell.AddTag<CostTag>().Cost = Cost.NewGeoCost(200);

            AbstractItem heavy_blow = Finder.GetItem(ItemNames.Heavy_Blow);
            heavy_blow.AddTag<CostTag>().Cost = Cost.NewGeoCost(350);
            heavy_blow.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.gaveSlykey);

            AbstractItem sprintmaster = Finder.GetItem(ItemNames.Sprintmaster);
            sprintmaster.AddTag<CostTag>().Cost = Cost.NewGeoCost(400);
            sprintmaster.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.gaveSlykey);

            AbstractItem rancid_egg = Finder.GetItem(ItemNames.Rancid_Egg);
            rancid_egg.AddTag<CostTag>().Cost = Cost.NewGeoCost(60);
            rancid_egg.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.slyRancidEgg);

            AbstractItem simple_key = Finder.GetItem(ItemNames.Simple_Key);
            simple_key.AddTag<CostTag>().Cost = Cost.NewGeoCost(950);
            simple_key.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.slySimpleKey);


            AbstractPlacement sly = new ShopPlacement
            {
                location = Finder.GetLocation(LocationNames.Sly) as ShopLocation,
                Items = new List<AbstractItem>
                {
                    mask1,
                    mask2,
                    mask3,
                    mask4,
                    vessel1,
                    vessel2,
                    elegant_key,
                    lantern,
                    gathering_swarm,
                    stalwart_shell,
                    heavy_blow,
                    sprintmaster,
                    rancid_egg,
                    simple_key,
                },
            };


            AbstractItem wayward_compass = Finder.GetItem(ItemNames.Wayward_Compass);
            wayward_compass.AddTag<CostTag>().Cost = Cost.NewGeoCost(220);

            AbstractItem quill = Finder.GetItem(ItemNames.Quill);
            quill.AddTag<CostTag>().Cost = Cost.NewGeoCost(120);

            AbstractItem crossroads_map = Finder.GetItem(ItemNames.Crossroads_Map);
            crossroads_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(40);
            crossroads_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_crossroadsLeft);
            crossroads_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapCrossroads);

            AbstractItem greenpath_map = Finder.GetItem(ItemNames.Greenpath_Map);
            greenpath_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(80);
            greenpath_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_greenpathLeft);
            greenpath_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapGreenpath);

            AbstractItem canyon_map = Finder.GetItem(ItemNames.Fog_Canyon_Map);
            canyon_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(200);
            canyon_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_fogCanyonLeft);
            canyon_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapFogCanyon);

            AbstractItem wastes_map = Finder.GetItem(ItemNames.Fungal_Wastes_Map);
            wastes_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);
            wastes_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_fungalWastesLeft);
            wastes_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapFungalWastes);

            AbstractItem city_map = Finder.GetItem(ItemNames.City_of_Tears_Map);
            city_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(120);
            city_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_cityLeft);
            city_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapCity);

            AbstractItem waterways_map = Finder.GetItem(ItemNames.Royal_Waterways_Map);
            waterways_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);
            waterways_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_waterwaysLeft);
            waterways_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapWaterways);

            AbstractItem mines_map = Finder.GetItem(ItemNames.Crystal_Peak_Map);
            mines_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(150);
            mines_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_minesLeft);
            mines_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapMines);

            AbstractItem rg_map = Finder.GetItem(ItemNames.Resting_Grounds_Map);
            rg_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(75);
            rg_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.visitedRestingGrounds);
            rg_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapRestingGrounds);

            AbstractItem cliffs_map = Finder.GetItem(ItemNames.Howling_Cliffs_Map);
            cliffs_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);
            cliffs_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_cliffsLeft);
            cliffs_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapCliffs);

            AbstractItem deepnest_map = Finder.GetItem(ItemNames.Deepnest_Map);
            deepnest_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(50);
            deepnest_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_deepnestLeft);
            deepnest_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapDeepnest);

            AbstractItem edge_map = Finder.GetItem(ItemNames.Kingdoms_Edge_Map);
            edge_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(150);
            edge_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_outskirtsLeft);
            edge_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapOutskirts);

            AbstractItem gardens_map = Finder.GetItem(ItemNames.Queens_Gardens_Map);
            gardens_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(200);
            gardens_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_royalGardensLeft);
            gardens_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapRoyalGardens);

            AbstractItem basin_map = Finder.GetItem(ItemNames.Ancient_Basin_Map);
            basin_map.AddTag<CostTag>().Cost = Cost.NewGeoCost(150);
            basin_map.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.corn_abyssLeft);
            basin_map.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.mapAbyss);

            AbstractItem bench_pin = Finder.GetItem(ItemNames.Bench_Pin);
            bench_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);

            AbstractItem cocoon_pin = Finder.GetItem(ItemNames.Cocoon_Pin);
            cocoon_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);

            AbstractItem root_pin = Finder.GetItem(ItemNames.Whispering_Root_Pin);
            root_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(150);
            root_pin.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.hasDreamNail);

            AbstractItem warrior_pin = Finder.GetItem(ItemNames.Warriors_Grave_Pin);
            warrior_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(180);
            warrior_pin.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.hasDreamNail);

            AbstractItem vendor_pin = Finder.GetItem(ItemNames.Vendor_Pin);
            vendor_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);

            AbstractItem stag_pin = Finder.GetItem(ItemNames.Stag_Station_Pin);
            stag_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);
            stag_pin.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.metStag);

            AbstractItem tram_pin = Finder.GetItem(ItemNames.Tram_Pin);
            tram_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);
            tram_pin.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.hasTramPass);

            AbstractItem spa_pin = Finder.GetItem(ItemNames.Hot_Spring_Pin);
            spa_pin.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);

            AbstractItem scarab_marker = Finder.GetItem(ItemNames.Scarab_Marker);
            scarab_marker.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);

            AbstractItem shell_marker = Finder.GetItem(ItemNames.Shell_Marker);
            shell_marker.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);

            AbstractItem token_marker = Finder.GetItem(ItemNames.Token_Marker);
            token_marker.AddTag<CostTag>().Cost = Cost.NewGeoCost(100);
            token_marker.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.hasDash);

            AbstractItem gleaming_marker = Finder.GetItem(ItemNames.Gleaming_Marker);
            gleaming_marker.AddTag<CostTag>().Cost = Cost.NewGeoCost(210);
            token_marker.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.hasDash);

            AbstractPlacement iselda = new ShopPlacement
            {
                location = Finder.GetLocation(LocationNames.Iselda) as ShopLocation,
                Items = new List<AbstractItem>
                {
                    wayward_compass,
                    quill,
                    crossroads_map,
                    greenpath_map,
                    canyon_map,
                    wastes_map,
                    city_map,
                    waterways_map,
                    mines_map,
                    rg_map,
                    cliffs_map,
                    deepnest_map,
                    edge_map,
                    gardens_map,
                    basin_map,
                    bench_pin,
                    cocoon_pin,
                    root_pin,
                    warrior_pin,
                    vendor_pin,
                    stag_pin,
                    tram_pin,
                    spa_pin,
                    scarab_marker,
                    shell_marker,
                    token_marker,
                    gleaming_marker
                },
            };

            AbstractItem fragile_heart = Finder.GetItem(ItemNames.Fragile_Heart);
            fragile_heart.AddTag<CostTag>().Cost = Cost.NewGeoCost(350);

            AbstractItem fragile_greed = Finder.GetItem(ItemNames.Fragile_Greed);
            fragile_greed.AddTag<CostTag>().Cost = Cost.NewGeoCost(250);

            AbstractItem fragile_strength = Finder.GetItem(ItemNames.Fragile_Strength);
            fragile_strength.AddTag<CostTag>().Cost = Cost.NewGeoCost(600);

            AbstractItem repair_heart = new BoolItem
            {
                fieldName = nameof(PlayerData.brokenCharm_23),
                setValue = false,
                name = "Fragile_Heart_(Repair)",
                UIDef = new UIDefs.UIDef
                {
                    nameKey = "CHARM_NAME_23_BRK",
                    shopDescKey = "SHOP_DESC_GLASS_HP_BRK",
                    spriteKey = "Charms.23_B",
                },
            };
            repair_heart.AddTag<CostTag>().Cost = Cost.NewGeoCost(200);
            repair_heart.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.fragileHealth_unbreakable);
            repair_heart.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.brokenCharm_23);

            AbstractItem repair_greed = new BoolItem
            {
                fieldName = nameof(PlayerData.brokenCharm_24),
                setValue = false,
                name = "Fragile_Greed_(Repair)",
                UIDef = new UIDefs.UIDef
                {
                    nameKey = "CHARM_NAME_24_BRK",
                    shopDescKey = "SHOP_DESC_GLASS_GEO_BRK",
                    spriteKey = "Charms.24_B",
                },
            };
            repair_greed.AddTag<CostTag>().Cost = Cost.NewGeoCost(150);
            repair_greed.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.fragileGreed_unbreakable);
            repair_greed.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.brokenCharm_24);

            AbstractItem repair_strength = new BoolItem
            {
                fieldName = nameof(PlayerData.brokenCharm_23),
                setValue = false,
                name = "Fragile_Strength_(Repair)",
                UIDef = new UIDefs.UIDef
                {
                    nameKey = "CHARM_NAME_25_BRK",
                    shopDescKey = "SHOP_DESC_GLASS_ATTACK_BRK",
                    spriteKey = "Charms.25_B",
                },
            };
            repair_strength.AddTag<CostTag>().Cost = Cost.NewGeoCost(350);
            repair_strength.AddTag<PDBoolShopRemoveTag>().fieldName = nameof(PlayerData.fragileStrength_unbreakable);
            repair_strength.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.brokenCharm_25);

            AbstractPlacement leg_eater = new ShopPlacement
            {
                location = Finder.GetLocation(LocationNames.Leg_Eater) as ShopLocation,
                Items = new List<AbstractItem>
                {
                    fragile_heart,
                    repair_heart,
                    fragile_greed,
                    repair_greed,
                    fragile_strength,
                    repair_strength
                },
            };

            AbstractItem lifeblood_heart = Finder.GetItem(ItemNames.Lifeblood_Heart);
            lifeblood_heart.AddTag<CostTag>().Cost = Cost.NewGeoCost(250);

            AbstractItem longnail = Finder.GetItem(ItemNames.Longnail);
            longnail.AddTag<CostTag>().Cost = Cost.NewGeoCost(300);

            AbstractItem steady_body = Finder.GetItem(ItemNames.Steady_Body);
            steady_body.AddTag<CostTag>().Cost = Cost.NewGeoCost(120);

            AbstractItem shaman_stone = Finder.GetItem(ItemNames.Shaman_Stone);
            shaman_stone.AddTag<CostTag>().Cost = Cost.NewGeoCost(220);

            AbstractItem quick_focus = Finder.GetItem(ItemNames.Quick_Focus);
            quick_focus.AddTag<CostTag>().Cost = Cost.NewGeoCost(800);
            
            AbstractItem notch1 = Finder.GetItem(ItemNames.Charm_Notch);
            notch1.AddTag<CostTag>().Cost = Cost.NewGeoCost(120) 
                + new PDIntCost { fieldName = nameof(PlayerData.charmsOwned), amount = 5, uiText = "Requires 5 charms" };
            notch1.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.salubraNotch1);

            AbstractItem notch2 = Finder.GetItem(ItemNames.Charm_Notch);
            notch2.AddTag<CostTag>().Cost = Cost.NewGeoCost(500)
                + new PDIntCost { fieldName = nameof(PlayerData.charmsOwned), amount = 10, uiText = "Requires 10 charms" };
            notch2.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.salubraNotch2);

            AbstractItem notch3 = Finder.GetItem(ItemNames.Charm_Notch);
            notch3.AddTag<CostTag>().Cost = Cost.NewGeoCost(900)
                + new PDIntCost { fieldName = nameof(PlayerData.charmsOwned), amount = 18, uiText = "Requires 18 charms" };
            notch3.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.salubraNotch3);

            AbstractItem notch4 = Finder.GetItem(ItemNames.Charm_Notch);
            notch4.AddTag<CostTag>().Cost = Cost.NewGeoCost(120)
                + new PDIntCost { fieldName = nameof(PlayerData.charmsOwned), amount = 25, uiText = "Requires 25 charms" };
            notch4.AddTag<SetPDBoolTag>().fieldName = nameof(PlayerData.salubraNotch4);

            AbstractItem blessing = new BoolItem 
            {
                name = "Salubra's_Blessing",
                fieldName = nameof(PlayerData.salubraBlessing),
                UIDef = new UIDefs.BigUIDef
                {
                    spriteKey = "ShopIcons.SalubrasBlessing.png",
                    nameKey = "SHOP_NAME_SALUBRA", // TODO: this key gives "My Blessing" not "Salubra's Blessing"
                    shopDescKey = "SHOP_SALUBRA_BLESSING",
                    bigSpriteKey = "Prompts.SalubrasBlessing.png",
                    descOneKey = "GET_BLESSING_1",
                    descTwoKey = "GET_BLESSING_2",
                    buttonKey = "RANDOMIZER_EMPTY",
                    takeKey = "GET_ITEM_INTRO8"
                },
            };
            blessing.AddTag<CostTag>().Cost = Cost.NewGeoCost(800);
            blessing.AddTag<PDBoolShopReqTag>().fieldName = nameof(PlayerData.salubraNotch4);

            AbstractPlacement salubra = new ShopPlacement
            {
                location = Finder.GetLocation(LocationNames.Salubra) as ShopLocation,
                Items = new List<AbstractItem>
                {
                    lifeblood_heart,
                    longnail,
                    steady_body,
                    shaman_stone,
                    quick_focus,
                    notch1,
                    notch2,
                    notch3,
                    notch4,
                    blessing
                },
            };

            AbstractPlacement[] vanilla = new[]
            {
                sly, iselda, salubra, leg_eater
            };

            Finder.Serialize("vanilla.json", vanilla);
        }
    }
}
