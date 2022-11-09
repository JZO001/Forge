using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.PersistenceCollectionsText.Entities
{
    [Serializable]
    [Class]
    public class PresistenceCacheEntities : EntityBase
    {
        [Property(NotNull = true)]
        private string name = String.Empty;

        [Property(NotNull = false)]
        private int testInt = 0;

        [Property(NotNull = true)]
        private string address = String.Empty;
    
    
        public PresistenceCacheEntities() : base()
        {

        }


        public virtual string Name
        {
            get { return name; }
            set
            {
                OnPropertyChanging("name");
                name = value;
                OnPropertyChanged("name");
            }
        }
    
        public virtual int TestInt
        {
            get { return testInt; }
            set
            {
                OnPropertyChanging("testInt");
                testInt = value;
                OnPropertyChanged("testInt");
            }
        }

        public virtual string Address
        {
            get { return address; }
            set
            {
                OnPropertyChanging("address");
                address = value;
                OnPropertyChanged("address");
            }
        }
    }
}
