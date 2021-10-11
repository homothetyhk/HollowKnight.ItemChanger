using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger.Items
{
    /// <summary>
    /// Item which gives Grimmchild and activates the Nightmare Lantern.
    /// </summary>
    public class Grimmchild1Item : AbstractItem
    {
        public override void GiveImmediate(GiveInfo info)
        {
            PlayerData.instance.SetBool(nameof(PlayerData.instance.gotCharm_40), true);
            PlayerData.instance.SetBool(nameof(PlayerData.nightmareLanternAppeared), true);
            PlayerData.instance.SetBool(nameof(PlayerData.nightmareLanternLit), true);
            PlayerData.instance.SetBool(nameof(PlayerData.troupeInTown), true);
            PlayerData.instance.SetBool(nameof(PlayerData.divineInTown), true);
            PlayerData.instance.SetBool(nameof(PlayerData.metGrimm), true);
            PlayerData.instance.SetInt(nameof(PlayerData.flamesRequired), 3);
            PlayerData.instance.SetInt(nameof(PlayerData.grimmChildLevel), 1);
        }

        public override bool Redundant()
        {
            return PlayerData.instance.GetBool(nameof(PlayerData.instance.gotCharm_40));
        }
    }
}
