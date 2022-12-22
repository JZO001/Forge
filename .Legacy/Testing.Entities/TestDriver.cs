using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities
{
    [Serializable]
    [UnionSubclass(Table = "TestDriver", ExtendsType = typeof(TestDriverBase))]
    public class TestDriver : TestDriverBase
    {
        #region Field(s)

        [Property]
        private int tValue = 0;

        [Property]
        private string tName = null;

        [ManyToOne(0, Name = "tCarE", ClassType = typeof(TestCar), Cascade = "none", Unique = true)]
        [Column(1, Name = "tCarE_restId")]
        [Column(2, Name = "tCarE_deviceId")]
        [Column(3, Name = "tCarE_id")]
        private TestCar tCarE = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDriver" /> class.
        /// </summary>
        public TestDriver()
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
        /// Gets or sets the tcar Entity.
        /// </summary>
        /// <value>
        /// The tcar Entity.
        /// </value>
        public virtual TestCar TcarE
        {
            get { return tCarE; }
            set { tCarE = value; }
        }

        #endregion

    }
}
