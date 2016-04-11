#if UNITY_IOS || UNITY_ANDROID
#define UNITY_MOBILE
#endif

#if UNITY_STANDALONE || UNITY_MOBILE
#define SYSTEM_IO
#endif

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PerfectParallel
{
    /// <summary>
    /// Platform related class, IO-system methods
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public partial class Platform : PlatformBase
    {
        #region Public Fields
        public static WebIO Web = null;
        public static SystemIOBase SystemIO = null;
        public static StandaloneIO Standalone = null;
        public static MobileIO Mobile = null;
        public static AndroidIO Android = null;
        #endregion

        #region External Methods
        /// <summary>
        /// Initialize platform
        /// </summary>
#if UNITY_EDITOR
        [MenuItem("GameObject/Hidden/Course Forge/Platform/Initialize")]
#endif
        public static void Initialize()
        {
            if (Application.isPlaying)
            {
#if UNITY_WEBPLAYER
                Web = new WebIO();
                IO = Web;
#elif SYSTEM_IO
                SystemIO = new SystemIOBase();
                IO = SystemIO;

#if UNITY_EDITOR
                Standalone = new EditorStandaloneIO();
                IO = Standalone;
#elif UNITY_STANDALONE
                Standalone = new StandaloneIO();
                IO = Standalone;
#elif UNITY_ANDROID
                Android = new AndroidIO();
                IO = Android;
#elif UNITY_MOBILE
                Mobile = new MobileIO();
                IO = Mobile;
#endif

#endif
            }
            else
            {
#if UNITY_WEBPLAYER
                Web = new WebIO();
                IO = Web;
#elif SYSTEM_IO
                SystemIO = new SystemIOBase();
                IO = SystemIO;

#if UNITY_EDITOR
                PlatformBase.Editor = new EditorCore();
                IO = new EditorIO();
#endif

#endif
            }
        }
        #endregion

        #region Constructors
        static Platform()
        {
            Initialize();
        }
        #endregion
    }
}