using NamelessOld.Libraries.Yggdrasil.Exceptions;
using System;
using System.IO;
using System.Reflection;
using static NamelessOld.Libraries.Yggdrasil.Resources.Errors;
namespace NamelessOld.Libraries.Yggdrasil.Lilith
{
    public class SuccubusAssembly : NamelessObject
    {
        /// <summary>
        /// The assembly file
        /// </summary>
        public AssemblyName Assembly;
        /// <summary>
        /// The assembly file data
        /// </summary>
        public FileInfo File;
        /// <summary>
        /// The assembly file current length
        /// </summary>
        public long Length { get { return this.File.Length; } }
        /// <summary>
        /// The architecture platform
        /// </summary>
        public ProcessorArchitecture Platform { get { return this.Assembly.ProcessorArchitecture; } }
        /// <summary>
        /// The current version of the software.
        /// </summary>
        public new Version Version { get { return this.Assembly.Version; } }
        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        /// <value>
        /// The name of the assembly
        /// </value>
        public String Name
        {
            get { return this.Assembly.Name; }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccubusAssembly"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public SuccubusAssembly(string path)
        {
            this.File = new FileInfo(path);
            this.Assembly = AssemblyName.GetAssemblyName(path);
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(System.Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            SuccubusAssembly assembly = obj as SuccubusAssembly;
            if ((object)assembly == null || !(obj is SuccubusAssembly))
                return false;
            // Return true if the fields match:
            Boolean sameName = this.Name == ((SuccubusAssembly)obj).Name,
                    sameVersion = this.IsSameVersion(this.Version, ((SuccubusAssembly)obj).Version);
            return sameName && sameVersion;
        }
        /// <summary>
        /// Determines whether [is same version] [the specified ver1].
        /// </summary>
        /// <param name="ver1">The ver1.</param>
        /// <param name="ver2">The ver2.</param>
        /// <returns>True if the build is the same version</returns>
        public bool IsSameVersion(Version ver1, Version ver2)
        {
            return ver1.Major == ver2.Major && ver1.Minor == ver2.Minor && ver1.Build == ver2.Build && ver1.Revision == ver2.Revision;
        }
        /// <summary>
        /// Equalses the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>True if both assemblies are equals</returns>
        public bool Equals(SuccubusAssembly assembly)
        {
            // Return true if the fields match:
            return this.Equals((object)assembly);
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="ver1">The first assembly version</param>
        /// <param name="ver2">The second assembly version</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(SuccubusAssembly ver1, SuccubusAssembly ver2)
        {
            // If one is null, but not both, return false.
            if (((object)ver1 == null) || ((object)ver2 == null))
                return false;
            // Return true if the fields match:
            return ver1.Equals(ver2);
        }
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="ver1">The first assembly version</param>
        /// <param name="ver2">The second assembly version</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(SuccubusAssembly ver1, SuccubusAssembly ver2)
        {
            return !(ver1 == ver2);
        }
        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        /// <param name="ver1">The first assembly version</param>
        /// <param name="ver2">The second assembly version</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator >(SuccubusAssembly ver1, SuccubusAssembly ver2)
        {
            if (ver1.Name != ver2.Name)
                throw new NamelessException(AssemblyDifferentName);
            if (ver1.Version.Major > ver2.Version.Major)
                return true;
            else if (ver1.Version.Major == ver2.Version.Major)
            {
                if (ver1.Version.Minor > ver2.Version.Minor)
                    return true;
                else if (ver1.Version.Minor == ver2.Version.Minor)
                {
                    if (ver1.Version.Build > ver2.Version.Build)
                        return true;
                    else if (ver1.Version.Build == ver2.Version.Build)
                    {
                        if (ver1.Version.Revision > ver2.Version.Revision)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        /// <param name="ver2">The first assembly version</param>
        /// <param name="ver1">The second assembly version</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator <(SuccubusAssembly ver1, SuccubusAssembly ver2)
        {
            if (ver1.Name != ver2.Name)
                throw new NamelessException(AssemblyDifferentName);
            if (ver2.Version.Major > ver1.Version.Major)
                return true;
            else if (ver2.Version.Major == ver1.Version.Major)
            {
                if (ver2.Version.Minor > ver1.Version.Minor)
                    return true;
                else if (ver2.Version.Minor == ver1.Version.Minor)
                {
                    if (ver2.Version.Build > ver1.Version.Build)
                        return true;
                    else if (ver2.Version.Build == ver1.Version.Build)
                    {
                        if (ver2.Version.Revision > ver1.Version.Revision)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0}, Version= {1}", this.Name, this.Version);
        }

    }
}
