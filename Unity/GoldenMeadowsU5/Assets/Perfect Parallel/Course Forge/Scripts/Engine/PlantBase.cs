using UnityEngine;
using System;
using System.Reflection;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Plant object base interface methods
    /// </summary>
    [Obfuscation(Exclude = true)]
    public interface IPlant
    {
        #region Properties
        Transform Transform
        {
            get;
        }
        PlantBase.BaseInfo PlantInfo
        {
            get;
        }
        Vector3 Position
        {
            get;
            set;
        }
        int HoleIndex
        {
            get;
            set;
        }
        int OrderIndex
        {
            get;
            set;
        }
        #endregion
    }

    /// <summary>
    /// Base class for plant objects
    /// </summary>
    [AddComponentMenu("GameObject/Hidden/Perfect Parallel/Course Forge/Plant")]
    public class PlantBase : MonoBehaviour
    {
        /// <summary>
        /// Plant object base info
        /// </summary>
        [Serializable]
        public class BaseInfo
        {
            #region Fields
            public Vector3Object position = new Vector3Object();
            public int holeIndex = -1;
            public int orderIndex = -1;
            #endregion
        }
    }
}