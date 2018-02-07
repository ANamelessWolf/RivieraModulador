using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.IO;
namespace NamelessOld.Libraries.Yggdrasil.Aerith
{
    public class FileScanner : NamelessObject
    {
        delegate void VoidScannerAction();
        delegate Object ScannerAction();
        VoidScannerAction VoidScanAction;
        ScannerAction ScanAction;
        /// <summary>
        /// Get the files found in the scanner
        /// Find method must be run before accessing its value
        /// </summary>
        public FileInfo[] Files;
        /// <summary>
        /// Get the directories found in the scanner
        /// Find method must be run before accessing its value
        /// </summary>
        public DirectoryInfo[] Directories;
        /// <summary>
        /// The total number of files
        /// </summary>
        public int FileCount { get { return Files.Length; } }
        /// <summary>
        /// The total number of directories
        /// </summary>
        public int DirectoryCount { get { return Files.Length; } }
        /// <summary>
        /// If true the scanner search in recursive deep search
        /// </summary>
        public Boolean DeepSearch;
        /// <summary>
        /// If true the scanner will ommit errors during the search
        /// Errors like accessing protected directories
        /// </summary>
        public Boolean OmitErrors;
        /// <summary>
        /// Get or Sets the current root directory
        /// </summary>
        public DirectoryInfo RootDirectory;
        /// <summary>
        /// Creates a new file scanner, the scanner works by selecting
        /// a root folder
        /// </summary>
        /// <param name="dir">The root directory</param>
        /// <param name="deepSearch">True if the scanner will work in deep search</param>
        /// <param name="omitErrors">if set to <c>true</c> [omit errors].</param>
        /// <exception cref="BlackMateriaException">The root directory does not exists</exception>
        public FileScanner(DirectoryInfo dir, Boolean deepSearch = false, Boolean omitErrors = false)
        {
            this.DeepSearch = deepSearch;
            this.RootDirectory = dir;
            this.OmitErrors = omitErrors;
            if (!Directory.Exists(dir.FullName))
                throw new BlackMateriaException(String.Format(Errors.RootDirectoryMissing, dir.FullName));
        }
        /// <summary>
        /// Creates a new file scanner, the scanner works by selecting
        /// a root folder
        /// </summary>
        /// <param name="dirPth">The path of the root directory</param>
        /// <param name="deepSearch">True if the scanner will work in deep search</param>
        public FileScanner(String dirPth, Boolean deepSearch = false, Boolean omitErrors = false) :
            this(new DirectoryInfo(dirPth), deepSearch, omitErrors)
        {

        }
        /// <summary>
        /// Find all the files and directories from the root directory
        /// If deep search is enable retrieves all sub folders and all files contained in 
        /// the root directory.
        /// </summary>
        public void Find()
        {
            this.VoidScanAction = delegate ()
            {
                if (DeepSearch)
                {
                    List<FileInfo> files = new List<FileInfo>();
                    List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                    this.Scan(ref files, ref dirs, this.RootDirectory);
                    this.Files = files.ToArray();
                    this.Directories = dirs.ToArray();
                }
                else
                {
                    this.Files = this.RootDirectory.GetFiles();
                    this.Directories = this.RootDirectory.GetDirectories();
                }
            };
            this.ScannerVoidTransaction();
        }
        /// <summary>
        /// Find all the files and directories from the root directory
        /// If deep search is enable retrieves all sub folders and all files contained in 
        /// the root directory.
        /// </summary>
        /// <param name="filter">Adds a filter to the search</param>
        public void Find(AerithFilter filter)
        {
            this.VoidScanAction = delegate ()
            {
                if (DeepSearch)
                {
                    List<FileInfo> files = new List<FileInfo>();
                    List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                    this.Scan(ref files, ref dirs, filter, this.RootDirectory);
                    this.Files = files.ToArray();
                    this.Directories = dirs.ToArray();
                }
                else
                {
                    this.Files = this.RootDirectory.GetFiles();
                    this.Directories = this.RootDirectory.GetDirectories();
                }
            };
            this.ScannerVoidTransaction();
        }
        /// <summary>
        /// Excecutes a full scan without a filter
        /// </summary>
        /// <param name="extensions">The list of found extensions</param>
        /// <param name="size">The search Byte size</param>
        public void FullScan(out String[] extensions, out AerithSize size)
        {
            this.ScanAction = delegate ()
            {
                List<String> exts = new List<string>();
                long totalSize = 0;
                if (DeepSearch)
                {
                    List<FileInfo> files = new List<FileInfo>();
                    List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                    this.Scan(ref files, ref dirs, ref exts, ref totalSize, this.RootDirectory);
                    this.Files = files.ToArray();
                    this.Directories = dirs.ToArray();
                }
                else
                {
                    this.Files = this.RootDirectory.GetFiles();
                    this.Directories = this.RootDirectory.GetDirectories();
                }
                return new Object[] { exts.ToArray(), new AerithSize(totalSize) };
            };
            Object[] res = this.ScannerTransaction() as Object[];
            extensions = res[0] as String[];
            size = res[1] as AerithSize;
        }
        /// <summary>
        /// Excecutes a full scan without a filter
        /// </summary>
        /// <param name="filter">Adds a filter to the search</param>
        /// <param name="extensions">The list of found extensions</param>
        /// <param name="size">The search Byte size</param>
        public void FullScan(AerithFilter filter, out String[] extensions, out AerithSize size)
        {
            this.ScanAction = delegate ()
            {
                List<String> exts = new List<string>();
                long totalSize = 0;
                if (DeepSearch)
                {
                    List<FileInfo> files = new List<FileInfo>();
                    List<DirectoryInfo> dirs = new List<DirectoryInfo>();
                    this.Scan(ref files, ref dirs, ref exts, ref totalSize, filter, this.RootDirectory);
                    this.Files = files.ToArray();
                    this.Directories = dirs.ToArray();
                }
                else
                {
                    this.Files = this.RootDirectory.GetFiles();
                    this.Directories = this.RootDirectory.GetDirectories();
                }
                return new Object[] { exts.ToArray(), new AerithSize(totalSize) };
            };
            Object[] res = this.ScannerTransaction() as Object[];
            extensions = res[0] as String[];
            size = res[1] as AerithSize;
        }
        /// <summary>
        /// Get the size of the directory
        /// </summary>
        /// <returns>Get the current directory size in Aerith format</returns>
        public AerithSize GetSize()
        {
            this.ScanAction = delegate ()
            {
                long size = 0;
                this.Scan(ref size, this.RootDirectory);
                return new AerithSize(size);
            };
            return this.ScannerTransaction() as AerithSize;
        }

