using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class MenderbugUnlock : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Crossroads_04, new("Mender Door", "Check State"), OnEnable);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Crossroads_04, new("Mender Door", "Check State"), OnEnable);
        }

        private void OnEnable(PlayMakerFSM fsm)
        {
            FsmState pause = fsm.GetState("Pause");
            FsmState opened = fsm.GetState("Opened");
            pause.Transitions[0].SetToState(opened);
        }
    }
}
