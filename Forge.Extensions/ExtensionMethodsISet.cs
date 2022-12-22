/* *********************************************************************
 * Date: 17 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using Forge.Shared;
using System;
using System.Collections.Generic;

namespace Forge
{

    /// <summary>
    /// Extension methods for ISet
    /// </summary>
    public static class ExtensionMethodsISet
    {

        /// <summary>
        /// Adds the entire collection to the current instance of set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set">The set.</param>
        /// <param name="collection">The collection.</param>
        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> collection)
        {
            if (set == null)
            {
                ThrowHelper.ThrowArgumentNullException("set");
            }
            if (collection != null)
            {
                foreach (T item in collection)
                {
                    set.Add(item);
                }
            }
        }

        /// <summary>
        /// Execute the specified action on each element of the set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set">The set.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this ISet<T> set, Action<T> action)
        {
            if (set == null)
            {
                ThrowHelper.ThrowArgumentNullException("set");
            }
            if (action == null)
            {
                ThrowHelper.ThrowArgumentNullException("action");
            }
            foreach (T item in set)
            {
                action(item);
            }
        }

        /// <summary>
        /// Examine the items of the set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set">The set.</param>
        /// <param name="match">The match.</param>
        /// <returns>True, if the all items matches with the predicate, otherwise false</returns>
        public static bool TrueForAll<T>(this ISet<T> set, Predicate<T> match)
        {
            if (set == null)
            {
                ThrowHelper.ThrowArgumentNullException("set");
            }
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException("match");
            }

            foreach (T item in set)
            {
                if (!match(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds the first item in the ISet which matches with the predicate and result true, otherwise false.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set">The set.</param>
        /// <param name="match">The match.</param>
        /// <returns>True, if an item matches with predicate, otherwise false.</returns>
        public static bool Exists<T>(this ISet<T> set, Predicate<T> match)
        {
            if (set == null)
            {
                ThrowHelper.ThrowArgumentNullException("set");
            }
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException("match");
            }

            foreach (T item in set)
            {
                if (match(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the first match element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set">The set.</param>
        /// <param name="match">The match.</param>
        /// <returns>Item</returns>
        public static T Find<T>(this ISet<T> set, Predicate<T> match)
        {
            if (set == null)
            {
                ThrowHelper.ThrowArgumentNullException("set");
            }
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException("match");
            }

            foreach (T item in set)
            {
                if (match(item))
                {
                    return item;
                }
            }

            return default(T);
        }

        /// <summary>
        /// Finds all element which are match with the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set">The set.</param>
        /// <param name="match">The match.</param>
        /// <returns>List of items</returns>
        public static List<T> FindAll<T>(this ISet<T> set, Predicate<T> match)
        {
            if (set == null)
            {
                ThrowHelper.ThrowArgumentNullException("set");
            }
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException("match");
            }

            List<T> list = new List<T>();

            foreach (T item in set)
            {
                if (match(item))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// Create and return an array of T from ISet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set">The set.</param>
        /// <returns>Array of the items</returns>
        public static T[] ToArray<T>(this ISet<T> set)
        {
            if (set == null)
            {
                ThrowHelper.ThrowArgumentNullException("set");
            }

            int index = 0;
            T[] result = new T[set.Count];
            foreach (T item in set)
            {
                result[index] = item;
                index++;
            }
            return result;
        }

    }

}
