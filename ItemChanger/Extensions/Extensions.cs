using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemChanger
{
    public static class Extensions
    {
        public static bool Compare(this int a, ComparisonOperator op, int b)
        {
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
