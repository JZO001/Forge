using System;
using NHibernate.Mapping.Attributes;

namespace Forge.Testing.Entities
{
    [Serializable]
    [UnionSubclass(Table = "CarInherited", ExtendsType = typeof(TestCar))]
    public class CarInherited : TestCar
    {
        #region Filed(s)

        [Property]
        private int testFieldInt = 0;

        [Property]
        private string testFieldString = null;

        [OneToOne(Name = "testDriverEntity", ClassType = typeof(DriverInherited), Cascade = "none", Constrained = false, PropertyRef = "testCarEntity")]
        private DriverInherited testDriverEntity = null;

        [ManyToOne(0, Name = "testMechanicEntityCar", ClassType = typeof(Mechanic), Cascade = "none", Unique = true)]
        [Column(1, Name = "testMechanicEntityCar_restId")]
        [Column(2, Name = "testMechanicEntityCar_deviceId")]
        [Column(3, Name = "testMechanicEntityCar_id")]
        private Mechanic testMechanicEntityCar = null;

        #endregion

        #region Contructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="CarInherited" /> class.
        /// </summary>
        public CarInherited()
        {

        }

        #endregion

        #region Properties

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
        /// Gets or sets the test driver entity.
        /// </summary>
        /// <value>
        /// The test driver entity.
        /// </value>
        public virtual DriverInherited TestDriverEntity
        {
            get { return testDriverEntity; }
            set { testDriverEntity = value; }
        }

        public virtual Mechanic TestMechanicEntityCar
        {
            get { return testMechanicEntityCar; }
            set { testMechanicEntityCar = value; }
        }

        #endregion

    }
}
