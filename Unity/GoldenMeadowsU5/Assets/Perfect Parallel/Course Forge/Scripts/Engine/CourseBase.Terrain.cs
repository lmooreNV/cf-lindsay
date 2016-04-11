using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
    public partial class CourseBase
    {
        #region Fields
        [SerializeField]
        float[] heightmapBackup = new float[0];
        #endregion

        #region Properties
        /// <summary>
        /// Camera Terrain Hit
        /// </summary>
        public static RaycastHit? CameraTerrainHit
        {
            get
            {
                if (PlatformBase.IO.IsEditor)
                {
                    RaycastHit[] hits = Physics.RaycastAll(PlatformBase.Editor.CameraRay);
                    for (int i = 0; i < hits.Length; ++i)
                        if (hits[i].transform == Terrain.transform)
                            return hits[i];
                }

                return null;
            }
        }
        /// <summary>
        /// Camera Mesh Hit
        /// </summary>
        public static RaycastHit? CameraMeshHit
        {
            get
            {
                if (PlatformBase.IO.IsEditor)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(PlatformBase.Editor.CameraRay, out hitInfo))
                        return new RaycastHit?(hitInfo);
                }

                return null;
            }
        }

        /// <summary>
        /// Is terrain selected now?
        /// </summary>
        public static bool TerrainSelected
        {
            get
            {
                return PlatformBase.IO.IsEditor && PlatformBase.Editor.ActiveTransform && PlatformBase.Editor.ActiveTransform == Terrain.transform;
            }
        }
        /// <summary>
        /// Is terrain lowered?
        /// </summary>
        public static bool TerrainLowered
        {
            get
            {
                return self.heightmapBackup.Length != 0;
            }
            set
            {
                if (value == false)
                {
                    self.heightmapBackup = new float[0];
                }
            }
        }

        /// <summary>
        /// Ground mask for all collisions (without water, UI, e.t.c.)
        /// </summary>
        public static LayerMask GroundMask
        {
            get
            {
                return 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water");
            }
        }
        /// <summary>
        /// Physics mask for all air Physics (without water, UI, e.t.c.)
        /// </summary>
        public static LayerMask TreesMask
        {
            get
            {
                return 1 << LayerMask.NameToLayer("Trees");
            }
        }
        #endregion

        #region State Methods
        /// <summary>
        /// Save terrain state
        /// </summary>
        public static void SaveTerrain()
        {
            Terrain terrain = Terrain;
            int width = terrain.terrainData.heightmapWidth;
            int height = terrain.terrainData.heightmapHeight;
            float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);

            float[] heightmapBackup = new float[width * height];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    heightmapBackup[y * height + x] = heights[y, x];

            self.heightmapBackup = heightmapBackup;
        }
        /// <summary>
        /// Restore terrain state
        /// </summary>
        public static void RestoreTerrain()
        {
            Terrain terrain = Terrain;
            int width = terrain.terrainData.heightmapWidth;
            int height = terrain.terrainData.heightmapHeight;
            float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);

            float[] heightmapBackup = self.heightmapBackup;
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                    heights[y, x] = heightmapBackup[y * height + x];

            terrain.terrainData.SetHeights(0, 0, heights);

            self.heightmapBackup = new float[0];
            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.SetDirty(self);
                PlatformBase.Editor.SaveScene();
            }
        }
        /// <summary>
        /// Lower terrain under the bunkers
        /// </summary>
        public static void LowerTerrain()
        {
            Terrain terrain = Terrain;
            int width = terrain.terrainData.heightmapWidth;
            int height = terrain.terrainData.heightmapHeight;
            Vector3 size = terrain.terrainData.size;
            Vector3 invertedSize = new Vector3(1.0f / size.x, 1.0f / size.y, 1.0f / size.z);
            float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);

            List<SplineBase> splines = Splines;
            for (int s = 0; s < splines.Count; ++s)
                if (splines[s].Layer.IsBunker)
                {
                    Vector3 min3 = Vector3.Scale(splines[s].Lines.RealMinPoint - terrain.transform.position, invertedSize);
                    Vector3 max3 = Vector3.Scale(splines[s].Lines.RealMaxPoint - terrain.transform.position, invertedSize);
                    Vector2 min = new Vector2(min3.x * width, min3.z * height);
                    Vector2 max = new Vector2(max3.x * width, max3.z * height);
                    int miny = (int)min.y;
                    int maxy = (int)max.y;
                    int minx = (int)min.x;
                    int maxx = (int)max.x;

                    List<Vector3> polygon = splines[s].Lines.OuterSplineData.points;
                    for (int y = miny; y < maxy; ++y)
                    {
                        for (int x = minx; x < maxx; ++x)
                        {
                            Vector3 p = new Vector3((float)x / width, 0, (float)y / height);
                            p = Vector3.Scale(p, size);
                            if (p.IsInside(polygon))
                            {
                                heights[y, x] -= (Info.offset * 5) / size.y;
                            }
                        }
                    }
                }

            terrain.terrainData.SetHeights(0, 0, heights);

            if (PlatformBase.IO.IsEditor)
            {
                PlatformBase.Editor.SetDirty(self);
                PlatformBase.Editor.SaveScene();
            }
        }
        #endregion

        #region Data Methods
        /// <summary>
        /// Terrain normal at position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector3 TerrainNormal(Vector3 position)
        {
            Vector3 terrainLocalPos = new Vector3(position.x, 0, position.z) - Terrain.transform.position;
            Vector3 normalizedPos = new Vector2(Mathf.InverseLerp(0, Terrain.terrainData.size.x, terrainLocalPos.x), Mathf.InverseLerp(0, Terrain.terrainData.size.z, terrainLocalPos.z));
            return Terrain.terrainData.GetInterpolatedNormal(normalizedPos.x, normalizedPos.y);
        }
        /// <summary>
        /// Terrain height at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float TerrainHeight(float x, float y)
        {
            return Terrain.SampleHeight(new Vector3(x, 0, y));
        }
        /// <summary>
        /// Terrain hit at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static RaycastHit? TerrainHit(float x, float y)
        {
            SplineBase insideSpline = null;
            List<SplineBase> splines = Splines;
            for (int i = 0; i < splines.Count; ++i)
            {
                SplineBase spline = splines[i];
                if (spline.HasChilds) continue;
                if (spline is HazardBase) continue;
                if (!spline.InBounds(new Vector3(x, 0, y))) continue;

                if (new Vector2(x, y).IsInside(spline.Lines.Points))
                {
                    insideSpline = spline;
                    break;
                }
            }

            if (insideSpline)
            {
                Ray ray = new Ray(new Vector3(x, 7777, y), -Vector3.up);
                RaycastHit hit;
                if (insideSpline.GetComponent<Collider>().Raycast(ray, out hit, 8888))
                {
                    return hit;
                }
            }

            if (true)
            {
                Ray ray = new Ray(new Vector3(x, 7777, y), -Vector3.up);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 8888))
                {
                    while (!hit.transform.gameObject.name.Contains("Spline"))
                    {
                        ray = new Ray(new Vector3(x, hit.point.y - 0.01f, y), -Vector3.up);
                        if (!Physics.Raycast(ray, out hit)) break;
                    }
                    return hit;
                }

                Debug.DrawLine(ray.origin, ray.origin + ray.direction * 8888, Color.green, 60);

                if (Terrain.GetComponent<Collider>().Raycast(ray, out hit, 8888))
                {
                    return hit;
                }
            }

            return null;
        }

        /// <summary>
        /// Mesh level at x,y (for terrain export)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float MeshLevel(float x, float y)
        {
            float terrainHeight = TerrainHeight(x, y);

            List<RaycastHit> hits = Physics.RaycastAll(new Vector3(x, 9000, y), Vector3.down, 10000).ToList();
            int splineIndex = hits.FindIndex(t => t.transform.name.Contains("Spline"));
            if (splineIndex != -1)
            {
                return Mathf.Max(terrainHeight, hits[splineIndex].point.y - Info.offset);
            }
            else
            {
                return terrainHeight;
            }
        }
        /// <summary>
        /// Mesh height at x,y (ignoring trees)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float MeshHeight(float x, float y, bool isGrid = false)
        {
            Ray ray = new Ray(new Vector3(x, 7777.0f, y), -Vector3.up);

            RaycastHit[] hits = Physics.RaycastAll(ray, 8888.0f, GroundMask);
            hits = hits.Where(hit => hit.transform.name.Contains("Spline")).OrderBy(hit => hit.distance).ToArray();
            if (hits.Length > 0)
            {
                if (hits[0].collider.name.Contains("holeCup") && isGrid == true)
                    return hits[0].point.y + 0.10795f;
                else
                    return hits[0].point.y;
            }
            return TerrainHeight(x, y);
        }

        /// <summary>
        /// All height at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float AllHeight(float x, float y)
        {
            Ray ray = new Ray(new Vector3(x, 9000, y), -Vector3.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point.y;
            }

            return TerrainHeight(x, y);
        }
        #endregion
    }
}