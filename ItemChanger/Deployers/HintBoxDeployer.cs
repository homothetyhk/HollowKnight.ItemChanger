using ItemChanger.Components;

namespace ItemChanger.Deployers
{
    /// <summary>
    /// Deployer which creates a HintBox, a region which displays a dream dialogue message when the hero enters.
    /// </summary>
    public record HintBoxDeployer : Deployer
    {
        public float Width { get; init; } = 5f;
        public float Height { get; init; } = 5f;
        public IString Text { get; init; }


        public override GameObject Instantiate()
        {
            HintBox box = HintBox.Create(new Vector2(X, Y), new Vector2(Width, Height));
            box.GetDisplayText = Text.GetValue;
            return box.gameObject;
        }
    }
}
