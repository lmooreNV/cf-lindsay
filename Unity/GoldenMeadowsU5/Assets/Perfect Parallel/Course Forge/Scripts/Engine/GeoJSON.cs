using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace PerfectParallel
{
    using CourseForge;

    /// <summary>
    /// GeoJSON export class, exports scene to a file
    /// </summary>
    [Obfuscation(Exclude = true)]
    public static class GeoJSON
    {
        public class File
        {
            public class CRS
            {
                public class Properties
                {
                    #region Fields
                    public string name = null;
                    #endregion
                }

                #region Fields
                public string type = null;
                public Properties properties = new Properties();
                #endregion
            }

            public class Feature
            {
                public class Geometry
                {
                    #region Fields
                    public string type = null;
                    public object coordinates = null;
                    #endregion
                }

                public class Properties
                {
                    #region Fields
                    public string Layer = null;
                    #endregion
                }

                #region Fields
                public string type = null;
                public string id = null;
                public Properties properties = new Properties();
                public Geometry geometry = new Geometry();
                #endregion
            }

            #region Fields
            public string type = null;
            public CRS crs = new File.CRS();
            public List<Feature> features = new List<Feature>();
            #endregion
        }

        #region Fields
        static Vector3 terrainSize = Vector3.zero;
        static float xoff = -1;
        static float zoff = -1;

        static float step = -1;
        static float x = -1;
        static float z = -1;
        static bool exporting = false;

        static int countX = -1;
        static int countZ = -1;
        static float[] heights = null;
        static int indexX = -1;
        static int indexY = -1;

        static string path = null;
        #endregion

        #region Properties
        /// <summary>
        /// Is exporting?
        /// </summary>
        public static bool Exporting
        {
            get
            {
                return exporting;
            }
        }
        /// <summary>
        /// Exporting course
        /// </summary>
        public static float Progress
        {
            get
            {
                return (float)(indexX + 1 + (countX - 1 - indexY) * countX) / (float)(countX * countZ);
            }
        }
        #endregion

        #region External Methods
        /// <summary>
        /// Start exporting terrain
        /// </summary>
        public static void ExportTerrain(string path, float step)
        {
            terrainSize = CourseBase.Terrain.terrainData.size;
            xoff = CourseBase.Info.geoX;
            zoff = CourseBase.Info.geoY;

            GeoJSON.path = path;
            GeoJSON.step = step;
            x = 0;
            z = terrainSize.z - step;

            countX = (int)(terrainSize.x / (float)step);
            countZ = (int)(terrainSize.z / (float)step);
            heights = new float[countX];
            indexX = 0;
            indexY = countZ - 1;

            try
            {
                PlatformBase.IO.DeleteFile(path);
                PlatformBase.IO.DeleteFile(path.Replace(".bil", ".hdr"));

                string hdr = "";
                hdr += "" + "ncols " + countX;
                hdr += "\n" + "nrows " + countZ;
                hdr += "\n" + "bbands " + 1;
                hdr += "\n" + "cellsize " + step;
                hdr += "\n" + "xllcorner " + xoff;
                hdr += "\n" + "yllcorner " + zoff;
                hdr += "\n" + "nodata_value " + "9999.000000";
                hdr += "\n" + "nbits " + 32;
                hdr += "\n" + "pixeltype " + "float";
                hdr += "\n" + "byteorder " + "I";
                PlatformBase.IO.WriteText(path.Replace(".bil", ".hdr"), hdr);

                exporting = true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                exporting = false;
            }
        }
        /// <summary>
        /// Do export step
        /// </summary>
        public static void DoExportTerrainStep()
        {
            if (!exporting) return;

            heights[indexX] = CourseBase.MeshLevel(x, z) + CourseBase.Info.geoZ;
            indexX++;


            if (indexX == countX)
            {
                indexX = 0;
                x = 0;

                byte[] bytes = new byte[heights.Length * sizeof(float)];
                Buffer.BlockCopy(heights, 0, bytes, 0, bytes.Length);

                PlatformBase.IO.AppendBytes(path, bytes);

                z -= step;
                indexY--;
                if (indexY == -1)
                {
                    exporting = false;
                }
            }
            else
            {
                x += step;
            }
        }
        /// <summary>
        /// Cancel export
        /// </summary>
        public static void CancelExport()
        {
            exporting = false;
        }

        /// <summary>
        /// Export all splines
        /// </summary>
        public static void ExportSplines(string path)
        {
            xoff = CourseBase.Info.geoX;
            zoff = CourseBase.Info.geoY;

            File export = new File();
            export.type = "FeatureCollection";
            export.crs.type = "name";
            export.crs.properties.name = "+proj=utm +zone=" + CourseBase.Info.utmZone + " +ellps=GRS80 +datum=NAD83 +units=m +no_defs";

            List<SplineBase> splines = CourseBase.Curves;
            for (int i = 0; i < splines.Count; ++i)
            {
                File.Feature feature = new File.Feature();
                feature.type = "Feature";
                feature.id = "id" + i.ToString();
                feature.geometry.type = "Polygon";
                feature.geometry.coordinates = new List<List<float[]>>();
                feature.properties.Layer = splines[i].Layer.name;

                List<List<float[]>> coordinates = (List<List<float[]>>)feature.geometry.coordinates;
                coordinates.Add(new List<float[]>());

                Vector3[] lines = splines[i].Lines.Lines;
                for (int j = 0; j < lines.Length; ++j)
                {
                    float[] xy = new float[2];
                    xy[0] = xoff + lines[j].x;
                    xy[1] = zoff + lines[j].z;
                    coordinates[0].Add(xy);
                }

                export.features.Add(feature);
            }

            PlatformBase.IO.WriteText(path, Utility.JsonWrite(export));
        }
        /// <summary>
        /// Import splines from file
        /// </summary>
        /// <param name="path"></param>
        public static void ImportSplines(string path)
        {
            try
            {
                File file = Utility.JsonRead<File>(PlatformBase.IO.ReadText(path));
                for (int i = 0; i < file.features.Count; ++i)
                {
                    File.Feature feature = file.features[i];
                    if (feature.geometry.type == "Polygon" && feature.geometry.coordinates.GetType() == typeof(Double[][][]))
                    {
                        Double[][][] coordinates = (Double[][][])feature.geometry.coordinates;

                        for (int p = 0; p < coordinates.Length; ++p)
                        {
                            List<Vector3> poly = new List<Vector3>();
                            for (int t = 0; t < coordinates[p].Length; ++t)
                                poly.Add(new Vector3((float)coordinates[p][t][0] - CourseBase.Info.geoX, 0, (float)coordinates[p][t][1] - CourseBase.Info.geoY));

                            for (int t = 0; t < poly.Count; ++t)
                                poly[t] = new Vector3(poly[t].x, CourseBase.TerrainHeight(poly[t].x, poly[t].z), poly[t].z);

                            if (CourseBase.IsHazard(file.features[i].properties.Layer))
                            {
                                CourseBase.CreateHazard(poly.ToArray(), CourseBase.GetLayer(file.features[i].properties.Layer, CourseBase.HazardLayers), 0, true);
                            }
                            else
                            {
                                CourseBase.CreateSpline(poly.ToArray(), CourseBase.GetLayer(file.features[i].properties.Layer, CourseBase.SplineLayers), 0, true);
                            }
                        }
                    }
                    else if (feature.geometry.type == "Polygon" && feature.geometry.coordinates.GetType() == typeof(object[][]))
                    {
                        object[][] coordinates = (object[][])feature.geometry.coordinates;

                        for (int p = 0; p < coordinates.Length; ++p)
                        {
                            List<Vector3> poly = new List<Vector3>();
                            for (int t = 0; t < coordinates[p].Length; ++t)
                            {
                                float x = 0;
                                float y = 0;
                                if (coordinates[p][t].GetType() == typeof(object[]))
                                {
                                    object[] xy = (object[])coordinates[p][t];
                                    x = System.Convert.ToSingle(xy[0]);
                                    y = System.Convert.ToSingle(xy[1]);
                                }
                                if (coordinates[p][t].GetType() == typeof(double[]))
                                {
                                    double[] xy = (double[])coordinates[p][t];
                                    x = System.Convert.ToSingle(xy[0]);
                                    y = System.Convert.ToSingle(xy[1]);
                                }
                                if (coordinates[p][t].GetType() == typeof(int[]))
                                {
                                    int[] xy = (int[])coordinates[p][t];
                                    x = System.Convert.ToSingle(xy[0]);
                                    y = System.Convert.ToSingle(xy[1]);
                                }
                                poly.Add(new Vector3(x - CourseBase.Info.geoX, 0, y - CourseBase.Info.geoY));
                            }

                            for (int t = 0; t < poly.Count; ++t)
                                poly[t] = new Vector3(poly[t].x, CourseBase.TerrainHeight(poly[t].x, poly[t].z), poly[t].z);

                            if (CourseBase.IsHazard(file.features[i].properties.Layer))
                            {
                                CourseBase.CreateHazard(poly.ToArray(), CourseBase.GetLayer(file.features[i].properties.Layer, CourseBase.HazardLayers), 0, true);
                            }
                            else
                            {
                                CourseBase.CreateSpline(poly.ToArray(), CourseBase.GetLayer(file.features[i].properties.Layer, CourseBase.SplineLayers), 0, true);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("Feature " + feature.id + "(" + feature.geometry.coordinates.GetType() + ") is not a Polygon");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion
    }
}