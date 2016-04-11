using UnityEngine;
using System;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Tee class
    /// </summary>
    [AddComponentMenu("Perfect Parallel/Course Forge/Plant/Tee")]
    public class TeeBase : PlantBase, IPlant
    {
        /// <summary>
        /// Info class, tee properties
        /// </summary>
        [Serializable]
        public class Info : BaseInfo
        {
            public enum Type
            {
                Championship = 0,
                Tournament,
                Back,
                Member,
                Forward,
                Ladies,
                Challenge
            }

            public enum Par
            {
                _2 = 0,
                _3,
                _4,
                _5,
                _6,
            }

            #region Fields
            public Type type = Type.Championship;
            public Par par = Par._4;
            public int strokeIndex = -1;
            public float width = 0;
            public float height = 0;
            #endregion
        }

        #region Fields
        [SerializeField]
        Info info = new Info();
        [SerializeField]
        GameObject testLeft = null;
        [SerializeField]
        GameObject testRight = null;
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info Position Change");
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info HoleIndex Change");
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info OrderIndex Change");
                    info.orderIndex = value;
                }
            }
        }

        /// <summary>
        /// Tee info
        /// </summary>
        public TeeBase.Info TeeInfo
        {
            get
            {
                return info;
            }
        }
        /// <summary>
        /// Tee type
        /// </summary>
        public TeeBase.Info.Type Type
        {
            get
            {
                return info.type;
            }
            set
            {
                if (info.type != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info Type Change");
                    info.type = value;
                }
            }
        }
        /// <summary>
        /// Tee par
        /// </summary>
        public TeeBase.Info.Par Par
        {
            get
            {
                return info.par;
            }
            set
            {
                if (info.par != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info Par Change");
                    info.par = value;
                }
            }
        }
        /// <summary>
        /// Stroke index
        /// </summary>
        public int StrokeIndex
        {
            get
            {
                return info.strokeIndex;
            }
            set
            {
                if (info.strokeIndex != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info StrokeIndex Change");
                    info.strokeIndex = value;
                }
            }
        }
        /// <summary>
        /// Tee random zone height
        /// </summary>
        public float Width
        {
            get
            {
                if (info.width == 0)
                {
                    if (Type == TeeBase.Info.Type.Championship) return CourseBase.Info.championshipWidth;
                    if (Type == TeeBase.Info.Type.Tournament) return CourseBase.Info.tournamentWidth;
                    if (Type == TeeBase.Info.Type.Back) return CourseBase.Info.backWidth;
                    if (Type == TeeBase.Info.Type.Member) return CourseBase.Info.memberWidth;
                    if (Type == TeeBase.Info.Type.Forward) return CourseBase.Info.forwardWidth;
                    if (Type == TeeBase.Info.Type.Ladies) return CourseBase.Info.ladiesWidth;
                    if (Type == TeeBase.Info.Type.Challenge) return CourseBase.Info.challengeWidth;
                }
                return info.width;
            }
            set
            {
                if (info.width != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info Width Change");
                    info.width = value;
                }
            }
        }
        /// <summary>
        /// Tee random zone height
        /// </summary>
        public float Height
        {
            get
            {
                if (info.height == 0)
                {
                    if (Type == TeeBase.Info.Type.Championship) return CourseBase.Info.championshipHeight;
                    if (Type == TeeBase.Info.Type.Tournament) return CourseBase.Info.tournamentHeight;
                    if (Type == TeeBase.Info.Type.Back) return CourseBase.Info.backHeight;
                    if (Type == TeeBase.Info.Type.Member) return CourseBase.Info.memberHeight;
                    if (Type == TeeBase.Info.Type.Forward) return CourseBase.Info.forwardHeight;
                    if (Type == TeeBase.Info.Type.Ladies) return CourseBase.Info.ladiesHeight;
                    if (Type == TeeBase.Info.Type.Challenge) return CourseBase.Info.challengeHeight;
                }
                return info.height;
            }
            set
            {
                if (info.height != value)
                {
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "Tee Info Height Change");
                    info.height = value;
                }
            }
        }
        /// <summary>
        /// CourseForge test tee marker (left)
        /// </summary>
        public GameObject TestLeft
        {
            get
            {
                return testLeft;
            }
            set
            {
                testLeft = value;
            }
        }
        /// <summary>
        /// CourseForge test tee marker (right)
        /// </summary>
        public GameObject TestRight
        {
            get
            {
                return testRight;
            }
            set
            {
                testRight = value;
            }
        }
        #endregion
    }
}