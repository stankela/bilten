using System;
using System.Collections.Generic;
using System.Text;

namespace Bilten
{
    public class StringUtil
    {
        public static string getListString(object[] items)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object item in items)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }

    }
}
