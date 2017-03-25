using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace Bilten.Util
{
    public class SortComparer<T> : IComparer<T>
    {
        private PropertyDescriptor[] propDesc;
        private ListSortDirection[] direction;

        public SortComparer(PropertyDescriptor propDesc, ListSortDirection direction)
        {
			if (propDesc == null)
				throw new ArgumentException();

			this.propDesc = new PropertyDescriptor[1] { propDesc };
			this.direction = new ListSortDirection[1] { direction };
        }

		public SortComparer(PropertyDescriptor[] propDesc, ListSortDirection[] direction)
		{
			if (propDesc.Length != direction.Length || propDesc.Length == 0)
				throw new ArgumentException();
			foreach (PropertyDescriptor pd in propDesc)
			{
				if (pd == null)
					throw new ArgumentException();
			}

			this.propDesc = propDesc;
			this.direction = direction;
        }

        #region IComparer<T> Members

        int IComparer<T>.Compare(T x, T y)
        {
			return RecursiveCompare(x, y, 0);
		}

        #endregion

		private int RecursiveCompare(T x, T y, int index)
		{
			if (index >= propDesc.Length)
				return 0; // termination condition

			object xValue = propDesc[index].GetValue(x);
			object yValue = propDesc[index].GetValue(y);

			int retValue = CompareValues(xValue, yValue, direction[index]);
			if (retValue != 0)
				return retValue;
			else
				return RecursiveCompare(x, y, ++index);
		}

		private int CompareValues(object xValue, object yValue, ListSortDirection direction)
        {
            int retValue = 0;
            if (xValue != null && yValue != null)
            {
                if (xValue is IComparable) // Can ask the x value
                {
                    retValue = ((IComparable)xValue).CompareTo(yValue);
                }
                else if (yValue is IComparable) //Can ask the y value
                {
                    retValue = ((IComparable)yValue).CompareTo(xValue);
                }
                else if (!xValue.Equals(yValue)) // not comparable, compare String representations
                {
                    retValue = xValue.ToString().CompareTo(yValue.ToString());
                }
            }
            // NOTE: Primetiti da u slucajevima kada je jedan od elemenata null odmah
            // vracam vrednost (ne vrsim invertovanje na osnovu smera sortiranja)
            // i to tako da je element koji nije null uvek ispred elementa koji
            // je null (i za Ascending i za Descending)
            else if (xValue != null) // yValue == null
            {
                return -1;
            }
            else if (yValue != null) // xValue == null
            {
                return 1;
            }

            if (direction == ListSortDirection.Ascending)
                return retValue;
            else
                return -retValue;
        }
    }
}
