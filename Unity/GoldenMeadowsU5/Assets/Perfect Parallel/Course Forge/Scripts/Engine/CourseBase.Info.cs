using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using JsonFx.Json;

namespace PerfectParallel.CourseForge
{
    public partial class CourseBase
    {
        [Serializable]
        public class CourseInfo
        {
            #region Fields
            [SerializeField]
            Texture splash = null;
            [SerializeField]
            Texture cameo = null;

            public string name = "Unknown";
            public string author = "Your Name";
            public string authorVersion = "1.00";
            public string description = "Unknown description";
            public string platform = "Unknown";

            public string splashName = "";
            public string cameoName = "";

            public float offset = 0.15f;
            public float geoX = 0, geoY = 0, geoZ = 0;
            public int utmZone = 0;

            public List<TeeBase.Info> tees = new List<TeeBase.Info>();
            public List<PinBase.Info> pins = new List<PinBase.Info>();
            public List<ShotBase.Info> shots = new List<ShotBase.Info>();
            public List<FlyByBase.Info> flyBys = new List<FlyByBase.Info>();

            [JsonIgnore]
            public GameObject championshipTeeMarker = null;
            [JsonIgnore]
            public GameObject tournamentTeeMarker = null;
            [JsonIgnore]
            public GameObject backTeeMarker = null;
            [JsonIgnore]
            public GameObject memberTeeMarker = null;
            [JsonIgnore]
            public GameObject forwardTeeMarker = null;
            [JsonIgnore]
            public GameObject ladiesTeeMarker = null;
            [JsonIgnore]
            public GameObject challengeTeeMarker = null;
            [JsonIgnore]
            public float championshipWidth = 0;
            [JsonIgnore]
            public float championshipHeight = 0;
            [JsonIgnore]
            public float tournamentWidth = 0;
            [JsonIgnore]
            public float tournamentHeight = 0;
            [JsonIgnore]
            public float backWidth = 0;
            [JsonIgnore]
            public float backHeight = 0;
            [JsonIgnore]
            public float memberWidth = 0;
            [JsonIgnore]
            public float memberHeight = 0;
            [JsonIgnore]
            public float forwardWidth = 0;
            [JsonIgnore]
            public float forwardHeight = 0;
            [JsonIgnore]
            public float ladiesWidth = 0;
            [JsonIgnore]
            public float ladiesHeight = 0;
            [JsonIgnore]
            public float challengeWidth = 0;
            [JsonIgnore]
            public float challengeHeight = 0;
            #endregion

            #region Properties
            /// <summary>
            /// Course splash image
            /// </summary>
            [JsonIgnore]
            public Texture Splash
            {
                get
                {
                    return splash;
                }
                set
                {
                    if (splash != value)
                    {
                        splash = value;
                        if (PlatformBase.IO.IsEditor) splashName = PlatformBase.IO.GetFileName(PlatformBase.Editor.ObjectToPath(value));
                    }
                }
            }
            /// <summary>
            /// Course cameo image
            /// </summary>
            [JsonIgnore]
            public Texture Cameo
            {
                get
                {
                    return cameo;
                }
                set
                {
                    if (cameo != value)
                    {
                        cameo = value;
                        if (PlatformBase.IO.IsEditor) cameoName = PlatformBase.IO.GetFileName(PlatformBase.Editor.ObjectToPath(value));
                    }
                }
            }
            #endregion
        }

        [Serializable]
        public class Hole
        {
            #region Fields
            public List<TeeBase> tees = new List<TeeBase>();
            public List<ShotBase> shots = new List<ShotBase>();
            public List<PinBase> pins = new List<PinBase>();
            public List<FlyByBase> flyBys = new List<FlyByBase>();
            #endregion
        }

