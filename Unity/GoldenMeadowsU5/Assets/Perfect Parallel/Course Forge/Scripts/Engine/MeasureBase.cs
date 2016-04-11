using UnityEngine;
using System;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Measure class
    /// </summary>
    [AddComponentMenu("Perfect Parallel/Course Forge/Plant/Measure")]
    public class MeasureBase : PlantBase, IPlant
    {
        /// <summary>
        /// Measure info container class
        /// </summary>
        [Serializable]
        public class Info : BaseInfo
        {
            /// <summary>
            /// Measure point container class, based on plant info
            /// </summary>
            [Serializable]
            public class Point : BaseInfo
            {
            }

            #region Fields
            public Units units = Units.Imperial;
            public Point start = new Point();
            public Point end = new Point();
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
                return (StartPosition + EndPosition) / 2;
            }
            set
            {
                if (transform.position != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Measure Info Position Change");
                    transform.position = (StartPosition + EndPosition) / 2;
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Measure Info HoleIndex Change");
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Measure Info OrderIndex Change");
                    info.orderIndex = value;
                }
            }
        }

        /// <summary>
        /// First point plant info
        /// </summary>
        public BaseInfo StartInfo
        {
            get
            {
                return info.start;
            }
        }
        /// <summary>
        /// Second point plant info
        /// </summary>
        public BaseInfo EndInfo
        {
            get
            {
                return info.end;
            }
        }
        /// <summary>
        /// First point position
        /// </summary>
        public Vector3 StartPosition
        {
            get
            {
                return info.start.position.ToVector3();
            }
        }
        /// <summary>
        /// Second point position
        /// </summary>
        public Vector3 EndPosition
        {
            get
            {
                return info.end.position.ToVector3();
            }
        }
        /// <summary>
        /// Measure units system
        /// </summary>
        public Units Units
        {
            get
            {
                return info.units;
            }
            set
            {
                if (info.units != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Measure Info Units Change");
                    info.units = value;
                }
            }
        }
        #endregion
    }
}