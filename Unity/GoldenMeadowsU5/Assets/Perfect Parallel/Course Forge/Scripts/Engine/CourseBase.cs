using UnityEngine;
using System;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Course related base class
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("Perfect Parallel/Course Forge/Course/Course")]
    public partial class CourseBase : MonoBehaviour
    {
        #region Fields
        public const string version = " 1.4";
        static CourseBase self = null;
        #endregion

        #region External Methods
        /// <summary>
        /// Initialize course base
        /// </summary>
        /// <returns></returns>
        public static bool Initialize()
        {
            if (PlatformBase.IO == null) return false;
            if (PlatformBase.IO.IsSystemIO)
            {
                PlatformBase.IO.CreateDirectory(PlatformBase.IO.DocumentsPath);
                PlatformBase.IO.CreateDirectory(PlatformBase.IO.CoursesPath);
                PlatformBase.IO.CreateDirectory(PlatformBase.IO.ConfigPath);
                PlatformBase.IO.CreateDirectory(PlatformBase.IO.LibrariesPath);
                PlatformBase.IO.CreateDirectory(PlatformBase.IO.VideosPath);
            }

            if (PlatformBase.IO.IsWeb)
            {
                if (CheckSelf() == false) return false;
            }
            else if (PlatformBase.IO.IsPlaying)
            {
                if (self) CheckLibraryGameLayers();
            }
            else if (PlatformBase.IO.IsEditor)
            {
                CheckLegacy();
                if (CheckSelf() == false) return false;
                if (LoadLibrary() == false) return false;
                ForceUpdate();
                CheckCamera();
            }

            if (self)
            {
                CheckSplines();
                CheckHideFlags();
            }

            return true;
        }
        /// <summary>
        /// Build course file
        /// <param name="buildTarget">int of build platform</param>
        /// </summary>
        public static void BuildCourse(int buildTarget)
        {
            Debug.ClearDeveloperConsole();

            if (TerrainLowered) RestoreTerrain();

            string name = PlatformBase.Editor.SceneName.ToLower();
            string folder = PlatformBase.IO.CoursesPath + "/" + name + "/";

            Info.tees.Clear();
            Info.pins.Clear();
            Info.shots.Clear();
            Info.flyBys.Clear();

            Info.shots.Clear();
            for (int i = 0; i < Shots.Count; ++i)
                Info.shots.Add(Shots[i].PlantInfo as ShotBase.Info);

            Info.pins.Clear();
            for (int i = 0; i < Pins.Count; ++i)
                Info.pins.Add(Pins[i].PlantInfo as PinBase.Info);

            Info.tees.Clear();
            for (int i = 0; i < Tees.Count; ++i)
                Info.tees.Add(Tees[i].PlantInfo as TeeBase.Info);

            PlatformBase.Editor.ExecuteMenuItem("Perfect Parallel/Course Forge Off");
            PlatformBase.IO.CreateEmptyDirectory(folder);
            PlatformBase.IO.WriteText(folder + name + ".description", Utility.JsonWrite(Info));

            if (Info.Splash) PlatformBase.IO.CopyFile(PlatformBase.Editor.ObjectToPath(Info.Splash), folder + Info.splashName);
            if (Info.Cameo) PlatformBase.IO.CopyFile(PlatformBase.Editor.ObjectToPath(Info.Cameo), folder + Info.cameoName);

            PlatformBase.Editor.BuildSceneAssetBundle(PlatformBase.Editor.ScenePath, folder, buildTarget);
            PlatformBase.IO.DeleteFile(folder + name);
            PlatformBase.IO.DeleteFile(folder + name + ".manifest");
            PlatformBase.IO.DeleteFile(folder + name + ".unity3d.manifest");
            Debug.Log("CourseForge {build " + name + "} " + DateTime.Now.ToString("dd MMM HH:mm:ss"));
            PlatformBase.Editor.ExitGUI();
        }
        /// <summary>
        /// Refresh course
        /// </summary>
        public static void RefreshCourse()
        {
            ReparentSplines();

            List<SplineBase> splines = Curves;
            for (int s = 0; s < splines.Count; ++s)
            {
                if (splines[s].Info.locked) continue;

                Vector3[] points = splines[s].Points;
                for (int i = 0; i < points.Length; ++i)
                    if (Mathf.Abs(points[i].y - TerrainHeight(points[i].x, points[i].z)) > 0.1f)
                    {
                        for (int j = 0; j < points.Length; ++j) points[j].y = TerrainHeight(points[j].x, points[j].z);
                        splines[s].UpdateLine();
                        splines[s].LineChanged();
                        break;
                    }
            }

            List<TeeBase> tees = Tees;
            for (int i = 0; i < tees.Count; ++i)
            {
                Vector3 position = tees[i].Position;
                position.y = TerrainHeight(position.x, position.z);

                tees[i].Position = position;
                tees[i].PlantInfo.position = position;
            }

            List<ShotBase> shots = Shots;
            for (int i = 0; i < shots.Count; ++i)
            {
                Vector3 position = shots[i].Position;
                position.y = TerrainHeight(position.x, position.z);

                shots[i].Position = position;
                shots[i].PlantInfo.position = position;
            }

            List<PinBase> pins = Pins;
            for (int i = 0; i < pins.Count; ++i)
            {
                Vector3 position = pins[i].Position;
                position.y = TerrainHeight(position.x, position.z);

                pins[i].Position = position;
                pins[i].PlantInfo.position = position;
            }


            List<MeasureBase> measures = Measures;
            for (int i = 0; i < measures.Count; ++i)
            {
                measures[i].StartInfo.position.y = TerrainHeight(measures[i].StartInfo.position.x, measures[i].StartInfo.position.z);
                measures[i].EndInfo.position.y = TerrainHeight(measures[i].EndInfo.position.x, measures[i].EndInfo.position.z);
            }
        }
        #endregion

        #region MenuItem Methods
        [ContextMenu("DEBUG ON")]
        protected void DebugOn()
        {
            CheckHideFlags(HideFlags.None);
        }
        [ContextMenu("DEBUG OFF")]
        protected void DebugOff()
        {
            CheckHideFlags(HideFlags.HideInHierarchy);
        }
        #endregion

        #region Base Methods
        void OnEnable()
        {
            self = this;
            ForceUpdate();
        }
        void OnDisable()
        {
            self = null;
            ForceUpdate();
        }
        void Update()
        {
            if (PlatformBase.IO != null && PlatformBase.IO.IsPlaying == false) ForceUpdate();
        }
        #endregion

        #region Support Methods
        static bool CheckSelf()
        {
            if (self != null) return true;

            CourseBase[] courseBase = (CourseBase[])GameObject.FindObjectsOfType(typeof(CourseBase));
            if (courseBase.Length == 0)
            {
                TerrainControllerBase[] terrainControllers = (TerrainControllerBase[])GameObject.FindObjectsOfType(typeof(TerrainControllerBase));
                if (terrainControllers.Length == 0)
                {
                    Terrain[] terrains = (Terrain[])GameObject.FindObjectsOfType(typeof(Terrain));
                    if (terrains.Length == 0)
                    {
                        if (PlatformBase.IO.IsEditor) PlatformBase.Editor.OkDialog("Initialization.", "Can't find Terrain. Shutting down Course Forge.");
                        return false;
                    }
                    else
                    {
                        (terrains[0].gameObject.AddComponent(Types.GetType("PerfectParallel.CourseForge.TerrainController", "Assembly-CSharp")) as TerrainControllerBase).Terrain = terrains[0];
                        terrains[0].gameObject.AddComponent(Types.GetType("PerfectParallel.CourseForge.Course", "Assembly-CSharp"));
                    }
                }
                else
                {
                    terrainControllers[0].gameObject.AddComponent(Types.GetType("PerfectParallel.CourseForge.Course", "Assembly-CSharp"));
                }
            }
            courseBase = (CourseBase[])GameObject.FindObjectsOfType(typeof(CourseBase));
            courseBase[0].OnEnable();

            return true;
        }
        static void CheckLegacy()
        {
            CheckLegacyConvert("PerfectParallel.CourseForge.Course", "None", "PerfectParallel.CourseForge.CourseBase", "ppCourse");
            CheckLegacyConvert("PerfectParallel.CourseForge.FlyBy", "None", "PerfectParallel.CourseForge.FlyByBase", "ppFlyBy");
            CheckLegacyConvert("PerfectParallel.CourseForge.Hazard", "Spline", "PerfectParallel.CourseForge.HazardBase");
            CheckLegacyConvert("PerfectParallel.CourseForge.Measure", "None", "PerfectParallel.CourseForge.MeasureBase", "ppMeasure");
            CheckLegacyConvert("PerfectParallel.CourseForge.Pin", "None", "PerfectParallel.CourseForge.PinBase", "ppPin");
            CheckLegacyConvert("PerfectParallel.CourseForge.Shot", "None", "PerfectParallel.CourseForge.ShotBase", "ppShot");
            CheckLegacyConvert("PerfectParallel.CourseForge.Spline", "Hazard", "PerfectParallel.CourseForge.SplineBase", "ppSpline");
            CheckLegacyConvert("PerfectParallel.CourseForge.Tee", "None", "PerfectParallel.CourseForge.TeeBase", "ppTee");
            CheckLegacyConvert("PerfectParallel.CourseForge.TerrainController", "None", "PerfectParallel.CourseForge.TerrainControllerBase", "ppTerrain");
        }
        static void CheckLegacyConvert(string toType, string ignoreType, params string[] fromType)
        {
            for (int i = 0; i < fromType.Length; ++i)
            {
                Type typeFrom = fromType[i].Contains("Base") ? Type.GetType(fromType[i]) : Types.GetType(fromType[i], "Assembly-CSharp");
                Type typeTo = Types.GetType(toType, "Assembly-CSharp");

                if (typeFrom == null || typeTo == null) continue;

                UnityEngine.Object[] objects = GameObject.FindObjectsOfType(typeFrom);
                for (int j = 0; j < objects.Length; ++j)
                {
                    if (!objects[j].GetType().Name.Contains(ignoreType) && objects[j].GetType() != typeTo)
                    {
                        if (PlatformBase.IO.IsEditor)
                        {
                            PlatformBase.Editor.CopySerialized(objects[j], ((Component)objects[j]).gameObject.AddComponent(typeTo));
                            DestroyImmediate(objects[j]);
                        }
                    }
                }
            }
        }
        static void CheckCamera()
        {
            if (Camera.main == null)
            {
                Camera camera = new GameObject("MainCamera").AddComponent<Camera>();
                camera.tag = "MainCamera";
            }
        }
        static void CheckSplines()
        {
            List<HazardBase> hazards = Hazards;
            for (int i = 0; i < hazards.Count; ++i)
            {
                HazardBase hazard = hazards[i];

                if (hazard.Info.layerName == "NoLayerName") hazard.Info.layerName = GetLayer(hazard.Info.layerIndex, Layers).name;

                if (hazard.Info.colorNames.Length == 0)
                {
                    hazard.Info.colorNames = new string[hazard.Info.colorIndex.Length];
                    for (int j = 0; j < hazard.Info.colorNames.Length; ++j)
                        hazard.Info.colorNames[j] = GetLayer(hazard.Info.colorIndex[j], HazardLayers).name;
                }
            }

            List<SplineBase> splines = Splines;
            for (int i = 0; i < splines.Count; ++i)
            {
                SplineBase spline = splines[i];

                if (spline.Info.pin)
                {
                    MonoBehaviour.DestroyImmediate(spline.gameObject);
                    splines.Remove(spline);
                    i = -1;
                    continue;
                }

                if (spline.Info.layerName == "NoLayerName") spline.Info.layerName = GetLayer(spline.Info.layerIndex, Layers).name;
            }
        }
        #endregion
    }
}