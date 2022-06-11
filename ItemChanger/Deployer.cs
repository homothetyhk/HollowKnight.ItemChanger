namespace ItemChanger
{
    /// <summary>
    /// Abstract IDeployer which creates an object and places it at the specified coordinates.
    /// Inheritors must specify how the object is instantiated; the base class handles positioning and activation.
    /// </summary>
    public abstract record Deployer : IDeployer
    {
        public string SceneName { get; init; }
        public float X { get; init; }
        public float Y { get; init; }
        public abstract GameObject Instantiate();

        /// <summary>
        /// Optional property. If non-null and evaluates false, the GameObject is not deployed.
        /// </summary>
        public IBool Test { get; init; } = null;

        public virtual void OnSceneChange(Scene to)
        {
            if (Test != null && !Test.Value) return;
            Deploy();
        }

        /// <summary>
        /// Method which calls Instantiate, and then appropriately places the new object. Returns the object to allow further action by overriders.
        /// <br/>The base implementation sets position, gives the object a safe position-based name, and updates any PersistentBoolItem, PersistentIntItem, or GeoRock components with the new name.
        /// </summary>
        public virtual GameObject Deploy()
        {
            GameObject obj = Instantiate();
            obj.transform.position = new(X, Y);
            
            obj.name = $"{obj.name.Replace(" (Clone)", string.Empty)}{(int)X}{(int)Y}";
            if (obj.GetComponent<PersistentBoolItem>() is PersistentBoolItem pbi)
            {
                pbi.persistentBoolData.id = obj.name;
                pbi.persistentBoolData.sceneName = SceneName;
            }
            else if (obj.GetComponent<PersistentIntItem>() is PersistentIntItem pii)
            {
                pii.persistentIntData.id = obj.name;
                pii.persistentIntData.sceneName = SceneName;
            }
            else if (obj.GetComponent<GeoRock>() is GeoRock gr)
            {
                gr.geoRockData.id = obj.name;
                gr.geoRockData.sceneName = SceneName;
            }

            obj.SetActive(true);
            return obj;
        }
    }
}
