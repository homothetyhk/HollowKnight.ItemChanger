using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    [DefaultModule]
    public class BaldurHealthCap : Module
    {
        public int cap = 5;

        public override void Initialize()
        {
            Events.AddFsmEdit(new("Blocker Control"), CapBaldurHP);
        }

        public override void Unload()
        {
            Events.RemoveFsmEdit(new("Blocker Control"), CapBaldurHP);
        }

        private void CapBaldurHP(PlayMakerFSM fsm)
        {
            if (!fsm.gameObject.name.StartsWith("Blocker")) return;

            HealthManager hm = fsm.GetComponent<HealthManager>();
            if (hm != null)
            {
                hm.hp = cap;
            }
            
            // remove VS check from baldur spawn
            if (fsm != null)
            {
                fsm.GetState("Can Roller?").RemoveActionsOfType<IntCompare>();
            }

        }
    }
}
