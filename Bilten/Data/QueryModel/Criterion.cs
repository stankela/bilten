using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten.Data.QueryModel
{
    public class Criterion
    {
        private string propertyName;
        private object value;
        private CriteriaOperator @operator;
        private StringMatchMode matchMode;
        private bool caseInsensitive;

        /// <summary>
        /// Initializes a new instance of the Criterion class.
        /// </summary>
        public Criterion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Criterion class that uses the property name, the value and the operator of the new Criterion.
        /// </summary>
        /// <param name="propertyName">The name of the property to map.</param>
        /// <param name="operator">The operator of the new Criterion object.</param>
        /// <param name="value">The value of the new Criterion object.</param>
        public Criterion(string propertyName, CriteriaOperator @operator, object value)
            : this()
        {
            this.propertyName = propertyName;
            this.value = value;
            this.@operator = @operator;
        }

        public Criterion(string propertyName, CriteriaOperator @operator, object value,
            StringMatchMode matchMode, bool caseInsensitive)
            : this(propertyName, @operator, value)
        {
            if (@operator != CriteriaOperator.Like && @operator != CriteriaOperator.NotLike)
                throw new ArgumentException();
            this.matchMode = matchMode;
            this.caseInsensitive = caseInsensitive;
        }

        /// <summary>
        /// Gets or sets the name of the property to be used by the criterion
        /// </summary>
        public string PropertyName
        {
            get
            {
                return propertyName;
            }
            set
            {
                propertyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the criterion
        /// </summary>
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator of the criterion
        /// </summary>
        public CriteriaOperator Operator
        {
            get
            {
                return @operator;
            }
            set
            {
                @operator = value;
            }
        }

        public StringMatchMode MatchMode
        {
            get { return matchMode; }
            set { matchMode = value; }
        }

        public bool CaseInsensitive
        {
            get { return caseInsensitive; }
            set { caseInsensitive = value; }
        }
    }
}
