using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag for destroying Seer rewards according to the flags of the enumeration.
    /// </summary>
    public class DestroySeerRewardTag : Tag
    {
        public SeerRewards destroyRewards;

        public override void Load(object parent)
        {
            for (int i = 0; i < 5; i++)
            {
                if ((destroyRewards & (SeerRewards)(1 << i)) != 0) PlayerData.instance.SetBool($"dreamReward{i + 1}", true);
            }
            if ((destroyRewards & SeerRewards.dreamReward5b) != 0) PlayerData.instance.SetBool(nameof(PlayerData.dreamReward5b), true);
            for (int i = 6; i <= 9; i++)
            {
                if ((destroyRewards & (SeerRewards)(1 << i)) != 0) PlayerData.instance.SetBool($"dreamReward{i}", true);
            }
        }

        public override void Unload(object parent)
        {
            
        }

    }
}
