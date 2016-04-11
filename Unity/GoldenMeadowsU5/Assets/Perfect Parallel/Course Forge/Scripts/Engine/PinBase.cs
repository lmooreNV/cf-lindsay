using UnityEngine;
using System;
using System.Reflection;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Pin class
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Perfect Parallel/Course Forge/Plant/Pin")]
    public class PinBase : PlantBase, IPlant
    {
        [Serializable]
        public class Info : BaseInfo
        {
            public enum Difficulty
            {
                Easy = 0,
                Medium,
                Difficult
            }

            #region Fields
            public Difficulty difficulty = Difficulty.Medium;
            #endregion
        }

        #region Fields
        [SerializeField]
        Info info = new Info();
        #endregion

        #region Properties
        public Transform Transform
        {
            get
            {
                return transform;
            }
        }
        public BaseInfo PlantInfo
        {
            get
            {
                return info;
            }
        }
        public Vector3 Position
        {
            get
            {
                return info.position.ToVector3();
            }
            set
            {
                if (transform.position != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Pin Info Position Change");
                    info.position.Set(value);
                    transform.position = value;
                }
            }
        }
        public int HoleIndex
        {
            get
            {
                return info.holeIndex;
            }
            set
            {
                if (info.holeIndex != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Pin Info HoleIndex Change");
                    info.holeIndex = value;
                }
            }
        }
        public int OrderIndex
        {
            get
            {
                return info.orderIndex;
            }
            set
            {
                if (info.orderIndex != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Pin Info OrderIndex Change");
                    info.orderIndex = value;
                }
            }
        }
        /// <summary>
        /// Pin difficulty
        /// </summary>
        public Info.Difficulty Difficulty
        {
            get
            {
                return info.difficulty;
            }
            set
            {
                if (info.difficulty != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Pin Info Difficulty Change");
                    info.difficulty = value;
                }
            }
        }
        #endregion
    }
}