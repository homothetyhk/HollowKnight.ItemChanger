using ItemChanger.Internal;

namespace ItemChanger
{
    public class AdditiveGroupTag : Tag, IModifyItemTag
    {
        public string AdditiveGroup { get; set; }

        public void ModifyItem(GiveEventArgs args)
        {
            if (Ref.Settings.AdditiveGroups.TryGetValue(AdditiveGroup, out AbstractItem[] itemList))
            {
                for (int i = 0; i < itemList.Length; i++)
                {
                    if (!itemList[i].Redundant())
                    {
                        args.Item = itemList[i];
                        return;
                    }
                }
                // TODO: invoke event here
            }
        }

        public override bool Intrinsic => true;
    }
}
