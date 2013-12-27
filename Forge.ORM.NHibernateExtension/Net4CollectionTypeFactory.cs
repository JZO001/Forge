using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using NHibernate;
using NHibernate.Collection;
using NHibernate.DebugHelpers;
using NHibernate.Engine;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Type;
using NHibernate.Util;

namespace Forge.ORM.NHibernateExtension
{

    /// <summary>
    /// Supports .NET 4 ISet interface and implementations for NHibernate.
    /// Add to your configuration:
    /// configuration.Properties[Environment.CollectionTypeFactoryClass] = typeof(Net4CollectionTypeFactory).AssemblyQualifiedName;
    /// </summary>
    public class Net4CollectionTypeFactory : DefaultCollectionTypeFactory
    {

        /// <summary>
        /// Sets the specified role.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="role">The role.</param>
        /// <param name="propertyRef">The property ref.</param>
        /// <param name="embedded">if set to <c>true</c> [embedded].</param>
        /// <returns></returns>
        public override CollectionType Set<T>(string role, string propertyRef, bool embedded)
        {
            return new GenericSetType<T>(role, propertyRef);
        }

        /// <summary>
        /// Sorteds the set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="role">The role.</param>
        /// <param name="propertyRef">The property ref.</param>
        /// <param name="embedded">if set to <c>true</c> [embedded].</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns></returns>
        public override CollectionType SortedSet<T>(string role, string propertyRef, bool embedded, IComparer<T> comparer)
        {
            return new GenericSortedSetType<T>(role, propertyRef, comparer);
        }

    }

    /// <summary>
    /// This class used by the infrastructure.
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    public class GenericSortedSetType<T> : GenericSetType<T>
    {

        private readonly IComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericSortedSetType&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="propertyRef">The property ref.</param>
        /// <param name="comparer">The comparer.</param>
        public GenericSortedSetType(string role, string propertyRef, IComparer<T> comparer)
            : base(role, propertyRef)
        {
            this.comparer = comparer;
        }

        /// <summary>
        /// Instantiates the specified anticipated size.
        /// </summary>
        /// <param name="anticipatedSize">Size of the anticipated.</param>
        /// <returns></returns>
        public override object Instantiate(int anticipatedSize)
        {
            return new SortedSet<T>(this.comparer);
        }

        /// <summary>
        /// Gets the comparer.
        /// </summary>
        public IComparer<T> Comparer
        {
            get
            {
                return this.comparer;
            }
        }

    }

    /// <summary>
    /// An <see cref="IType" /> that maps an <see cref="ISet{T}" /> collection
    /// to the database. This class used by the infrastructure.
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    public class GenericSetType<T> : SetType
    {

        /// <summary>
        /// Initializes a new instance of a <see cref="GenericSetType{T}"/> class for
        /// a specific role.
        /// </summary>
        /// <param name="role">The role the persistent collection is in.</param>
        /// <param name="propertyRef">The name of the property in the
        /// owner object containing the collection ID, or <see langword="null" /> if it is
        /// the primary key.</param>
        public GenericSetType(string role, string propertyRef)
            : base(role, propertyRef, false)
        {
        }

        /// <summary>
        ///   <see cref="P:NHibernate.Type.AbstractType.ReturnedClass"/>
        /// </summary>
        public override Type ReturnedClass
        {
            get { return typeof(ISet<T>); }
        }

        /// <summary>
        /// Instantiates a new <see cref="IPersistentCollection"/> for the set.
        /// </summary>
        /// <param name="session">The current <see cref="ISessionImplementor"/> for the set.</param>
        /// <param name="persister">The current <see cref="ICollectionPersister" /> for the set.</param>
        /// <param name="key"></param>
        public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister,
                                                          object key)
        {
            return new PersistentGenericSet<T>(session);
        }