        /// <summary>
        /// Get the type of extensions defined on the directory
        /// </summary>
        /// <returns>The collection of extensions found in the roor directory</returns>
        public String[] GetExtensions()
        {
            this.ScanAction = delegate ()
            {
                List<String> extensions = new List<String>();
                this.Scan(ref extensions, this.RootDirectory);
                return extensions;
            };
            return (this.ScannerTransaction() as List<String>).ToArray();
        }
        /// <summary>
        /// Scan for files and directories
        /// </summary>
        /// <param name="files">The list of found files</param>
        /// <param name="dirs">The list of directories</param>
        /// <param name="rootDir">The search </param>
        void Scan(ref List<FileInfo> files, ref List<DirectoryInfo> dirs, DirectoryInfo rootDir)
        {
            if (this.OmitErrors)
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    try
                    {
                        dirs.Add(dir);
                        this.Scan(ref files, ref dirs, dir);
                    }
                    catch (Exception) { }
                }
                foreach (FileInfo file in rootDir.GetFiles())
                    try
                    {
                        files.Add(file);
                    }
                    catch (Exception) { }
            }
            else
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    dirs.Add(dir);
                    this.Scan(ref files, ref dirs, dir);
                }
                foreach (FileInfo file in rootDir.GetFiles())
                    files.Add(file);
            }
        }
        /// <summary>
        /// Scan for files and directories using a filter
        /// </summary>
        /// <param name="files">The list of found files</param>
        /// <param name="dirs">The list of directories</param>
        /// <param name="rootDir">The search </param>
        private void Scan(ref List<FileInfo> files, ref List<DirectoryInfo> dirs, AerithFilter filter, DirectoryInfo rootDir)
        {
            if (OmitErrors)
            {
                foreach (FileInfo file in rootDir.GetFiles())
                    try
                    {
                        if (filter.IsFileValid(file))
                            files.Add(file);
                    }
                    catch (Exception) { }
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    try
                    {
                        if (filter.IsDirectoryValid(dir))
                            dirs.Add(dir);
                        this.Scan(ref files, ref dirs, filter, dir);
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                foreach (FileInfo file in rootDir.GetFiles())
                    if (filter.IsFileValid(file))
                        files.Add(file);
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    if (filter.IsDirectoryValid(dir))
                        dirs.Add(dir);
                    this.Scan(ref files, ref dirs, filter, dir);
                }
            }

        }
        /// <summary>
        /// Get the size of the root directory
        /// </summary>
        /// <param name="size">The current root directory size</param>
        /// <param name="rootDir">The root directory to search file</param>
        void Scan(ref long size, DirectoryInfo rootDir)
        {
            if (OmitErrors)
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                    try
                    {
                        this.Scan(ref size, dir);
                    }
                    catch (Exception) { }

                foreach (FileInfo file in rootDir.GetFiles())
                    try
                    {
                        size += file.Length;
                    }
                    catch (Exception) { }
            }
            else
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                    this.Scan(ref size, dir);

                foreach (FileInfo file in rootDir.GetFiles())
                    size += file.Length;
            }
        }
        /// <summary>
        /// Get the list of extension files found in the root directory
        /// </summary>
        /// <param name="extensions">The current list of extensions</param>
        /// <param name="rootDir">The root directory to search file</param>
        void Scan(ref List<string> extensions, DirectoryInfo rootDir)
        {
            String ext;
            if (OmitErrors)
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                    try
                    {
                        this.Scan(ref extensions, dir);
                    }
                    catch (Exception) { }
                foreach (FileInfo file in rootDir.GetFiles())
                {
                    try
                    {
                        ext = file.Extension.Replace(".", "").ToUpper();
                        if (!extensions.Contains(ext))
                            extensions.Add(ext);
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                    this.Scan(ref extensions, dir);
                foreach (FileInfo file in rootDir.GetFiles())
                {
                    ext = file.Extension.Replace(".", "").ToUpper();
                    if (!extensions.Contains(ext))
                        extensions.Add(ext);
                }
            }
        }
        /// <summary>
        /// Full Scan for files and directories
        /// </summary>
        /// <param name="files">The list of found files</param>
        /// <param name="dirs">The list of directories</param>
        /// <param name="extensions">The current list of extensions</param>
        /// <param name="size">The current root directory size</param>
        /// <param name="rootDir">The search </param>
        private void Scan(ref List<FileInfo> files, ref List<DirectoryInfo> dirs, ref List<string> extensions, ref long size, DirectoryInfo rootDir)
        {
            String ext;
            if (OmitErrors)
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    try
                    {
                        dirs.Add(dir);
                        this.Scan(ref files, ref dirs, dir);
                    }
                    catch (Exception) { }
                }
                foreach (FileInfo file in rootDir.GetFiles())
                {
                    try
                    {
                        ext = file.Extension.Replace(".", "").ToUpper();
                        if (!extensions.Contains(ext))
                            extensions.Add(ext);
                        size += file.Length;
                        files.Add(file);
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    dirs.Add(dir);
                    this.Scan(ref files, ref dirs, dir);
                }
                foreach (FileInfo file in rootDir.GetFiles())
                {
                    ext = file.Extension.Replace(".", "").ToUpper();
                    if (!extensions.Contains(ext))
                        extensions.Add(ext);
                    size += file.Length;
                    files.Add(file);
                }
            }
        }
        /// <summary>
        /// Full Scan for files and directories with filter
        /// </summary>
        /// <param name="files">The list of found files</param>
        /// <param name="dirs">The list of directories</param>
        /// <param name="extensions">The current list of extensions</param>
        /// <param name="size">The current root directory size</param>
        /// <param name="rootDir">The search </param>
        private void Scan(ref List<FileInfo> files, ref List<DirectoryInfo> dirs, ref List<string> extensions, ref long size, AerithFilter filter, DirectoryInfo rootDir)
        {
            String ext;
            if (OmitErrors)
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    try
                    {
                        if (filter.IsDirectoryValid(dir))
                            dirs.Add(dir);
                        this.Scan(ref files, ref dirs, dir);
                    }
                    catch (Exception) { }
                }
                foreach (FileInfo file in rootDir.GetFiles())
                {
                    try
                    {
                        if (filter.IsFileValid(file))
                        {
                            ext = file.Extension.Replace(".", "").ToUpper();
                            if (!extensions.Contains(ext))
                                extensions.Add(ext);
                            size += file.Length;
                            files.Add(file);
                        }
                    }
                    catch (Exception) { }
                }
            }
            else
            {
                foreach (DirectoryInfo dir in rootDir.GetDirectories())
                {
                    if (filter.IsDirectoryValid(dir))
                        dirs.Add(dir);
                    this.Scan(ref files, ref dirs, dir);
                }
                foreach (FileInfo file in rootDir.GetFiles())
                {
                    if (filter.IsFileValid(file))
                    {
                        ext = file.Extension.Replace(".", "").ToUpper();
                        if (!extensions.Contains(ext))
                            extensions.Add(ext);
                        size += file.Length;
                        files.Add(file);
                    }
                }
            }
        }
        /// <summary>
        /// Scanner void transaction
        /// </summary>
        void ScannerVoidTransaction()
        {
            //Excecute a void transaction
            try
            {
                this.VoidScanAction();
            }
            //Catch and reports the possible exceptions
            catch (UnauthorizedAccessException exc)
            {
                throw (new BlackMateriaException(exc.Message, exc.InnerException));
            }
            catch (PathTooLongException exc)
            {
                throw (new BlackMateriaException(exc.Message, exc.InnerException));
            }
            catch (Exception exc)
            {
                throw (new BlackMateriaException(exc.Message, exc.InnerException));
            }
        }

        /// <summary>
        /// Scanner transaction
        /// </summary>
        Object ScannerTransaction()
        {
            //Excecute a transaction
            try
            {
                return this.ScanAction();
            }
            //Catch and reports the possible exceptions
            catch (UnauthorizedAccessException exc)
            {
                throw (new BlackMateriaException(exc.Message, exc.InnerException));
            }
            catch (PathTooLongException exc)
            {
                throw (new BlackMateriaException(exc.Message, exc.InnerException));
            }
            catch (Exception exc)
            {
                throw (new BlackMateriaException(exc.Message, exc.InnerException));
            }
        }


    }
}
