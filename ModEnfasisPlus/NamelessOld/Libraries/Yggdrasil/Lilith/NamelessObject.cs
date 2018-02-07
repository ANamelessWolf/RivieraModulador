using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Assembly = System.Reflection.Assembly;
namespace NamelessOld.Libraries.Yggdrasil.Lilith
{
    public class NamelessObject
    {
        #region Propiedades
        /// <summary>
        /// Meet the maker
        /// The Nameless Author, the API maker.
        /// </summary>
        public String Author { get { return "Ing. Miguel Ángel Alanis Montes"; } }
        /// <summary>
        /// The Nameless Company
        /// </summary>
        public String Company { get { return "Nameless Studios"; } }
        /// <summary>
        /// The namespace of the current project.
        /// </summary>
        public String Namespace { get { return (this.type).FullName.Replace("." + this.Class, ""); } }
        /// <summary>
        /// The class name that generate the exception.
        /// </summary>
        public String Class { get { return (this.type).Name; } }
        /// <summary>
        /// The class name that generate the exception.
        /// </summary>
        public String API { get { return Assembly.GetAssembly(this.type).GetName().Name; } }
        /// <summary>
        /// The method or function that is currently running
        /// </summary>
        public String MethodName { get { return GetCurrentMethod(); } }
        /// <summary>
        /// Gets the compilation date
        /// </summary>
        public String CompileDate { get { return new FileInfo(Assembly.GetAssembly(this.type).Location).LastWriteTime.ToShortDateString(); } }
        /// <summary>
        /// The current version of the software.
        /// </summary>
        public String Version { get { return Assembly.GetAssembly(this.type).GetName().Version.ToString(4); } }
        /// <summary>
        /// Gets the Nameless Data
        /// </summary>
        public String NamelessData { get { return this.ToString(); } }
        /// <summary>
        /// Gets the id for the given Object
        /// </summary>
        public Guid Id;
        /// <summary>
        /// Gets the object type
        /// </summary>
        private Type type;
        #endregion
        #region Constructor
        /// <summary>
        /// Creates a new nameless object
        /// </summary>
        /// <param name="langle">The current nameless object language</param>
        public NamelessObject()
        {
            this.type = this.GetType();
            this.Id = new Guid();
        }
        /// <summary>
        /// Creates a new nameless object
        /// </summary>
        /// <param name="type">The type of nameless Object</param>
        public NamelessObject(Type type)
        {
            this.type = type;
            this.Id = new Guid();
        }
        #endregion
        #region Actions
        /// <summary>
        /// Access the name of the current Method
        /// </summary>
        /// <returns>The current method name</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            return sf.GetMethod().Name;
        }
        /// <summary>
        /// Gets the nameless data on a String format
        /// </summary>
        /// <returns>The nameless data string</returns>
        public override string ToString()
        {
            return String.Format(Resources.Notices.NamelessData,
                                 this.API,
                                 this.Version,
                                 this.Namespace,
                                 this.Class,
                                 this.CompileDate,
                                 this.Author,
                                 this.Company,
                                  DateTime.Now.Year);
        }

        #endregion
    }
}
