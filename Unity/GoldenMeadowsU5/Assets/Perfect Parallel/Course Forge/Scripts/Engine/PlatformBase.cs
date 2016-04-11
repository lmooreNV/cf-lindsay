using UnityEngine;

namespace PerfectParallel
{
    /// <summary>
    /// Platform related base class, IO-system methods
    /// </summary>
    public partial class PlatformBase
    {
        #region Fields
        public static EditorBase Editor = null;
        public static IOBase IO = null;
        #endregion
    }
}