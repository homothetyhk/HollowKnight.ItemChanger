using Modding;
using ItemChanger.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ItemChanger.Locations;
using ItemChanger.Internal;
using Newtonsoft.Json;

namespace ItemChanger
{
    // The api uses Activator.CreateInstance to construct local settings when entering the main menu.
    // Since Settings is intended to be nullable, we only let the api work with a wrapper.
    public readonly struct SaveSettings
    {
        public static readonly SaveSettings Empty = default;
        [Newtonsoft.Json.JsonConstructor]
        public SaveSettings(Settings value) => this.value = value;
        public readonly Settings value;
        public static implicit operator SaveSettings(Settings value) => new(value);
        public static implicit operator Settings(SaveSettings s) => s.value;
    }

    public class Settings
    {
        public Settings() 
        {
            mods = ModuleCollection.Create();
        }

        public ModuleCollection mods;
        public Dictionary<string, AbstractPlacement> Placements = new Dictionary<string, AbstractPlacement>();
        [JsonConverter(typeof(Transition.TransitionDictConverter))]
        public Dictionary<Transition, ITransition> TransitionOverrides = new Dictionary<Transition, ITransition>();
        public List<IDeployer> Deployers = new();

        public StartDef Start = null;

        public IEnumerable<AbstractItem> GetItems() => Placements.SelectMany(kvp => kvp.Value.Items);
        public IEnumerable<AbstractPlacement> GetPlacements() => (Placements ?? (Placements = new Dictionary<string, AbstractPlacement>())).Select(kvp => kvp.Value);

        internal void SavePlacements(IEnumerable<AbstractPlacement> placements)
        {
            ItemChangerMod.AddPlacements(placements, PlacementConflictResolution.MergeKeepingNew);
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

        internal static bool loaded = false;

        internal void Load()
        {
            if (loaded) throw new InvalidOperationException("An instance of Settings was already loaded.");
            loaded = true;
            mods.Initialize();
            foreach (AbstractPlacement p in GetPlacements()) p.Load();
            foreach (IDeployer d in Deployers) Events.AddSceneChangeEdit(d.SceneName, d.OnSceneChange);
        }

        internal void Unload()
        {
            if (!loaded) throw new InvalidOperationException("No instance of Settings was loaded.");
            loaded = false;
            mods.Unload();
            foreach (var loc in GetPlacements()) loc.Unload();
            foreach (IDeployer d in Deployers) Events.RemoveSceneChangeEdit(d.SceneName, d.OnSceneChange);
        }
    }
}
