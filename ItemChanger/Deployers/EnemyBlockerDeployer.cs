namespace ItemChanger.Deployers
{
    /// <summary>
    /// Deployer which creates a region which enemies cannot enter.
    /// </summary>
    public record EnemyBlockerDeployer : Deployer
    {
        public float Width { get; init; }
        public float Height { get; init; }

        public override GameObject Instantiate()
        {
            GameObject go = new() { layer = 15 };
            BoxCollider2D box = go.AddComponent<BoxCollider2D>();
            box.size = new(Width, Height);
            return go;
        }
    }
}
