using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;
using Forge.ORM.NHibernateExtension.Criterias;

namespace Forge.Testing.Entities.MTM_Bi
{

    // ManyToMany, bidirectional. Both entities contain an IList to hold the list of the other type of entities.
    [Serializable]
    [Class]
    public class Driver : EntityBase
    {

        private const string constTest = "HELLO";
        private readonly string readonlyTest = "Readonly";

        [Property]
        private string name = string.Empty;

        [Set(0, Name = "cars", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "Car_Driver_Switch", Inverse = true)]
        [Key(1)]
        [Column(2, Name = "driver_restId")]
        [Column(3, Name = "driver_deviceId")]
        [Column(4, Name = "driver_id")]
        //[Index(5)]
        //[Column(6, Name = "cars_restId")]
        //[Column(7, Name = "cars_deviceId")]
        //[Column(8, Name = "cars_id")]
        [ManyToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(Car))]
        [Column(6, Name = "car_restId")]
        [Column(7, Name = "car_deviceId")]
        [Column(8, Name = "car_id")]
        private ISet<Car> cars = new HashSet<Car>();

        [Property]
        private byte[] binaryData = null;

        [Property]
        private OrderModeEnum orderMode = OrderModeEnum.Asc;

        /// <summary>
        /// Initializes a new instance of the <see cref="Driver"/> class.
        /// </summary>
        public Driver()
        {
            Console.WriteLine(readonlyTest);
        }

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual ISet<Car> Cars
        {
            get { return cars; }
            set { cars = value; }
        }

        public virtual byte[] BinaryData
        {
            get { return binaryData; }
            set { binaryData = value; }
        }

        public virtual OrderModeEnum OrderMode
        {
            get { return orderMode; }
            set { orderMode = value; }
        }

    }

}
