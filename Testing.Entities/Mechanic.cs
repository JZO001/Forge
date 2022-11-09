using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    [Serializable]
    [Class]
    public class Mechanic : EntityBase
    {
        #region Field(s)

        [Property]
        private string mName = null;

        [Property]
        private int mAge = 0;

        [OneToOne(Name = "driverEntity", ClassType = typeof(DriverInherited), Cascade = "none", Constrained = false, PropertyRef = "testMechanicEntity")]
        private DriverInherited driverEntity = null;

        [OneToOne(Name = "carIEntity", ClassType = typeof(CarInherited), Cascade = "none", Constrained = false, PropertyRef = "testMechanicEntityCar")]
        private CarInherited carIEntity = null;

        [ManyToOne(0, Name = "testMechanicEntity", ClassType = typeof(MechanicEntity), Cascade = "none", Unique = true)]
        [Column(1, Name = "testMechanicEntity_restId")]
        [Column(2, Name = "testMechanicEntity_deviceId")]
        [Column(3, Name = "testMechanicEntity_id")]
        private MechanicEntity testMechanicEntity = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="Mechanic" /> class.
        /// </summary>
        public Mechanic()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the driver entity.
        /// </summary>
        /// <value>
        /// The driver entity.
        /// </value>
        public virtual DriverInherited DriverEntity
        {
            get { return driverEntity; }
            set { driverEntity = value; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public virtual int Age
        {
            get { return mAge; }
            set { mAge = value; }
        }

        public virtual MechanicEntity TestMechanicEntity
        {
            get { return testMechanicEntity; }
            set { testMechanicEntity = value; }
        }

        public virtual CarInherited CarIEntity
        {
            get { return carIEntity; }
            set { carIEntity = value; }
        }

        #endregion
    }
}
