/* *********************************************************************
 * Date: 10 Dec 2010
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using Forge.Configuration.Shared;

namespace Forge.Threading.ConfigSection
{

    /// <summary>
    /// Represents the confoguration section of the threadpools
    /// </summary>
    public sealed class ThreadPoolSection : ConfigurationSectionStandard
    {

        #region Field(s)

        // The ThreadPools
        private ConfigurationProperty mThreadPools;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolSection"/> class.
        /// </summary>
        public ThreadPoolSection() : base()
        {
            // Property initialization
            mThreadPools = new ConfigurationProperty("ThreadPools", typeof(ThreadPools));
            Properties.Add(mThreadPools);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the thread pools.
        /// </summary>
        /// <value>
        /// The thread pools.
        /// </value>
		[ConfigurationProperty("ThreadPools", IsRequired = true)]
        public ThreadPools ThreadPools
        {
            get
            {
                return (ThreadPools)this["ThreadPools"];
            }
            set
            {
                this["ThreadPools"] = value;
            }
        }

        #endregion

    }

    /// <summary>
    /// Represents the configuration section content of the threadpool configuration
    /// </summary>
    [Serializable]
    [ConfigurationCollection(typeof(ThreadPoolItem))]
    public class ThreadPools : ConfigurationElementCollection
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPools"/> class.
        /// </summary>
        public ThreadPools()
        {
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ThreadPoolItem();
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
            return ((ThreadPoolItem)element).Name;
        }

        /// <summary>
        /// Adds the specified element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void Add(ThreadPoolItem element)
        {
            this.BaseAdd(element);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Remove(string key)
        {
            this.BaseRemove(key);
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
        public ThreadPoolItem this[int index]
        {
            get
            {
                return (ThreadPoolItem)this.BaseGet(index);
            }
        }

    }

    /// <summary>
    /// Represents the threadpool configuration item
    /// </summary>
    [Serializable]
    public class ThreadPoolItem : ConfigurationElement
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolItem"/> class.
        /// </summary>
        public ThreadPoolItem() : base()
        {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("name", IsRequired = false, IsKey = true, DefaultValue = "")]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the min thread number.
        /// </summary>
        /// <value>
        /// The min thread number.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("minThreadNumber", IsRequired = false, IsKey = false, DefaultValue = 1)]
        public int MinThreadNumber
        {
            get { return (int)this["minThreadNumber"]; }
            set { this["minThreadNumber"] = value; }
        }

        /// <summary>
        /// Gets or sets the max thread number.
        /// </summary>
        /// <value>
        /// The max thread number.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("maxThreadNumber", IsRequired = false, IsKey = false, DefaultValue = 20)]
        public int MaxThreadNumber
        {
            get { return (int)this["maxThreadNumber"]; }
            set { this["maxThreadNumber"] = value; }
        }

        /// <summary>
        /// Gets or sets the max concurrent execution.
        /// </summary>
        /// <value>
        /// The max concurrent execution.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("maxConcurrentExecution", IsRequired = false, IsKey = false, DefaultValue = 20)]
        public int MaxConcurrentExecution
        {
            get { return (int)this["maxConcurrentExecution"]; }
            set { this["maxConcurrentExecution"] = value; }
        }

        /// <summary>
        /// Gets or sets the shut down idle thread time.
        /// </summary>
        /// <value>
        /// The shut down idle thread time.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("shutDownIdleThreadTime", IsRequired = false, IsKey = false, DefaultValue = 120000)]
        public int ShutDownIdleThreadTime
        {
            get { return (int)this["shutDownIdleThreadTime"]; }
            set { this["shutDownIdleThreadTime"] = value; }
        }

        /// <summary>
        /// Gets or sets the size of the max stack.
        /// </summary>
        /// <value>
        /// The size of the max stack.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("maxStackSize", IsRequired = false, IsKey = false, DefaultValue = 0)]
        public int MaxStackSize
        {
            get { return (int)this["maxStackSize"]; }
            set { this["maxStackSize"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set read only flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [set read only flag]; otherwise, <c>false</c>.
        /// </value>
        [DebuggerHidden]
        [ConfigurationProperty("setReadOnly", IsRequired = false, IsKey = false, DefaultValue = false)]
        public bool SetReadOnlyFlag
        {
            get { return (bool)this["setReadOnly"]; }
            set { this["setReadOnly"] = value; }
        }

    }

}
