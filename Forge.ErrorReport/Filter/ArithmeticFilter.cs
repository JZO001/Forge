/* *********************************************************************
 * Date: 24 Oct 2013
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System;
using Forge.Configuration.Shared;
using Forge.Reflection;

namespace Forge.ErrorReport.Filter
{

    /// <summary>
    /// Arithmetic filter
    /// </summary>
    [Serializable]
    public class ArithmeticFilter : FilterMemberNameAndValue
    {

        #region Field(s)

        private const string CONFIG_OPERAND = "Operand";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="ArithmeticFilter"/> class.
        /// </summary>
        public ArithmeticFilter()
        {
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the operand.
        /// </summary>
        /// <value>
        /// The operand.
        /// </value>
        public ArithmeticFilterOperandEnum Operand { get; set; }

        #endregion

        #region Public method(s)

        /// <summary>
        /// Initializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Initialize(CategoryPropertyItem item)
        {
            base.Initialize(item);

            this.Operand = ArithmeticFilterOperandEnum.Equal;
            if (item.PropertyItems != null)
            {
                ArithmeticFilterOperandEnum operand = ArithmeticFilterOperandEnum.Equal;
                if (ConfigurationAccessHelper.ParseEnumValue<ArithmeticFilterOperandEnum>(item.PropertyItems, CONFIG_OPERAND, ref operand))
                {
                    this.Operand = operand;
                }
            }

            this.IsInitialized = true;
        }

        /// <summary>
        /// Filters the specified package.
        /// </summary>
        /// <param name="package">The package.</param>
        /// <returns>
        /// True, if the filter criterias matches, otherwise False.
        /// </returns>
        public override bool Filter(ReportPackage package)
        {
            DoInitializationCheck();

            bool result = false;

            IComparable value = (IComparable)ExtractObjectData.Create(this.MemberName).GetValue(package);

            switch (this.Operand)
            {
                case ArithmeticFilterOperandEnum.Equal:
                    {
                        if (value == null && this.Value == null)
                        {
                            result = true;
                        }
                        else if (value == null || this.Value == null)
                        {
                        }
                        else
                        {
                            result = value.Equals((IComparable)Convert.ChangeType(this.Value, value.GetType()));
                        }
                    }
                    break;

                case ArithmeticFilterOperandEnum.NotEqual:
                    {
                        if (value == null && this.Value == null)
                        {
                        }
                        else if (value == null || this.Value == null)
                        {
                            result = true;
                        }
                        else
                        {
                            result = !value.Equals((IComparable)Convert.ChangeType(this.Value, value.GetType()));
                        }
                    }
                    break;

                case ArithmeticFilterOperandEnum.Greater:
                    {
                        if (this.Value == null)
                        {
                            throw new NullReferenceException("Filter value is null");
                        }
                        if (value == null)
                        {
                            throw new NullReferenceException(string.Format("Field or property value is null '{0}'", this.MemberName));
                        }
                        result = value.CompareTo((IComparable)Convert.ChangeType(this.Value, value.GetType())) < 0;
                    }
                    break;

                case ArithmeticFilterOperandEnum.Lower:
                    {
                        if (this.Value == null)
                        {
                            throw new NullReferenceException("Filter value is null");
                        }
                        if (value == null)
                        {
                            throw new NullReferenceException(string.Format("Field or property value is null '{0}'", this.MemberName));
                        }
                        result = value.CompareTo((IComparable)Convert.ChangeType(this.Value, value.GetType())) > 0;
                    }
                    break;

                case ArithmeticFilterOperandEnum.GreaterOrEqual:
                    {
                        if (this.Value == null)
                        {
                            throw new NullReferenceException("Filter value is null");
                        }
                        if (value == null)
                        {
                            throw new NullReferenceException(string.Format("Field or property value is null '{0}'", this.MemberName));
                        }
                        result = value.CompareTo((IComparable)Convert.ChangeType(this.Value, value.GetType())) <= 0;
                    }
                    break;

                case ArithmeticFilterOperandEnum.LowerOrEqual:
                    {
                        if (this.Value == null)
                        {
                            throw new NullReferenceException("Filter value is null");
                        }
                        if (value == null)
                        {
                            throw new NullReferenceException(string.Format("Field or property value is null '{0}'", this.MemberName));
                        }
                        result = value.CompareTo((IComparable)Convert.ChangeType(this.Value, value.GetType())) >= 0;
                    }
                    break;
            }

            return result;
        }

        #endregion

        #region Protected method(s)

        /// <summary>
        /// Documents the initialization check.
        /// </summary>
        /// <exception cref="InitializationException">Member name has not been definied.</exception>
        protected override void DoInitializationCheck()
        {
            base.DoInitializationCheck();
            if (string.IsNullOrEmpty(this.MemberName))
            {
                throw new InitializationException("Member name has not been definied.");
            }
        }

        #endregion

    }

}
