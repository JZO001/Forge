using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    [Serializable]
    [Class]
    public class MechanicEntity : EntityBase
    {
        #region Field(s)

        [ManyToOne(0, Name = "driverEntity", ClassType = typeof(DriverInherited), Cascade = "none", Unique = true)]
        [Column(1, Name = "driverEntity_restId")]
        [Column(2, Name = "driverEntity_deviceId")]
        [Column(3, Name = "driverEntity_id")]
        private DriverInherited driverEntity = null;

        [OneToOne(Name = "mEntity", ClassType = typeof(Mechanic), Cascade = "none", Constrained = false, PropertyRef = "testMechanicEntity")]
        private Mechanic mEntity = null;

        [Property]
        private int meTestFieldInt = 0;

        [Property]
        private string meTestFieldString = null;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="MechanicEntity" /> class.
        /// </summary>
        public MechanicEntity()
        {

        }

        #endregion

        #region Properties
        
        public virtual DriverInherited DriverEntity
        {
            get { return driverEntity; }
            set { driverEntity = value; }
        }

        public virtual int MEtestFieldInt
        {
            get { return meTestFieldInt; }
            set { meTestFieldInt = value; }
        }

        public virtual string MEtestFieldString
        {
            get { return meTestFieldString; }
            set { meTestFieldString = value; }
        }

        public virtual Mechanic MEntity
        {
            get { return mEntity; }
            set { mEntity = value; }
        }

        #endregion

    }
}
