namespace ItemChanger
{
    /// <summary>
    /// Interface used to select a cost displayer for a given shop
    /// </summary>
    public interface ICostDisplayerSelectionStrategy
    {
        /// <summary>
        /// Gets a cost displayer for a given item based on the chosen strategy.
        /// </summary>
        /// <param name="item">The item to use to choose the correct type of cost displayer</param>
        public CostDisplayer GetCostDisplayer(AbstractItem item);
    }

    /// <summary>
    /// A cost displayer selection strategy which applies a single cost displayer to all items.
    /// </summary>
    public class SingleCostDisplayerSelectionStrategy : ICostDisplayerSelectionStrategy
    {
        /// <summary>
        /// The cost displayer to use.
        /// </summary>
        public required CostDisplayer CostDisplayer { get; set; }

        /// <inheritdoc/>
        public CostDisplayer GetCostDisplayer(AbstractItem item) => CostDisplayer;
    }
}
