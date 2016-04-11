using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
    public partial class CourseBase
    {
        #region Fields
        [SerializeField]
        LibraryFile library = null;
        [SerializeField]
        string libraryPath = null;
        #endregion

        #region Properties
        /// <summary>
        /// Library container
        /// </summary>
        public static LibraryFile Library
        {
            get
            {
                return self.library;
            }
        }
        /// <summary>
        /// Library name
        /// </summary>
        public static string LibraryName
        {
            get
            {
                return self.libraryPath;
            }
            set
            {
                self.libraryPath = value;
            }
        }

        /// <summary>
        /// List of all layers
        /// </summary>
        public static List<Layer> Layers
        {
            get
            {
                return Library.layers;
            }
        }
        /// <summary>
        /// Array of names for the splines
        /// </summary>
        public static List<Layer> SplineLayers
        {
            get
            {
                return Layers.Where(x => !IsHazard(x.name)).Select(x => x as Layer).ToList();
            }
        }
        /// <summary>
        /// Array of names for the hazards
        /// </summary>
        public static List<Layer> HazardLayers
        {
            get
            {
                return Layers.Where(x => IsHazard(x.name)).Select(x => x as Layer).ToList();
            }
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// Called on layer change, updates all the splines
        /// </summary>
        public static void OnLayerChange()
        {
            CheckLibraryLegacyNames();
            if (PlatformBase.IO.IsEditor) SaveLibrary();

            List<SplineBase> splines = Curves;
            for (int i = 0; i < splines.Count; ++i)
            {
                splines[i].UpdateLine();
                splines[i].SetRefresh();
            }
        }
        /// <summary>
        /// Called on layer material change, updates all the splines
        /// </summary>
        public static void OnLayerMaterialChange()
        {
            if (PlatformBase.IO.IsEditor) SaveLibrary();

            List<SplineBase> splines = Curves;
            for (int i = 0; i < splines.Count; ++i)
            {
                if (splines[i].Info.locked) continue;
                splines[i].Renderer.NeedMaterialRefresh = true;
            }
        }
        #endregion

        #region Layer Methods
        /// <summary>
        /// Get layer at index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Layer GetLayer(int index, List<Layer> layers)
        {
            Layer layer = layers.Find(x => layers.IndexOf(x) == index);
            if (layer != null) return layer;

            if (layers.Count == 0) return null;
            return layers[0];
        }
        /// <summary>
        /// Get layer from name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Layer GetLayer(string name, List<Layer> layers)
        {
            Layer layer = layers.Find(x => x.name.Trim().ToLower() == name.Trim().ToLower());
            if (layer != null) return layer;

            layer = layers.Find(x => x.name.Trim().ToLower().Contains(name.Trim().ToLower()));
            if (layer != null) return layer;

            if (layers.Count == 0) return null;
            return layers[0];
        }

        /// <summary>
        /// Gets the pin layer
        /// </summary>
        /// <returns></returns>
        public static Layer GetPinLayer()
        {
            return GetLayer("__Pin__", SplineLayers);
        }
        /// <summary>
        /// Gets the pin lip layer
        /// </summary>
        /// <returns></returns>
        public static Layer GetPinLipLayer()
        {
            return GetLayer("__Pin_Lip__", SplineLayers);
        }
        #endregion

        #region Library Methods
        /// <summary>
        /// Load library
        /// </summary>
        /// <returns></returns>
        public static bool LoadLibrary()
        {
            try
            {
                PlatformBase.IO.CreateDirectory(PlatformBase.IO.LibrariesPath + "/default");
                PlatformBase.IO.CreateDirectory(PlatformBase.IO.LibrariesPath + "/default/default Content");
                PlatformBase.IO.CreateDirectory(PlatformBase.Editor.LibrariesPath + "/default");
                PlatformBase.IO.CreateDirectory(PlatformBase.Editor.LibrariesPath + "/default/default Content");

                if (!PlatformBase.IO.FileExists(PlatformBase.IO.LibrariesPath + "/default/default.cfl") && PlatformBase.IO.FileExists(PlatformBase.Editor.LibrariesPath + "/default/default.cfl"))
                {
                    PlatformBase.IO.CopyFile(PlatformBase.Editor.LibrariesPath + "/default/default.cfl", PlatformBase.IO.LibrariesPath + "/default/default.cfl");
                    PlatformBase.IO.CopyDirectory(PlatformBase.Editor.LibrariesPath + "/default/default Content", PlatformBase.IO.LibrariesPath + "/default/default Content");
                }

                if (LibraryName == null) LibraryName = "default";
                LibraryName = PlatformBase.IO.GetFileNameWithoutExtension(LibraryName);

                string name = LibraryName;
                string path = PlatformBase.Editor.LibrariesPath + "/" + name;

                string text = PlatformBase.IO.ReadText(path + "/" + name + ".cfl");
                self.library = Utility.JsonRead<LibraryFile>(text);
                self.libraryPath = name;
                self.library.ApplyPatches(text);

                string contentPath = path + "/" + name + " Content";
                LibraryFile library = Library;
                for (int i = 0; i < library.layers.Count; ++i)
                {
                    if (library.layers[i].diffuse.TileTexture == null && library.layers[i].diffuse.HasTexture)
                        library.layers[i].diffuse.TileTexture = PlatformBase.Editor.PathToObject(contentPath + "/" + library.layers[i].diffuse.textureName) as Texture;

                    if (library.layers[i].detail.TileTexture == null && library.layers[i].detail.HasTexture)
                        library.layers[i].detail.TileTexture = PlatformBase.Editor.PathToObject(contentPath + "/" + library.layers[i].detail.textureName) as Texture;

                    if (library.layers[i].normal.TileTexture == null && library.layers[i].normal.HasTexture)
                        library.layers[i].normal.TileTexture = PlatformBase.Editor.PathToObject(contentPath + "/" + library.layers[i].normal.textureName) as Texture;

                    if (library.layers[i].detailNormal.TileTexture == null && library.layers[i].detailNormal.HasTexture)
                        library.layers[i].detailNormal.TileTexture = PlatformBase.Editor.PathToObject(contentPath + "/" + library.layers[i].detailNormal.textureName) as Texture;

                    if (library.layers[i].PhysicsMaterial == null && library.layers[i].physicMaterialGUID != "" && PlatformBase.Editor.GUIDToObject(library.layers[i].physicMaterialGUID))
                        library.layers[i].PhysicsMaterial = PlatformBase.Editor.GUIDToObject(library.layers[i].physicMaterialGUID) as PhysicMaterial;

                    if (library.layers[i].HazardPost == null && library.layers[i].hazardPostGUID != "" && PlatformBase.Editor.GUIDToObject(library.layers[i].hazardPostGUID))
                        library.layers[i].HazardPost = PlatformBase.Editor.GUIDToObject(library.layers[i].hazardPostGUID) as GameObject;
                }

                CheckLibraryHazards();
                CheckLibraryLegacyNames();
            }
            catch (Exception e)
            {
                if (PlatformBase.Editor.OkCancelDialog("CourseForge loading error", e.ToString()))
                {
                    self.library = new LibraryFile();
                    self.libraryPath = "unknown";
                    self.library.layers.Add(new Layer());
                }
                else
                {
                    return false;
                }
            }
            finally
            {
                OnLayerMaterialChange();
            }
            return true;
        }
        /// <summary>
        /// Save library
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public static void SaveLibrary(string path = null, string name = null)
        {
            if (name == null) name = LibraryName;
            if (path == null) path = PlatformBase.Editor.LibrariesPath + "/" + name;

            string contentPath = path + "/" + name + " Content";
            PlatformBase.IO.CreateDirectory(path);
            PlatformBase.IO.CreateDirectory(contentPath);
            PlatformBase.IO.WriteText(path + "/" + name + ".cfl", Utility.JsonWrite(Library));
            PlatformBase.Editor.SetDirty(self);
        }
        /// <summary>
        /// Import library
        /// </summary>
        /// <param name="path"></param>
        public static void ImportLibrary(string path)
        {
            string name = PlatformBase.IO.GetFileNameWithoutExtension(path);
            string contentPath = PlatformBase.IO.GetDirectoryName(path) + "/" + name + " Content";

            string editorPath = PlatformBase.Editor.LibrariesPath + "/" + name;
            string editorContentPath = editorPath + "/" + name + " Content";

            PlatformBase.IO.CreateDirectory(editorPath);
            PlatformBase.IO.CreateDirectory(editorContentPath);

            if (contentPath != editorContentPath)
            {
                PlatformBase.IO.CopyFile(path, PlatformBase.Editor.LibrariesPath + "/" + name + "/" + name + ".cfl");
                PlatformBase.IO.CopyDirectory(contentPath, editorContentPath, true);
                PlatformBase.Editor.RefreshAssetDatabase();
            }

            string text = PlatformBase.IO.ReadText(path);
            self.library = Utility.JsonRead<LibraryFile>(text);
            self.libraryPath = name;
            self.library.ApplyPatches(text);

            LoadLibrary();
        }
        /// <summary>
        /// Export library
        /// </summary>
        /// <param name="path"></param>
        public static void ExportLibrary(string path)
        {
            string name = PlatformBase.IO.GetFileNameWithoutExtension(path);
            string contentPath = PlatformBase.IO.GetDirectoryName(path) + "/" + name + " Content";

            string editorPath = PlatformBase.Editor.LibrariesPath + "/" + name;
            string editorContentPath = editorPath + "/" + name + " Content";

            SaveLibrary(PlatformBase.IO.GetDirectoryName(path), PlatformBase.IO.GetFileNameWithoutExtension(path));

            if (contentPath != editorContentPath)
            {
                for (int i = 0; i < Library.layers.Count; ++i)
                {
                    Layer layer = Library.layers[i];
                    if (layer.diffuse.HasTexture && layer.diffuse.TileTexture) PlatformBase.IO.CopyFile(PlatformBase.Editor.ObjectToPath(layer.diffuse.TileTexture), contentPath + "/" + layer.diffuse.textureName, true);
                    if (layer.detail.HasTexture && layer.detail.TileTexture) PlatformBase.IO.CopyFile(PlatformBase.Editor.ObjectToPath(layer.detail.TileTexture), contentPath + "/" + layer.detail.textureName, true);
                    if (layer.normal.HasTexture && layer.normal.TileTexture) PlatformBase.IO.CopyFile(PlatformBase.Editor.ObjectToPath(layer.normal.TileTexture), contentPath + "/" + layer.normal.textureName, true);
                    if (layer.detailNormal.HasTexture && layer.detailNormal.TileTexture) PlatformBase.IO.CopyFile(PlatformBase.Editor.ObjectToPath(layer.detailNormal.TileTexture), contentPath + "/" + layer.detailNormal.textureName, true);
                }
            }
        }
        #endregion

        #region Support Methods
        static void CheckLibraryGameLayers()
        {
            if (Layers.FindIndex(x => x.name == "__Pin__") == -1)
            {
                Layer layer = new Layer(GetLayer("green", SplineLayers));
                layer.name = "__Pin__";
                layer.pattern.extra = false;
                layer.pattern.shrink = false;
                layer.pattern.transitionLength = Utility.holeTransition;
                Layers.Add(layer);
            }
            if (Layers.FindIndex(x => x.name == "__Pin_Lip__") == -1)
            {
                Layer layer = new Layer(GetLayer("green", SplineLayers));
                layer.name = "__Pin_Lip__";
                layer.pattern.extra = false;
                layer.pattern.shrink = false;
                layer.pattern.transitionLength = Utility.holeTransition;
                Layers.Add(layer);
            }
        }
        static void CheckLibraryHazards()
        {
            if (Layers.Find(x => HazardType(x.name) == HazardBase.Type.Out_of_Bounds) == null)
            {
                Layer layer = new Layer();
                layer.name = Utility.GetName(HazardBase.Type.Out_of_Bounds);
                layer.LayerColor = Color.white;
                layer.pattern.transitionLength = 0.05f;
                layer.metersPerOnePoint = 5;
                layer.pointsPerEdge = 10;
                layer.hazardColor = Color.white;
                layer.hazardColor.a = 0.5f;
                layer.HazardPost = Resources.Load<GameObject>("StakeWhite");
                layer.metersPerOnePost = 5;
                layer.pointsPerEdge = 40;
                Layers.Add(layer);
            }
            if (Layers.Find(x => HazardType(x.name) == HazardBase.Type.Water_Hazard) == null)
            {
                Layer layer = new Layer();
                layer.name = Utility.GetName(HazardBase.Type.Water_Hazard);
                layer.LayerColor = Color.yellow;
                layer.pattern.transitionLength = 0.05f;
                layer.metersPerOnePoint = 5;
                layer.pointsPerEdge = 10;
                layer.hazardColor = Color.yellow;
                layer.hazardColor.a = 0.5f;
                layer.HazardPost = Resources.Load<GameObject>("StakeYellow");
                layer.metersPerOnePost = 5;
                layer.pointsPerEdge = 40;
                Layers.Add(layer);
            }
            if (Layers.Find(x => HazardType(x.name) == HazardBase.Type.Lateral_Water_Hazard) == null)
            {
                Layer layer = new Layer();
                layer.name = Utility.GetName(HazardBase.Type.Lateral_Water_Hazard);
                layer.LayerColor = Color.red;
                layer.pattern.transitionLength = 0.05f;
                layer.metersPerOnePoint = 5;
                layer.pointsPerEdge = 10;
                layer.hazardColor = Color.red;
                layer.hazardColor.a = 0.5f;
                layer.HazardPost = Resources.Load<GameObject>("StakeRed");
                layer.metersPerOnePost = 5;
                layer.pointsPerEdge = 40;
                Layers.Add(layer);
            }
            if (Layers.Find(x => HazardType(x.name) == HazardBase.Type.Free_Drop) == null)
            {
                Layer layer = new Layer();
                layer.name = Utility.GetName(HazardBase.Type.Free_Drop);
                layer.LayerColor = Color.grey;
                layer.pattern.transitionLength = 0.05f;
                layer.metersPerOnePoint = 5;
                layer.pointsPerEdge = 10;
                layer.hazardColor = Color.grey;
                layer.hazardColor.a = 0.5f;
                layer.metersPerOnePost = 5;
                layer.pointsPerEdge = 40;
                Layers.Add(layer);
            }
        }
        static void CheckLibraryLegacyNames()
        {
            for (int i = SplineLayers.Count - 1; i >= 0; --i)
            {
                Layer layer = SplineLayers[i];
                SplineBase.SplinePattern pattern = layer.pattern;

                if (pattern.extraLayerName == "NoLayerName")
                {
                    pattern.extraLayerName = GetLayer(pattern.extraLayerIndex, Layers).name;
                }
            }
        }
        #endregion
    }
}