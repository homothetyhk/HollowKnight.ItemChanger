using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public static class Events
    {
        public static event Action<ReadOnlyGiveEventArgs> BeforeGive;
        public static event Action<GiveEventArgs> ModifyItem;
        public static event Action<GiveEventArgs> ModifyRedundantItem;
        public static event Action<ReadOnlyGiveEventArgs> OnGive;
        public static event Action<ReadOnlyGiveEventArgs> AfterGive;

        internal static void BeforeGiveInvoke(ReadOnlyGiveEventArgs args) => BeforeGive?.Invoke(args);
        internal static void ModifyItemInvoke(GiveEventArgs args) => ModifyItem?.Invoke(args);
        internal static void ModifyRedundantItemInvoke(GiveEventArgs args) => ModifyRedundantItem?.Invoke(args);
        internal static void OnGiveInvoke(ReadOnlyGiveEventArgs args) => OnGive?.Invoke(args);
        internal static void AfterGiveInvoke(ReadOnlyGiveEventArgs args) => AfterGive?.Invoke(args);
    }
}
