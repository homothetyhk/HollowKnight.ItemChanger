using ItemChanger.Components;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Util;

namespace ItemChanger.Containers
{
    public class BluggsacContainer : Container
    {
        // reference: sharedassets81

        public override string Name => Container.Bluggsac;

        public override void AddGiveEffectToFsm(PlayMakerFSM fsm, ContainerGiveInfo info)
        {
            FsmState init = fsm.GetState("Init");
            init.ClearActions();

            FsmState burst = fsm.GetState("Burst");
            burst.Actions = new[]
            {
                burst.Actions[0], // SendEventByName EnemyKillShake
                burst.Actions[1], // tk2dplayanimationwithevents burst
                // burst.Actions[2], // activategameobject egg
                // burst.Actions[3], // setparent egg
                new Lambda(InstantiateShiniesAndGiveEarly),
                burst.Actions[4], // audioplayeroneshotsingle
                burst.Actions[5], // audiostop
                burst.Actions[6], // playparticleemitter
                burst.Actions[7], // stopparticleemitter
                burst.Actions[8], // setparent pt blow
            };

            void InstantiateShiniesAndGiveEarly()
            {
                GiveInfo gi = new()
                {
                    Container = Container.Bluggsac,
                    FlingType = info.flingType,
                    Transform = fsm.transform,
                    MessageType = MessageType.Corner,
                };
                GameObject itemParent = new("Item parent");
                itemParent.transform.position = fsm.transform.position;

                foreach (AbstractItem item in info.items)
                {
                    if (!item.IsObtained())
                    {
                        if (item.GiveEarly(Container.Bluggsac))
                        {
                            item.Give(info.placement, gi);
                        }
                        else
                        {
                            GameObject shiny = ShinyUtility.MakeNewShiny(info.placement, item, info.flingType);
                            ShinyUtility.PutShinyInContainer(itemParent, shiny);
                        }
                    }
                }

                foreach (Transform t in itemParent.transform)
                {
                    t.gameObject.SetActive(true);
                }
                info.placement.AddVisitFlag(VisitState.Opened);
            }

        }


    }
}
