using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Course Forge library tool, editor to change the layers
    /// </summary>
    public class LibraryToolUI : ToolUI
    {
        #region Fields
        static int layerIndex = 0;
        #endregion

        #region Properties
        static Layer Layer
        {
            get
            {
                return CourseBase.GetLayer(layerIndex, CourseBase.Layers);
            }
            set
            {
                layerIndex = CourseBase.Layers.IndexOf(value);
            }
        }
        static bool IsHazardLayer
        {
            get
            {
                return CourseBase.IsHazard(Layer.name);
            }
        }
        static bool IsSplineLayer
        {
            get
            {
                return !IsHazardLayer;
            }
        }

        /// <summary>
        /// Selected layers
        /// </summary>
        public static Layer SelectedLayer
        {
            get
            {
                return Layer;
            }
            set
            {
                Layer = CourseBase.Layers.Find(x => x.name == value.name);
            }
        }
        #endregion

        #region Methods
        public override void OnUI(bool selected)
        {
            #region Sort Layers
            Layer layer = Layer;
            Course.Library.layers = Course.Library.layers.OrderBy(x => x.name).OrderBy(x => Course.IsHazard(x.name)).ToList();
            layerIndex = Course.Layers.IndexOf(layer);
            #endregion

            List<Layer> layers = CourseBase.Layers;
            int layersCount = layers.Count;
            string libraryName = PlatformBase.IO.GetFileNameWithoutExtension(CourseBase.LibraryName);
            Layer oldLayer = new Layer(Layer);


            BeginUI();

            MoveToBottom();
            Move(1, -5.75f);
            MovePixels(1, 0);

            if (layers.GroupBy(x => x.name.ToLower()).Any(y => y.Count() > 1))
            {
                Move(0, -0.8f);
                EditorGUI.HelpBox(GetRect(5, 0.8f), "Multiple layers with the same name detected. Each layer should have it's own unique name. ", MessageType.Warning);
                Move(0, 0.8f);
            }

            Background(5, 1, 0.5f);

            #region Library
            Label("Loaded library ---", 2.5f);
            {
                Move(2.5f, 0);
                MovePixels(1, 1);
                SetColor(Black);
                Label(libraryName + (libraryName == "default" ? " (Read-only)" : ""), 2.5f);
                MovePixels(-1, -1);
                SetColor(Green);
                Label(libraryName + (libraryName == "default" ? " (Read-only)" : ""), 2.5f);
                SetColor();
                Move(-2.5f, 0);
            }
            #endregion

            #region Import/Export
            Move(0, 0.4f);
            EditorGUI.BeginDisabledGroup(!PlatformBase.IO.IsEditor);
            {
                if (Button("load...", 2.5f, 0.6f))
                {
                    try
                    {
                        string path = PlatformBase.Editor.OpenFileDialog("Load Course Library", PlatformBase.IO.LibrariesPath, "cfl");
                        if (path != "")
                        {
                            CourseBase.ImportLibrary(path);
                            Debug.Log("CourseForge {load " + PlatformBase.IO.GetFileNameWithoutExtension(path) + "} " + DateTime.Now.ToString("dd MMM HH:mm:ss"));
                        }
                    }
                    catch (Exception e)
                    {
                        EditorUtility.DisplayDialog("Load error.", e.ToString(), "ok");
                    }
                }
                Move(2.5f, 0);
                if (Button("save as...", 2.5f, 0.6f))
                {
                    try
                    {
                        string path = PlatformBase.Editor.SaveFileDialog("Save Course Library", PlatformBase.IO.LibrariesPath, PlatformBase.IO.GetFileNameWithoutExtension(CourseBase.LibraryName), "cfl");
                        if (path != "")
                        {
                            CourseBase.ExportLibrary(path);
                            Debug.Log("CourseForge {save " + PlatformBase.IO.GetFileNameWithoutExtension(path) + "} " + DateTime.Now.ToString("dd MMM HH:mm:ss"));
                        }
                    }
                    catch (Exception e)
                    {
                        EditorUtility.DisplayDialog("Export error.", e.ToString(), "ok");
                    }
                }
                Move(-2.5f, 0);
            }
            EditorGUI.EndDisabledGroup();
            Move(0, 0.6f);
            MovePixels(0, 2);
            #endregion

            #region Layer Popup
            Background(5, 5.75f, 0.75f);
            Label("Selected layer", 2.5f);

            Move(1.85f, 0);
            if (IsHazardLayer) SetColor(Green);
            else SetColor(HalfGrey);
            if (Button("H", 0.7f, 0.6f))
            {
                if (IsHazardLayer && CourseBase.SplineLayers.Count != 0) Layer = (Layer)CourseBase.SplineLayers[0];
                else if (!IsHazardLayer && CourseBase.HazardLayers.Count != 0) Layer = (Layer)CourseBase.HazardLayers[0];
                else layerIndex = 0;

                oldLayer = new Layer(Layer);
            }
            SetColor();
            Move(-1.85f, 0);

            Move(0, 0.4f);

            List<string> names = new List<string>(layers.Select(x => (layers.IndexOf(x) + 1) + " " + x.name));
            if (IsSplineLayer) names = names.Take(Course.SplineLayers.Count).ToList();

            if (libraryName != "default" && IsSplineLayer)
            {
                names.Add("Clone");
                names.Add("New");
                names.Add("Delete");
            }

            int index = Popup(layerIndex, names.ToArray(), 4, 0.6f);
            if (index == -1 || index >= names.Count)
            {
            }
            else if (names[index] == "Clone")
            {
                layers.Add(new Layer(Layer));
                Layer = layers[layers.Count - 1];
                Layer.name += " (Clone)";
                oldLayer = new Layer(Layer);
            }
            else if (names[index] == "New")
            {
                layers.Add(new Layer());
                Layer = layers[layers.Count - 1];
                oldLayer = new Layer(Layer);
            }
            else if (names[index] == "Delete")
            {
                if (CourseBase.SplineLayers.Count > 1 && PlatformBase.Editor.OkCancelDialog("Layer library", "Are you sure you want to remove (" + Layer.name + ") layer?"))
                {
                    layers.Remove(Layer);
                    Layer = CourseBase.SplineLayers[0];
                    oldLayer = new Layer(Layer);
                }
            }
            else
            {
                layerIndex = index;
            }
            #endregion

            if (libraryName == "default") EditorGUI.BeginDisabledGroup(true);

            #region Clear
            Move(4, 0);
            SetColor(Red);

            if (IsHazardLayer) EditorGUI.BeginDisabledGroup(true);
            if (Button("CLR", 1, 0.6f))
            {
                if (PlatformBase.Editor.OkCancelDialog("Layer library", "Are you sure you want to clear (" + Layer.name + ") layer?"))
                {
                    layers[layerIndex] = new Layer();
                    oldLayer = new Layer(Layer);
                }
            }
            if (IsHazardLayer) EditorGUI.EndDisabledGroup();

            SetColor();
            Move(-4, 0.35f);
            MovePixels(0, 1);
            #endregion

            #region Spline Zone
            Label("| Spline Apperance", 3.5f);
            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(5, 1, 0.75f);
            {
                Label("  Name", 2.5f);
                Move(2.5f, 0);
                if (IsHazardLayer) EditorGUI.BeginDisabledGroup(true);
                Layer.name = TextField(Layer.name, 2.5f, 0.6f);
                if (IsHazardLayer) EditorGUI.EndDisabledGroup();
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Color", 2.5f);
                Move(2.5f, 0);
                Layer.LayerColor = ColorField(Layer.LayerColor, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.5f);

            Label("| Spline creation", 2.5f);
            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(5, 0.6f, 0.75f);
            {
                Label("  Meters per Point", 2.5f);
                Move(2.5f, 0);
                Layer.metersPerOnePoint = FloatField(Layer.metersPerOnePoint, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.5f);
            #endregion

            if (IsHazardLayer)
            {
                #region Mesh Zone
                Label("| Mesh creation", 2.5f);
                Move(0, 0.5f);
                MovePixels(0, 1);
                Background(5, 1.1f, 0.75f);

                Move(0, 0.4f);
                {
                    Label("  Points per edge", 2.5f);
                    Move(2.5f, 0);
                    Layer.pointsPerEdge = IntField(Layer.pointsPerEdge, 2.5f, 0.6f);
                    Move(-2.5f, 0);
                }
                Move(0, 0.5f);
                #endregion

                MoveToBottom();
                Move(5, -2.15f);
                MovePixels(1, 0);
                Background(5, 4.75f, 0.5f);

                #region Visual
                Label("| Visual", 2.5f);
                Move(0, 0.5f);
                MovePixels(0, 1);
                Background(5, 11, 0.75f);

                Label("  Hazard Color ", 2.5f);
                Move(2.5f, 0);
                Layer.hazardColor = ColorField(Layer.hazardColor, 2.5f, 0.6f);
                Move(-2.5f, 0);
                Move(0, 0.5f);

                Label("  Post", 2.5f);
                Move(2.5f, 0);
                Layer.HazardPost = ObjectField<GameObject>(Layer.HazardPost, 2.5f, 0.6f);
                Move(-2.5f, 0);
                Move(0, 0.5f);

                Label("  Meters per Post", 2.5f);
                Move(2.5f, 0);
                Layer.metersPerOnePost = FloatField(Layer.metersPerOnePost, 2.5f, 0.6f);
                Move(-2.5f, 0);
                #endregion
            }
            else
            {
                #region Mesh Zone
                Label("| Mesh creation", 2.5f);
                Move(0, 0.5f);
                MovePixels(0, 1);
                Background(5, 1.1f, 0.75f);
                {
                    Move(0.2f, 0);
                    Layer.fillType = (Layer.FillType)Popup((int)Layer.fillType, Utility.EnumNames<Layer.FillType>(), 2.5f, 0.6f);
                    Move(-0.2f, 0);

                    Move(2.5f, 0);
                    Layer.resolution = FloatField(Layer.resolution, 2.5f, 0.6f);
                    Move(-2.5f, 0);
                }
                Move(0, 0.4f);
                {
                    Label("  Points per edge", 2.5f);
                    Move(2.5f, 0);
                    Layer.pointsPerEdge = IntField(Layer.pointsPerEdge, 2.5f, 0.6f);
                    Move(-2.5f, 0);
                }
                Move(0, 0.5f);
                #endregion

                MoveToBottom();
                Move(5, -8.5f);
                MovePixels(1, 0);
                Background(5, 11, 0.5f);

                #region Visual
                Label("| Visual", 2.5f);
                Move(0, 0.5f);
                MovePixels(0, 1);
                Background(5, 11, 0.75f);
                {
                    Move(0, 0);
                    {
                        Label("  Main Color", 2.5f);
                        Move(2.5f, 0);
                        Layer.mainColor = ColorField(Layer.mainColor, 2.5f, 0.6f);
                        Move(-2.5f, 0);
                    }
                    Move(0, 0.5f);
                    {
                        Label("  Specular Color", 2.5f);
                        Move(2.5f, 0);
                        Layer.specularColor = ColorField(Layer.specularColor, 2.5f, 0.6f);
                        Move(-2.5f, 0);
                    }
                    Move(0, 0.5f);
                    {
                        Label("  Texture 1", 2.5f);
                        Move(2.5f, 0);
                        Label("Texture 2", 2.5f);
                        Move(-2.5f, 0);
                        Move(0, 0.3f);

                        Move(0.2f, 0);
                        Layer.diffuse.TileTexture = ObjectField(Layer.diffuse.TileTexture, 2.3f, 2.3f);
                        Move(2.3f, 0);
                        Layer.detail.TileTexture = ObjectField(Layer.detail.TileTexture, 2.3f, 2.3f);
                        Move(-0.2f, 0);
                        Move(-2.3f, 0);
                        Move(0, 2.1f);

                        Move(0.2f, 0);
                        Layer.diffuse.tile = FloatField(Layer.diffuse.tile, 2.3f, 0.6f);
                        Move(2.3f, 0);
                        Layer.detail.tile = FloatField(Layer.detail.tile, 2.3f, 0.6f);
                        Move(-0.2f, 0);
                        Move(-2.3f, 0);

                        Move(0, 0.6f);

                        Label("  Normal 1", 2.5f);
                        Move(2.5f, 0);
                        Label("Normal 2", 2.5f);
                        Move(-2.5f, 0);
                        Move(0, 0.3f);

                        Move(0.2f, 0);
                        Layer.normal.TileTexture = ObjectField(Layer.normal.TileTexture, 2.3f, 2.3f);
                        Move(2.3f, 0);
                        Layer.detailNormal.TileTexture = ObjectField(Layer.detailNormal.TileTexture, 2.3f, 2.3f);
                        Move(-0.2f, 0);
                        Move(-2.3f, 0);
                        Move(0, 2.1f);

                        Move(0.2f, 0);
                        Layer.normal.tile = FloatField(Layer.normal.tile, 2.3f, 0.6f);
                        Move(2.3f, 0);
                        Layer.detailNormal.tile = FloatField(Layer.detailNormal.tile, 2.3f, 0.6f);
                        Move(-0.2f, 0);
                        Move(-2.3f, 0);

                        Move(0, 0.4f);

                        Move(0.2f, 0);
                        Layer.normal.bump = FloatSlider(Layer.normal.bump, 0, 1, 2.3f, 0.6f);
                        Move(2.3f, 0);
                        Layer.detailNormal.bump = FloatSlider(Layer.detailNormal.bump, 0, 1, 2.3f, 0.6f);
                        Move(-0.2f, 0);
                        Move(-2.3f, 0);
                    }
                    Move(0, 0.6f);
                    {
                        Label("  Fresnel Intensity", 2.5f);
                        Move(2.5f, 0);
                        MovePixels(-3, 0);
                        Layer.fresnelIntensity = FloatSlider(Layer.fresnelIntensity, 0.0f, 1, 2.65f, 0.6f);
                        MovePixels(3, 0);
                        Move(-2.5f, 0);
                    }
                }
                #endregion

                MoveToBottom();
                Move(5, -4.75f);
                MovePixels(1, 1);
                Background(5, 4.75f, 0.5f);

                SplineBase.SplinePattern pattern = Layer.pattern;
                #region Shape Zone
                Label("| Mesh shape", 2.5f);
                Move(2.5f, 0);
                Layer.PhysicsMaterial = ObjectField(Layer.PhysicsMaterial, 2.5f, 0.6f);
                Move(-2.5f, 0);
                {
                    Move(0, 0.5f);
                    MovePixels(0, 1);
                    Background(5, 1.0f, 0.75f);
                    {
                        MovePixels(-3, 0);
                        Move(0.2f, 0);
                        if (pattern.transition) SetColor(Green);
                        if (Button("Blend", 1.2f, 0.6f))
                        {
                            pattern.transition = !pattern.transition;
                        }
                        SetColor();
                        Move(-0.2f, 0);
                        MovePixels(3, 0);

                        if (pattern.transition)
                        {
                            MovePixels(-3, 0);
                            Label("              Width", 2.5f);
                            Move(2.5f, 0);
                            pattern.transitionLength = FloatSlider(pattern.transitionLength, 0.001f, 1, 2.65f, 0.6f);
                            Move(-2.5f, 0);
                            MovePixels(3, 0);
                        }
                    }
                    Move(0, 0.4f);
                    {
                        if (pattern.transition)
                        {
                            MovePixels(-3, 0);
                            Label("              Depth", 2.5f);
                            Move(2.5f, 0);
                            pattern.transitionDepth = FloatSlider(pattern.transitionDepth, -1, 1, 2.65f, 0.6f);
                            Move(-2.5f, 0);
                            MovePixels(3, 0);
                        }
                    }
                    Move(0, 0.6f);
                    MovePixels(0, 1);
                    MovePixels(0, 1);

                    Background(5, 2.1f, 0.75f);
                    {
                        MovePixels(-3, 0);
                        Move(0.2f, 0);
                        if (pattern.extra) SetColor(Green);
                        if (Button("Fringe", 1.2f, 0.6f))
                        {
                            pattern.extra = !pattern.extra;
                        }
                        SetColor();
                        Move(-0.2f, 0);
                        Move(2.5f, 0);
                        if (pattern.extra)
                        {
                            pattern.extraLayerName = LayerPopup(CourseBase.GetLayer(pattern.extraLayerName, CourseBase.SplineLayers), CourseBase.SplineLayers, 2.65f, 0.6f).name;
                        }
                        Move(-2.5f, 0);
                        MovePixels(3, 0);
                    }
                    Move(0, 0.4f);
                    if (pattern.extra)
                    {
                        MovePixels(-3, 0);
                        Label("              Width", 2.5f);
                        Move(2.5f, 0);
                        pattern.extraLength = FloatSlider(pattern.extraLength, 0.001f, 1, 2.65f, 0.6f);
                        Move(-2.5f, 0);
                        MovePixels(3, 0);
                    }
                    Move(0, 0.4f);
                    if (pattern.extra)
                    {
                        MovePixels(-3, 0);
                        Label("              Depth", 2.5f);
                        Move(2.5f, 0);
                        pattern.extraDepth = FloatSlider(pattern.extraDepth, -1, 1, 2.65f, 0.6f);
                        Move(-2.5f, 0);
                        MovePixels(3, 0);
                    }
                    Move(0, 0.4f);
                    if (pattern.extra)
                    {
                        MovePixels(-3, 0);
                        Move(0.2f, 0);
                        if (pattern.extraTransition) SetColor(Green);
                        if (Button("Blend", 1.2f, 0.6f))
                        {
                            pattern.extraTransition = !pattern.extraTransition;
                        }
                        SetColor();
                        Move(-0.2f, 0);
                        MovePixels(3, 0);

                        if (pattern.extraTransition)
                        {
                            MovePixels(-3, 0);
                            Label("              Width", 2.5f);
                            Move(2.5f, 0);
                            pattern.extraTransitionLength = FloatSlider(pattern.extraTransitionLength, 0.001f, 1, 2.65f, 0.6f);
                            Move(-2.5f, 0);
                            MovePixels(3, 0);
                        }
                    }
                    Move(0, 0.4f);
                    if (pattern.extra && pattern.extraTransition)
                    {
                        MovePixels(-3, 0);
                        Label("              Depth", 2.5f);
                        Move(2.5f, 0);
                        pattern.extraTransitionDepth = FloatSlider(pattern.extraTransitionDepth, -1, 1, 2.65f, 0.6f);
                        Move(-2.5f, 0);
                        MovePixels(3, 0);
                    }
                    Move(0, 0.5f);
                    MovePixels(0, 1);
                    MovePixels(0, 1);

                    Background(5, 1.3f, 0.75f);
                    {
                        MovePixels(-3, 0);
                        Move(0.2f, 0);
                        if (pattern.shrink) SetColor(Green);
                        if (Button("Shrink", 1.2f, 0.6f))
                        {
                            pattern.shrink = !pattern.shrink;
                        }
                        SetColor();
                        Move(-0.2f, 0);
                        MovePixels(3, 0);
                    }
                    if (pattern.shrink)
                    {
                        MovePixels(-3, 0);
                        Label("              Width", 2.5f);
                        Move(2.5f, 0);
                        pattern.shrinkLength = FloatSlider(pattern.shrinkLength, 0.001f, 1, 2.65f, 0.6f);
                        Move(-2.5f, 0);
                        MovePixels(3, 0);
                    }
                    Move(0, 0.4f);
                    if (pattern.shrink)
                    {
                        MovePixels(-3, 0);
                        Label("              Depth", 2.5f);
                        Move(2.5f, 0);
                        pattern.shrinkDepth = FloatSlider(pattern.shrinkDepth, -1, 1, 2.65f, 0.6f);
                        Move(-2.5f, 0);
                        MovePixels(3, 0);
                    }
                    Move(0, 0.5f);
                }
                #endregion
            }

            #region Change Sync
            bool materialChanged = false;
            bool layerChanged = false;

            if (layersCount != CourseBase.Layers.Count)
            {
                layerChanged = true;
                materialChanged = true;
            }
            if (oldLayer.name != Layer.name) layerChanged = true;
            if (oldLayer.LayerColor != Layer.LayerColor) materialChanged = true;
            if (oldLayer.mainColor.ToColor() != Layer.mainColor.ToColor()) materialChanged = true;
            if (oldLayer.specularColor.ToColor() != Layer.specularColor.ToColor()) materialChanged = true;
            if (oldLayer.hazardColor.ToColor() != Layer.hazardColor.ToColor()) materialChanged = true;
            if (oldLayer.diffuse.TileTexture != Layer.diffuse.TileTexture) materialChanged = true;
            if (oldLayer.detail.TileTexture != Layer.detail.TileTexture) materialChanged = true;
            if (oldLayer.normal.TileTexture != Layer.normal.TileTexture) materialChanged = true;
            if (oldLayer.detailNormal.TileTexture != Layer.detailNormal.TileTexture) materialChanged = true;
            if (oldLayer.fresnelIntensity != Layer.fresnelIntensity) materialChanged = true;
            if (oldLayer.diffuse.tile != Layer.diffuse.tile) materialChanged = true;
            if (oldLayer.detail.tile != Layer.detail.tile) materialChanged = true;
            if (oldLayer.normal.tile != Layer.normal.tile) materialChanged = true;
            if (oldLayer.normal.bump != Layer.normal.bump) materialChanged = true;
            if (oldLayer.detailNormal.tile != Layer.detailNormal.tile) materialChanged = true;
            if (oldLayer.detailNormal.bump != Layer.detailNormal.bump) materialChanged = true;
            if (oldLayer.resolution != Layer.resolution) layerChanged = true;
            if (oldLayer.PhysicsMaterial != Layer.PhysicsMaterial) materialChanged = true;
            if (oldLayer.HazardPost != Layer.HazardPost) materialChanged = true;
            if (oldLayer.metersPerOnePoint != Layer.metersPerOnePoint) layerChanged = true;
            if (oldLayer.metersPerOnePost != Layer.metersPerOnePost) layerChanged = true;
            if (oldLayer.pointsPerEdge != Layer.pointsPerEdge) layerChanged = true;

            if (oldLayer.pattern.transition != Layer.pattern.transition) layerChanged = true;
            if (oldLayer.pattern.transitionLength != Layer.pattern.transitionLength) layerChanged = true;
            if (oldLayer.pattern.transitionDepth != Layer.pattern.transitionDepth) layerChanged = true;
            if (oldLayer.pattern.extra != Layer.pattern.extra) layerChanged = true;
            if (oldLayer.pattern.extraLayerIndex != Layer.pattern.extraLayerIndex) layerChanged = true;
            if (oldLayer.pattern.extraLayerName != Layer.pattern.extraLayerName) layerChanged = true;
            if (oldLayer.pattern.extraLength != Layer.pattern.extraLength) layerChanged = true;
            if (oldLayer.pattern.extraDepth != Layer.pattern.extraDepth) layerChanged = true;
            if (oldLayer.pattern.extraTransition != Layer.pattern.extraTransition) layerChanged = true;
            if (oldLayer.pattern.extraTransitionLength != Layer.pattern.extraTransitionLength) layerChanged = true;
            if (oldLayer.pattern.extraTransitionDepth != Layer.pattern.extraTransitionDepth) layerChanged = true;
            if (oldLayer.pattern.shrink != Layer.pattern.shrink) layerChanged = true;
            if (oldLayer.pattern.shrinkLength != Layer.pattern.shrinkLength) layerChanged = true;
            if (oldLayer.pattern.shrinkDepth != Layer.pattern.shrinkDepth) layerChanged = true;

            if (layerChanged)
            {
                CourseBase.OnLayerChange();
            }
            if (materialChanged)
            {
                CourseBase.OnLayerMaterialChange();
            }
            #endregion

            MoveToLeft();
            MoveToBottom();
            Move(1, -5.75f);
            Touch(15, 5.75f);

            if (libraryName == "default") EditorGUI.EndDisabledGroup();
            EndUI();
        }
        #endregion
    }
}