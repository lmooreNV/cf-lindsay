using UnityEngine;
using Object = UnityEngine.Object;

namespace PerfectParallel
{
    public partial class PlatformBase
    {
        /// <summary>
        /// Editor related base class
        /// </summary>
        public abstract class EditorBase
        {
            #region Properties
            /// <summary>
            /// Path to editor libraries folder
            /// </summary>
            public string LibrariesPath
            {
                get
                {
                    return Application.dataPath + "/Perfect Parallel Libraries";
                }
            }

            /// <summary>
            /// Scene Name
            /// </summary>
            public abstract string SceneName { get; }
            /// <summary>
            /// Scene Path
            /// </summary>
            public abstract string ScenePath { get; }

            /// <summary>
            /// Current active transform
            /// </summary>
            public abstract Transform ActiveTransform { get; set; }
            /// <summary>
            /// Current camera ray
            /// </summary>
            public abstract Ray CameraRay { get; }
            #endregion

            #region Object Methods
            /// <summary>
            /// Registers created object scene state
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="name"></param>
            public abstract void RegisterCreatedObjectUndo(Object obj, string name);
            /// <summary>
            /// Records object state
            /// </summary>
            /// <param name="obj"></param>
            /// <param name="name"></param>
            public abstract void RecordObject(Object obj, string name);
            /// <summary>
            /// Sets gameObject static flags
            /// </summary>
            /// <param name="gameObject"></param>
            /// <param name="flags"></param>
            public abstract void SetStaticFlags(GameObject gameObject, int flags = 1);
            /// <summary>
            /// Copies source properties to dest
            /// </summary>
            /// <param name="source"></param>
            /// <param name="dest"></param>
            public abstract void CopySerialized(Object source, Object dest);
            /// <summary>
            /// Marks object in the scene
            /// </summary>
            /// <param name="obj"></param>
            public abstract void SetDirty(Object obj);
            #endregion

            #region Editor Methods
            /// <summary>
            /// Saves the scene
            /// </summary>
            public abstract void SaveScene();
            /// <summary>
            /// Exits Editor UI callback
            /// </summary>
            public abstract void ExitGUI();
            /// <summary>
            /// Execute Unity MenuItem
            /// </summary>
            /// <param name="name"></param>
            public abstract void ExecuteMenuItem(string name);
            #endregion

            #region Dialog Methods
            /// <summary>
            /// Opens Open File Dialog
            /// </summary>
            /// <param name="title"></param>
            /// <param name="directory"></param>
            /// <param name="extension"></param>
            /// <returns></returns>
            public abstract string OpenFileDialog(string title, string directory, string extension);
            /// <summary>
            /// Opens Save File Dialog
            /// </summary>
            /// <param name="title"></param>
            /// <param name="directory"></param>
            /// <param name="defaultName"></param>
            /// <param name="extension"></param>
            /// <returns></returns>
            public abstract string SaveFileDialog(string title, string directory, string defaultName, string extension);
            /// <summary>
            /// Opens Ok Dialog
            /// </summary>
            /// <param name="title"></param>
            /// <param name="message"></param>
            public abstract void OkDialog(string title, string message);
            /// <summary>
            /// Opens Ok/Cancel Dialog
            /// </summary>
            /// <param name="title"></param>
            /// <param name="message"></param>
            /// <returns></returns>
            public abstract bool OkCancelDialog(string title, string message);
            #endregion

            #region Asset Methods
            /// <summary>
            /// Builds scene into an asset bundle
            /// </summary>
            /// <param name="name"></param>
            /// <param name="dest"></param>
            /// <param name="buildTarget">integer value of BuildTarget enum</param>
            public abstract void BuildSceneAssetBundle(string name, string dest, int buildTarget);
            /// <summary>
            /// Refresh asset database
            /// </summary>
            public abstract void RefreshAssetDatabase();
            #endregion

            #region Path Methods
            /// <summary>
            /// Convert editor relative path to absolute path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public string EditorRelativeToAbsolutePath(string path)
            {
                return Application.dataPath + path.Remove(0, 6);
            }
            /// <summary>
            /// Convert absolute path to editor relative path
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public string AbsolutePathToEditorRelative(string path)
            {
                return path.Remove(0, path.IndexOf("Assets"));
            }
            #endregion

            #region GUID Methods
            /// <summary>
            /// Convert asset path to object
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public abstract Object PathToObject(string path);
            /// <summary>
            /// Convert GUID to object
            /// </summary>
            /// <param name="guid"></param>
            /// <returns></returns>
            public abstract Object GUIDToObject(string guid);
            /// <summary>
            /// Convert object to GUID
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public abstract string ObjectToGUID(Object obj);
            /// <summary>
            /// Convert object to GUID
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public abstract string ObjectToPath(Object obj);
            /// <summary>
            /// Convert GUID to asset path
            /// </summary>
            /// <param name="GUID"></param>
            /// <returns></returns>
            public abstract string GUIDToPath(string GUID);
            #endregion
        }
    }
}