using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag for destroying grub rewards according to the flags of the enumeration.
    /// </summary>
    public class DestroyGrubRewardTag : Tag
    {
        public GrubfatherRewards destroyRewards;

        public override void Load(object parent)
        {
            Events.AddSceneChangeEdit(SceneNames.Crossroads_38, DestroyGrubRewards);
        }

        public override void Unload(object parent)
        {
            Events.RemoveSceneChangeEdit(SceneNames.Crossroads_38, DestroyGrubRewards);
        }

        private void DestroyGrubRewards(Scene to)
        {
            for (int i = 0; i < 46; i++)
            {
                if ((destroyRewards & (GrubfatherRewards)(1L << i)) != 0)
                {
                    UnityEngine.Object.Destroy(to.FindGameObject($"Grub King\\Reward {i + 1}"));
                }
            }
        }
    }
}
