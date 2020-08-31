using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PetitionsList
{
    static class JCollection
    {
        public static void AddAll(this IList c, ICollection elements)
        {
            foreach (var e in elements)
            {
                c.Add(e);
            }
        }

        public static void AddAll(this IList c, params object[] elements)
        {
            foreach (var e in elements)
            {
                c.Add(e);
            }
        }

        public static void AddAll<T>(this ICollection<T> c, ICollection<T> elements)
        {
            foreach(var e in elements)
            {
                c.Add(e);
            }
        }

        public static void AddAll<T>(this ICollection<T> c, params T[] elements)
        {
            foreach (var e in elements)
            {
                c.Add(e);
            }
        }
    }
}
