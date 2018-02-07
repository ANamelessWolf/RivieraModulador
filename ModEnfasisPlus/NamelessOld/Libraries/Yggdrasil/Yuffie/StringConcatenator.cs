using System;
using System.Text;

namespace NamelessOld.Libraries.Yggdrasil.Yuffie
{
    /// <summary>
    /// A concanator set of functions, all of this methods are static.
    /// Yuffie has a great set of tools, this tools can add prefix or suffix or just insert text to an existance string.
    /// </summary>
    public static class StringConcatenator
    {
        /// <summary>
        /// Turns a string array into a string. The chars are concatenate in ascending order.
        /// </summary>
        /// <param name="chars">The chars to be concatenate</param>
        /// <returns>The string result</returns>
        public static string GetString(this char[] chars)
        {
            StringBuilder sB = new StringBuilder();
            foreach (Char ch in chars)
                sB.Append(ch);
            return sB.ToString();
        }
        /// <summary>
        /// Turns a string array into a string. The chars are concatenate in ascending order.
        /// Also a character(needle) is inserted between strings
        /// </summary>
        /// <param name="needle">The character inserted between strings</param>
        /// <param name="chars">The chars to be concatenate</param>
        /// <returns>The string result</returns>
        public static string GetString(this char[] chars, char needle)
        {
            StringBuilder sB = new StringBuilder();
            for (int i = 0; i < chars.Length; i++)
            {
                sB.Append(chars[0]);
                if (i < chars.Length - 1)
                    sB.Append(needle);
            }
            return sB.ToString();
        }

        /// <summary>
        /// Add a prefix enumeration to the string
        /// The enumeration is formatted in alphabetical order, adding zeros to the left
        /// </summary>
        /// <param name="str">The string to add the prefix</param>
        /// <param name="index">The index to be added</param>
        /// <param name="size">The size of the enum, adding zeros to match size</param>
        /// <param name="addWhiteSpace">Adds a white space after the enum if true</param>
        /// <returns>The string result</returns>
        public static string AddPrefixEnum(this String str, int index, int size, bool addWhiteSpace = true)
        {
            return String.Format("{0}{1}",
                index.ToAlphabeticalFormat((int)Math.Pow(10, size)) + (addWhiteSpace ? " " : ""), str);
        }
        /// <summary>
        /// Concatenate an array by a give character
        /// </summary>
        /// <param name="str">The string collection</param>
        /// <param name="ch">The separation character</param>
        /// <returns>The string result</returns>
        public static string ConcatenateByCharacter(this String[] collection, char ch)
        {
            if (collection.Length == 0)
                return String.Empty;
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (String s in collection)
                    sb.Append(String.Format("{0}{1}", s, ch));

                return sb.ToString().Substring(0, sb.ToString().Length - 1);
            }
        }

    }
}
