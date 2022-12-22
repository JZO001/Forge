using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities
{
    [Serializable]
    [UnionSubclass(Table = "TestCar", ExtendsType = typeof(TestCarBase))]
    public class TestCar : TestCarBase
    {
        #region Field(s)

        [Property]
        private int tValue = 0;

        [Property]
        private string tName = null;

        [OneToOne(Name = "tDriverE", ClassType = typeof(TestDriver), Cascade = "none", Constrained = false, PropertyRef = "tCarE")]
        private TestDriver tDriverE = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCar" /> class.
        /// </summary>
        public TestCar()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the tvalue.
        /// </summary>
        /// <value>
        /// The tvalue.
        /// </value>
        public virtual int Tvalue
        {
            get { return tValue; }
            set { tValue = value; }
        }

        /// <summary>
        /// Gets or sets the tname.
        /// </summary>
        /// <value>
        /// The tname.
        /// </value>
        public virtual string Tname
        {
            get { return tName; }
            set { tName = value; }
        }

        /// <summary>
        /// Gets or sets the tdriver Entity.
        /// </summary>
        /// <value>
        /// The tdriver Entitiy.
        /// </value>
        public virtual TestDriver TdriverE
        {
            get { return tDriverE; }
            set { tDriverE = value; }
        }

        #endregion
    }
}
