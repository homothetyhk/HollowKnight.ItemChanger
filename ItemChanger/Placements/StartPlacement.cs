using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;

namespace ItemChanger.Placements
{
    public class StartPlacement : AbstractPlacement
    {
        [NonSerialized]
        [Newtonsoft.Json.JsonIgnore]
        private bool inGame;

        public override void OnLoad()
        {
            inGame = true;
            base.OnLoad();
        }

        public override void OnUnload()
        {
            inGame = false;
            base.OnUnload();
        }


        public StartLocation location;
        public override AbstractLocation Location => location;

        public override void AddItem(AbstractItem item)
        {
            if (inGame) item.Give(this, new GiveInfo
            {
                Container = Container.Unknown,
                FlingType = Location.flingType,
                MessageType = MessageType.Corner,
                Transform = Location.Transform,
            });

            Items.Add(item);
        }
    }
}
