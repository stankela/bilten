using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Bilten
{
    public class RounderToZero
    {
        //TODO2: kada se izracunava ocena za preskok kod koje je jedna penalizacija zadata
        //a druga nije, greskom se dopisuje druga penalizacija da ima vrednost 0.0.

        public static decimal round(decimal value, int digits)
        {
            string valueStr = value.ToString();
            if (isFractional(valueStr) && isHalf(valueStr, digits))
                return Decimal.Parse(truncateHalf(valueStr));
            else
                return Math.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        private static bool isFractional(string s)
        {
            string decSeparator =
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            return s.IndexOf(decSeparator) != -1;
        }

        private static bool isHalf(string value, int digits)
        {
            string fractionalString = getFractionalString(value);
            if (fractionalString.Length <= digits)
                return false;
            if (fractionalString[digits] != '5')
                return false;
            for (int i = digits + 1; i < fractionalString.Length; i++)
            {
                if (fractionalString[i] != '0')
                    return false;
            }
            return true;
        }

        private static string truncateHalf(string value)
        {
            while (value[value.Length - 1] == '0')
                value = value.Remove(value.Length - 1);
            if (value[value.Length - 1] != '5')
                throw new Exception("Greska u klasi RounderToZero");
            return value.Remove(value.Length - 1);
        }

        private static string getFractionalString(string s)
        {
            string decSeparator =
                CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string[] parts = s.Split(new string[1] { decSeparator }, StringSplitOptions.None);
            return parts[1].Trim();
        }

    }
}
