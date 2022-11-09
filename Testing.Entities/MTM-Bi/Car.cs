using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities.MTM_Bi
{

    // ManyToMany, bidirectional. Both entities contain an IList to hold the list of the other type of entities.
    [Serializable]
    [Class]
    public class Car : EntityBase
    {

        [Property]
        private string model = string.Empty;

        [Set(0, Name = "drivers", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "Car_Driver_Switch")]
        [Key(1)]
        [Column(2, Name = "car_restId")]
        [Column(3, Name = "car_deviceId")]
        [Column(4, Name = "car_id")]
        //[Index(5)]
        //[Column(6, Name = "drivers_restId")]
        //[Column(7, Name = "drivers_deviceId")]
        //[Column(8, Name = "drivers_id")]
        [ManyToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(Driver))]
        [Column(6, Name = "driver_restId")]
        [Column(7, Name = "driver_deviceId")]
        [Column(8, Name = "driver_id")]
        private ISet<Driver> drivers = new HashSet<Driver>();

        public Car()
        {
        }

        public virtual string Model
        {
            get { return model; }
            set { model = value; }
        }

        public virtual ISet<Driver> Drivers
        {
            get { return drivers; }
            set { drivers = value; }
        }

    }

}
