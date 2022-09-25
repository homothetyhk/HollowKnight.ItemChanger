namespace ItemChanger.Locations
{
    /// <summary>
    /// Helper location meant to be unpacked into a DualPlacement
    /// </summary>
    public class DualLocation : AbstractLocation
    {
        protected override void OnLoad()
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }


        public IBool Test;
        public AbstractLocation falseLocation;
        public AbstractLocation trueLocation;

        public override AbstractPlacement Wrap()
        {
            return new Placements.DualPlacement(name)
            {
                Test = Test,
                falseLocation = falseLocation,
                trueLocation = trueLocation,   
                tags = tags,
            };
        }
    }
}
