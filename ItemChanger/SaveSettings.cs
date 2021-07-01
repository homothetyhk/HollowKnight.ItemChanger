using Modding;
using SereCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;

namespace ItemChanger
{
    public class Settings : ModSettings
    {
        public CustomSkills CustomSkills = new CustomSkills();
        public WorldEvents WorldEvents = new WorldEvents();
        public Dictionary<string, AbstractItem[]> AdditiveGroups = new Dictionary<string, AbstractItem[]>();

        public AbstractPlacement[] Placements = new AbstractPlacement[0];


        public IEnumerable<AbstractItem> GetItems() => Placements.SelectMany(l => l.Items);
        public IEnumerable<AbstractPlacement> GetPlacements() => Placements ?? (Placements = new AbstractPlacement[0]);

        internal void SavePlacements(AbstractPlacement[] locations)
        {
            Placements = locations.ToArray();
        }

        internal void ResetSemiPersistentItems()
        {
            foreach (var item in Ref.Settings.GetItems())
            {
                if (item.GetTag<Tags.IPersistenceTag>(out var tag) && tag.Persistence == Persistence.SemiPersistent)
                {
                    item.RefreshObtained();
                }
            }
        }

        internal void ResetPersistentItems()
        {
            foreach (var item in Ref.Settings.GetItems())
            {
                if (item.GetTag<Tags.IPersistenceTag>(out var tag) && tag.Persistence == Persistence.Persistent)
                {
                    item.RefreshObtained();
                }
            }
        }


    }

    public class SaveSettings : BaseSettings
    {
        SerializableBoolDictionary obtainedItems = new SerializableBoolDictionary();

        public SaveSettings() 
        {
            AfterDeserialize += () => obtainedItems = obtainedItems ?? new SerializableBoolDictionary();
        }

        public bool gotSlyCharm
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool canFocus
        {
            get => GetBool(false);
            set => SetBool(value);
        }

        public bool CheckObtained(string id)
        {
            if (obtainedItems.TryGetValue(id, out bool val)) return val;
            return false;
        }

        public void SetObtained(string id)
        {
            try
            {
                obtainedItems[id] = true;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
