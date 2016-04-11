using UnityEngine;
using Object = UnityEngine.Object;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PerfectParallel
{
    public partial class Platform
    {
        /// <summary>
        /// Editor related core class
        /// </summary>
        public class EditorCore : EditorBase
        {
            #region Properties
            public override string SceneName
            {
                get
                {
#if UNITY_EDITOR
                    return IO.GetFileNameWithoutExtension(ScenePath);
#else
                    throw new NotImplementedException();
#endif
                }
            }
            public override string ScenePath
            {
                get
                {
#if UNITY_EDITOR
                    return EditorApplication.currentScene;
#else
                    throw new NotImplementedException();
#endif
                }
            }

            public override Transform ActiveTransform
            {
                get
                {
#if UNITY_EDITOR
                    return Selection.activeTransform;
#else
                    throw new NotImplementedException();
#endif
                }
                set
                {
#if UNITY_EDITOR
                    Selection.activeTransform = value;
#else
                    throw new NotImplementedException();
#endif
                }
            }
            public override Ray CameraRay
            {
                get
                {
#if UNITY_EDITOR
                    return HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
#else
                    throw new NotImplementedException();
#endif
                }
            }
            #endregion

            #region Object Methods
            public override void RegisterCreatedObjectUndo(Object obj, string name)
            {
#if UNITY_EDITOR
                Undo.RegisterCreatedObjectUndo(obj, name);
#endif
            }
            public override void RecordObject(Object obj, string name)
            {
#if UNITY_EDITOR
                Undo.RecordObject(obj, name);
#endif
            }
            public override void SetStaticFlags(GameObject gameObject, int flags = 1)
            {
#if UNITY_EDITOR
                GameObjectUtility.SetStaticEditorFlags(gameObject, (StaticEditorFlags)flags);
#endif
            }
            public override void CopySerialized(Object source, Object dest)
            {
#if UNITY_EDITOR
                EditorUtility.CopySerialized(source, dest);
#endif
            }
            public override void SetDirty(Object obj)
            {
#if UNITY_EDITOR
                EditorUtility.SetDirty(obj);
#endif
            }
            #endregion

            #region Editor Methods
            public override void SaveScene()
            {
#if UNITY_EDITOR
                EditorApplication.SaveScene();
#endif
            }
            public override void ExitGUI()
            {
#if UNITY_EDITOR
                EditorGUIUtility.ExitGUI();
#endif
            }
            public override void ExecuteMenuItem(string name)
            {
#if UNITY_EDITOR
                EditorApplication.ExecuteMenuItem(name);
#endif
            }
            #endregion

            #region Dialog Methods
            public override string OpenFileDialog(string title, string directory, string extension)
            {
#if UNITY_EDITOR
                return EditorUtility.OpenFilePanel(title, directory, extension);
#else
                throw new NotImplementedException();
#endif
            }
            public override string SaveFileDialog(string title, string directory, string defaultName, string extension)
            {
#if UNITY_EDITOR
                return EditorUtility.SaveFilePanel(title, directory, defaultName, extension);
#else
                throw new NotImplementedException();
#endif
            }
            public override void OkDialog(string title, string message)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog(title, message, "ok");
#endif
            }
            public override bool OkCancelDialog(string title, string message)
            {
#if UNITY_EDITOR
                return EditorUtility.DisplayDialog(title, message, "ok", "cancel");
#else
                throw new NotImplementedException();
#endif
            }
            #endregion

            #region Asset Methods
            public override void BuildSceneAssetBundle(string name, string dest, int buildTarget)
            {
#if UNITY_EDITOR
                AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetBundleName = PlatformBase.IO.GetFileName(name) + "3d";
                assetBundleBuild.assetNames = new string[] { name };

                BuildTarget platformCached = EditorUserBuildSettings.activeBuildTarget;
                {
                    BuildPipeline.BuildAssetBundles(dest, new AssetBundleBuild[] { assetBundleBuild }, BuildAssetBundleOptions.None, (BuildTarget)buildTarget);
                }
                EditorUserBuildSettings.SwitchActiveBuildTarget((BuildTarget)buildTarget);
                EditorUserBuildSettings.SwitchActiveBuildTarget(platformCached);
#endif
            }
            public override void RefreshAssetDatabase()
            {
#if UNITY_EDITOR
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
#endif
            }
            #endregion

            #region GUID Methods
            public override Object PathToObject(string path)
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadMainAssetAtPath(AbsolutePathToEditorRelative(path));
#else
                throw new NotImplementedException();
#endif
            }
            public override Object GUIDToObject(string guid)
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));
#else
                throw new NotImplementedException();
#endif
            }
            public override string ObjectToGUID(Object obj)
            {
                if (obj == null) return "";
#if UNITY_EDITOR
                return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
#else
                throw new NotImplementedException();
#endif
            }
            public override string ObjectToPath(Object obj)
            {
                if (obj == null) return "";
#if UNITY_EDITOR
                return EditorRelativeToAbsolutePath(AssetDatabase.GetAssetPath(obj));
#else
                throw new NotImplementedException();
#endif
            }
            public override string GUIDToPath(string GUID)
            {
#if UNITY_EDITOR
                return EditorRelativeToAbsolutePath(AssetDatabase.GUIDToAssetPath(GUID));
#else
                throw new NotImplementedException();
#endif
            }
            #endregion
        }
    }
}