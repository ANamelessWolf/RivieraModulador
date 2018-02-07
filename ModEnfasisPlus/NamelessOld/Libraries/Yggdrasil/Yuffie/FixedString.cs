using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Yuffie
{
    public static class FixedString
    {

        /// <summary>
        /// Remove the specific characters from the string
        /// </summary>
        /// <param name="charsToBeRemoved">The characters to be removed</param>
        /// <returns>The string with the removed characters.</returns>
        public static String Remove(this String str, params Char[] charsToBeRemoved)
        {
            string cleanString = String.Empty;
            foreach (char ch in str)
                if (!charsToBeRemoved.Contains(ch))
                    cleanString += ch;
            return cleanString;
        }
        /// <summary>
        /// Remove the white spaces from a string
        /// </summary>
        /// <param name="charsToBeRemoved">The characters to be removed</param>
        /// <returns>The string with the removed characters.</returns>
        public static String RemoveWhiteSpaces(this String str)
        {
            return str.Replace(" ", "");
        }
        /// <summary>
        /// Reverse the string
        /// </summary>
        /// <param name="str">The string to be reversed</param>
        /// <returns>The string reversed</returns>
        public static String Reverse(this String str)
        {
            String s = string.Empty;
            for (int i = str.Length - 1; i <= 0; i--)
                s += str[i];
            return s;
        }
        /// <summary>
        /// This function gets a substring from the first string between quotation marks.
        /// If the string doesn't had quotation marks, the string result is an empty string.
        /// </summary>
        /// <param name="str">The string to search</param>
        /// <returns>The substring in quotation marks.</returns>
        public static string FirtsInQuotation(this string str)
        {
            string word = "";
            if (str.Contains("\""))
            {
                int firstComma = str.IndexOf("\"") + 1;
                str = str.Substring(firstComma, str.Length - firstComma);
                int secondComma = str.IndexOf("\"");
                word = str.Substring(0, secondComma);
            }
            return word;
        }
        /// <summary>
        /// Filter the string allowing to pass just the characteres defined on the
        /// string filter
        /// </summary>
        /// <param name="str">The string to filter</param>
        /// <param name="filter">The allowed characters on the filter</param>
        /// <returns>The substring in quotation marks.</returns>
        public static string Filter(this string str, string filter)
        {
            return new String(str.Where(x => filter.Contains(x)).ToArray());
        }
    }
}
