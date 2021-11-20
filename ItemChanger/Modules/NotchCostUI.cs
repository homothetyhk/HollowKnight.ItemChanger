namespace ItemChanger.Modules
{
    /// <summary>
    /// Nondefault module which causes the notch cost to be displayed after a charm's name, when sent through a LanguageString.
    /// </summary>
    public class NotchCostUI : Module
    {
        public override void Initialize()
        {
            Events.OnStringGet += AddNotchCostToCharmName;
        }

        public override void Unload()
        {
            Events.OnStringGet -= AddNotchCostToCharmName;
        }

        private void AddNotchCostToCharmName(StringGetArgs args)
        {
            if (args.Source is LanguageString ls && ls.key.StartsWith("CHARM_NAME_"))
            {
                string i = ls.key[11..]; // remove "CHARM_NAME_" prefix
                int j = i.IndexOf('_');
                if (j != -1) i = i[..j]; // remove "_A" suffix, etc such as on White Fragment
                args.Current += $" [{PlayerData.instance.GetInt($"charmCost_{i}")}]";
            }
        }
    }
}
