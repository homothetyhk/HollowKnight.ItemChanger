﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
