using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Tags
{
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
                if ((destroyRewards & (GrubfatherRewards)((long)1 << i)) != 0)
                {
                    UnityEngine.Object.Destroy(to.FindGameObject($"Grub King\\Reward {i + 1}"));
                }
            }
        }
    }
}
