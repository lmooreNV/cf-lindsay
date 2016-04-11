#if UNITY_IOS || UNITY_ANDROID
#define UNITY_MOBILE
#endif

#if UNITY_STANDALONE || UNITY_MOBILE
#define SYSTEM_IO
#endif

#if SYSTEM_IO
using System.IO;
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PerfectParallel
{
    public partial class Platform
    {
        /// <summary>
        /// Web-related I/O class
        /// </summary>
        public class WebIO : IOBase
        {
            #region Fields
            public string webDocumentsPath = "http://www.perfectparallel.com/Documents";
            public string webCoursesPath = "";
            #endregion

            #region Properties
            public override string DocumentsPath
            {
                get
                {
                    return webDocumentsPath + "/Perfect%20Parallel";
                }
            }
            public override string CoursesPath
            {
                get
                {
                    if (webCoursesPath != "") return webCoursesPath;
                    return base.CoursesPath;
                }
            }

            public override bool IsWeb
            {
                get
                {
                    return true;
                }
            }
            #endregion

            #region IO Methods
            public override IEnumerator IEGetDirectories(string path, List<string> directories)
            {
#if UNITY_WEBPLAYER
                WWW www = null;
                while (true)
                {
                    www = new WWW(path + "/getFiles.php");
                    www.threadPriority = ThreadPriority.High;
                    yield return www;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(www.url + " (" + www.error + ")");
                        yield return new WaitForSeconds(30.0f);
                    }
                    else
                    {
                        break;
                    }
                }

                string[] names = Utility.JsonRead<string[]>(www.text);
                for (int i = 0; i < names.Length; ++i) directories.Add(path + "/" + names[i]);
#else
                throw new NotImplementedException();
#endif
            }
            public override IEnumerator IEGetFiles(string path, List<string> files)
            {
#if UNITY_WEBPLAYER
                WWW www = null;
                while (true)
                {
                    www = new WWW(path + "/getFiles.php");
                    www.threadPriority = ThreadPriority.High;
                    yield return www;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(www.url + " (" + www.error + ")");
                        yield return new WaitForSeconds(30.0f);
                    }
                    else
                    {
                        break;
                    }
                }

                string[] names = Utility.JsonRead<string[]>(www.text);
                for (int i = 0; i < names.Length; ++i) files.Add(path + "/" + names[i]);
#else
                throw new NotImplementedException();
#endif
            }
            #endregion
        }

        /// <summary>
        /// System-IO related I/O class
        /// </summary>
        public class SystemIOBase : IOBase
        {
            #region Properties
            public override string DocumentsPath
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override bool IsSystemIO
            {
                get
                {
                    return true;
                }
            }
            #endregion

            #region IO Methods
            public override string ReadText(string path)
            {
#if SYSTEM_IO
                return File.ReadAllText(path);
#else
                throw new NotImplementedException();
#endif
            }
            public override byte[] ReadBytes(string path, int offset = 0, int count = -1)
            {
#if SYSTEM_IO
                FileStream stream = File.OpenRead(path);
                if (count != -1)
                {
                    byte[] bytes = new byte[count];
                    stream.Read(bytes, offset, count);
                    return bytes;
                }
                return File.ReadAllBytes(path);
#else
                throw new NotImplementedException();
#endif
            }
            public override IEnumerator IEReadBytes(string path, byte[] bytes, int offset, int count)
            {
#if SYSTEM_IO
                FileStream stream = File.OpenRead(path);
                stream.BeginRead(bytes, offset, count, delegate (IAsyncResult asyncResult) { ((FileStream)asyncResult.AsyncState).EndRead(asyncResult); }, stream);
                while (stream.Position < count) yield return null;
#else
                throw new NotImplementedException();
#endif
            }
            public override IEnumerator IEGetDirectories(string path, List<string> directories)
            {
#if SYSTEM_IO
                string[] names = Directory.GetDirectories(path);
                for (int i = 0; i < names.Length; ++i) directories.Add(Path.GetFullPath(names[i]));
                yield return null;
#else
                throw new NotImplementedException();
#endif
            }
            public override IEnumerator IEGetFiles(string path, List<string> files)
            {
#if SYSTEM_IO
                string[] names = Directory.GetFiles(path);
                for (int i = 0; i < names.Length; ++i) files.Add(Path.GetFullPath(names[i]));
                yield return null;
#else
                throw new NotImplementedException();
#endif
            }

            public override void WriteText(string path, string text)
            {
#if SYSTEM_IO
                File.WriteAllText(path, text);
#endif
            }
            public override void AppendBytes(string path, byte[] bytes)
            {
#if SYSTEM_IO
                FileStream stream = new FileStream(path, FileMode.Append);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
                stream = null;
#endif
            }

            public override bool FileExists(string path)
            {
#if SYSTEM_IO
                return File.Exists(path);
#else
                throw new NotImplementedException();
#endif
            }
            public override bool DirectoryExists(string path)
            {
#if SYSTEM_IO
                return Directory.Exists(path);
#else
                throw new NotImplementedException();
#endif
            }

            public override void CreateEmptyDirectory(string path)
            {
#if SYSTEM_IO
                DeleteDirectory(path);
                CreateDirectory(path);
#endif
            }
            public override void CreateDirectory(string path)
            {
#if SYSTEM_IO
                if (!DirectoryExists(path)) Directory.CreateDirectory(path);
#endif
            }
            #endregion
        }

        /// <summary>
        /// Standalone related I/O class
        /// </summary>
        public class StandaloneIO : SystemIOBase
        {
            #region Properties
            public override string DocumentsPath
            {
                get
                {
                    string path = null;
#if SYSTEM_IO
#if UNITY_STANDALONE_OSX
                    path = GetFullPath(Application.dataPath + "/../../");
#else
                    path = GetFullPath(Application.dataPath + "/../");
#endif
                    if (!DirectoryExists(path + "Courses")) path = GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + "/Perfect Parallel";
#endif
                    return path;
                }
            }

            public override bool IsStandalone
            {
                get
                {
                    return true;
                }
            }
            #endregion

            #region IO Methods
            public override void RunProcessAndWait(string command, string args, string path)
            {
#if SYSTEM_IO
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command, args);
                info.WorkingDirectory = IO.GetDirectoryName(path);

                System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
                while (!process.HasExited) System.Threading.Thread.Sleep(1000);
#endif
            }
            public override bool PluginExists(string name)
            {
#if UNITY_STANDALONE_OSX
                return FileExists(Application.dataPath + "/Plugins/" + name + ".bundle");
#else
                return FileExists(Application.dataPath + "/Plugins/" + name + ".dll");
#endif
            }
            #endregion
        }
        /// <summary>
        /// Editor standalone related I/O class
        /// </summary>
        public class EditorStandaloneIO : StandaloneIO
        {
            #region Properties
            public override string DocumentsPath
            {
                get
                {
                    string path = null;
#if SYSTEM_IO
#if UNITY_EDITOR_OSX
                    path = GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + "/Documents/Perfect Parallel";
#else
                    path = GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + "/Perfect Parallel";
#endif
#endif
                    return path;
                }
            }

            public override bool IsEditor
            {
                get
                {
                    return true;
                }
            }
            #endregion
        }

        /// <summary>
        /// Editor related I/O class
        /// </summary>
        public class EditorIO : StandaloneIO
        {
            #region Properties
            public override string DocumentsPath
            {
                get
                {
                    string path = null;
#if SYSTEM_IO
#if UNITY_EDITOR_OSX
                    path = GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + "/Documents/Perfect Parallel";
#else
                    path = GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + "/Perfect Parallel";
#endif
#endif
                    return path;
                }
            }

            public override bool IsEditor
            {
                get
                {
                    return true;
                }
            }
            #endregion

            #region IO Methods
            public override void CopyFile(string source, string dest, bool overwrite = true)
            {
#if SYSTEM_IO
                if (FileExists(source)) File.Copy(source, dest, overwrite);
#endif
            }
            public override void CopyDirectory(string source, string dest, bool importAssets = false)
            {
#if SYSTEM_IO
                CreateEmptyDirectory(dest);

                string[] directories = Directory.GetDirectories(source);
                for (int i = 0; i < directories.Length; ++i)
                    CreateDirectory(directories[i].Replace(source, dest));

                string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; ++i)
                    CopyFile(files[i], files[i].Replace(source, dest), true);
#endif
            }

            public override void DeleteFile(string path)
            {
#if SYSTEM_IO
                if (FileExists(path)) File.Delete(path);
#endif
            }
            public override void DeleteDirectory(string path)
            {
#if SYSTEM_IO
                if (DirectoryExists(path)) Directory.Delete(path, true);
#endif
            }
            #endregion
        }

        /// <summary>
        /// Mobile related I/O class
        /// </summary>
        public class MobileIO : SystemIOBase
        {
            #region Properties
            public override string DocumentsPath
            {
                get
                {
                    return GetFullPath(Application.dataPath + "/../");
                }
            }

            public override bool IsMobile
            {
                get
                {
                    return true;
                }
            }
            #endregion
        }

        /// <summary>
        /// Android related I/O class
        /// </summary>
        public class AndroidIO : MobileIO
        {
            #region Properties
            public override string DocumentsPath
            {
                get
                {
                    return "/mnt/sdcard/perfectparallel/";
                }
            }

            public override bool IsAndroid
            {
                get
                {
                    return true;
                }
            }
            #endregion
        }
    }
}
