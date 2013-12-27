/* *********************************************************************
 * Date: 25 Apr 2012
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.ORM.NHibernateExtension.Model;

namespace Forge.ORM.NHibernateExtension.Criterias
{

    /// <summary>
    /// Represents a criteria which used to examine an entity with reflection.
    /// </summary>
    public interface IReflectionCriteria : ICloneable
    {

        /// <summary>
        /// Examine criteria match on the provided entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        bool ResultForEntity(EntityBaseWithoutId entity);

    }

}
