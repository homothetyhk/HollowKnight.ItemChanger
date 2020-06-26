using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ItemChanger.Location;

namespace ItemChanger
{
    internal static class PDHooks
    {
        private static HashSet<SpecialPDHook> specialPDHooks;
        public static void AddSpecialPDHook(SpecialPDHook pd) => specialPDHooks.Add(pd);
        public static void ResetSpecialPDHooks() => specialPDHooks.Clear(); 



        public static void Hook()
        {
            specialPDHooks = new HashSet<SpecialPDHook>();
            ModHooks.Instance.GetPlayerBoolHook += GetBoolOverride;
            ModHooks.Instance.SetPlayerBoolHook += SetBoolOverride;
        }
        public static void Unhook()
        {
            ModHooks.Instance.GetPlayerBoolHook -= GetBoolOverride;
            ModHooks.Instance.SetPlayerBoolHook -= SetBoolOverride;
        }

        private static bool GetBoolOverride(string boolName)
        {
            if (boolName.StartsWith("ItemChanger.")) return ItemChanger.instance.Settings.CheckObtained(boolName.Split('.').Last());
            switch (boolName)
            {
                default:
                    return PlayerData.instance.GetBoolInternal(boolName);

                case nameof(PlayerData.hasCharm):
                    return true;
                case nameof(PlayerData.unchainedHollowKnight):
                case nameof(PlayerData.encounteredMimicSpider):
                case nameof(PlayerData.infectedKnightEncountered):
                case nameof(PlayerData.mageLordEncountered):
                case nameof(PlayerData.mageLordEncountered_2):
                    return true;

                case nameof(PlayerData.nailsmithSheo) when specialPDHooks.Contains(SpecialPDHook.BlockNailsmithSheo):
                    return false;

                case nameof(PlayerData.gotSlyCharm) when specialPDHooks.Contains(SpecialPDHook.OverrideSlyBasementFlag):
                    return ItemChanger.instance.Settings.gotSlyCharm;

                case nameof(PlayerData.metGrimm) when specialPDHooks.Contains(SpecialPDHook.BlockFirstGrimmMeeting):
                    return true;

                case nameof(PlayerData.godseekerUnlocked) when specialPDHooks.Contains(SpecialPDHook.UnlockGodseeker):
                    return true;

                case nameof(PlayerData.xunRewardGiven) when specialPDHooks.Contains(SpecialPDHook.OverrideFlowerQuestRewardGiven):
                    return PlayerData.instance.GetBool(nameof(PlayerData.xunFlowerGiven));

                case nameof(PlayerData.dreamReward1) when specialPDHooks.Contains(SpecialPDHook.dreamReward1):
                    return true;

                case nameof(PlayerData.dreamReward3) when specialPDHooks.Contains(SpecialPDHook.dreamReward3):
                    return true;

                case nameof(PlayerData.dreamReward4) when specialPDHooks.Contains(SpecialPDHook.dreamReward4):
                    return true;

                case nameof(PlayerData.dreamReward5) when specialPDHooks.Contains(SpecialPDHook.dreamReward5):
                    return true;

                case nameof(PlayerData.dreamReward5b) when specialPDHooks.Contains(SpecialPDHook.dreamReward5b):
                    return true;

                case nameof(PlayerData.dreamReward6) when specialPDHooks.Contains(SpecialPDHook.dreamReward6):
                    return true;

                case nameof(PlayerData.dreamReward7) when specialPDHooks.Contains(SpecialPDHook.dreamReward7):
                    return true;

                case nameof(PlayerData.dreamReward8) when specialPDHooks.Contains(SpecialPDHook.dreamReward8):
                    return true;
            }
            
        }

        private static void SetBoolOverride(string boolName, bool value)
        {
            if (boolName.StartsWith("ItemChanger."))
            {
                if (value)
                {
                    string id = boolName.Split('.').Last();
                    GiveItemActions.GiveItem(ILP.ILPs[id]);
                }
                return;
            }

            PlayerData.instance.SetBoolInternal(boolName, value);
            switch (boolName)
            {
                case nameof(PlayerData.hasAcidArmour) when value:
                    PlayMakerFSM.BroadcastEvent("GET ACID ARMOUR");
                    break;

                case nameof(PlayerData.hasDreamGate) when value:
                    FSMUtility.LocateFSM(HeroController.instance.gameObject, "Dream Nail").FsmVariables
                    .GetFsmBool("Dream Warp Allowed").Value = true;
                    break;

                case nameof(PlayerData.hasCyclone) when value:
                case nameof(PlayerData.hasDashSlash) when value:
                case nameof(PlayerData.hasUpwardSlash) when value:
                    PlayerData.instance.SetBoolInternal(nameof(PlayerData.hasNailArt), true);
                    if (PlayerData.instance.hasCyclone && PlayerData.instance.hasDashSlash && PlayerData.instance.hasUpwardSlash)
                    {
                        PlayerData.instance.SetBoolInternal(nameof(PlayerData.hasAllNailArts), true);
                    }
                    break;
            }
        }
    }
}
