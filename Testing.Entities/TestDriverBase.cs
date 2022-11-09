using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    [Serializable]
    [Class]
    public class TestDriverBase : EntityBase
    {
        #region Field(s)

        [Property]
        private string name = string.Empty;

        [Property]
        private int zipCode = 0;

        [ManyToOne(0, Name = "tCar", ClassType = typeof(TestCarBase), Cascade = "none", Unique = true)]
        [Column(1, Name = "tCar_restId")]
        [Column(2, Name = "tCar_deviceId")]
        [Column(3, Name = "tCar_id")]
        private TestCarBase tCar = null;

        [Property]
        private int age = 0;

        [Property]
        private long value = 0L;

        [Set(0, Name = "testCars", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "TestCarBase_TestDriverBase_Switch", Inverse = true)]
        [Key(1)]
        [Column(2, Name = "testDrivers_restId")]
        [Column(3, Name = "testDrivers_deviceId")]
        [Column(4, Name = "testDrivers_id")]
        [ManyToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(TestCarBase))]
        [Column(6, Name = "testCars_restId")]
        [Column(7, Name = "testCars_deviceId")]
        [Column(8, Name = "testCars_id")]
        private ISet<TestCarBase> testCars = new HashSet<TestCarBase>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDriverBase" /> class.
        /// </summary>
        public TestDriverBase()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        /// <value>
        /// The zip code.
        /// </value>
        public virtual int ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        /// <summary>
        /// Gets or sets the T car.
        /// </summary>
        /// <value>
        /// The T car.
        /// </value>
        public virtual TestCarBase TCar
        {
            get { return tCar; }
            set { tCar = value; }
        }

        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>
        /// The age.
        /// </value>
        public virtual int Age
        {
            get { return age; }
            set { age = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public virtual long Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets or sets the test cars.
        /// </summary>
        /// <value>
        /// The test cars.
        /// </value>
        public virtual ISet<TestCarBase> TestCars
        {
            get { return testCars; }
            set { testCars = value; }
        }

        #endregion
    }
}
