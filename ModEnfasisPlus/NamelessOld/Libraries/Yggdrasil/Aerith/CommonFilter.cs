using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    public class CommonFilter : AerithFilter
    {
        #region Date Rules
        /// <summary>
        /// Sets or Get the a date rule for files created before this date.
        /// </summary>
        public DateTime CreatedBefore;
        /// <summary>
        /// Sets or Get the a date rule for files created after this date.
        /// </summary>
        public DateTime CreatedAfter;
        /// <summary>
        /// Sets or Get the a date rule for files last access before this date.
        /// </summary>
        public DateTime LastAccessAfter;
        /// <summary>
        /// Sets or Get the a date rule for files last access after this date.
        /// </summary>
        public DateTime LastAccessBefore;
        /// <summary>
        /// Sets or Get the a date rule for files last written before this date.
        /// </summary>
        public DateTime LastWriteBefore;
        /// <summary>
        /// Sets or Get the a date rule for files last written after this date.
        /// </summary>
        public DateTime LastWriteAfter;
        #endregion
        #region Size Rules
        /// <summary>
        /// Sets or Get the a size rule for files greater than this size.
        /// </summary>
        public AerithSize FileSizeGreaterThan;
        /// <summary>
        /// Sets or Get the a size rule for files lower than this size.
        /// </summary>
        public AerithSize FileSizeLowerThan;
        #endregion
        #region NameProperties
        /// <summary>
        /// The name of the parent directory is like this name, it use the property DirectoryInfo.Name
        /// Empty String will ignore this rule
        /// Use regular expression format from class System.Text.RegularExpressions.Regex
        /// </summary>
        public String ParentNameLike;
        /// <summary>
        /// The file name has to be like this name, it use the property FileInfo.Name
        /// Empty String will ignore this rule
        /// Use regular expression format from class System.Text.RegularExpressions.Regex
        /// </summary>
        public String FileNameLike;
        /// <summary>
        /// The directory name has to be like this name, it use the property DirectoryInfo.Name
        /// Empty String will ignore this rule
        /// Use regular expression format from class System.Text.RegularExpressions.Regex
        /// </summary>
        public String DirectoryNameLike;
        #endregion
        #region Extensions Properties
        /// <summary>
        /// Define a group of extensions that are valid in the file
        /// The '.' character is not needed to define the extension
        /// Its not case sensitive
        /// </summary>
        public String[] AllowedExtensions;
        #endregion
        /// <summary>
        /// Creates a common filter
        /// </summary>
        public CommonFilter()
        {
            this.CreatedAfter = DateTime.MinValue;
            this.LastAccessAfter = DateTime.MinValue;
            this.LastWriteAfter = DateTime.MinValue;
            this.CreatedBefore = DateTime.MaxValue;
            this.LastAccessBefore = DateTime.MaxValue;
            this.LastWriteBefore = DateTime.MaxValue;
            this.FileNameLike = String.Empty;
            this.DirectoryNameLike = String.Empty;
            this.ParentNameLike = String.Empty;
            this.FileSizeLowerThan = new AerithSize(long.MaxValue);
            this.FileSizeGreaterThan = new AerithSize(0);
            this.AllowedExtensions = new String[0];
        }
        /// <summary>
        /// Creates a common filter
        /// </summary>
        public CommonFilter(params String[] allowedExtensions)
            : this()
        {
            this.AllowedExtensions = allowedExtensions;
        }
        /// <summary>
        /// Validates if the file is valid in the current filter
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <returns>True if the file is allowed in the filter</returns>
        public override bool IsFileValid(FileInfo file)
        {
            Boolean dateRule =
                file.CreationTime <= this.CreatedBefore && file.CreationTime >= this.CreatedAfter &&
                file.LastAccessTime <= this.LastAccessBefore && file.LastAccessTime >= this.LastAccessAfter &&
                file.LastWriteTime <= this.LastWriteBefore && file.LastWriteTime >= this.LastWriteAfter,
                sizeRule = file.Length <= FileSizeLowerThan.Length && file.Length >= FileSizeGreaterThan.Length,
                validExtensions = this.AllowedExtensions.Length == 0 ? true : this.AllowedExtensions.Select<String, String>(x => x.ToUpper()).Contains(file.Extension.Replace(".", "").ToUpper()),
                regexRule = true;

            Regex r;
            if (this.ParentNameLike != String.Empty)
            {
                r = new Regex(this.ParentNameLike);
                regexRule = regexRule && r.IsMatch(file.Directory.Name);
            }
            if (this.FileNameLike != String.Empty)
            {
                r = new Regex(this.FileNameLike);
                regexRule = regexRule && r.IsMatch(file.Name);
            }
            return dateRule && sizeRule && validExtensions && regexRule;
        }
        /// <summary>
        /// Validates if the directory is valid
        /// </summary>
        /// <param name="directory">The directory to validate</param>
        /// <returns>True if the directory is allowed in the filter</returns>
        public override bool IsDirectoryValid(DirectoryInfo directory)
        {
            Boolean dateRule =
                directory.CreationTime <= this.CreatedBefore && directory.CreationTime >= this.CreatedAfter &&
                directory.LastAccessTime <= this.LastAccessBefore && directory.LastAccessTime >= this.LastAccessAfter &&
                directory.LastWriteTime <= this.LastWriteBefore && directory.LastWriteTime >= this.LastWriteAfter, regexRule = true;
            Regex r;
            if (this.ParentNameLike != String.Empty)
            {
                r = new Regex(this.ParentNameLike);
                regexRule = regexRule && r.IsMatch(directory.Parent.Name);
            }
            if (this.DirectoryNameLike != String.Empty)
            {
                r = new Regex(this.DirectoryNameLike);
                regexRule = regexRule && r.IsMatch(directory.Name);
            }
            return dateRule && regexRule;
        }
    }
}
