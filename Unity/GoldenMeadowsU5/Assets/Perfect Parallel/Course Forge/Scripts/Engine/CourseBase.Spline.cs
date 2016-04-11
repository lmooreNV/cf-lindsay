using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
    public partial class CourseBase
    {
        [Serializable]
        public class HazardHit
        {
            #region Fields
            public bool water = false;
            public HazardBase.Type type = HazardBase.Type.None;
            public Vector3Object point = Vector3.zero;
            public int pointIndex = -1;
            public Vector3Object point2 = Vector3.zero;
            public int InstanceID = -1;
            #endregion
        }

        #region Fields
        static List<SplineBase> buildLineSplines = new List<SplineBase>();
        static List<SplineBase> buildSplines = new List<SplineBase>();
        static int buildSplinesCount = 0;
        #endregion

        #region Properties
        /// <summary>
        /// List of splines, prepared to build their line data
        /// </summary>
        public static List<SplineBase> BuildLineSplines
        {
            get
            {
                return buildLineSplines;
            }
            set
            {
                buildLineSplines = value;
            }
        }
        /// <summary>
        /// List of all splines, prepared to build their mesh data
        /// </summary>
        public static List<SplineBase> BuildSplines
        {
            get
            {
                return buildSplines;
            }
            set
            {
                buildSplines = value;
            }
        }
        /// <summary>
        /// Count of splines that are in process of build
        /// </summary>
        public static int BuildSplinesCount
        {
            get
            {
                return buildSplinesCount;
            }
            set
            {
                buildSplinesCount = value;
            }
        }

        /// <summary>
        /// Array of all hazard colors
        /// </summary>
        public static Color[] HazardColors
        {
            get
            {
                return Layers.Where(x => IsHazard(x.name)).Select(x => x.hazardColor.ToColor()).ToArray();
            }
        }
        #endregion

        #region Create Methods
        /// <summary>
        /// Creates spline
        /// </summary>
        /// <param name="points"></param>
        /// <param name="layer"></param>
        /// <param name="flags"></param>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public static SplineBase CreateSpline(Vector3[] points, Layer layer, SplineBase.SplineInfo.Flags flags, int instanceID)
        {
            GameObject gameObject = new GameObject("Spline " + instanceID.ToString());
            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.RegisterCreatedObjectUndo(gameObject, "Spline tool Creation");
                PlatformBase.Editor.SetStaticFlags(gameObject);
                gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

            SplineBase spline = (SplineBase)gameObject.AddComponent(Types.GetType("PerfectParallel.CourseForge.Spline", "Assembly-CSharp"));
            spline.Info.layerName = layer.name;
            spline.Points = (Vector3[])points.Clone();
            spline.Info.flags = (int)flags;
            spline.Info.InstanceID = instanceID;

            spline.UpdateLine();
            splines.Add(spline);
            spline.LineChanged();

            return spline;
        }
        /// <summary>
        /// Creates spline
        /// </summary>
        /// <param name="points"></param>
        /// <param name="layer"></param>
        /// <param name="flags"></param>
        /// <param name="rearrange"></param>
        /// <returns></returns>
        public static SplineBase CreateSpline(Vector3[] points, Layer layer, SplineBase.SplineInfo.Flags flags, bool rearrange = false)
        {
            if ((flags & SplineBase.SplineInfo.Flags.Square) == 0 || rearrange) points = points.ToList().RearrangePoints(layer.metersPerOnePoint).ToArray();
            if (points.Length < 3) return null;

            SplineBase spline = CreateSpline(points, layer, flags, FreeSplineInstanceID());

            spline.UpdateLine();
            splines.Add(spline);
            spline.LineChanged();

            return spline;
        }
        /// <summary>
        /// Creates spline
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static SplineBase CreateSpline(SplineBase.SplineInfo info)
        {
            SplineBase spline = CreateSpline(info.points.ToVector3(), CourseBase.GetLayer(info.layerName, SplineLayers), (SplineBase.SplineInfo.Flags)info.flags, info.InstanceID);
            spline.Info.pin = info.pin;

            spline.UpdateLine();
            splines.Add(spline);
            spline.LineChanged();

            return spline;
        }

        /// <summary>
        /// Creates hazard
        /// </summary>
        /// <param name="points"></param>
        /// <param name="layer"></param>
        /// <param name="flags"></param>
        /// <param name="rearrange"></param>
        /// <returns></returns>
        public static HazardBase CreateHazard(Vector3[] points, Layer layer, SplineBase.SplineInfo.Flags flags, bool rearrange = false)
        {
            if ((flags & SplineBase.SplineInfo.Flags.Square) == 0 || rearrange) points = points.ToList().RearrangePoints(layer.metersPerOnePoint).ToArray();
            if (points.Length < 3) return null;

            int instanceID = FreeSplineInstanceID();

            GameObject gameObject = new GameObject("Hazard " + instanceID.ToString());
            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.RegisterCreatedObjectUndo(gameObject, "Spline tool Creation");
                PlatformBase.Editor.SetStaticFlags(gameObject);
                gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

            HazardBase hazard = (HazardBase)gameObject.AddComponent(Types.GetType("PerfectParallel.CourseForge.Hazard", "Assembly-CSharp"));
            hazard.Info.layerName = layer.name;
            hazard.Points = (Vector3[])points.Clone();

            hazard.Info.colorNames = new string[points.Length];
            for (int i = 0; i < hazard.Info.colorNames.Length; ++i) hazard.Info.colorNames[i] = layer.name;

            hazard.Info.flags = (int)flags;
            hazard.Info.InstanceID = instanceID;

            hazard.UpdateLine();
            splines.Add(hazard);
            hazard.LineChanged();

            return hazard;
        }

        /// <summary>
        /// Creates pin hole spline
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static SplineBase CreatePinHole(Vector3 position)
        {
            Vector3Object[] points = MakeCircle(position);

            SplineBase.SplineInfo info = new SplineBase.SplineInfo();
            info.layerName = GetPinLayer().name;
            info.points = points;
            info.InstanceID = FreeSplineInstanceID();

            SplineBase spline = CreateSpline(info);
            spline.Info.pin = true;
            return spline;
        }
        /// <summary>
        /// Creates pin lip hole spline
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static SplineBase CreatePinLipHole(Vector3 position)
        {
            Vector3Object[] points = MakeCircle(position, Utility.holeLip);

            SplineBase.SplineInfo info = new SplineBase.SplineInfo();
            info.layerName = GetPinLipLayer().name;
            info.points = points;
            info.InstanceID = FreeSplineInstanceID();

            SplineBase spline = CreateSpline(info);
            spline.Info.pin = true;
            return spline;
        }
        #endregion

        #region Update Methods
        /// <summary>
        /// Reparents splines
        /// </summary>
        public static void ReparentSplines()
        {
            List<SplineBase> splines = Splines;
            for (int i = 0; i < splines.Count; ++i)
            {
                SplineBase spline = splines[i];
                SplineBase cachedParent = null;
                Transform cachedParentTransform = null;

                for (int j = 0; j < splines.Count; ++j)
                {
                    if (i == j) continue;

                    SplineBase j_Parent = splines[j];
                    if (IsSplineParent(spline, j_Parent))
                    {
                        if (cachedParent == null)
                        {
                            cachedParent = j_Parent;
                            cachedParentTransform = j_Parent.Transform;
                        }
                        else if (!IsSplineParent(cachedParent, j_Parent))
                        {
                            cachedParent = j_Parent;
                            cachedParentTransform = j_Parent.Transform;
                        }
                    }
                }
                spline.transform.parent = cachedParentTransform;
            }

            List<HazardBase> hazards = Hazards;
            for (int i = 0; i < hazards.Count; ++i)
            {
                hazards[i].transform.parent = null;
            }
        }
        /// <summary>
        /// Start baking splines
        /// </summary>
        /// <param name="force">force to build all</param>
        public static void BakeSplines(bool force = false)
        {
            RefreshCourse();

            int count = 0;
            List<SplineBase> splines = Curves;
            for (int i = 0; i < splines.Count; ++i)
            {
                if (ActiveSpline && splines[i] != ActiveSpline)
                {
                    bool special = false;
                    if (IsSplineParent(splines[i], ActiveSpline)) special = true;
                    if (splines[i] == ActiveSpline.Parent) special = true;
                    if (!special) continue;
                }

                if (force) splines[i].SetRefresh();

                if (splines[i].Lines.NeedMeshRefresh || splines[i].Renderer.NeedMaterialRefresh || splines[i].Info.locked)
                {
                    buildLineSplines.Add(splines[i]);
                    buildSplines.Add(splines[i]);
                    count++;
                }
            }
            buildSplinesCount = buildSplines.Count;

            if (count > 10)
                if (PlatformBase.IO.IsEditor)
                    if (!PlatformBase.Editor.OkCancelDialog("Spline creation", "There are " + count + " splines to update, do you want to proceed?"))
                    {
                        BuildLineSplines.Clear();
                        BuildSplines.Clear();
                        BuildSplinesCount = 0;
                        return;
                    }
        }
        /// <summary>
        /// Do spline bake step
        /// <returns></returns>
        /// </summary>
        public static Exception DoSplineBakeStep()
        {
            Exception exception = null;

            if (true)
            {
                List<SplineBase> splines = buildLineSplines;
                if (splines.Count != 0)
                {
                    try
                    {
                        if (splines[0].Lines.NeedMeshRefresh || splines[0].Info.locked) splines[0].UpdateLine();
                        if (splines[0].Lines.NeedMeshRefresh) splines[0].FormLines();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    finally
                    {
                        buildLineSplines.RemoveAt(0);
                    }
                    return exception;
                }
            }
            if (true)
            {
                List<SplineBase> splines = buildSplines;
                if (splines.Count != 0)
                {
                    try
                    {
                        if (splines[0].Lines.NeedMeshRefresh) splines[0].UpdateMesh();
                        if (splines[0].Renderer.NeedMaterialRefresh) splines[0].UpdateMaterial();
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    finally
                    {
                        buildSplines.RemoveAt(0);
                    }
                    return exception;
                }
            }

            buildSplinesCount = 0;
            return exception;
        }
        #endregion

        #region Hazard Methods
        /// <summary>
        /// Is name is hazard?
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsHazard(string name)
        {
            return HazardType(name) != HazardBase.Type.None;
        }
        /// <summary>
        /// Hazard type from name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HazardBase.Type HazardType(string name)
        {
            if (name.Contains("Out of Bounds")) return HazardBase.Type.Out_of_Bounds;
            if (name.Contains("Lateral Water Hazard")) return HazardBase.Type.Lateral_Water_Hazard;
            if (name.Contains("Water Hazard")) return HazardBase.Type.Water_Hazard;
            if (name.Contains("Free Drop")) return HazardBase.Type.Free_Drop;
            return HazardBase.Type.None;
        }
        /// <summary>
        /// Raycast against all hazards
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static HazardHit RaycastHazards(Ray ray, float distance = 10)
        {
            ray.direction = new Vector3(ray.direction.x, 0, ray.direction.z).normalized;

            HazardHit hit = new HazardHit();
            bool hitted = false;
            bool insideOOB = false;

            List<HazardBase> hazards = Hazards;
            for (int i = 0; i < hazards.Count; ++i)
            {
                HazardHit temp = new HazardHit();
                if (hazards[i].Raycast(ray, temp, distance))
                {
                    switch (temp.type)
                    {
                        case HazardBase.Type.Out_of_Bounds:
                            insideOOB = true;
                            break;

                        default:
                            hitted = true;
                            hit = temp;
                            break;
                    }
                }
            }

            if (hitted)
            {
                return hit;
            }
            else
            {
                if (insideOOB)
                {
                    return hit;
                }
                else
                {
                    Layer layer = CourseBase.GetLayer(Utility.GetName(HazardBase.Type.Out_of_Bounds), CourseBase.HazardLayers);
                    if (Hazards.Find(x => x.Layer == layer))
                    {
                        hit.type = HazardBase.Type.Out_of_Bounds;
                        hit.InstanceID = Hazards.Find(x => x.Layer == layer).Info.InstanceID;
                        return hit;
                    }
                    else
                    {
                        hit.type = HazardBase.Type.Out_of_Bounds;
                        return hit;
                    }
                }
            }
        }
        #endregion

        #region Support Methods
        static Vector3Object[] MakeCircle(Vector3 position, float radius = 0)
        {
            int edgeCount = 6;
            Vector3Object[] points = new Vector3Object[edgeCount];
            radius += Utility.holeRadius;

            for (int i = 0; i < edgeCount; ++i)
            {
                float angle = ((float)i / (float)edgeCount) * 2 * Mathf.PI;
                points[i] = new Vector3Object(position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius);
                points[i].y = TerrainHeight((float)points[i].x, (float)points[i].z);
            }

            return points;
        }
        static int FreeSplineInstanceID()
        {
            List<SplineBase> splines = Curves;
            for (int i = 0; i < int.MaxValue; ++i)
            {
                bool ok = true;
                for (int s = 0; s < splines.Count; ++s)
                {
                    if (splines[s].Info.InstanceID == i)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    return i;
                }
            }

            return -1;
        }
        static bool IsSplineParent(SplineBase spline, SplineBase parent)
        {
            if (parent.IsHazard) return false;
            Vector3 splineMin = spline.Lines.MinPoint;
            Vector3 splineMax = spline.Lines.MaxPoint;
            Vector3 parentMin = parent.Lines.MinPoint;
            Vector3 parentMax = parent.Lines.MaxPoint;

            if (splineMin.x < parentMin.x || splineMax.x > parentMax.x || splineMin.z < parentMin.z || splineMax.z > parentMax.z) return false;

            Vector3[] splineLinePoints = spline.Lines.Lines;
            Vector3[] parentSplineLinePoints = parent.Lines.Lines;

            for (int i = 0; i < splineLinePoints.Length; ++i)
                if (!splineLinePoints[i].IsInside(parentSplineLinePoints))
                    return false;

            return true;
        }
        #endregion
    }
}