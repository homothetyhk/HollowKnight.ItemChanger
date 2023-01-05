using ItemChanger.Internal;
using ItemChanger.Util;
using Modding;

namespace ItemChanger
{
    public class ItemChangerMod : Mod, ILocalSettings<SaveSettings>, IGlobalSettings<GlobalSettings>, ICustomMenuMod
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
                DialogueCenter.Hook();
                SceneDataUtil.Hook();
                ShopUtil.HookShops();
                StartDef.Hook();
                Events.Hook();
                NamedBoolFunction.Clear();
                NamedStringFunction.Clear();
                NamedSpriteFunction.Clear();
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
                DialogueCenter.Unhook();
                SceneDataUtil.Unhook();
                ShopUtil.UnhookShops();
                StartDef.Unhook();
                Events.Unhook();
                MessageController.Clear();
                NamedBoolFunction.Clear();
                NamedStringFunction.Clear();
                NamedSpriteFunction.Clear();
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        /// <summary>
        /// Create default ItemChanger settings for the save. Required before all operations which modify settings data.
        /// </summary>
        /// <param name="overwrite">If settings data already exists, should it be overwritten?</param>
        /// <exception cref="InvalidOperationException">Settings have already been loaded.</exception>
        public static void CreateSettingsProfile(bool overwrite = true) => CreateSettingsProfile(overwrite, true);

        /// <summary>
        /// Create default ItemChanger settings for the save, with an option for whether default modules should be automatically added. Required before all operations which modify settings data.
        /// </summary>
        /// <param name="overwrite">If settings data already exists, should it be overwritten?</param>
        /// <param name="createDefaultModules">If a new profile is created, should it include all default modules?</param>
        /// <exception cref="InvalidOperationException">Settings have already been loaded.</exception>
        public static void CreateSettingsProfile(bool overwrite, bool createDefaultModules)
        {
            if (overwrite && Settings.loaded) throw new InvalidOperationException("Cannot overwrite loaded settings.");

            if (SET == null || overwrite)
            {
                SET = new(createDefaultModules);
                if (!instance._hooked) instance.HookItemChanger();
            }
        }

        /// <summary>
        /// Create a settings profile from an existing Settings object (e.g. settings loaded from an external serialized source).
        /// </summary>
        /// <param name="settings">The Settings object to save to local settings.</param>
        /// <exception cref="InvalidOperationException">Settings have already been loaded.</exception>
        public static void CreateSettingsProfile(Settings settings)
        {
            if (Settings.loaded) throw new InvalidOperationException("Cannot overwrite loaded settings.");

            SET = settings;
            if (!instance._hooked) instance.HookItemChanger();
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
                            if (Settings.loaded) foreach (AbstractItem i in p.Items) i.Load();
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
            return _version;
        }

        private static readonly string _sha1;
        private static readonly string _version;
        static ItemChangerMod()
        {
            System.Reflection.Assembly a = typeof(ItemChangerMod).Assembly;

            using var sha1 = System.Security.Cryptography.SHA1.Create();
            using var sr = File.OpenRead(a.Location);
            _sha1 = Convert.ToBase64String(sha1.ComputeHash(sr));

            int buildHash;
            unchecked // stable string hash code
            {
                int hash1 = 5381;
                int hash2 = hash1;
                string str = _sha1;

                for (int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                buildHash = hash1 + (hash2 * 1566083941);
                buildHash = Math.Abs(buildHash) % 997;
            }

            Version v = a.GetName().Version;
            _version = $"{v.Major}.{v.Minor}.{v.Build}+{buildHash.ToString().PadLeft(3, '0')}";
        }

        public override int LoadPriority() => -2;

        public override List<(string, string)> GetPreloadNames()
        {
            return ObjectCache.GetPreloadNames();
        }

        void ILocalSettings<SaveSettings>.OnLoadLocal(SaveSettings s)
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

        SaveSettings ILocalSettings<SaveSettings>.OnSaveLocal()
        {
            return SET;
        }

        void IGlobalSettings<GlobalSettings>.OnLoadGlobal(GlobalSettings s)
        {
            GS = s;
        }

        GlobalSettings IGlobalSettings<GlobalSettings>.OnSaveGlobal()
        {
            return GS;
        }

        bool ICustomMenuMod.ToggleButtonInsideMenu => false;

        MenuScreen ICustomMenuMod.GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            return Internal.Menu.ItemChangerMenu.GetMenuScreen(modListMenu, toggleDelegates);
        }
    }
}