        #region Fields
        static List<SplineBase> curves = new List<SplineBase>();
        static List<SplineBase> splines = new List<SplineBase>();
        static List<HazardBase> hazards = new List<HazardBase>();
        static List<TeeBase> tees = new List<TeeBase>();
        static List<ShotBase> shots = new List<ShotBase>();
        static List<PinBase> pins = new List<PinBase>();
        static List<MeasureBase> measures = new List<MeasureBase>();
        static List<FlyByBase> flyBys = new List<FlyByBase>();

        static List<Hole> holes = new List<Hole>();
        static Terrain terrain = null;

        static SplineBase activeSpline = null;
        static HazardBase activeHazard = null;
        static Transform activePlant = null;
        static TeeBase activeTee = null;
        static PinBase activePin = null;
        static MeasureBase activeMeasure = null;
        static FlyByBase activeFlyBy = null;
        static ShotBase activeShot = null;

        [SerializeField]
        CourseInfo info = new CourseInfo();
        #endregion

        #region Properties
        /// <summary>
        /// Gets all splines.
        /// </summary>
        /// <value>
        /// All splines.
        /// </value>
        public static List<SplineBase> Curves
        {
            get
            {
                for (int i = 0; i < curves.Count; ++i)
                {
                    if (curves[i] == null)
                    {
                        curves.Clear();
                        return curves;
                    }
                }

                return curves;
            }
        }
        /// <summary>
        /// List of all splines
        /// </summary>
        public static List<SplineBase> Splines
        {
            get
            {
                for (int i = 0; i < splines.Count; ++i)
                {
                    if (splines[i] == null)
                    {
                        splines.Clear();
                        return splines;
                    }
                }

                return splines;
            }
        }
        /// <summary>
        /// List of all hazards
        /// </summary>
        public static List<HazardBase> Hazards
        {
            get
            {
                for (int i = 0; i < hazards.Count; ++i)
                {
                    if (hazards[i] == null)
                    {
                        hazards.Clear();
                        return hazards;
                    }
                }
                return hazards;
            }
        }
        /// <summary>
        /// List of all tees
        /// </summary>
        public static List<TeeBase> Tees
        {
            get
            {
                for (int i = 0; i < tees.Count; ++i)
                {
                    if (tees[i] == null)
                    {
                        tees.Clear();
                        return tees;
                    }
                }

                return tees;
            }
        }
        /// <summary>
        /// List of all shots
        /// </summary>
        public static List<ShotBase> Shots
        {
            get
            {
                for (int i = 0; i < shots.Count; ++i)
                {
                    if (shots[i] == null)
                    {
                        shots.Clear();
                        return shots;
                    }
                }

                return shots;
            }
        }
        /// <summary>
        /// List of all pins
        /// </summary>
        public static List<PinBase> Pins
        {
            get
            {
                for (int i = 0; i < pins.Count; ++i)
                {
                    if (pins[i] == null)
                    {
                        pins.Clear();
                        return pins;
                    }
                }

                return pins;
            }
        }
        /// <summary>
        /// List of all measures
        /// </summary>
        public static List<MeasureBase> Measures
        {
            get
            {
                for (int i = 0; i < measures.Count; ++i)
                {
                    if (measures[i] == null)
                    {
                        measures.Clear();
                        return measures;
                    }
                }

                return measures;
            }
        }
        /// <summary>
        /// List of all flybys
        /// </summary>
        public static List<FlyByBase> FlyBys
        {
            get
            {
                for (int i = 0; i < flyBys.Count; ++i)
                {
                    if (flyBys[i] == null)
                    {
                        flyBys.Clear();
                        return flyBys;
                    }
                }

                return flyBys;
            }
        }

        /// <summary>
        /// List of 18 holes with data per hole
        /// </summary>
        public static List<Hole> Holes
        {
            get
            {
                return holes;
            }
        }
        /// <summary>
        /// Current terrain
        /// </summary>
        public static Terrain Terrain
        {
            get
            {
                return terrain;
            }
        }

