using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Tags
{
    public class DestroyGrubRewardTag : Tag, IActiveSceneChangedTag
    {
        public GrubfatherRewards destroyRewards;

        public void OnActiveSceneChanged(Scene from, Scene to)
        {
            if (to.name == "Crossroads_38")
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
}
