using ItemChanger.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    [Obsolete("Override AbstractPlacement and/or use an ILocation instead.")]
    public struct _Location
    {
        public string name;
        public string sceneName;

        public string objectName;
        public string altObjectName;

        public bool shop;
        public string requiredPlayerDataBool;
        public bool dungDiscount;

        public bool oldShiny;
        public string oldShinyFsm;

        public bool replaceObject;

        public bool newShiny;
        public float x;
        public float y;

        public bool start;

        public bool changeSceneAfterPickup;
        public string changeToScene;
        
        public bool shinyChest;
        public bool geoChest;
        public string chestName;
        public string chestFsm;

        public string[] destroyObjects;

        public int cost;
        public CostType costType;
        public SpecialFSMEdit specialFSMEdit;
        public SpecialPDHook specialPDHook;
        public LocationPool pool;

        public _Location(string defaultLocationName)
        {
            if (!XmlManager.Locations.TryGetValue(defaultLocationName, out _Location val))
            {
                Modding.Logger.LogError($"No default location found corresponding to {defaultLocationName}");
                throw new KeyNotFoundException();
            }
            this = val;
        }
        public override string ToString()
        {
            return name;
        }

        public enum SpecialPDHook
        {
            None = 0,
            OverrideSlyBasementFlag,
            BlockNailsmithSheo,
            BlockFirstGrimmMeeting,
            UnlockGodseeker,
            OverrideFlowerQuestRewardGiven,
            dreamReward1,
            dreamReward3,
            dreamReward4,
            dreamReward5,
            dreamReward5b,
            dreamReward6,
            dreamReward7,
            dreamReward8,
        }

        public enum SpecialFSMEdit
        {
            None = 0,
            MothwingCloakArena,
            DreamNailCutscene,
            SlyBasement,
            DesolateDive,
            TutTablet,
            BroodingMawlek,
            PaleLurker,
            VoidHeart,
            Colosseum1,
            Colosseum2,
            CollectorGrubs,
            Stag,
            WhisperingRoot
        }

        public enum LocationPool
        {
            None,
            Start,
            Shop,
            Dreamer,
            Skill,
            Charm,
            Key,
            Mask,
            Vessel,
            Ore,
            Notch,
            Geo,
            Egg,
            Relic,
            Map,
            Stag,
            Grub,
            Root,

            Cursed,
            Custom,
        }
    }
}
