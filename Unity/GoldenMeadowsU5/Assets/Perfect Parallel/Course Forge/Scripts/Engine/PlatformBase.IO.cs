using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PerfectParallel
{
    public partial class PlatformBase
    {
        /// <summary>
        /// I/O related base class
        /// </summary>
        public abstract class IOBase
        {
            #region Properties
            /// <summary>
            /// Path to documents
            /// </summary>
            public abstract string DocumentsPath
            {
                get;
            }
            /// <summary>
            /// Path to courses
            /// </summary>
            public virtual string CoursesPath
            {
                get
                {
                    return DocumentsPath + "/Courses";
                }
            }
            /// <summary>
            /// Path to config
            /// </summary>
            public string ConfigPath
            {
                get
                {
                    return DocumentsPath + "/Config";
                }
            }
            /// <summary>
            /// Path to libraries
            /// </summary>
            public string LibrariesPath
            {
                get
                {
                    return DocumentsPath + "/Libraries";
                }
            }
            /// <summary>
            /// Path to videos
            /// </summary>
            public string VideosPath
            {
                get
                {
                    return DocumentsPath + "/Videos";
                }
            }


            /// <summary>
            /// Gets a value indicating whether this instance is editor.
            /// </summary>
            /// <value>
            /// true if this instance is editor; otherwise, false.
            /// </value>
            public virtual bool IsEditor
            {
                get
                {
                    return false;
                }
            }
            /// <summary>
            /// Gets a value indicating whether this instance is playing.
            /// </summary>
            /// <value>
            /// true if this instance is playing; otherwise, false.
            /// </value>
            public virtual bool IsPlaying
            {
                get
                {
                    return Application.isPlaying;
                }
            }
            /// <summary>
            /// Gets a value indicating whether this instance is web.
            /// </summary>
            /// <value>
            ///   <c>true</c> if this instance is web; otherwise, <c>false</c>.
            /// </value>
            public virtual bool IsWeb
            {
                get
                {
                    return false;
                }
            }
            /// <summary>
            /// Gets a value indicating whether this instance is standalone.
            /// </summary>
            /// <value>
            /// true if this instance is standalone; otherwise, false.
            /// </value>
            public virtual bool IsStandalone
            {
                get
                {
                    return false;
                }
            }
            /// <summary>
            /// Gets a value indicating whether this instance is system io.
            /// </summary>
            /// <value>
            /// true if this instance is system io; otherwise, false.
            /// </value>
            public virtual bool IsSystemIO
            {
                get
                {
                    return false;
                }
            }
            /// <summary>
            /// Gets a value indicating whether this instance is android.
            /// </summary>
            /// <value>
            /// true if this instance is android; otherwise, false.
            /// </value>
            public virtual bool IsAndroid
            {
                get
                {
                    return false;
                }
            }
            /// <summary>
            /// Gets a value indicating whether this instance is mobile.
            /// </summary>
            /// <value>
            /// true if this instance is mobile; otherwise, false.
            /// </value>
            public virtual bool IsMobile
            {
                get
                {
                    return false;
                }
            }
            #endregion

            #region IO Methods
            /// <summary>
            /// Read text
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public virtual string ReadText(string path)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Read bytes
            /// </summary>
            /// <param name="path"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public virtual byte[] ReadBytes(string path, int offset = 0, int count = -1)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Read bytes async
            /// </summary>
            /// <param name="path"></param>
            /// <param name="bytes"></param>
            /// <param name="offset"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public virtual IEnumerator IEReadBytes(string path, byte[] bytes, int offset, int count)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Get directories async
            /// </summary>
            /// <param name="path"></param>
            /// <param name="directories"></param>
            /// <returns></returns>
            public abstract IEnumerator IEGetDirectories(string path, List<string> directories);
            /// <summary>
            /// Get files async
            /// </summary>
            /// <param name="path"></param>
            /// <param name="files"></param>
            /// <returns></returns>
            public abstract IEnumerator IEGetFiles(string path, List<string> files);

            /// <summary>
            /// Write text
            /// </summary>
            /// <param name="path"></param>
            /// <param name="text"></param>
            public virtual void WriteText(string path, string text)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Append bytes
            /// </summary>
            /// <param name="path"></param>
            /// <param name="bytes"></param>
            public virtual void AppendBytes(string path, byte[] bytes)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Is file exists?
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public virtual bool FileExists(string path)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Is directory exists?
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public virtual bool DirectoryExists(string path)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Create new (clean) directory
            /// </summary>
            /// <param name="path"></param>
            public virtual void CreateEmptyDirectory(string path)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Create directory if not exist
            /// </summary>
            /// <param name="path"></param>
            public virtual void CreateDirectory(string path)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Delete file
            /// </summary>
            /// <param name="path"></param>
            public virtual void DeleteFile(string path)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Delete directory
            /// </summary>
            /// <param name="path"></param>
            public virtual void DeleteDirectory(string path)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Copy file
            /// </summary>
            /// <param name="source"></param>
            /// <param name="dest"></param>
            /// <param name="overwrite"></param>
            public virtual void CopyFile(string source, string dest, bool overwrite = true)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Copy directory
            /// </summary>
            /// <param name="source"></param>
            /// <param name="dest"></param>
            /// <param name="importAssets"></param>
            public virtual void CopyDirectory(string source, string dest, bool importAssets = false)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Runs process of command with arguments at path
            /// </summary>
            /// <param name="command"></param>
            /// <param name="args"></param>
            /// <param name="path"></param>
            public virtual void RunProcessAndWait(string command, string args, string path)
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Is plugin exist?
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public virtual bool PluginExists(string name)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region Path Methods
            /// <summary>
            /// Gets file name
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public string GetFileName(string path)
            {
                return Path.GetFileName(path);
            }
            /// <summary>
            /// Gets the full path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public string GetFullPath(string path)
            {
                return Path.GetFullPath(path);
            }
            /// <summary>
            /// Gets file name without extension
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public string GetFileNameWithoutExtension(string path)
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            /// <summary>
            /// Gets directory name
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public string GetDirectoryName(string path)
            {
                return Path.GetDirectoryName(path);
            }
            #endregion
        }
    }
}