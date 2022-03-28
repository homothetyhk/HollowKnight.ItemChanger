namespace ItemChanger.Tags
{
    /// <summary>
    /// Tag which carries ordered lists of predecessors and successors item names.
    /// <br/>Hooks AbstractItem.ModifyItem, and returns the first item in the sequence which is not redundant.
    /// <br/> Note that unlike ItemChainTag, this does not recursively check tags of the predecessors and successors.
    /// </summary>
    public class ItemTreeTag : Tag, IItemModifierTag
    {
        public string[] predecessors;
        public string[] successors;
        /// <summary>
        /// If true, the first nonredundant item starting from the first element in the list will be chosen.
        /// <br/>Otherwise, the search will begin at the parent item, and will assume that predecessors of a redundant item are redundant.
        /// <br/>Only relevant when predecessors is nonempty.
        /// </summary>
        public bool strictEvaluation;

        public override void Load(object parent)
        {
            AbstractItem item = (AbstractItem)parent;
            item.ModifyItem += ModifyItem;
        }

        public override void Unload(object parent)
        {
            AbstractItem item = (AbstractItem)parent;
            item.ModifyItem -= ModifyItem;
        }


        protected virtual AbstractItem GetItem(string name)
        {
            return Finder.GetItem(name);
        }


        public void ModifyItem(GiveEventArgs args)
        {
            if (strictEvaluation)
            {
                if (predecessors != null)
                {
                    foreach (string s in predecessors)
                    {
                        AbstractItem item = GetItem(s);
                        if (!item.Redundant())
                        {
                            args.Item = item;
                            return;
                        }
                    }
                }
                if (!args.Item.Redundant())
                {
                    return;
                }
                if (successors != null)
                {
                    foreach (string s in successors)
                    {
                        AbstractItem item = GetItem(s);
                        if (!item.Redundant())
                        {
                            args.Item = item;
                            return;
                        }
                    }
                }
                // all redundant
                args.Item = null;
                return;
            }
            else
            {
                if (args.Item.Redundant())
                {
                    if (successors != null)
                    {
                        foreach (string s in successors)
                        {
                            AbstractItem item = GetItem(s);
                            if (!item.Redundant())
                            {
                                args.Item = item;
                                return;
                            }
                        }
                    }
                    // all redundant
                    args.Item = null;
                    return;
                }
                else
                {
                    if (predecessors != null)
                    {
                        foreach (string s in predecessors.Reverse())
                        {
                            AbstractItem item = GetItem(s);
                            if (!item.Redundant())
                            {
                                args.Item = item;
                            }
                            else return;
                        }
                    }
                    // none redundant
                    return;
                }
            }
        }
    }
}