        /// <summary>
        /// Selected plant
        /// </summary>
        public static IPlant ActivePlant
        {
            get
            {
                if (activeTee != null) return activeTee;
                if (activeShot != null) return activeShot;
                if (activePin != null) return activePin;
                if (activeMeasure != null) return activeMeasure;
                if (activeFlyBy != null) return activeFlyBy;

                return null;
            }
            set
            {
                if (value == null)
                {
                    if (activePlant != null && PlatformBase.IO.IsEditor && PlatformBase.Editor.ActiveTransform == activePlant) PlatformBase.Editor.ActiveTransform = null;
                    activePlant = null;

                    activeTee = null;
                    activeShot = null;
                    activePin = null;
                    activeMeasure = null;
                    activeFlyBy = null;
                }
                else
                {
                    activePlant = value.Transform;

                    if (value is TeeBase) activeTee = value as TeeBase;
                    if (value is ShotBase) activeShot = value as ShotBase;
                    if (value is PinBase) activePin = value as PinBase;
                    if (value is MeasureBase) activeMeasure = value as MeasureBase;
                    if (value is FlyByBase) activeFlyBy = value as FlyByBase;
                }
            }
        }
        /// <summary>
        /// Selected spline
        /// </summary>
        public static SplineBase ActiveSpline
        {
            get
            {
                return activeSpline;
            }
            set
            {
                if (activeSpline != null && PlatformBase.IO.IsEditor && PlatformBase.Editor.ActiveTransform == activeSpline.Transform) PlatformBase.Editor.ActiveTransform = null;
                activeSpline = value;
            }
        }
        /// <summary>
        /// Selected spline
        /// </summary>
        public static HazardBase ActiveHazard
        {
            get
            {
                return activeHazard;
            }
            set
            {
                if (activeHazard != null && PlatformBase.IO.IsEditor && PlatformBase.Editor.ActiveTransform == activeHazard.Transform) PlatformBase.Editor.ActiveTransform = null;
                activeHazard = value;
            }
        }

        /// <summary>
        /// Array of names for the holes i.e. [0 1 2 3 ...]
        /// </summary>
        public static string[] HoleNames
        {
            get
            {
                return new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18" };
            }
        }

        /// <summary>
        /// Course information container
        /// </summary>
        public static CourseInfo Info
        {
            get
            {
                return self.info;
            }
        }
        #endregion

        #region External Methods
        /// <summary>
        /// Force update
        /// </summary>
        public static void ForceUpdate()
        {
            curves.Clear();
            curves.AddRange((SplineBase[])GameObject.FindObjectsOfType(typeof(SplineBase)));

            hazards.Clear();
            hazards.AddRange((HazardBase[])GameObject.FindObjectsOfType(typeof(HazardBase)));

            splines.Clear();
            splines = curves.Except(hazards.Cast<SplineBase>()).ToList();

            tees.Clear();
            tees.AddRange((TeeBase[])GameObject.FindObjectsOfType(typeof(TeeBase)));

            shots.Clear();
            shots.AddRange((ShotBase[])GameObject.FindObjectsOfType(typeof(ShotBase)));

            pins.Clear();
            pins.AddRange((PinBase[])GameObject.FindObjectsOfType(typeof(PinBase)));

            measures.Clear();
            measures.AddRange((MeasureBase[])GameObject.FindObjectsOfType(typeof(MeasureBase)));

            flyBys.Clear();
            flyBys.AddRange((FlyByBase[])GameObject.FindObjectsOfType(typeof(FlyByBase)));

            holes.Clear();
            for (int i = 0; i < 18; ++i)
                holes.Add(new Hole());

            for (int i = 0; i < tees.Count; ++i)
                holes[tees[i].HoleIndex].tees.Add(tees[i]);

            for (int i = 0; i < shots.Count; ++i)
                holes[shots[i].HoleIndex].shots.Add(shots[i]);

            for (int i = 0; i < pins.Count; ++i)
                holes[pins[i].HoleIndex].pins.Add(pins[i]);

            for (int i = 0; i < flyBys.Count; ++i)
                holes[flyBys[i].HoleIndex].flyBys.Add(flyBys[i]);

            for (int i = 0; i < 18; ++i)
                holes[i].shots.Sort((x, y) => x.OrderIndex.CompareTo(y.OrderIndex));

            for (int i = 0; i < 18; ++i)
                for (int j = 0; j < holes[i].shots.Count; ++j)
                {
                    holes[i].shots[j].OrderIndex = j;
                    holes[i].shots[j].name = GetShotName(holes[i].shots[j].HoleIndex, j);
                }

            if (terrain == null && self) terrain = self.GetComponent<Terrain>();
        }

