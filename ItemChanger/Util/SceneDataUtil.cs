namespace ItemChanger.Util
{
    public static class SceneDataUtil
    {
        private static List<PersistentBoolData> QueuedPersistentBoolData = new();

        public static void Hook()
        {
            On.GameManager.SaveLevelState += SavePersistentBoolItems;
        }

        public static void Unhook()
        {
            On.GameManager.SaveLevelState -= SavePersistentBoolItems;
            QueuedPersistentBoolData.Clear();
        }

        // Save our PersistentBoolData after the game does, so we overwrite the game's data rather than the other way round
        private static void SavePersistentBoolItems(On.GameManager.orig_SaveLevelState orig, GameManager self)
        {
            orig(self);
            foreach (PersistentBoolData pbd in QueuedPersistentBoolData)
            {
                SceneData.instance.SaveMyState(pbd);
            }
            QueuedPersistentBoolData.Clear();
        }

        public static void Save(string sceneName, string id, bool activated = true, bool semiPersistent = false)
        {
            SavePersistentBoolItemState(new PersistentBoolData
            {
                sceneName = sceneName,
                id = id,
                activated = activated,
                semiPersistent = semiPersistent
            });
        }

        public static void SavePersistentBoolItemState(PersistentBoolData pbd)
        {
            GameManager.instance.sceneData.SaveMyState(pbd);
            QueuedPersistentBoolData.Add(pbd);
        }
    }
}
