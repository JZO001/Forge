using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities
{
    [Serializable]
    [UnionSubclass(Table = "DriverInherited", ExtendsType = typeof(TestDriver))]
    public class DriverInherited : TestDriver
    {
        #region Field(s)

        [Property]
        private int testFieldInt = 0;

        [Property]
        private string testFieldString = null;

        [ManyToOne(0, Name = "testCarEntity", ClassType = typeof(CarInherited), Cascade = "none", Unique = true)]
        [Column(1, Name = "testCarEntity_restId")]
        [Column(2, Name = "testCarEntity_deviceId")]
        [Column(3, Name = "testCarEntity_id")]
        private CarInherited testCarEntity = null;

        [ManyToOne(0, Name = "testMechanicEntity", ClassType = typeof(Mechanic), Cascade = "none", Unique = true)]
        [Column(1, Name = "testMechanicEntity_restId")]
        [Column(2, Name = "testMechanicEntity_deviceId")]
        [Column(3, Name = "testMechanicEntity_id")]
        private Mechanic testMechanicEntity = null;

        [OneToOne(Name = "mechanicEntity", ClassType = typeof(MechanicEntity), Cascade = "none", Constrained = false, PropertyRef = "driverEntity")]
        private MechanicEntity mechanicEntity = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverInherited" /> class.
        /// </summary>
        public DriverInherited()
        {

        }

        #endregion

        #region Property

        /// <summary>
        /// Gets or sets the test field int.
        /// </summary>
        /// <value>
        /// The test field int.
        /// </value>
        public virtual int TestFieldInt
        {
            get { return testFieldInt; }
            set { testFieldInt = value; }
        }

        /// <summary>
        /// Gets or sets the test field string.
        /// </summary>
        /// <value>
        /// The test field string.
        /// </value>
        public virtual string TestFieldString
        {
            get { return testFieldString; }
            set { testFieldString = value; }
        }

        /// <summary>
        /// Gets or sets the test car entity.
        /// </summary>
        /// <value>
        /// The test car entity.
        /// </value>
        public virtual CarInherited TestCarEntity
        {
            get { return testCarEntity; }
            set { testCarEntity = value; }
        }

        /// <summary>
        /// Gets or sets the test mechanic entity.
        /// </summary>
        /// <value>
        /// The test mechanic entity.
        /// </value>
        public virtual Mechanic TestMechanicEntity
        {
            get { return testMechanicEntity; }
            set { testMechanicEntity = value; }
        }

        /// <summary>
        /// Gets or sets the mechanic entity.
        /// </summary>
        /// <value>
        /// The mechanic entity.
        /// </value>
        public virtual MechanicEntity MechanicEntity
        {
            get { return mechanicEntity; }
            set { mechanicEntity = value; }
        }

        #endregion

    }
}
