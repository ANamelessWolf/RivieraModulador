using NamelessOld.Libraries.Yggdrasil.Lilith;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Yuffie
{
    public static class StringValidator
    {
        /// <summary>
        /// Validate a string as an int number
        /// </summary>
        /// <param name="str">The string to be validated</param>
        /// <returns>True if the string is an int number</returns>
        public static Boolean IsInt(this String str)
        {
            int num;
            return int.TryParse(str, out num);
        }
        /// <summary>
        /// Validate a char as an int number
        /// </summary>
        /// <param name="str">The string to be validated</param>
        /// <returns>True if the string is an int number</returns>
        public static Boolean IsInt(this Char ch)
        {
            return IsInt(ch.ToString());
        }

        /// <summary>
        /// Validate a string as an IP adress
        /// The ip address is valid from [0.0.0.0]-[255.255.255.255]
        /// </summary>
        /// <param name="str">The string to be validated</param>
        /// <returns>True if the string is a valid ip adress</returns>
        public static Boolean IsIPAddress(this String str)
        {
            StringBuilder sbRegex = new StringBuilder();
            sbRegex.Append("^(");                //Inicio
            sbRegex.Append("(");                 //Inicio de definición 3 primeros octetos de red
            sbRegex.Append("([0-9])");           //Valores de 0-9
            sbRegex.Append('|');                 //OR
            sbRegex.Append("([1-9][0-9])");      //Valores de 10-99
            sbRegex.Append('|');                 //OR
            sbRegex.Append("(1[0-9][0-9])");     //Valores de 100-199
            sbRegex.Append('|');                 //OR
            sbRegex.Append("(2[0-4][0-9])");     //Valores de 200-249
            sbRegex.Append('|');                 //OR
            sbRegex.Append("(25[0-5])");         //Valores de 250-255
            sbRegex.Append(@")\.){3}");             //Fin de definición 3 primeros octetos de red
            sbRegex.Append("(");                 //Inici de definición último octeto de red
            sbRegex.Append("([0-9])");           //Valores de 1-9
            sbRegex.Append('|');                 //OR
            sbRegex.Append("([1-9][0-9])");      //Valores de 10-99
            sbRegex.Append('|');                 //OR
            sbRegex.Append("(1[0-9][0-9])");     //Valores de 100-199
            sbRegex.Append('|');                 //OR
            sbRegex.Append("(2[0-4][0-9])");     //Valores de 200-249
            sbRegex.Append('|');                 //OR
            sbRegex.Append("(25[0-5])");         //Valores de 250-255
            sbRegex.Append(")");                 //Fin de definición último octeto de red
            sbRegex.Append("$");                //FIN
            return Regex.IsMatch(str, sbRegex.ToString());
        }


    }
}
