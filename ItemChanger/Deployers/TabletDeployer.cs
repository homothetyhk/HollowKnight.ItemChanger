namespace ItemChanger.Deployers
{
    /// <summary>
    /// A Deployer which creates a lore tablet at the specified point. 
    /// The tablet will not be readable unless the y value is at least 0.1 higher than the knight's position at ground level.
    /// </summary>
    public record TabletDeployer : Deployer
    {
        /// <summary>
        /// The text to display when the tablet is read by the player.
        /// </summary>
        public IString Text { get; init; }

        public override GameObject Instantiate()
        {
            return Util.TabletUtility.MakeNewTablet("Deployed Tablet", Text.GetValue);
        }

        public override GameObject Deploy()
        {
            GameObject obj = Instantiate();
            Container.GetContainer(Container.Tablet).ApplyTargetContext(obj, X, Y, 0);
            return obj;
        }
    }
}