        /// <summary>
        /// Is hole enabled? (there are at least 1 tee and 1 pin)
        /// </summary>
        /// <param name="holeIndex"></param>
        /// <returns>true if enabled</returns>
        public static bool IsHoleEnabled(int holeIndex)
        {
            return Holes[holeIndex].tees.Count != 0 && Holes[holeIndex].pins.Count != 0;
        }
        /// <summary>
        /// Calculate distances
        /// </summary>
        /// <param name="teePosition"></param>
        /// <param name="shotPositions"></param>
        /// <param name="pinPosition"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static int CalculateDistances(Vector3 teePosition, List<Vector3> shotPositions, Vector3 pinPosition, Units units)
        {
            float distance = 0;
            Vector3 start = teePosition;

            for (int i = 0; i < shotPositions.Count; ++i)
            {
                distance += (shotPositions[i] - start).magnitude;
                start = shotPositions[i];
            }

            return (distance + (pinPosition - start).magnitude).ToInt(units);
        }
        /// <summary>
        /// Get the aim point from this position (i.e. next shot or pin)
        /// </summary>
        /// <param name="position"></param>
        /// <param name="tee"></param>
        /// <param name="shots"></param>
        /// <param name="pin"></param>
        /// <returns></returns>
        public static Vector3 AimPoint(Vector3 position, TeeBase tee, List<ShotBase> shots, PinBase pin)
        {
            if (shots.Count == 0) return pin.Position;
            if (Vector3.Distance(position, pin.Position) < 245) return pin.Position;
            Vector3 aimPoint = Vector3.zero;
            for (int i = 0; i < shots.Count; ++i)
            {
                ShotBase shot = shots[i];
                if ((shot.Position - position).magnitude < 50.0f) continue;

                if (aimPoint == Vector3.zero && (position - shot.Position).magnitude < (position - pin.Position).magnitude && (shot.Position - pin.Position).magnitude < (position - pin.Position).magnitude)
                {
                    aimPoint = shot.Position;
                }
                else if (aimPoint != Vector3.zero && (position - shot.Position).magnitude < (position - aimPoint).magnitude && (shot.Position - pin.Position).magnitude < (aimPoint - pin.Position).magnitude)
                {
                    aimPoint = shot.Position;
                }
            }

            if (aimPoint != Vector3.zero) return aimPoint;
            else return pin.Position;
        }
        #endregion

        #region Support Methods
        static void CheckHideFlags(HideFlags hideFlags = HideFlags.HideInHierarchy)
        {
            Curves.ForEach(x => x.gameObject.hideFlags = hideFlags);
            Hazards.ForEach(x => x.gameObject.hideFlags = hideFlags);
            Tees.ForEach(x => x.gameObject.hideFlags = hideFlags);
            Shots.ForEach(x => x.gameObject.hideFlags = hideFlags);
            Pins.ForEach(x => x.gameObject.hideFlags = hideFlags);
            Measures.ForEach(x => x.gameObject.hideFlags = hideFlags);
            FlyBys.ForEach(x => x.gameObject.hideFlags = hideFlags);
        }
        #endregion
    }
}