using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
    public partial class CourseBase
    {
        #region Create Methods
        /// <summary>
        /// Creates pin
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="difficulty"></param>
        /// <param name="position"></param>
        public static void CreatePin(int holeIndex, PinBase.Info.Difficulty difficulty, Vector3 position)
        {
            PinBase pin = (PinBase)new GameObject(GetPinName(holeIndex, FreePlantIndex(Pins, holeIndex), difficulty)).AddComponent(Types.GetType("PerfectParallel.CourseForge.Pin", "Assembly-CSharp"));
            pin.Position = position;
            pin.Difficulty = difficulty;
            pin.HoleIndex = holeIndex;
            pin.OrderIndex = FreePlantIndex(Pins, holeIndex);

            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.RegisterCreatedObjectUndo(pin.gameObject, "Plant tool Creation");
                pin.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }
        /// <summary>
        /// Creates tee
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="type"></param>
        /// <param name="par"></param>
        /// <param name="strokeIndex"></param>
        /// <param name="width">random box width</param>
        /// <param name="height">random box height</param>
        public static void CreateTee(int holeIndex, TeeBase.Info.Type type, TeeBase.Info.Par par, int strokeIndex, float width, float height)
        {
            TeeBase tee = (TeeBase)new GameObject(GetTeeName(holeIndex, type, par, strokeIndex)).AddComponent(Types.GetType("PerfectParallel.CourseForge.Tee", "Assembly-CSharp"));
            tee.Position = CameraTerrainHit.Value.point;
            tee.Type = type;
            tee.Par = par;
            tee.StrokeIndex = strokeIndex;
            tee.Width = width;
            tee.Height = height;
            tee.HoleIndex = holeIndex;
            tee.OrderIndex = FreePlantIndex(Tees, holeIndex);

            if (PlatformBase.IO.IsEditor) 
            {
                PlatformBase.Editor.RegisterCreatedObjectUndo(tee.gameObject, "Plant tool Creation");
                tee.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }
        /// <summary>
        /// Creates shot
        /// </summary>
        /// <param name="holeIndex"></param>
        public static void CreateShot(int holeIndex)
        {
            int orderIndex = FreePlantIndex(Shots, holeIndex);

            ShotBase shot = (ShotBase)new GameObject(GetShotName(holeIndex, orderIndex)).AddComponent(Types.GetType("PerfectParallel.CourseForge.Shot", "Assembly-CSharp"));
            shot.Position = CameraTerrainHit.Value.point;
            shot.HoleIndex = holeIndex;
            shot.OrderIndex = orderIndex;

            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.RegisterCreatedObjectUndo(shot.gameObject, "Plant tool Creation");
                shot.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }
        /// <summary>
        /// Creates measure
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="units"></param>
        public static void CreateMeasure(int holeIndex, Vector3 startPosition, Vector3 endPosition, Units units)
        {
            MeasureBase measure = (MeasureBase)new GameObject(GetMeasureName(holeIndex, FreePlantIndex(Measures, holeIndex))).AddComponent(Types.GetType("PerfectParallel.CourseForge.Measure", "Assembly-CSharp"));
            measure.HoleIndex = holeIndex;
            measure.OrderIndex = FreePlantIndex(Measures, holeIndex);
            measure.StartInfo.position.Set(startPosition);
            measure.EndInfo.position.Set(endPosition);
            measure.Units = units;

            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.RegisterCreatedObjectUndo(measure.gameObject, "Plant tool Creation");
                measure.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }
        /// <summary>
        /// Creates flyby
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="points"></param>
        /// <param name="type"></param>
        public static void CreateFlyBy(int holeIndex, Vector3[] points, FlyByBase.Info.Type type)
        {
            FlyByBase flyBy = (FlyByBase)new GameObject(GetFlyByName(holeIndex, FreePlantIndex(FlyBys, holeIndex), type)).AddComponent(Types.GetType("PerfectParallel.CourseForge.FlyBy", "Assembly-CSharp"));
            flyBy.HoleIndex = holeIndex;
            flyBy.OrderIndex = FreePlantIndex(FlyBys, holeIndex);
            flyBy.Type = type;

            List<FlyByBase.Info.Node> nodes = new List<FlyByBase.Info.Node>();
            for (int i = 0; i < points.Length; ++i)
            {
                FlyByBase.Info.Node node = new FlyByBase.Info.Node();
                node.position.Set(points[i]);

                Vector3 right = Vector3.right;
                float distance = 3.0f;

                if (i != points.Length - 1)
                {
                    right = Vector3.Cross((points[i + 1] - points[i]).normalized, Vector3.up);
                    distance = (points[i + 1] - points[i]).magnitude / 2;
                }

                node.probe.position.Set(node.position.ToVector3() + right * distance);
                nodes.Add(node);
            }

            (flyBy.PlantInfo as FlyByBase.Info).nodes = nodes;
            (flyBy.PlantInfo as FlyByBase.Info).UpdatePoints();

            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.RegisterCreatedObjectUndo(flyBy.gameObject, "Plant tool Creation");
                flyBy.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }

        /// Get name for the tee
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="type"></param>
        /// <param name="par"></param>
        /// <param name="strokeIndex"></param>
        /// <returns></returns>
        public static string GetTeeName(int holeIndex, TeeBase.Info.Type type, TeeBase.Info.Par par, int strokeIndex)
        {
            return "H" + (holeIndex + 1) + "\\" + type.ToString() + " PAR" + par.ToString().Replace("_", "") + " S" + strokeIndex.ToString();
        }
        /// <summary>
        /// Get name for the shot
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="orderIndex"></param>
        /// <returns></returns>
        public static string GetShotName(int holeIndex, int orderIndex)
        {
            return "H" + (holeIndex + 1) + "\\Shot " + (orderIndex + 1);
        }
        /// <summary>
        /// Get name for the pin
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="orderIndex"></param>
        /// <param name="difficulty"></param>
        /// <returns></returns>
        public static string GetPinName(int holeIndex, int orderIndex, PinBase.Info.Difficulty difficulty)
        {
            return "H" + (holeIndex + 1) + "\\" + difficulty + " " + (orderIndex + 1);
        }
        /// <summary>
        /// Get name for the measure
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="orderIndex"></param>
        /// <returns></returns>
        public static string GetMeasureName(int holeIndex, int orderIndex)
        {
            return "H" + (holeIndex + 1) + "\\Measure " + (orderIndex + 1);
        }
        /// <summary>
        /// Get name for the flyby
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="orderIndex"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFlyByName(int holeIndex, int orderIndex, FlyByBase.Info.Type type)
        {
            return "H" + (holeIndex + 1) + "\\FlyBy " + (orderIndex + 1) + type.ToString();
        }

        /// <summary>
        /// Gets the tee marker prefab
        /// </summary>
        /// <param name="type">tee type</param>
        /// <returns></returns>
        public static GameObject GetTeeMarker(TeeBase.Info.Type type)
        {
            if (type == TeeBase.Info.Type.Championship && Info.championshipTeeMarker) return Info.championshipTeeMarker;
            if (type == TeeBase.Info.Type.Tournament && Info.tournamentTeeMarker) return Info.tournamentTeeMarker;
            if (type == TeeBase.Info.Type.Back && Info.backTeeMarker) return Info.backTeeMarker;
            if (type == TeeBase.Info.Type.Member && Info.memberTeeMarker) return Info.memberTeeMarker;
            if (type == TeeBase.Info.Type.Forward && Info.forwardTeeMarker) return Info.forwardTeeMarker;
            if (type == TeeBase.Info.Type.Ladies && Info.ladiesTeeMarker) return Info.ladiesTeeMarker;
            if (type == TeeBase.Info.Type.Challenge && Info.challengeTeeMarker) return Info.challengeTeeMarker;
            return null;
        }
        #endregion

        #region Editor Methods
        /// <summary>
        /// Free tee types for the hole
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <returns></returns>
        public static string[] FreeTees(int holeIndex)
        {
            List<string> names = new List<string>(Enum.GetNames(typeof(TeeBase.Info.Type)));

            for (int i = 0; i < Tees.Count; ++i)
                if (Tees[i].HoleIndex == holeIndex) names.Remove(Tees[i].Type.ToString());

            return names.ToArray();
        }
        /// <summary>
        /// Free tee stroke index for the hole
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <param name="teeType"></param>
        /// <returns></returns>
        public static string[] FreeTeesStrokeIndex(int holeIndex, TeeBase.Info.Type teeType)
        {
            List<string> names = new List<string>(HoleNames);

            for (int i = 0; i < Tees.Count; ++i)
                if (Tees[i].Type == teeType && Tees[i].StrokeIndex != -1 && names.Contains(Tees[i].StrokeIndex.ToString())) names.Remove(Tees[i].StrokeIndex.ToString());

            return names.ToArray();
        }
        /// <summary>
        /// Free flyby for the hole
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <returns></returns>
        public static string[] FreeFlyBys(int holeIndex)
        {
            List<string> names = new List<string>(Enum.GetNames(typeof(FlyByBase.Info.Type)));
            for (int i = 0; i < FlyBys.Count; ++i)
                if (FlyBys[i].HoleIndex == holeIndex) names.Remove(FlyBys[i].Type.ToString());

            return names.ToArray();
        }
        #endregion

        #region Support Methods
        [Obfuscation(Exclude = true)]
        static int FreePlantIndex<T>(List<T> plants, int holeIndex) where T : IPlant
        {
            int i = 0;
            for (;;)
            {
                int initialI = i;

                for (int p = 0; p < plants.Count; ++p)
                    if (plants[p].HoleIndex == holeIndex && plants[p].OrderIndex == i)
                    {
                        i++;
                        break;
                    }

                if (initialI == i)
                    break;
            }

            return i;
        }
        #endregion
    }
}