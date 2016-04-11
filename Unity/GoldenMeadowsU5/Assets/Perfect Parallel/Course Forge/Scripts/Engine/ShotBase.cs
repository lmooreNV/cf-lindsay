using UnityEngine;
using System;
using System.Reflection;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Shot class
    /// </summary>
    [AddComponentMenu("Perfect Parallel/Course Forge/Plant/Shot")]
    public class ShotBase : PlantBase, IPlant
    {
        [Serializable]
        public class Info : BaseInfo
        {
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Shot Info Position Change");
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Shot Info HoleIndex Change");
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Shot Info OrderIndex Change");
                    info.orderIndex = value;
                }
            }
        }
        #endregion
    }
}