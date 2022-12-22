/* *********************************************************************
 * Date: 19 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Forge.Collections
{

    /// <summary>
    /// Mutable list implementation for .NET
    /// <example>
    /// <code>
    /// IList queryResult = (System.Collections.IList)mGenericQueryMethod.MakeGenericMethod(queryData.EntityType).Invoke(null, new object[] { session, queryData, logQuery });
    ///
    /// IListSpecialized&lt;EntityBase&gt; result = new ListSpecialized&lt;EntityBase&gt;();
    /// foreach (object b in queryResult)
    /// {
    ///     result.Add((EntityBase)b);
    /// }
    ///
    /// return result;
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable, DebuggerDisplay("Count = {Count}")]
    public class ListSpecialized<T> : AbstractList<T>
    {

        #region Field(s)

        /// <summary>
        /// The inner list
        /// </summary>
        protected readonly List<T> mList = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSpecialized&lt;T&gt;"/> class.
        /// </summary>
        public ListSpecialized()
        {
            this.mList = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSpecialized&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ListSpecialized(IEnumerable<T> collection)
        {
            this.mList = new List<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSpecialized&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public ListSpecialized(int capacity)
        {
            this.mList = new List<T>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSpecialized&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        protected ListSpecialized(List<T> list)
        {
            this.mList = list;
        }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(T item)
        {
            this.mList.Add(item);
            this.mVersion++;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public override void AddRange(IEnumerable<T> collection)
        {
            this.mList.AddRange(collection);
            this.mVersion++;
        }

        /// <summary>
        /// Ases the read only.
        /// </summary>
        /// <returns></returns>
        public virtual ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }

        /// <summary>
        /// Searches the entire sorted System.Collections.Generic.List&lt;T&gt; for an element
        /// using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The zero-based index of item in the sorted List,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of System.Collections.Generic.List&lt;T&gt;.Count.
        /// </returns>
        public virtual int BinarySearch(T item)
        {
            return this.mList.BinarySearch(item);
        }

        /// <summary>
        /// Searches the entire sorted System.Collections.Generic.List&lt;T&gt; for an element
        /// using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// The zero-based index of item in the sorted List,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of System.Collections.Generic.List&lt;T&gt;.Count.
        /// </returns>
        public virtual int BinarySearch(T item, IComparer<T> comparer)
        {
            return this.mList.BinarySearch(item, comparer);
        }

        /// <summary>
        /// Searches the entire sorted System.Collections.Generic.List&lt;T&gt; for an element
        /// using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="item">The item.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>
        /// The zero-based index of item in the sorted List,
        /// if item is found; otherwise, a negative number that is the bitwise complement
        /// of the index of the next element that is larger than item or, if there is
        /// no larger element, the bitwise complement of System.Collections.Generic.List&lt;T&gt;.Count.
        /// </returns>
        public virtual int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return this.mList.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            this.mList.Clear();
            this.mVersion++;
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public override bool Contains(T item)
        {
            return this.mList.Contains(item);
        }

        /// <summary>
        /// Converts the elements in the current List to
        /// another type, and returns a list containing the converted elements.
        /// </summary>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="converter">The converter.</param>
        /// <returns>
        /// A List of the target type containing the converted
        /// elements from the current List.
        /// </returns>
        public virtual ListSpecialized<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return new ListSpecialized<TOutput>(this.mList.ConvertAll<TOutput>(converter));
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        public virtual void CopyTo(T[] array)
        {
            this.mList.CopyTo(array);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public override void CopyTo(T[] array, int arrayIndex)
        {
            this.mList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <param name="count">The count.</param>
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            this.mList.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Existses the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>True, if the predicate matches, otherwise False.</returns>
        public virtual bool Exists(Predicate<T> match)
        {
            return this.mList.Exists(match);
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>Item</returns>
        public virtual T Find(Predicate<T> match)
        {
            return this.mList.Find(match);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>List of items</returns>
        public virtual ListSpecialized<T> FindAll(Predicate<T> match)
        {
            return new ListSpecialized<T>(this.mList.FindAll(match));
        }

        /// <summary>
        /// Finds the index.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        public virtual int FindIndex(Predicate<T> match)
        {
            return this.mList.FindIndex(match);
        }

        /// <summary>
        /// Finds the index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="match">The match.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        public virtual int FindIndex(int startIndex, Predicate<T> match)
        {
            return this.mList.FindIndex(startIndex, match);
        }

        /// <summary>
        /// Finds the index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="match">The match.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        public virtual int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return this.mList.FindIndex(startIndex, count, match);
        }

        /// <summary>
        /// Finds the last.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>
        /// The last element that matches the conditions defined by the specified predicate,
        /// if found; otherwise, the default value for type T.
        /// </returns>
        public virtual T FindLast(Predicate<T> match)
        {
            return this.mList.FindLast(match);
        }

        /// <summary>
        /// Finds the last index.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        public virtual int FindLastIndex(Predicate<T> match)
        {
            return this.mList.FindLastIndex(match);
        }

        /// <summary>
        /// Finds the last index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="match">The match.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        public virtual int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return this.mList.FindLastIndex(startIndex, match);
        }

        /// <summary>
        /// Finds the last index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="match">The match.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the
        /// conditions defined by match, if found; otherwise, –1.
        /// </returns>
        public virtual int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return this.mList.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ForEach(System.Action<T> action)
        {
            this.mList.ForEach(action);
        }

        /// <summary>
        /// Gets the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns>A shallow copy of a range of elements in the source ListSpecialized&lt;T&gt;.</returns>
        public virtual ListSpecialized<T> GetRange(int index, int count)
        {
            return new ListSpecialized<T>(this.mList.GetRange(index, count));
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire ListSpecialized&lt;T&gt;
        /// if found; otherwise, –1.
        /// </returns>
        public override int IndexOf(T item)
        {
            return this.mList.IndexOf(item);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire ListSpecialized&lt;T&gt;
        /// if found; otherwise, –1.
        /// </returns>
        public virtual int IndexOf(T item, int index)
        {
            return this.mList.IndexOf(item, index);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the entire ListSpecialized&lt;T&gt;
        /// if found; otherwise, –1.
        /// </returns>
        public virtual int IndexOf(T item, int index, int count)
        {
            return this.mList.IndexOf(item, index, count);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public override void Insert(int index, T item)
        {
            this.mList.Insert(index, item);
            this.mVersion++;
        }

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="collection">The collection.</param>
        public virtual void InsertRange(int index, IEnumerable<T> collection)
        {
            this.mList.InsertRange(index, collection);
            this.mVersion++;
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the entire the
        /// ListSpecialized&lt;T&gt;, if found; otherwise, –1.
        /// </returns>
        public virtual int LastIndexOf(T item)
        {
            return this.mList.LastIndexOf(item);
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the entire the
        /// ListSpecialized&lt;T&gt;, if found; otherwise, –1.
        /// </returns>
        public virtual int LastIndexOf(T item, int index)
        {
            return this.mList.LastIndexOf(item, index);
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of item within the entire the
        /// ListSpecialized&lt;T&gt;, if found; otherwise, –1.
        /// </returns>
        public virtual int LastIndexOf(T item, int index, int count)
        {
            return this.mList.LastIndexOf(item, index, count);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True, if the collection changed, otherwise False.</returns>
        public override bool Remove(T item)
        {
            if (this.mList.Remove(item))
            {
                this.mVersion++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The number of removed items</returns>
        public virtual int RemoveAll(Predicate<T> match)
        {
            int result = this.mList.RemoveAll(match);
            if (result > 0)
            {
                this.mVersion++;
            }
            return result;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public override void RemoveAt(int index)
        {
            this.mList.RemoveAt(index);
            this.mVersion++;
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public virtual void RemoveRange(int index, int count)
        {
            this.mList.RemoveRange(index, count);
            this.mVersion++;
        }

        /// <summary>
        /// Reverses this instance.
        /// </summary>
        public virtual void Reverse()
        {
            this.mList.Reverse();
            this.mVersion++;
        }

        /// <summary>
        /// Reverses the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public virtual void Reverse(int index, int count)
        {
            this.mList.Reverse(index, count);
            this.mVersion++;
        }

        /// <summary>
        /// Sorts this instance.
        /// </summary>
        public virtual void Sort()
        {
            this.mList.Sort();
            this.mVersion++;
        }

        /// <summary>
        /// Sorts the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public virtual void Sort(IComparer<T> comparer)
        {
            this.mList.Sort(comparer);
            this.mVersion++;
        }

        /// <summary>
        /// Sorts the specified comparison.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        public virtual void Sort(Comparison<T> comparison)
        {
            this.mList.Sort(comparison);
            this.mVersion++;
        }

        /// <summary>
        /// Sorts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="comparer">The comparer.</param>
        public virtual void Sort(int index, int count, IComparer<T> comparer)
        {
            this.mList.Sort(index, count, comparer);
            this.mVersion++;
        }

        /// <summary>
        /// Toes the array.
        /// </summary>
        /// <returns>Array of items</returns>
        public override T[] ToArray()
        {
            return this.mList.ToArray();
        }

        /// <summary>
        /// Trims the excess.
        /// </summary>
        public virtual void TrimExcess()
        {
            this.mList.TrimExcess();
        }

        /// <summary>
        /// Trues for all.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>True, if the predicate mataches on all elements</returns>
        public virtual bool TrueForAll(Predicate<T> match)
        {
            return this.mList.TrueForAll(match);
        }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public virtual int Capacity
        {
            get { return this.mList.Capacity; }
            set { this.mList.Capacity = value; }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public override int Count
        {
            get { return this.mList.Count; }
        }

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <value>
        /// The value
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Item</returns>
        public override T this[int index]
        {
            get { return this.mList[index]; }
            set
            {
                this.mList[index] = value;
                this.mVersion++;
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of generic items</returns>
        public override IEnumeratorSpecialized<T> GetEnumerator()
        {
            return new Enumerator<T>(this, 0, this.Count);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public override void CopyTo(Array array, int index)
        {
            ((ICollection)this.mList).CopyTo(array, index);
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection,
        /// </returns>
        public override int Add(object value)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, "item");
            try
            {
                ((AbstractSubList<T>)this).Add((T)value);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
            }
            return (this.Count - 1);
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public override bool Contains(object value)
        {
            return ((IList)this.mList).Contains(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The index of <paramref name="value" /> if found in the list; otherwise, -1.
        /// </returns>
        public override int IndexOf(object value)
        {
            return ((IList)this.mList).IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public override void Insert(int index, object value)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, "item");
            try
            {
                this.mList.Insert(index, (T)value);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
            }
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void Remove(object value)
        {
            ((IList)this.mList).Remove(value);
            this.mVersion++;
        }

        /// <summary>
        /// Subs the list.
        /// </summary>
        /// <param name="fromIndex">From index.</param>
        /// <param name="toIndex">To index.</param>
        /// <returns>Sub list of the items</returns>
        public override ISubList<T> SubList(int fromIndex, int toIndex)
        {
            return new SubListImpl<T>(this, fromIndex, toIndex);
        }

        #endregion

    }

}
