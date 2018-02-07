using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Yuffie
{
    public static class FormattedString
    {
        /// <summary>
        /// Gets the invalid characters for a file.
        /// </summary>
        public static Char[] InvalidXmlNodeCharacters { get { return CommonStrings.InvalidXmlNodeNameCharacters.ToCharArray(); } }
        /// <summary>
        /// Remove unsopported characters for a xml node name
        /// </summary>
        /// <returns>A valid name.</returns>
        public static String ToXmlName(this String str)
        {
            String xmlName = str.Replace(" ", "");
            return str.RemoveWhiteSpaces().Remove(InvalidXmlNodeCharacters);
        }

        /// <summary>
        /// Get a substring of the first N characters.
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="length">The string lenghth</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(this string str, int length)
        {
            if (str.Length > length)
                str = str.Substring(0, length);
            return str;
        }
        /// <summary>
        /// Get a substring of the first N characters,
        /// the last 3 characters are always "..."
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="length">The string lenghth</param>
        /// <returns>The truncated string</returns>
        public static String ToSuspensionPointString(this String str, int length)
        {
            if (str.Length > length - 3)
                return String.Format("{0}...", str.Truncate(length - 3));
            else if (str.Length > 3)
                return String.Format("{0}...", str.Truncate(str.Length - 3));
            else
                return "...";
        }

        /// <summary>
        /// Put a number in string with zeros depending on the max number.
        /// Example
        /// max = 100, format 000, 
        /// max = 30000, format 00000
        /// </summary>
        /// <param name="number">The number to be formatted.</param>
        /// <param name="maxNumber">The count of numbers to be renamed</param>
        /// <returns>The number in an alphabetical format.</returns>
        public static string ToAlphabeticalFormat(this int number, int maxNumber)
        {
            String zeros = String.Empty, 
                   numStr = number.ToString();
            while (zeros.Length != maxNumber.ToString().Length)
                zeros += "0";
            return String.Format("{0:" + zeros + "}", number);
        }
        /// <summary>
        /// Gets the line formatted from a copied line from
        /// wikipedia
        /// Must follow this rules
        /// First digit must be a number
        /// The string must has words between quotations marks
        /// </summary>
        /// <param name="str">The string to be formatted</param>
        /// <returns>The formatted string</returns>
        public static String ToWikiStyle(this string str)
        {
            if (str[0].IsInt())
                return str.FirtsInQuotation();
            else
                return String.Empty;
        }

    }
}
