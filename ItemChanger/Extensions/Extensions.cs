using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ItemChanger
{
    public static class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();
            if (t == null) return go.AddComponent<T>();
            else return t;
        }

        /// <summary>
        /// Returns true when the collection has a previously given item, or is null or empty.
        /// </summary>
        public static bool AnyEverObtained(this IEnumerable<AbstractItem> items)
        {
            return items == null || !items.Any() || items.Any(i => i.WasEverObtained());
        }

        public static bool Compare(this int a, ComparisonOperator op, int b)
        {
            GameObject g;
            switch (op)
            {
                default:
                case ComparisonOperator.Eq:
                    return a == b;
                case ComparisonOperator.Neq:
                    return a != b;
                case ComparisonOperator.Ge:
                    return a >= b;
                case ComparisonOperator.Gt:
                    return a > b;
                case ComparisonOperator.Le:
                    return a <= b;
                case ComparisonOperator.Lt:
                    return a < b;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> ts, T t)
        {
            return ts.TakeWhile(u => !EqualityComparer<T>.Default.Equals(u, t)).Count();
        }

        public static IEnumerable<T> Yield<T>(this T t)
        {
            yield return t;
        }

        public static FsmBool AddFsmBool(this PlayMakerFSM fsm, string name, bool value)
        {
            FsmBool fb = new FsmBool
            {
                Name = name,
                Value = value
            };

            FsmBool[] bools = new FsmBool[fsm.FsmVariables.BoolVariables.Length + 1];
            fsm.FsmVariables.BoolVariables.CopyTo(bools, 0);
            bools[bools.Length - 1] = fb;
            fsm.FsmVariables.BoolVariables = bools;

            return fb;
        }

    }
}