        /// <summary>
        /// Wraps an <see cref="IList{T}"/> in a <see cref="PersistentGenericSet&lt;T&gt;"/>.
        /// </summary>
        /// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
        /// <param name="collection">The unwrapped <see cref="IList{T}"/>.</param>
        /// <returns>
        /// An <see cref="PersistentGenericSet&lt;T&gt;"/> that wraps the non NHibernate <see cref="IList{T}"/>.
        /// </returns>
        public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
        {
            var set = collection as ISet<T>;
            if (set == null)
            {
                var stronglyTypedCollection = collection as ICollection<T>;
                if (stronglyTypedCollection == null)
                    throw new HibernateException(Role + " must be an implementation of ISet<T> or ICollection<T>");
                set = new HashSet<T>(stronglyTypedCollection);
            }
            return new PersistentGenericSet<T>(session, set);
        }

        /// <summary>
        /// Instantiates the specified anticipated size.
        /// </summary>
        /// <param name="anticipatedSize">Size of the anticipated.</param>
        /// <returns></returns>
        public override object Instantiate(int anticipatedSize)
        {
            return new HashSet<T>();
        }

        /// <summary>
        /// Clears the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        protected override void Clear(object collection)
        {
            ((ISet<T>)collection).Clear();
        }

        /// <summary>
        /// Adds the specified collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="element">The element.</param>
        protected override void Add(object collection, object element)
        {
            ((ISet<T>)collection).Add((T)element);
        }

    }

    /// <summary>
    /// A persistent wrapper for an <see cref="ISet{T}" />
    /// This class used by the infrastructure.
    /// </summary>
    /// <typeparam name="T">Generic type</typeparam>
    [Serializable]
    [DebuggerTypeProxy(typeof(CollectionProxy<>))]
    public class PersistentGenericSet<T> : AbstractPersistentCollection, ISet<T>
    {
        /// <summary>
        /// The <see cref="ISet{T}"/> that NHibernate is wrapping.
        /// </summary>
        protected ISet<T> set;

        /// <summary>
        /// A temporary list that holds the objects while the PersistentSet is being
        /// populated from the database.  
        /// </summary>
        /// <remarks>
        /// This is necessary to ensure that the object being added to the PersistentSet doesn't
        /// have its' <c>GetHashCode()</c> and <c>Equals()</c> methods called during the load
        /// process.
        /// </remarks>
        [NonSerialized]
        private IList<T> tempList;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentGenericSet&lt;T&gt;" /> class.
        /// </summary>
        public PersistentGenericSet()
        {
        }

        // needed for serialization

        /// <summary>
        /// Constructor matching super.
        /// Instantiates a lazy set (the underlying set is un-initialized).
        /// </summary>
        /// <param name="session">The session to which this set will belong.</param>
        public PersistentGenericSet(ISessionImplementor session)
            : base(session)
        {
        }

        /// <summary>
        /// Instantiates a non-lazy set (the underlying set is constructed
        /// from the incoming set reference).
        /// </summary>
        /// <param name="session">The session to which this set will belong.</param>
        /// <param name="original">The underlying set data.</param>
        public PersistentGenericSet(ISessionImplementor session, ISet<T> original)
            : base(session)
        {
            // Sets can be just a view of a part of another collection.
            // do we need to copy it to be sure it won't be changing
            // underneath us?
            // ie. this.set.addAll(set);
            set = original;
            SetInitialized();
            IsDirectlyAccessible = true;
        }

        /// <summary>
        /// Gets a value indicating whether [row update possible].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [row update possible]; otherwise, <c>false</c>.
        /// </value>
        public override bool RowUpdatePossible
        {
            get { return false; }
        }

        /// <summary>
        /// Is the initialized collection empty?
        /// </summary>
        public override bool Empty
        {
            get { return set.Count == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty
        {
            get { return ReadSize() ? CachedSize == 0 : (set.Count == 0); }
        }

        /// <summary>
        /// Gets the sync root.
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is synchronized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is synchronized; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronized
        {
            get { return false; }
        }

        #region ISet<T> Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            Read();
            return set.GetEnumerator();
        }

        /// <summary>
        /// Determines whether [contains] [the specified o].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified o]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T o)
        {
            bool? exists = ReadElementExistence(o);
            return exists == null ? set.Contains(o) : exists.Value;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            Read();
            Array.Copy(set.ToArray(), 0, array, arrayIndex, Count);
        }

        //public bool ContainsAll(ICollection c)
        //{
        //    Read();
        //    return set.ContainsAll(c);
        //}

        /// <summary>
        /// Adds the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public bool Add(T o)
        {
            bool? exists = IsOperationQueueEnabled ? ReadElementExistence(o) : null;
            if (!exists.HasValue)
            {
                Initialize(true);
                if (set.Add(o))
                {
                    Dirty();
                    return true;
                }
                return false;
            }
            if (exists.Value)
            {
                return false;
            }
            QueueOperation(new SimpleAddDelayedOperation(this, o));
            return true;
        }

        /// <summary>
        /// Unions the with.
        /// </summary>
        /// <param name="other">The other.</param>
        public void UnionWith(IEnumerable<T> other)
        {
            Read();
            set.UnionWith(other);
        }

        /// <summary>
        /// Intersects the with.
        /// </summary>
        /// <param name="other">The other.</param>
        public void IntersectWith(IEnumerable<T> other)
        {
            Read();
            set.IntersectWith(other);
        }

        /// <summary>
        /// Excepts the with.
        /// </summary>
        /// <param name="other">The other.</param>
        public void ExceptWith(IEnumerable<T> other)
        {
            Read();
            set.ExceptWith(other);
        }

        /// <summary>
        /// Symmetrics the except with.
        /// </summary>
        /// <param name="other">The other.</param>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Read();
            set.SymmetricExceptWith(other);
        }

        /// <summary>
        /// Determines whether [is subset of] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if [is subset of] [the specified other]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            Read();
            return set.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Determines whether [is superset of] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if [is superset of] [the specified other]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            Read();
            return set.IsSupersetOf(other);
        }

        /// <summary>
        /// Determines whether [is proper superset of] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if [is proper superset of] [the specified other]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            Read();
            return set.IsProperSupersetOf(other);
        }

        /// <summary>
        /// Determines whether [is proper subset of] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>
        ///   <c>true</c> if [is proper subset of] [the specified other]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            Read();
            return set.IsProperSubsetOf(other);
        }

        /// <summary>
        /// Overlapses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool Overlaps(IEnumerable<T> other)
        {
            Read();
            return set.Overlaps(other);
        }

        /// <summary>
        /// Sets the equals.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public bool SetEquals(IEnumerable<T> other)
        {
            Read();
            return set.SetEquals(other);
        }

        /// <summary>
        /// Removes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public bool Remove(T o)
        {
            bool? exists = PutQueueEnabled ? ReadElementExistence(o) : null;
            if (!exists.HasValue)
            {
                Initialize(true);
                if (set.Remove(o))
                {
                    Dirty();
                    return true;
                }
                return false;
            }
            if (exists.Value)
            {
                QueueOperation(new SimpleRemoveDelayedOperation(this, o));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            if (ClearQueueEnabled)
            {
                QueueOperation(new ClearDelayedOperation(this));
            }
            else
            {
                Initialize(true);
                if (set.Count != 0)
                {
                    set.Clear();
                    Dirty();
                }
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get { return ReadSize() ? CachedSize : set.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            Read();
            return set.GetEnumerator();
        }

        #endregion

        #region DelayedOperations

        #region Nested type: ClearDelayedOperation

        /// <summary>
        /// Clear delayed operation
        /// </summary>
        protected sealed class ClearDelayedOperation : IDelayedOperation
        {
            private readonly PersistentGenericSet<T> enclosingInstance;

            /// <summary>
            /// Initializes a new instance of the <see cref="PersistentGenericSet&lt;T&gt;.ClearDelayedOperation"/> class.
            /// </summary>
            /// <param name="enclosingInstance">The enclosing instance.</param>
            public ClearDelayedOperation(PersistentGenericSet<T> enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            #region IDelayedOperation Members

            /// <summary>
            /// Gets the added instance.
            /// </summary>
            public object AddedInstance
            {
                get { return null; }
            }

            /// <summary>
            /// Gets the orphan.
            /// </summary>
            /// <value>
            /// The orphan.
            /// </value>
            /// <exception cref="System.NotSupportedException">queued clear cannot be used with orphan delete</exception>
            public object Orphan
            {
                get { throw new NotSupportedException("queued clear cannot be used with orphan delete"); }
            }

            /// <summary>
            /// Operates this instance.
            /// </summary>
            public void Operate()
            {
                enclosingInstance.set.Clear();
            }

            #endregion

        }

        #endregion

        #region Nested type: SimpleAddDelayedOperation

        /// <summary>
        /// Simple add delayed operation
        /// </summary>
        protected sealed class SimpleAddDelayedOperation : IDelayedOperation
        {
            private readonly PersistentGenericSet<T> enclosingInstance;
            private readonly T value;

            /// <summary>
            /// Initializes a new instance of the <see cref="PersistentGenericSet&lt;T&gt;.SimpleAddDelayedOperation" /> class.
            /// </summary>
            /// <param name="enclosingInstance">The enclosing instance.</param>
            /// <param name="value">The value.</param>
            public SimpleAddDelayedOperation(PersistentGenericSet<T> enclosingInstance, T value)
            {
                this.enclosingInstance = enclosingInstance;
                this.value = value;
            }

            #region IDelayedOperation Members

            /// <summary>
            /// Gets the added instance.
            /// </summary>
            public object AddedInstance
            {
                get { return value; }
            }

            /// <summary>
            /// Gets the orphan.
            /// </summary>
            public object Orphan
            {
                get { return null; }
            }

            /// <summary>
            /// Operates this instance.
            /// </summary>
            public void Operate()
            {
                enclosingInstance.set.Add(value);
            }

            #endregion

        }

        #endregion

        #region Nested type: SimpleRemoveDelayedOperation

        /// <summary>
        /// Simple remove delayed operation
        /// </summary>
        protected sealed class SimpleRemoveDelayedOperation : IDelayedOperation
        {
            private readonly PersistentGenericSet<T> enclosingInstance;
            private readonly T value;

            /// <summary>
            /// Initializes a new instance of the <see cref="PersistentGenericSet&lt;T&gt;.SimpleRemoveDelayedOperation"/> class.
            /// </summary>
            /// <param name="enclosingInstance">The enclosing instance.</param>
            /// <param name="value">The value.</param>
            public SimpleRemoveDelayedOperation(PersistentGenericSet<T> enclosingInstance, T value)
            {
                this.enclosingInstance = enclosingInstance;
                this.value = value;
            }

            #region IDelayedOperation Members

            /// <summary>
            /// Gets the added instance.
            /// </summary>
            public object AddedInstance
            {
                get { return null; }
            }

            /// <summary>
            /// Gets the orphan.
            /// </summary>
            public object Orphan
            {
                get { return value; }
            }

            /// <summary>
            /// Operates this instance.
            /// </summary>
            public void Operate()
            {
                enclosingInstance.set.Remove(value);
            }

            #endregion

        }

        #endregion

        #endregion

        /// <summary>
        /// Gets the snapshot.
        /// </summary>
        /// <param name="persister">The persister.</param>
        /// <returns></returns>
        public override ICollection GetSnapshot(ICollectionPersister persister)
        {
            var entityMode = Session.EntityMode;
            var clonedSet = new SetSnapShot<T>(set.Count);
            var enumerable = from object current in set
                             select persister.ElementType.DeepCopy(current, entityMode, persister.Factory);
            foreach (var copied in enumerable)
            {
                clonedSet.Add((T)copied);
            }
            return clonedSet;
        }

        /// <summary>
        /// Get all "orphaned" elements
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public override ICollection GetOrphans(object snapshot, string entityName)
        {
            var sn = new SetSnapShot<object>((IEnumerable<object>)snapshot);
            if (set.Count == 0) return sn;
            if (((ICollection)sn).Count == 0) return sn;
            return GetOrphans(sn, set.ToArray(), entityName, Session);
        }

        /// <summary>
        /// Equalses the snapshot.
        /// </summary>
        /// <param name="persister">The persister.</param>
        /// <returns></returns>
        public override bool EqualsSnapshot(ICollectionPersister persister)
        {
            var elementType = persister.ElementType;
            var snapshot = (ISetSnapshot<T>)GetSnapshot();
            if (((ICollection)snapshot).Count != set.Count)
            {
                return false;
            }

            return !(from object obj in set
                     let oldValue = snapshot[(T)obj]
                     where oldValue == null || elementType.IsDirty(oldValue, obj, Session)
                     select obj).Any();
        }

        /// <summary>
        /// Determines whether [is snapshot empty] [the specified snapshot].
        /// </summary>
        /// <param name="snapshot">The snapshot.</param>
        /// <returns>
        ///   <c>true</c> if [is snapshot empty] [the specified snapshot]; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsSnapshotEmpty(object snapshot)
        {
            return ((ICollection)snapshot).Count == 0;
        }

        /// <summary>
        /// Called before any elements are read into the collection,
        /// allowing appropriate initializations to occur.
        /// </summary>
        /// <param name="persister">The underlying collection persister.</param>
        /// <param name="anticipatedSize">The anticipated size of the collection after initilization is complete.</param>
        public override void BeforeInitialize(ICollectionPersister persister, int anticipatedSize)
        {
            set = (ISet<T>)persister.CollectionType.Instantiate(anticipatedSize);
        }

        /// <summary>
        /// Initializes this PersistentSet from the cached values.
        /// </summary>
        /// <param name="persister">The CollectionPersister to use to reassemble the PersistentSet.</param>
        /// <param name="disassembled">The disassembled PersistentSet.</param>
        /// <param name="owner">The owner object.</param>
        public override void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner)
        {
            var array = (object[])disassembled;
            int size = array.Length;
            BeforeInitialize(persister, size);
            for (int i = 0; i < size; i++)
            {
                var element = (T)persister.ElementType.Assemble(array[i], Session, owner);
                if (element != null)
                {
                    set.Add(element);
                }
            }
            SetInitialized();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            Read();
            return StringHelper.CollectionToString(set.ToArray());
        }

        /// <summary>
        /// Reads from.
        /// </summary>
        /// <param name="rs">The rs.</param>
        /// <param name="role">The role.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <param name="owner">The owner.</param>
        /// <returns></returns>
        public override object ReadFrom(IDataReader rs, ICollectionPersister role, ICollectionAliases descriptor, object owner)
        {
            var element = (T)role.ReadElement(rs, owner, descriptor.SuffixedElementAliases, Session);
            if (element != null)
            {
                tempList.Add(element);
            }
            return element;
        }

        /// <summary>
        /// Set up the temporary List that will be used in the EndRead() 
        /// to fully create the set.
        /// </summary>
        public override void BeginRead()
        {
            base.BeginRead();
            tempList = new List<T>();
        }

        /// <summary>
        /// Takes the contents stored in the temporary list created during <c>BeginRead()</c>
        /// that was populated during <c>ReadFrom()</c> and write it to the underlying 
        /// PersistentSet.
        /// </summary>
        public override bool EndRead(ICollectionPersister persister)
        {
            foreach (T item in tempList)
            {
                set.Add(item);
            }
            tempList = null;
            SetInitialized();
            return true;
        }

        /// <summary>
        /// Entrieses the specified persister.
        /// </summary>
        /// <param name="persister">The persister.</param>
        /// <returns></returns>
        public override IEnumerable Entries(ICollectionPersister persister)
        {
            return set;
        }

        /// <summary>
        /// Disassemble the collection, ready for the cache
        /// </summary>
        /// <param name="persister"></param>
        /// <returns></returns>
        public override object Disassemble(ICollectionPersister persister)
        {
            var result = new object[set.Count];
            int i = 0;

            foreach (object obj in set)
            {
                result[i++] = persister.ElementType.Disassemble(obj, Session, null);
            }
            return result;
        }

        /// <summary>
        /// Get all the elements that need deleting
        /// </summary>
        /// <param name="persister"></param>
        /// <param name="indexIsFormula"></param>
        /// <returns></returns>
        public override IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula)
        {
            IType elementType = persister.ElementType;
            var sn = (ISetSnapshot<T>)GetSnapshot();
            var deletes = new List<T>(((ICollection<T>)sn).Count);

            deletes.AddRange(sn.Where(obj => !set.Contains(obj)));

            deletes.AddRange(from obj in set
                             let oldValue = sn[obj]
                             where oldValue != null && elementType.IsDirty(obj, oldValue, Session)
                             select oldValue);

            return deletes;
        }

        /// <summary>
        /// Do we need to insert this element?
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="i"></param>
        /// <param name="elemType"></param>
        /// <returns></returns>
        public override bool NeedsInserting(object entry, int i, IType elemType)
        {
            var sn = (ISetSnapshot<T>)GetSnapshot();
            object oldKey = sn[(T)entry];
            // note that it might be better to iterate the snapshot but this is safe,
            // assuming the user implements equals() properly, as required by the PersistentSet
            // contract!
            return oldKey == null || elemType.IsDirty(oldKey, entry, Session);
        }

        /// <summary>
        /// Do we need to update this element?
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="i"></param>
        /// <param name="elemType"></param>
        /// <returns></returns>
        public override bool NeedsUpdating(object entry, int i, IType elemType)
        {
            return false;
        }

        /// <summary>
        /// Get the index of the given collection entry
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="i"></param>
        /// <param name="persister"></param>
        /// <returns></returns>
        public override object GetIndex(object entry, int i, ICollectionPersister persister)
        {
            throw new NotSupportedException("Sets don't have indexes");
        }

        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public override object GetElement(object entry)
        {
            return entry;
        }

        /// <summary>
        /// Gets the snapshot element.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public override object GetSnapshotElement(object entry, int i)
        {
            throw new NotSupportedException("Sets don't support updating by element");
        }

        /// <summary>
        /// Called by any read-only method of the collection interface
        /// </summary>
        public new void Read()
        {
            base.Read();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            var that = other as ISet<T>;
            if (that == null)
            {
                return false;
            }
            Read();
            return set.SequenceEqual(that);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            Read();
            return set.GetHashCode();
        }

        /// <summary>
        /// Does an element exist at this entry in the collection?
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public override bool EntryExists(object entry, int i)
        {
            return true;
        }

        /// <summary>
        /// Is this the wrapper for the given underlying collection instance?
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public override bool IsWrapper(object collection)
        {
            return set == collection;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Array array, int index)
        {
            // NH : we really need to initialize the set ?
            Read();
            Array.Copy(set.ToArray(), 0, array, index, Count);
        }

        #region Nested type: ISetSnapshot

        private interface ISetSnapshot<TData> : ICollection<TData>, ICollection
        {
            TData this[TData element] { get; }
        }

        #endregion

        #region Nested type: SetSnapShot

        [Serializable]
        private class SetSnapShot<TData> : ISetSnapshot<TData>
        {
            private readonly List<TData> elements;

            private SetSnapShot()
            {
                elements = new List<TData>();
            }

            public SetSnapShot(int capacity)
            {
                elements = new List<TData>(capacity);
            }

            public SetSnapShot(IEnumerable<TData> collection)
            {
                elements = new List<TData>(collection);
            }

            #region ISetSnapshot<TData> Members

            public IEnumerator<TData> GetEnumerator()
            {
                return elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(TData item)
            {
                elements.Add(item);
            }

            public void Clear()
            {
                throw new InvalidOperationException();
            }

            public bool Contains(TData item)
            {
                return elements.Contains(item);
            }

            public void CopyTo(TData[] array, int arrayIndex)
            {
                elements.CopyTo(array, arrayIndex);
            }

            public bool Remove(TData item)
            {
                throw new InvalidOperationException();
            }

            public void CopyTo(Array array, int index)
            {
                ((ICollection)elements).CopyTo(array, index);
            }

            int ICollection.Count
            {
                get { return elements.Count; }
            }

            public object SyncRoot
            {
                get { return ((ICollection)elements).SyncRoot; }
            }

            public bool IsSynchronized
            {
                get { return ((ICollection)elements).IsSynchronized; }
            }

            int ICollection<TData>.Count
            {
                get { return elements.Count; }
            }

            public bool IsReadOnly
            {
                get { return ((ICollection<TData>)elements).IsReadOnly; }
            }

            public TData this[TData element]
            {
                get
                {
                    int idx = elements.IndexOf(element);
                    if (idx >= 0)
                    {
                        return elements[idx];
                    }
                    return default(TData);
                }
            }

            #endregion

        }

        #endregion

    }

}
