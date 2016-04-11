using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using JsonFx.Json;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// FlyBy class, stores flyby nodes
    /// </summary>
    [AddComponentMenu("Perfect Parallel/Course Forge/Plant/FlyBy")]
    public class FlyByBase : PlantBase, IPlant
    {
        /// <summary>
        /// FlyBy info container
        /// </summary>
        [Serializable]
        public class Info : BaseInfo
        {
            /// <summary>
            /// FlyBy node container
            /// </summary>
            [Serializable]
            public class Node : BaseInfo
            {
                public enum Target
                {
                    Probe,
                    Tee,
                    Pin,
                    Shot,
                    GameObject,
                    Next,
                    Previous,
                    NextProbe,
                    PreviousProbe,
                }

                #region Public Fields
                public Target target = Target.Pin;
                public BaseInfo probe = new BaseInfo();
                public float velocity = 15.0f;
                public float time = 0;
                #endregion
                #region Private Fields
                [SerializeField]
                Transform cameraTarget = null;
                [SerializeField]
                AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);
                #endregion

                #region Properties
                /// <summary>
                /// This node target
                /// </summary>
                [JsonIgnore]
                public Transform CameraTarget
                {
                    get
                    {
                        return cameraTarget;
                    }
                    set
                    {
                        cameraTarget = value;
                    }
                }
                /// <summary>
                /// Curve to blend the rotation
                /// </summary>
                [JsonIgnore]
                public AnimationCurve RotationCurve
                {
                    get
                    {
                        return rotationCurve;
                    }
                    set
                    {
                        rotationCurve = value;
                    }
                }
                #endregion

                #region Methods
                /// <summary>
                /// Get next node from the list
                /// </summary>
                /// <param name="nodes"></param>
                /// <returns></returns>
                public Node GetNext(List<Node> nodes)
                {
                    int index = nodes.IndexOf(this as Node);
                    if (index == nodes.Count - 1) return this as Node;
                    return nodes[index + 1];
                }
                /// <summary>
                /// Get previous node from the list
                /// </summary>
                /// <param name="nodes"></param>
                /// <returns></returns>
                public Node GetPrevious(List<Node> nodes)
                {
                    int index = nodes.IndexOf(this as Node);
                    if (index == 0) return this as Node;
                    return nodes[index - 1];
                }
                /// <summary>
                /// Retrieve target position based on the hole information
                /// </summary>
                /// <param name="holeIndex"></param>
                /// <param name="nodes"></param>
                /// <param name="pinPosition"></param>
                /// <param name="shotPosition"></param>
                /// <param name="teePosition"></param>
                /// <returns></returns>
                public Vector3 TargetPosition(int holeIndex, List<Node> nodes, Vector3 pinPosition, Vector3 shotPosition, Vector3 teePosition)
                {
                    switch (target)
                    {
                        case Target.Probe:
                            return probe.position.ToVector3();

                        case Target.Tee:
                            if (CourseBase.IsHoleEnabled(holeIndex))
                            {
                                if (teePosition != Vector3.zero) return teePosition;
                                else return CourseBase.Holes[holeIndex].tees[0].Position;
                            }
                            break;

                        case Target.Pin:
                            if (CourseBase.IsHoleEnabled(holeIndex))
                            {
                                if (pinPosition != Vector3.zero) return pinPosition;
                                else return CourseBase.Holes[holeIndex].pins[0].Position;
                            }
                            break;

                        case Target.Shot:
                            if (CourseBase.IsHoleEnabled(holeIndex))
                            {
                                if (shotPosition != Vector3.zero) return shotPosition;
                                else if (CourseBase.Holes[holeIndex].shots.Count > 0) return CourseBase.Holes[holeIndex].shots[0].Position;
                            }
                            break;

                        case Target.GameObject:
                            if (cameraTarget == null) return position.ToVector3();
                            else return cameraTarget.position;

                        case Target.Next:
                            if (this == nodes[nodes.Count - 1]) return position.ToVector3();
                            return GetNext(nodes).position.ToVector3();

                        case Target.Previous:
                            if (this == nodes[0]) return position.ToVector3();
                            return GetPrevious(nodes).position.ToVector3();

                        case Target.NextProbe:
                            if (this == nodes[nodes.Count - 1]) return position.ToVector3();
                            return GetNext(nodes).TargetPosition(holeIndex, nodes, pinPosition, shotPosition, teePosition);

                        case Target.PreviousProbe:
                            if (this == nodes[0]) return position.ToVector3();
                            return GetPrevious(nodes).TargetPosition(holeIndex, nodes, pinPosition, shotPosition, teePosition);

                        default:
                            return Vector3.zero;
                    }
                    return Vector3.zero;
                }
                #endregion
            }

            public enum Type
            {
                A,
                B,
                C,
                D
            }

            #region Public Fields
            public List<Node> nodes = new List<Node>();
            public List<Vector3Object> points = new List<Vector3Object>();
            public Type type = Type.A;
            #endregion
            #region Private Fields
            Vector3[] temp = new Vector3[0];
            #endregion

            #region Properties
            /// <summary>
            /// Array of node points
            /// </summary>
            [JsonIgnore]
            public Vector3[] Points
            {
                get
                {
                    return temp;
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Cache node points
            /// </summary>
            public void UpdatePoints()
            {
                List<Vector3> originalPoints = new List<Vector3>();
                for (int i = 0; i < nodes.Count; ++i)
                    originalPoints.Add(nodes[i].position.ToVector3());

                Vector3[] array = originalPoints.ToArray().CatmullRom(32, false).ToArray();
                points.Clear();
                for (int i = 0; i < array.Length; ++i) points.Add(array[i]);

                temp = array.ToArray();
            }
            /// <summary>
            /// Calculate time based on velocity
            /// </summary>
            public void UpdateTimes()
            {
                for (int i = 1; i < nodes.Count; ++i)
                {
                    Vector3 prevPos = nodes[i - 1].position.ToVector3();
                    Vector3 currPos = nodes[i - 0].position.ToVector3();
                    float distance = 0;

                    bool working = false;
                    for (int p = 0; p < points.Count; ++p)
                    {
                        if (points[p].ToVector3() == prevPos)
                        {
                            working = true;
                            continue;
                        }
                        if (working)
                        {
                            distance += Vector3.Distance(points[p].ToVector3(), points[p - 1].ToVector3());
                        }
                        if (points[p].ToVector3() == currPos)
                        {
                            break;
                        }
                    }

                    float prevVel = nodes[i - 1].velocity;
                    float currVel = nodes[i - 0].velocity;

                    float dv = (prevVel + (currVel - prevVel) / 2);
                    nodes[i - 1].time = distance / dv;
                }
            }
            /// <summary>
            /// Get position at time
            /// </summary>
            /// <param name="time"></param>
            /// <returns></returns>
            public Vector3 GetPosition(float time)
            {
                float local = 0;
                for (int i = 1; i < nodes.Count; ++i)
                {
                    float delta = nodes[i - 1].time;
                    local += delta;
                    if (local > time)
                    {
                        float sincePrevTime = (time - (local - delta));
                        float sincePrevTimeLerp = sincePrevTime / delta;
                        float prevVel = nodes[i - 1].velocity;
                        float currVel = prevVel + (nodes[i - 0].velocity - prevVel) * sincePrevTimeLerp;
                        float dv = (prevVel + (currVel - prevVel) / 2);
                        float traveled = dv * (time - (local - delta));

                        Vector3 prevPos = nodes[i - 1].position.ToVector3();
                        Vector3 currPos = nodes[i - 0].position.ToVector3();
                        float distance = 0;

                        bool working = false;
                        for (int p = 0; p < points.Count; ++p)
                        {
                            if (points[p].ToVector3() == prevPos)
                            {
                                working = true;
                                continue;
                            }
                            if (working)
                            {
                                float d = Vector3.Distance(points[p].ToVector3(), points[p - 1].ToVector3());
                                distance += d;
                                if (distance > traveled)
                                {
                                    float prevDistance = distance - d;
                                    float lerp = (traveled - prevDistance) / d;
                                    return points[p - 1].ToVector3() + (points[p].ToVector3() - points[p - 1].ToVector3()) * lerp;
                                }
                            }
                            if (points[p].ToVector3() == currPos)
                            {
                                break;
                            }
                        }
                    }
                }

                return points[points.Count - 1].ToVector3();
            }
            /// <summary>
            /// Get target position at time
            /// </summary>
            /// <param name="time"></param>
            /// <param name="pinPosition">pin position</param>
            /// <param name="shotPosition">shot position</param>
            /// <param name="teePosition">tee position</param>
            /// <returns></returns>
            public Vector3 GetTargetPosition(float time, Vector3 pinPosition, Vector3 shotPosition, Vector3 teePosition)
            {
                float local = 0;
                for (int i = 1; i < nodes.Count; ++i)
                {
                    float delta = nodes[i - 1].time;
                    local += delta;
                    if (local > time)
                        return Vector3.Lerp(nodes[i - 1].TargetPosition(holeIndex, nodes, pinPosition, shotPosition, teePosition), nodes[i].TargetPosition(holeIndex, nodes, pinPosition, shotPosition, teePosition), nodes[i - 1].RotationCurve.Evaluate((time - (local - delta)) / delta));
                }

                return nodes[nodes.Count - 1].TargetPosition(holeIndex, nodes, pinPosition, shotPosition, teePosition);
            }
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "FlyBy Info Position Change");
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "FlyBy Info HoleIndex Change");
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
                    if (PlatformBase.IO.IsEditor) PlatformBase.Editor.RecordObject(gameObject, "FlyBy Info OrderIndex Change");
                    info.orderIndex = value;
                }
            }
        }

        /// <summary>
        /// FlyBy fly time
        /// </summary>
        public float Time
        {
            get
            {
                float time = 0;

                for (int i = 0; i < Nodes.Count - 1; ++i)
                {
                    time += Nodes[i].time;
                }

                return time;
            }
        }
        /// <summary>
        /// List of all flyby nodes
        /// </summary>
        public List<Info.Node> Nodes
        {
            get
            {
                return info.nodes;
            }
        }
        /// <summary>
        /// Get node points
        /// </summary>
        public Vector3[] Points
        {
            get
            {
                if (info.Points.Length == 0) info.UpdatePoints();
                return info.Points;
            }
        }
        /// <summary>
        /// FlyBy type
        /// </summary>
        public Info.Type Type
        {
            get
            {
                return info.type;
            }
            set
            {
                info.type = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get flyby camera position
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Vector3 GetPosition(float time)
        {
            return info.GetPosition(time);
        }
        /// <summary>
        /// Get flyby target position
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pinPosition"></param>
        /// <param name="shotPosition"></param>
        /// <param name="teePosition"></param>
        /// <returns></returns>
        public Vector3 GetTargetPosition(float time, Vector3 pinPosition, Vector3 shotPosition, Vector3 teePosition)
        {
            return info.GetTargetPosition(time, pinPosition, shotPosition, teePosition);
        }
        /// <summary>
        /// Update flyby points and time
        /// </summary>
        public void UpdateFlyBy()
        {
            info.UpdatePoints();
            info.UpdateTimes();
        }
        #endregion
    }
}