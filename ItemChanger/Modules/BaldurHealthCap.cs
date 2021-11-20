using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace ItemChanger.Modules
{
    /// <summary>
    /// Module which sets Baldur hp to the specified value and allows Baldurs to spit before Vengeful Spirit is obtained.
    /// </summary>
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
