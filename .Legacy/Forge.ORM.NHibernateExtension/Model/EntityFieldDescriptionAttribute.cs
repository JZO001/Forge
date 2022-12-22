/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;

namespace Forge.ORM.NHibernateExtension.Model
{

    /// <summary>
    /// Represents the description of an entity field. For example an UI editor use this information to display user-friendly information.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EntityFieldDescriptionAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFieldDescriptionAttribute"/> class.
        /// </summary>
        public EntityFieldDescriptionAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFieldDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        public EntityFieldDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

    }

}
