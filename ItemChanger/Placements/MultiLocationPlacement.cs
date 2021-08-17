using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Placements
{
    public abstract class MultiLocationPlacement : AbstractPlacement
    {
        /// <summary>
        /// Additional locations managed by the placment, not including Location
        /// </summary>
        public abstract IEnumerable<AbstractLocation> SecondaryLocations { get; }

        public override void OnLoad()
        {
            base.OnLoad();
            foreach (var loc in SecondaryLocations)
            {
                loc.Placement = this;
                loc.OnLoad();
            }
        }

        public override void OnUnload()
        {
            base.OnUnload();
            foreach (var loc in SecondaryLocations)
            {
                loc.OnUnload();
            }
        }

        public override void OnEnableLocal(PlayMakerFSM fsm)
        {
            base.OnEnableLocal(fsm);
            // TODO: Should this check scene?
            foreach (var loc in SecondaryLocations)
            {
                loc.OnEnableLocal(fsm);
            }
        }

        public override void OnEnableGlobal(PlayMakerFSM fsm)
        {
            base.OnEnableGlobal(fsm);
            foreach (var loc in SecondaryLocations)
            {
                loc.OnEnableGlobal(fsm);
            }
        }

        public override void OnActiveSceneChanged(Scene from, Scene to)
        {
            base.OnActiveSceneChanged(from, to);
            foreach (var loc in SecondaryLocations)
            {
                loc.OnActiveSceneChanged(from, to);
            }
        }

        public override void OnLanguageGet(LanguageGetArgs args)
        {
            base.OnLanguageGet(args);
            foreach (var loc in SecondaryLocations)
            {
                loc.OnLanguageGet(args);
            }
        }

        public override void OnNextSceneReady(Scene next)
        {
            base.OnNextSceneReady(next);
            foreach (var loc in SecondaryLocations)
            {
                loc.OnNextSceneReady(next);
            }
        }

        public override void OnSceneFetched(Scene target)
        {
            base.OnSceneFetched(target);
            foreach (var loc in SecondaryLocations)
            {
                loc.OnSceneFetched(target);
            }
        }
    }
}
