using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemChanger.Tags
{
    /// <summary>
    /// Similar to ItemChainTag, but allows for predecessors and successors to be ordered lists.
    /// <br/> Unlike ItemChainTag, does not recursively check tags to traverse item tree; i.e. all tree members must be predecessors or successors.
    /// </summary>
    public class ItemTreeTag : Tag
    {
        public string[] predecessors;
        public string[] successors;

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
                args.Item = null;
                return;
            }
            else
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
                return;
            }
        }
    }
}
