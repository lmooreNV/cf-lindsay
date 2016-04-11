using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Hazard class
    /// </summary>
    [AddComponentMenu("Perfect Parallel/Course Forge/Spline/Hazard")]
    public class HazardBase : SplineBase
    {
        public enum Type
        {
            None = 0,
            Out_of_Bounds,
            Lateral_Water_Hazard,
            Water_Hazard,
            Free_Drop
        }

        #region Fields
        [SerializeField]
        List<GameObject> posts = new List<GameObject>();
        #endregion

        #region Transform Properties
        public override SplineBase Parent
        {
            get
            {
                return null;
            }
        }

        public override bool HasParent
        {
            get
            {
                return false;
            }
        }
        public override bool HasChilds
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region Spline properties
        public override bool IsHazard
        {
            get
            {
                return true;
            }
        }
        public override Layer Layer
        {
            get
            {
                return CourseBase.GetLayer(Info.layerName, CourseBase.HazardLayers);
            }
            set
            {
                Info.layerName = value.name;
            }
        }
        #endregion

        #region Methods
        public override void UpdateMaterial()
        {
            base.UpdateMaterial();

            for (int i = 0; i < posts.Count; ++i)
            {
                MonoBehaviour.DestroyImmediate(posts[i]);
                posts.RemoveAt(i);
                i = -1;
                continue;
            }

            Vector3[] path = Lines.Lines;
            Color[] colors = Lines.LinesColors;
            for (int i = 1; i < path.Length; ++i)
            {
                if (colors[i - 1] != colors[i])
                {
                    Layer layer1 = CourseBase.HazardLayers.Find(h => h.hazardColor == colors[i - 1]);
                    if (layer1 == null) continue;
                    GameObject prefab1 = layer1.HazardPost;
                    if (prefab1 == null) continue;

                    GameObject post1 = MonoBehaviour.Instantiate(prefab1) as GameObject;
                    post1.transform.parent = transform;
                    post1.transform.localPosition = path[i - 1];
                    post1.transform.forward = path[i].Direction(path[i - 1]);
                    post1.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                    posts.Add(post1);

                    Layer layer2 = CourseBase.HazardLayers.Find(h => h.hazardColor == colors[i]);
                    if (layer2 == null) continue;
                    GameObject prefab2 = layer2.HazardPost;
                    if (prefab2 == null) continue;

                    GameObject post2 = MonoBehaviour.Instantiate(prefab2) as GameObject;
                    post2.transform.parent = transform;
                    post2.transform.localPosition = path[i];
                    post2.transform.forward = path[i].Direction(path[i - 1]);
                    post2.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                    posts.Add(post2);
                }
            }

            float length = Lines.Lines.LineLength();
            for (float x = 0; x < length - Layer.metersPerOnePost; x += Layer.metersPerOnePost)
            {
                Vector3 position = Position(x);
                position.y = CourseBase.MeshHeight(position.x, position.z);

                if (posts.Find(h => (h.transform.position - position).magnitude < Layer.metersPerOnePost * 0.75f)) continue;

                Vector3 nextPosition = Position(x + 0.1f);
                nextPosition.y = CourseBase.MeshHeight(nextPosition.x, nextPosition.z);

                Color color = Color(x);
                Layer layer = CourseBase.HazardLayers.Find(h => h.hazardColor == color);
                if (layer == null) continue;
                GameObject prefab = CourseBase.HazardLayers.Find(h => h.hazardColor == color).HazardPost;
                if (prefab == null) continue;
                GameObject post = MonoBehaviour.Instantiate(prefab) as GameObject;
                post.transform.parent = transform;
                post.transform.localPosition = position;
                post.transform.forward = nextPosition.Direction2D(position);
                post.transform.localRotation *= Quaternion.Euler(-90, 0, 0);
                posts.Add(post);
            }
        }

        /// <summary>
        /// Raycast the hazard for intesections
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="hit"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
		public bool Raycast(Ray ray, CourseBase.HazardHit hit, float distance = 10)
        {
            Vector3[] points = Lines.Points;
            Color[] colors = Lines.Colors;

            if (!InBounds(ray.origin)) return false;
            if (!ray.origin.IsInside(points)) return false;

            distance = Mathf.Max(distance, Lines.Points.LineLength());
            Vector3 startPosition = ray.origin + ray.direction.normalized * distance;

            bool crossed = false;

            for (int i = 0; i < points.Length; ++i)
                if (MathUtility.IsSegmentSegment(ray.origin.ToVector2(), startPosition.ToVector2(), points.GetAt(i - 1).ToVector2(), points.GetAt(i).ToVector2()))
                {
                    string name = CourseBase.HazardLayers.Find(h => h.hazardColor == colors.GetAt(i - 1)).name;
                    if (CourseBase.IsHazard(name))
                    {
                        Vector3 point = MathUtility.LineLine(ray.origin.ToVector2(), startPosition.ToVector2(), points.GetAt(i - 1).ToVector2(), points.GetAt(i).ToVector2()).Value;
                        point = new Vector3(point.x, CourseBase.MeshHeight(point.x, point.y), point.y);

                        if (hit.point == Vector3.zero || (point - ray.origin).magnitude < (hit.point - ray.origin).magnitude)
                        {
                            hit.type = CourseBase.HazardType(name);
                            hit.point = point;
                            hit.pointIndex = i;
                            hit.InstanceID = Info.InstanceID;
                            crossed = true;
                        }
                    }
                }

            return crossed;
        }
        #endregion

        #region Support methods
        Vector3 Position(float traveled)
        {
            Vector3[] path = Lines.Lines;

            float distance = 0;
            for (int i = 1; i < path.Length; ++i)
            {
                float localDistance = (path[i] - path[i - 1]).magnitude;
                if (distance + localDistance < traveled)
                {
                    distance += localDistance;
                }
                else
                {
                    float delta = traveled - distance;
                    return path[i - 1] + (path[i] - path[i - 1]).normalized * delta;
                }
            }
            return path[path.Length - 1];
        }
        Color Color(float traveled)
        {
            Vector3[] path = Lines.Lines;
            Color[] colors = Lines.LinesColors;

            float distance = 0;
            for (int i = 1; i < path.Length; ++i)
            {
                float localDistance = (path[i] - path[i - 1]).magnitude;
                if (distance + localDistance < traveled)
                {
                    distance += localDistance;
                }
                else
                {
                    return colors[i - 1];
                }
            }
            return colors[colors.Length - 1];
        }
        #endregion
    }
}