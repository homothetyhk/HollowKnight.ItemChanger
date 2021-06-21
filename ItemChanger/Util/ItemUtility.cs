using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Util
{
    public static class ItemUtility
    {
        /// <summary>
        /// Gives each unobtained item from the collection (asynchronously), and executes the callback after the final item.
        /// </summary>
        public static void GiveSequentially(IEnumerable<AbstractItem> items, AbstractPlacement placement, GiveInfo info, Action callback = null)
        {
            IEnumerator<AbstractItem> enumerator = items.GetEnumerator();
            GiveRecursive(enumerator, placement, info, callback);
        }

        private static void GiveRecursive(IEnumerator<AbstractItem> enumerator, AbstractPlacement placement, GiveInfo info, Action callback)
        {
            if (enumerator.MoveNext())
            {
                if (enumerator.Current.IsObtained())
                {
                    GiveRecursive(enumerator, placement, info, callback);
                }
                else if (!enumerator.Current.IsObtained())
                {
                    GiveInfo next = info.Clone(); // info may be modified by Give, so we preserve the original
                    next.Callback = _ => GiveRecursive(enumerator, placement, info, callback);
                    enumerator.Current.Give(placement, next);
                }
            }
            else
            {
                callback?.Invoke();
            }
        }

    }
}
