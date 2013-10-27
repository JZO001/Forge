/* *********************************************************************
 * Date: 20 Feb 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Forge.Configuration.Shared
{

    /// <summary>
    /// CategoryProperty classes for general configuration structure
    /// </summary>
    [Serializable]
    [ConfigurationCollection(typeof(CategoryPropertyItem))]
    public class CategoryPropertyItems : ConfigurationElementCollection, ICloneable, IEnumerable<CategoryPropertyItem>
    {

        private CategoryPropertyItem mParent = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryPropertyItems"/> class.
        /// </summary>
        public CategoryPropertyItems()
        {
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement" />.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            CategoryPropertyItem newItem = new CategoryPropertyItem();
            newItem.Parent = this;
            return newItem;
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CategoryPropertyItem)element).Id;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(CategoryPropertyItem element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.Parent = this;
            this.BaseAdd(element);
        }

        /// <summary>
        /// Removes the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Remove(CategoryPropertyItem element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            this.BaseRemove(element.Id);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }

        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The specified property, attribute, or child element
        /// </returns>
        public CategoryPropertyItem this[int index]
        {
            get
            {
                CategoryPropertyItem item = this.BaseGet(index) as CategoryPropertyItem;
                if (item != null)
                {
                    item.Parent = this;
                }
                return item;
            }
        }

        /// <summary>
        /// Gets or sets a property, attribute, or child element of this configuration element.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// The specified property, attribute, or child element
        /// </returns>
        public new CategoryPropertyItem this[string key]
        {
            get
            {
                CategoryPropertyItem item = this.BaseGet(key) as CategoryPropertyItem;
                if (item != null)
                {
                    item.Parent = this;
                }
                return item;
            }
        }

        /// <summary>
        /// Gets the category items.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns>Collection of CategoryPropertyItem</returns>
        /// <exception cref="System.ArgumentNullException">categoryName</exception>
        public ICollection<CategoryPropertyItem> GetCategoryItems(String categoryName)
        {
            if (categoryName == null)
            {
                throw new ArgumentNullException("categoryName");
            }
            List<CategoryPropertyItem> result = new List<CategoryPropertyItem>();
            if (this.Count > 0)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    CategoryPropertyItem item = (CategoryPropertyItem)this.BaseGet(i);
                    if (item.EntryName.Equals(categoryName))
                    {
                        item.Parent = this;
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        [DebuggerHidden]
        public CategoryPropertyItem Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            CategoryPropertyItems items = new CategoryPropertyItems();
            if (this.Count > 0)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    items.Add(((ICloneable)this.BaseGet(i)).Clone() as CategoryPropertyItem);
                }
            }
            return items;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Enumerator of CategoryPropertyItems</returns>
        public new IEnumerator<CategoryPropertyItem> GetEnumerator()
        {
            List<CategoryPropertyItem> result = new List<CategoryPropertyItem>();
            if (this.Count > 0)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    CategoryPropertyItem item = (CategoryPropertyItem)this.BaseGet(i);
                    item.Parent = this;
                    result.Add(item);
                }
            }
            return result.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>
        /// Enumerator of CategoryPropertyItems
        /// </returns>
        IEnumerator<CategoryPropertyItem> IEnumerable<CategoryPropertyItem>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.IEnumerator" /> which is used to iterate through the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> which is used to iterate through the <see cref="T:System.Configuration.ConfigurationElementCollection" />
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }

    /// <summary>
    /// CategoryProperty classes for general configuration structure
    /// </summary>
    [Serializable]
    public class CategoryPropertyItem : ConfigurationElement, ICloneable, IEnumerable<CategoryPropertyItem>
    {

        private CategoryPropertyItems mParent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryPropertyItem"/> class.
        /// </summary>
        public CategoryPropertyItem()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("id", IsRequired = true, IsKey = true, DefaultValue = "")]
        public string Id
        {
            get
            {
                string result = (string)this["id"];
                if (Encrypted)
                {
                    result = DecryptData(result);
                }
                return result;
            }
            set
            {
                this["id"] = Encrypted ? EncryptData(value) : value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the entry.
        /// </summary>
        /// <value>
        /// The name of the entry.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("entryName", IsRequired = false, IsKey = false, DefaultValue = "")]
        public string EntryName
        {
            get
            {
                string result = (string)this["entryName"];
                if (Encrypted)
                {
                    result = DecryptData(result);
                }
                return result;
            }
            set
            {
                this["entryName"] = Encrypted ? EncryptData(value) : value;
            }
        }

        /// <summary>
        /// Gets or sets the entry value.
        /// </summary>
        /// <value>
        /// The entry value.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("entryValue", IsRequired = false, IsKey = false, DefaultValue = "")]
        public string EntryValue
        {
            get
            {
                string result = (string)this["entryValue"];
                if (Encrypted)
                {
                    result = DecryptData(result);
                }
                return result;
            }
            set
            {
                this["entryValue"] = Encrypted ? EncryptData(value) : value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CategoryPropertyItem"/> is encrypted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if encrypted; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("encrypted", IsRequired = false, IsKey = false, DefaultValue = false)]
        public bool Encrypted
        {
            get { return (bool)this["encrypted"]; }
            set
            {
                bool currentValue = (bool)this["encrypted"];
                if (currentValue != value)
                {
                    string id = this.Id;
                    string entryName = this.EntryName;
                    string entryValue = this.EntryValue;
                    this["encrypted"] = value;
                    this.Id = id;
                    this.EntryName = entryName;
                    this.EntryValue = entryValue;
                }
            }
        }

        /// <summary>
        /// Gets or sets the property items.
        /// </summary>
        /// <value>
        /// The property items.
        /// </value>
        [ConfigurationProperty("PropertyItems", IsRequired = false, IsKey = false)]
        public CategoryPropertyItems PropertyItems
        {
            get
            {
                CategoryPropertyItems items = this["PropertyItems"] as CategoryPropertyItems;
                if (items != null)
                {
                    items.Parent = this;
                }
                return items;
            }
            set
            {
                this["PropertyItems"] = value;
                if (value != null)
                {
                    value.Parent = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        [DebuggerHidden]
        public CategoryPropertyItems Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            CategoryPropertyItem item = new CategoryPropertyItem();
            item.Encrypted = this.Encrypted;
            item.Id = this.Id;
            item.EntryName = this.EntryName;
            item.EntryValue = this.EntryValue;
            item.PropertyItems = this.PropertyItems.Clone() as CategoryPropertyItems;
            return item;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<CategoryPropertyItem> GetEnumerator()
        {
            IEnumerator<CategoryPropertyItem> result = null;

            CategoryPropertyItems pis = PropertyItems;
            if (pis == null)
            {
                result = new List<CategoryPropertyItem>().GetEnumerator();
            }
            else
            {
                result = pis.GetEnumerator();
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Security

        private static readonly byte[] KEY = new byte[] { 45, 65, 210, 98, 23, 155, 233, 254, 0, 201,
                                                    178, 199, 12, 101, 202, 67, 86, 56, 95, 23,
                                                    59, 36, 127, 178, 139, 127, 43, 73, 21, 10,
                                                    99, 167 }; // 32

        private static readonly byte[] IV = new byte[] { 47, 26, 153, 98, 77, 42, 87, 137, 49, 188,
                                                    88, 42, 98, 188, 45, 123 }; // 16

        static CategoryPropertyItem()
        {
            CreateKeyFromHostName();
            CreateInitVectorFromMACAddress();
        }

        private static void CreateKeyFromHostName()
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(System.Net.Dns.GetHostName());
            Array.Copy(bytes, KEY, bytes.Length <= KEY.Length ? bytes.Length : KEY.Length);
        }

        private static void CreateInitVectorFromMACAddress()
        {
            NetworkInterface[] nets = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nets)
            {
                byte[] mac = ni.GetPhysicalAddress().GetAddressBytes();
                Array.Copy(mac, IV, mac.Length <= IV.Length ? mac.Length : IV.Length);
                break;
            }
        }

        private static string EncryptData(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return text;
            }

            byte[] data = Encoding.UTF8.GetBytes(text);

            ICryptoTransform encryptor = null;
            Rijndael rijndael = Rijndael.Create();
            byte[] result = null;

            try
            {
                using (rijndael)
                {
                    using (encryptor = rijndael.CreateEncryptor(KEY, IV))
                    {
                        result = encryptor.TransformFinalBlock(data, 0, data.Length);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                rijndael.Clear();
            }

            return Convert.ToBase64String(result);
        }

        private static string DecryptData(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return text;
            }

            byte[] data = Convert.FromBase64String(text);

            ICryptoTransform decryptor = null;
            Rijndael rijndael = Rijndael.Create();
            byte[] result = null;

            try
            {
                using (rijndael)
                {
                    using (decryptor = rijndael.CreateDecryptor(KEY, IV))
                    {
                        result = decryptor.TransformFinalBlock(data, 0, data.Length);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                rijndael.Clear();
            }

            return Encoding.UTF8.GetString(result);
        }

        #endregion

        /// <summary>
        /// Creates a property item.
        /// </summary>
        /// <returns></returns>
        public PropertyItem CreatePropertyItem()
        {
            PropertyItem result = new PropertyItem(this.Id, this.EntryValue);

            foreach (CategoryPropertyItem pi in this.PropertyItems)
            {
                result.PropertyItems[pi.Id] = pi.CreatePropertyItem();
            }

            return result;
        }

    }

}
