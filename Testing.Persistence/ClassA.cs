using System;

namespace Testing.Persistence
{

    /// <summary>
    /// Test class
    /// </summary>
    [Serializable]
    public class ClassA : ClassBase
    {

        private DateTime mDate = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassA"/> class.
        /// </summary>
        public ClassA()
        {
        }

        /// <summary>
        /// Gets or sets the method A.
        /// </summary>
        /// <value>
        /// The method A.
        /// </value>
        public string MethodA { get; set; }

        /// <summary>
        /// Gets or sets the method B.
        /// </summary>
        /// <value>
        /// The method B.
        /// </value>
        public string MethodB { get; set; }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <returns></returns>
        public DateTime GetDate()
        {
            return mDate;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!obj.GetType().Equals(GetType()))
            {
                return false;
            }
            if (base.Equals(obj))
            {
                return true;
            }
            ClassA other = (ClassA)obj;
            return GetDate().Equals(other.GetDate()) && MethodA == other.MethodA && MethodB == other.MethodB;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
