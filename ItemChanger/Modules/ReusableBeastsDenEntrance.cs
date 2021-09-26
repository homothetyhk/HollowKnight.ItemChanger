using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class ReusableBeastsDenEntrance : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(SceneNames.Deepnest_Spider_Town, new("RestBench Spider", "Fade"), PreventBeastDenCutsceneSave);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(SceneNames.Deepnest_Spider_Town, new("RestBench Spider", "Fade"), PreventBeastDenCutsceneSave);
        }

        private void PreventBeastDenCutsceneSave(PlayMakerFSM fsm)
        {
            FsmState denHardSave = fsm.GetState("Land");
            denHardSave.RemoveActionsOfType<CallMethodProper>();
            denHardSave.RemoveActionsOfType<SendMessage>();
            denHardSave.RemoveActionsOfType<SetPlayerDataBool>();
        }

    }
}
