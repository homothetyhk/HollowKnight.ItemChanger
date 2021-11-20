using ItemChanger.Internal;
using ItemChanger.Util;
using Modding;

namespace ItemChanger
{
    public class ItemChangerMod : Mod, ILocalSettings<SaveSettings>, IGlobalSettings<GlobalSettings>, IMenuMod
    {
        internal static ItemChangerMod instance;
        internal static Settings SET;
        internal static GlobalSettings GS = new();
        private bool _hooked = false;

        public ItemChangerMod()
        {
            if (instance != null) throw new NotSupportedException("Cannot construct multiple instances of ItemChangerMod.");

            instance = this;
            Finder.Load();
            LanguageStringManager.Load();
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            ObjectCache.Setup(preloadedObjects);
            MessageController.Setup();
        }

        internal void HookItemChanger()
        {
            Log("Hooking ItemChanger.");
            try
            {
                if (SET == null) throw new NullReferenceException("ItemChanger hooked with null settings.");
                if (_hooked) throw new InvalidOperationException("Attempted to rehook ItemChanger.");
                _hooked = true;
                LanguageStringManager.Hook();
                Events.Hook();
                DialogueCenter.Hook();
                SceneDataUtil.Hook();
                ShopUtil.HookShops();
                StartDef.Hook();
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        internal void UnhookItemChanger()
        {
            Log("Unhooking ItemChanger.");
            try
            {
                if (!_hooked) throw new InvalidOperationException("Attempted to unhook ItemChanger before hooked.");
                _hooked = false;
                LanguageStringManager.Unhook();
                Events.Unhook();
                DialogueCenter.Unhook();
                SceneDataUtil.Unhook();
                ShopUtil.UnhookShops();
                StartDef.Unhook();
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        /// <summary>
        /// Required before all operations which modify settings data.
        /// </summary>
        /// <param name="overwrite">If settings data already exists, should it be overwritten?</param>
        public static void CreateSettingsProfile(bool overwrite = true) => CreateSettingsProfile(overwrite, true);

        /// <summary>
        /// Required before all operations which modify settings data.
        /// </summary>
        /// <param name="overwrite">If settings data already exists, should it be overwritten?</param>
        /// <param name="createDefaultModules">If a new profile is created, should it include all default modules?</param>
        public static void CreateSettingsProfile(bool overwrite, bool createDefaultModules)
        {
            if (overwrite && Settings.loaded) throw new InvalidOperationException("Cannot overwrite loaded settings.");

            if (SET == null || overwrite)
            {
                SET = new(createDefaultModules);
                if (!instance._hooked) instance.HookItemChanger();
            }
        }

        public static ModuleCollection Modules => SET.mods;

        /// <summary>
        /// Adds the override to SaveSettings. Overwrites any existing override for source.
        /// </summary>
        public static void AddTransitionOverride(Transition source, ITransition target)
        {
            if (ItemChanger.Modules.RemoveInfectedBlockades.BlockedTransitions.Contains(source))
            {
                Modules.GetOrAdd<Modules.RemoveInfectedBlockades>();
            }
            
            SET.TransitionOverrides[source] = target;
        }

        /// <summary>
        /// Add an event to instantiate an object at a given place to Settings. Primarily used for adding platforms, and similar preloaded objects.
        /// </summary>
        public static void AddDeployer(IDeployer deployer)
        {
            SET.Deployers.Add(deployer);
            if (Settings.loaded) Events.AddSceneChangeEdit(deployer.SceneName, deployer.OnSceneChange);
        }

        public static void ChangeStartGame(StartDef start)
        {
            SET.Start = start;
        }

        /// <summary>
        /// Adds placements to local settings, with handling for placements with the same name.
        /// </summary>
        /// <param name="placements">The placements to add to the local settings.</param>
        /// <param name="conflictResolution">The action if a placement already exists in settings with the same name.</param>
        public static void AddPlacements(IEnumerable<AbstractPlacement> placements, PlacementConflictResolution conflictResolution = PlacementConflictResolution.MergeKeepingNew)
        {
            foreach (var p in placements)
            {
                if (SET.Placements.TryGetValue(p.Name, out var existsP))
                {
                    switch (conflictResolution)
                    {
                        case PlacementConflictResolution.MergeKeepingNew:
                            p.Items.AddRange(existsP.Items);
                            SET.Placements[p.Name] = p;
                            if (Settings.loaded) existsP.Unload();
                            break;
                        case PlacementConflictResolution.MergeKeepingOld:
                            existsP.Items.AddRange(p.Items);
                            break;
                        case PlacementConflictResolution.Replace:
                            SET.Placements[p.Name] = p;
                            if (Settings.loaded) existsP.Unload();
                            break;
                        case PlacementConflictResolution.Ignore:
                            break;
                        case PlacementConflictResolution.Throw:
                            throw new ArgumentException($"A placement with name {p.Name} already exists.");
                    }
                }
                else SET.Placements.Add(p.Name, p);
                if (Settings.loaded && p == SET.Placements[p.Name]) p.Load();
            }
        }

        public override string GetVersion()
        {
            return "1.0.1";
        }

        public override int LoadPriority() => -2;

        public override List<(string, string)> GetPreloadNames()
        {
            return ObjectCache.GetPreloadNames();
        }

        public void OnLoadLocal(SaveSettings s)
        {
            if (Settings.loaded)
            {
                try
                {
                    SET.Unload();
                }
                catch (Exception e)
                {
                    LogError($"Error unloading settings:\n{e}");
                }
            }

            SET = s;
            if (SET == null && _hooked)
            {
                UnhookItemChanger();
            }
            else if (SET != null && !_hooked)
            {
                HookItemChanger();
            }
        }

        public SaveSettings OnSaveLocal()
        {
            return SET;
        }

        public void OnLoadGlobal(GlobalSettings s)
        {
            GS = s;
        }

        public GlobalSettings OnSaveGlobal()
        {
            return GS;
        }

        public bool ToggleButtonInsideMenu => false;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new();
            GS.AddEntries(entries);
            return entries;
        }
    }
}
