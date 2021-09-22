using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace ItemChanger.Locations
{
    public class StartLocation : AutoLocation
    {
        public MessageType MessageType;

        protected override void OnLoad()
        {
            On.GameManager.FinishedEnteringScene += OnFinishedEnteringScene;
        }

        protected override void OnUnload()
        {
            On.GameManager.FinishedEnteringScene -= OnFinishedEnteringScene;
        }


        private void OnFinishedEnteringScene(On.GameManager.orig_FinishedEnteringScene orig, GameManager self)
        {
            orig(self);
            GiveItems();
        }

        private void GiveItems()
        {
            if (!Placement.AllObtained())
            {
                Placement.GiveAll(new GiveInfo
                {
                    MessageType = MessageType,
                    Container = "Start",
                    FlingType = flingType,
                    Transform = null,
                    Callback = null,
                });
            }
        }


        public override AbstractPlacement Wrap()
        {
            return new Placements.AutoPlacement(name)
            {
                Location = this,
            };
        }
    }
}
