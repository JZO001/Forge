using System;
using NHibernate.Mapping.Attributes;
using Forge.ORM.NHibernateExtension.Model.Distributed;

namespace Forge.Testing.PersistenceCollectionsText.Entities
{
    [Serializable]
    [Class]
    public class PersistentDictionaryEntity : EntityBase
    {
        [Property(NotNull = true)]
        private string name = String.Empty;

        [Property(NotNull = false)]
        private int testInt = 0;



        public PersistentDictionaryEntity() : base()
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
    }
}
