using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public abstract class DualLocation : ContainerLocation
    {
        public abstract bool Decide();
        public ContainerLocation trueLocation;
        public ContainerLocation falseLocation;


        public override bool Supports(Container container)
        {
            return trueLocation.Supports(container) && falseLocation.Supports(container);
        }

        public override void OnSceneFetched(Scene target)
        {
            if (target.name != sceneName) return;

            if (Decide())
            {
                trueLocation.OnSceneFetched(target);
            }
            else
            {
                falseLocation.OnSceneFetched(target);
            }
        }

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            if (to.name != sceneName) return;

            if (Decide())
            {
                trueLocation.OnActiveSceneChanged(from, to);
            }
            else
            {
                falseLocation.OnActiveSceneChanged(from, to);
            }
        }

        public override void OnNextSceneReady(Scene next)
        {
            if (next.name != sceneName) return;

            if (Decide())
            {
                trueLocation.OnNextSceneReady(next);
            }
            else
            {
                falseLocation.OnNextSceneReady(next);
            }
        }

        public override void OnEnable(PlayMakerFSM fsm)
        {
            if (Decide())
            {
                trueLocation.OnEnable(fsm);
            }
            else
            {
                falseLocation.OnEnable(fsm);
            }
        }

        public override void OnLoad()
        {
            trueLocation.Placement = Placement;
            falseLocation.Placement = Placement;

            trueLocation.OnLoad();
            falseLocation.OnLoad();
        }

        public override void OnUnload()
        {
            trueLocation.OnUnload();
            falseLocation.OnUnload();
        }
    }
}
