using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class PreventZoteDeath : Module
    {
        public override void Initialize()
        {
            Events.AddFsmEdit(new("Check Zote Death"), PreventZoteDeathCheck);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Check Zote Death"), PreventZoteDeathCheck);
        }

        private void PreventZoteDeathCheck(PlayMakerFSM fsm)
        {
            UnityEngine.Object.Destroy(fsm);
        }
    }
}
