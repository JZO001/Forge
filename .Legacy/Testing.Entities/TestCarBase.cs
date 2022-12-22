using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.Entities
{
    [Serializable]
    [Class]
    public class TestCarBase : EntityBase
    {
        #region Field(s)

        [Property]
        private int age = 0;

        [Property]
        private string model = string.Empty;

        [Property]
        private int hp = 0;

        [Property]
        private int speed = 0;

        [OneToOne(Name = "tDriver", ClassType = typeof(TestDriverBase), Cascade = "none", Constrained = false, PropertyRef = "tCar")]
        private TestDriverBase tDriver = null;

        [Set(0, Name = "testDrivers", Cascade = "none", Generic = true, Lazy = CollectionLazy.True, Table = "TestCarBase_TestDriverBase_Switch")]
        [Key(1)]
        [Column(2, Name = "testCars_restId")]
        [Column(3, Name = "testCars_deviceId")]
        [Column(4, Name = "testCars_id")]
        [ManyToMany(5, NotFound = NotFoundMode.Exception, ClassType = typeof(TestDriverBase))]
        [Column(6, Name = "testDrivers_restId")]
        [Column(7, Name = "testDrivers_deviceId")]
        [Column(8, Name = "testDrivers_id")]
        private ISet<TestDriverBase> testDrivers = new HashSet<TestDriverBase>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCarBase" /> class.
        /// </summary>
        public TestCarBase()
        {

        }

        #endregion

        #region Properties

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
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public virtual string Model
        {
            get { return model; }
            set { model = value; }
        }

        /// <summary>
        /// Gets or sets the hp.
        /// </summary>
        /// <value>
        /// The hp.
        /// </value>
        public virtual int Hp
        {
            get { return hp; }
            set { hp = value; }
        }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        public virtual int Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        /// <summary>
        /// Gets or sets the T driver.
        /// </summary>
        /// <value>
        /// The T driver.
        /// </value>
        public virtual TestDriverBase TDriver
        {
            get { return tDriver; }
            set { tDriver = value; }
        }

        /// <summary>
        /// Gets or sets the test drivers.
        /// </summary>
        /// <value>
        /// The test drivers.
        /// </value>
        public virtual ISet<TestDriverBase> TestDrivers
        {
            get { return testDrivers; }
            set { testDrivers = value; }
        }

        #endregion

    }
}
