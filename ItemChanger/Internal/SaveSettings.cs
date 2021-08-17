using Modding;
using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using ItemChanger.Internal;

namespace ItemChanger
{
    public class Settings
    {
        public CustomSkills CustomSkills = new CustomSkills();
        public WorldEvents WorldEvents = new WorldEvents();
        public Dictionary<string, AbstractPlacement> Placements = new Dictionary<string, AbstractPlacement>();

        public StartDef Start = null;

        public IEnumerable<AbstractItem> GetItems() => Placements.SelectMany(kvp => kvp.Value.Items);
        public IEnumerable<AbstractPlacement> GetPlacements() => (Placements ?? (Placements = new Dictionary<string, AbstractPlacement>())).Select(kvp => kvp.Value);

        internal void SavePlacements(IEnumerable<AbstractPlacement> placements)
        {
            ItemChangerMod.AddPlacements(placements, ItemChangerMod.PlacementConflictResolution.MergeKeepingNew);
        }

        internal void ResetSemiPersistentItems()
        {
            foreach (var item in GetItems())
            {
                if (item.GetTag<Tags.IPersistenceTag>(out var tag) && tag.Persistence == Persistence.SemiPersistent)
                {
                    item.RefreshObtained();
                }
            }
        }

        internal void ResetPersistentItems()
        {
            foreach (var item in GetItems())
            {
                if (item.GetTag<Tags.IPersistenceTag>(out var tag) && tag.Persistence == Persistence.Persistent)
                {
                    item.RefreshObtained();
                }
            }
        }


    }

}
