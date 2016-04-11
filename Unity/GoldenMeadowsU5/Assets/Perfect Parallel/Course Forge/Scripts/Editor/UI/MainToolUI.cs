using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Course Forge main tool
    /// entry point of Course Forge
    /// controls all tools, progress bar, scene settings
    /// </summary>
    public class MainToolUI : ToolUI
    {
        public enum Tool
        {
            None = 0,
            FileTool,
            SplineTool,
            PlantTool
        }

        #region Fields
        static MainToolUI instance = null;
        static string initScene = "unknown_gg55_ffZBACL13";
        static bool progressBar = false;

        FileToolUI fileTool = new FileToolUI();
        SplineToolUI splineTool = new SplineToolUI();
        PlantToolUI plantTool = new PlantToolUI();
        LibraryToolUI layerTool = new LibraryToolUI();
        InfoToolUI infoTool = new InfoToolUI();
        PopupToolUI popupTool = new PopupToolUI();
        AndroidToolUI androidTool = new AndroidToolUI();
        #endregion

        #region Properties
        public override string ToolName
        {
            get
            {
                if (SelectedTool == Tool.FileTool) return fileTool.ToolName;
                if (SelectedTool == Tool.SplineTool) return splineTool.ToolName;
                if (SelectedTool == Tool.PlantTool) return plantTool.ToolName;
                return SelectedTool.ToString();
            }
        }

        bool ToolLocked
        {
            get
            {
                return EULA == false || CourseBase.TerrainSelected || SceneView.lastActiveSceneView.orthographic || Tools.current != UnityEditor.Tool.View;
            }
        }

        /// <summary>
        /// Selected Tool
        /// </summary>
        public static Tool SelectedTool
        {
            get
            {
                return (Tool)EditorPrefs.GetInt("Course Forge/SelectedTool", 0);
            }
            set
            {
                EditorPrefs.SetInt("Course Forge/SelectedTool", (int)value);
            }
        }
        /// <summary>
        /// Normal mode
        /// </summary>
        public static bool NormalMode
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/NormalMode", true);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/NormalMode", value);
            }
        }
        /// <summary>
        /// Community mode
        /// </summary>
        public static bool CommunityMode
        {
            get
            {
                if (EditorPrefs.GetString("Course Forge/CommunityMode", "") == "a83504b8544334b1e9db4b4824c344f3") return false;
                if (EditorPrefs.GetString("Course Forge/CommunityMode", "") == "2044291771162133d8d2b6fa283fd839") return false;
                if (EditorPrefs.GetString("Course Forge/CommunityMode", "") == "a0c0bf7a6014757a48cf1221b3e13bba") return false;
                if (EditorPrefs.GetString("Course Forge/CommunityMode", "") == "78ab998cc0e9f41b16c595c1ed24360f") return false;
                return true;
            }
        }
        /// <summary>
        /// Current units
        /// </summary>
        public static Units CurrentUnits
        {
            get
            {
                return (Units)EditorPrefs.GetInt("Course Forge/Units", 1);
            }
            set
            {
                EditorPrefs.SetInt("Course Forge/Units", (int)value);
            }
        }
        /// <summary>
        /// Current EULA
        /// </summary>
        public static bool EULA
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/EULA", false);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/EULA", value);
            }
        }
        #endregion

        #region Constructors
        MainToolUI()
        {
            List<SplineBase> splines = CourseBase.Splines;
            for (int i = 0; i < splines.Count; ++i)
                splines[i].Renderer.UpdateVisibility(splines[i], !HideMeshes);
        }
        #endregion

        #region Entry Methods
        [MenuItem("Perfect Parallel/Course Forge On %&e", true)]
        public static bool EnableValidate()
        {
            return instance == null;
        }
        [MenuItem("Perfect Parallel/Course Forge On %&e", false, 9)]
        public static void EnablePerform()
        {
            EditorApplication.ExecuteMenuItem("GameObject/Hidden/Course Forge/Platform/Initialize");
            ToolUI.Initialize();

            if (CourseBase.Initialize())
            {
                instance = new MainToolUI();
                initScene = EditorApplication.currentScene;

                SceneView.onSceneGUIDelegate = null;
                SceneView.onSceneGUIDelegate += OnScene;
            }
        }

        [MenuItem("Perfect Parallel/Course Forge Off", true)]
        public static bool DisableValidation()
        {
            return instance != null;
        }
        [MenuItem("Perfect Parallel/Course Forge Off", false, 10)]
        public static void DisablePerform()
        {
            SceneView.onSceneGUIDelegate -= OnScene;
            SceneView.onSceneGUIDelegate = null;

            if (instance != null)
            {
                if (PlatformBase.IO.IsEditor) PlatformBase.Editor.SaveScene();
                instance.OnCleanup();
                instance = null;
            }
            initScene = "unknown_gg55_ffZBACL13";
        }

        [MenuItem("Perfect Parallel/Course Forge Settings/Normal Mode", true)]
        public static bool SettingsNormalValidate()
        {
            return !NormalMode;
        }
        [MenuItem("Perfect Parallel/Course Forge Settings/Normal Mode", false, 13)]
        public static void SettingNormalPerform()
        {
            NormalMode = true;
        }
        [MenuItem("Perfect Parallel/Course Forge Settings/Advanced Mode", true)]
        public static bool SettingsAdvancedValidate()
        {
            return NormalMode;
        }
        [MenuItem("Perfect Parallel/Course Forge Settings/Advanced Mode", false, 14)]
        public static void SettingsAdvancedPerform()
        {
            NormalMode = false;
        }

        [MenuItem("Perfect Parallel/Course Forge Settings/Units Metric", true)]
        public static bool UnitsMetricValidate()
        {
            return CurrentUnits == Units.Imperial;
        }
        [MenuItem("Perfect Parallel/Course Forge Settings/Units Metric", false, 31)]
        public static void UnitsMetricPerform()
        {
            CurrentUnits = Units.Metric;
        }
        [MenuItem("Perfect Parallel/Course Forge Settings/Units Imperial", true)]
        public static bool UnitsImperialValidate()
        {
            return CurrentUnits == Units.Metric;
        }
        [MenuItem("Perfect Parallel/Course Forge Settings/Units Imperial", false, 32)]
        public static void UnitsImperialPerform()
        {
            CurrentUnits = Units.Imperial;
        }
        #endregion

        #region UI Methods
        bool ToolButton(MainToolUI.Tool tool, Texture2D texture)
        {
            if (SelectedTool == tool) SetColor(Green);
            bool clicked = Button(texture);
            if (SelectedTool == tool) SetColor(White);

            if (clicked)
            {
                if (SelectedTool == tool) SelectedTool = MainToolUI.Tool.None;
                else SelectedTool = tool;

                if (SelectedTool != MainToolUI.Tool.FileTool) InfoPanel = false;
            }

            return clicked;
        }
        #endregion

        #region Methods
        static void OnScene(SceneView view)
        {
            if (instance == null || initScene != EditorApplication.currentScene)
            {
                DisablePerform();
            }
            else
            {
                instance.OnScene(true);
            }
        }

        public override void OnScene(bool selected)
        {
            #region ProgressBar
            if (CourseBase.BuildSplinesCount != 0)
            {
                float count = CourseBase.BuildSplinesCount;
                float buildLineCount = CourseBase.BuildLineSplines.Count;
                float buildCount = CourseBase.BuildSplines.Count;

                float progress = buildLineCount / count * 0.5f + buildCount / count * 0.5f;
                string text = "Done";
                if (buildLineCount != 0) text = "Updating " + CourseBase.BuildLineSplines[0].name;
                else if (buildCount != 0) text = "Building " + CourseBase.BuildSplines[0].name;

                if (EditorUtility.DisplayCancelableProgressBar("Build progress bar", text, 1.0f - progress))
                {
                    CourseBase.BuildLineSplines.Clear();
                    CourseBase.BuildSplines.Clear();
                    CourseBase.BuildSplinesCount = 0;
                }
                else
                {
                    SplineBase spline = null;
                    if (CourseBase.BuildLineSplines.Count != 0) spline = CourseBase.BuildLineSplines[0];
                    else if (CourseBase.BuildSplines.Count != 0) spline = CourseBase.BuildSplines[0];

                    Exception exception = CourseBase.DoSplineBakeStep();
                    if (exception != null)
                    {
                        string message = exception.Message;

                        if (exception.StackTrace.Contains("Poly2Tri.DTSweep.FinalizationPolygon (Poly2Tri.DTSweepContext tcx)"))
                            message = "Too big extra/shrink.";
                        if (exception.StackTrace.Contains("New points from triangulation."))
                            message = "Wrong spline shape or/and lines are crossed.";
                        if (exception.Message.Contains("stack overflow"))
                            message = "Too many vertices in one mesh.";

                        Debug.LogException(exception);
                        if (spline && !string.IsNullOrEmpty(message)) PopupToolUI.Add(message, "Focus", spline.name);
                    }
                }
                progressBar = true;
            }
            else if (GeoJSON.Exporting)
            {
                float progress = GeoJSON.Progress;
                string text = "Done";
                if (progress != 1) text = "Exporting " + (progress * 100).ToString("F2") + "%";

                if (EditorUtility.DisplayCancelableProgressBar("Export progress bar", text, progress))
                {
                    GeoJSON.CancelExport();
                }
                else
                {
                    for (int i = 0; i < 10000; ++i) GeoJSON.DoExportTerrainStep();
                }
                progressBar = true;
            }
            else
            {
                if (progressBar)
                {
                    EditorUtility.ClearProgressBar();
                    progressBar = false;
                }
            }
            #endregion

            if (selected)
            {
                OnAutoHide();
                OnAutoTerrain();
                Update();
                if (!HideLines) splineTool.OnScene(SelectedTool == Tool.SplineTool);
                if (!HideLines) plantTool.OnScene(SelectedTool == Tool.PlantTool);
            }

            OnUI(selected);

            if (selected)
            {
                if (!ToolLocked)
                {
                    if (LayerPanel) layerTool.OnUI(true);
                    if (InfoPanel) infoTool.OnUI(true);
                    if (WarningPanel) popupTool.OnUI(true);
                    if (AndroidPanel) androidTool.OnUI(true);
                    fileTool.OnUI(SelectedTool == Tool.FileTool);
                    plantTool.OnUI(SelectedTool == Tool.PlantTool);
                    splineTool.OnUI(SelectedTool == Tool.SplineTool);
                }
                HandleUtility.Repaint();
            }

            #region Touch
            BeginUI();
            if (!ToolLocked) Touch(1, Screen.height / BoxSize);
            if (SelectedTool != Tool.None) Touch(Screen.width / BoxSize - 2, 1);
            EndUI();
            #endregion

            #region Tooltip
            if (!string.IsNullOrEmpty(GUI.tooltip))
            {
                string text = GUI.tooltip;

                Rect rect = new Rect();
                rect.x = Event.current.mousePosition.x;
                rect.y = Event.current.mousePosition.y;
                rect.width = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                rect.height = GUI.skin.label.CalcSize(new GUIContent(text)).y;

                BeginUI();
                MoveToPixels((int)rect.x, (int)rect.y);
                SetBoxSize((int)rect.width, (int)rect.height);
                Background(1, 1, 0.9f);
                LabelNoOffset(text);
                EndUI();
            }
            #endregion
        }
        public override void OnUI(bool selected)
        {
            #region Key Events
            if (Event.current.keyCode == KeyCode.Tab && Event.current.type == EventType.keyDown)
            {
                if (EULA == false) EULA = true;
                else if (SceneView.lastActiveSceneView.orthographic) SceneView.lastActiveSceneView.orthographic = false;
                else if (Tools.current != UnityEditor.Tool.View) Tools.current = UnityEditor.Tool.View;
                else if (PlatformBase.IO.IsEditor)
                {
                    if (PlatformBase.Editor.ActiveTransform == null) PlatformBase.Editor.ActiveTransform = CourseBase.Terrain.transform;
                    else PlatformBase.Editor.ActiveTransform = null;
                }
                
                Event.current.Use();
            }
            #endregion

            BeginUI();

            MoveToRight();
            Move(-3, 0);
            Background();
            if (Button(xTexture))
            {
                DisablePerform();
            }
            MoveToLeft();

            Background();
            if (!ToolLocked)
            {
                Move(0, 1);
                Background(1, Screen.height / BoxSize - 1);
                Move(0, -1);
            }

            Move(1, 0);
            if (SelectedTool != Tool.None) Background(Screen.width / BoxSize - 3);
            Move(-1, 0);

            #region Banner Messages
            if (EULA == false)
            {
                if (Button(wwwTexture)) SelectedTool = SelectedTool;

                Move(1, 0);
                SetColor(HalfGreen);
                if (Button("End User License Agreement - Press TAB to agree", 8))
                {
                    EULA = true;
                }
                SetColor();
                Move(-1, 0);

                Move(0, 1);
                Background(Screen.width / BoxSize - 2, Screen.height / BoxSize - 1);
                Background(Screen.width / BoxSize - 2, Screen.height / BoxSize - 1);

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.wordWrap = true;
                labelStyle.normal.textColor = Color.white;

                Rect scrollViewRect = GetRect(Screen.width / BoxSize - 2, (Screen.height - TopBoxHeight) / BoxSize - 1);

                float textHeight = 0;
                for (int i = 0; i < eulaText.Length; i += 4096)
                {
                    string text = eulaText.Substring(i, Mathf.Min(eulaText.Length - i, 4096));
                    if (text.Contains("\n") && text.Length == 4096)
                    {
                        int index = text.LastIndexOf("\n");
                        text = eulaText.Substring(i, index);
                        i += index;
                        i -= 4096;
                    }

                    textHeight += labelStyle.CalcHeight(new GUIContent(text), scrollViewRect.width - 20);
                }

                Rect textRect = new Rect(0, 0, scrollViewRect.width - 20, textHeight);
                eulaScrollPos = GUI.BeginScrollView(scrollViewRect, eulaScrollPos, textRect);

                for (int i = 0; i < eulaText.Length; i += 4096)
                {
                    string text = eulaText.Substring(i, Mathf.Min(eulaText.Length - i, 4096));
                    if (text.Contains("\n") && text.Length == 4096)
                    {
                        int index = text.LastIndexOf("\n");
                        text = eulaText.Substring(i, index);
                        i += index;
                        i -= 4096;
                    }

                    textRect.height = labelStyle.CalcHeight(new GUIContent(text), scrollViewRect.width - 20);

                    SetColor(Black);
                    GUI.Label(new Rect(textRect.x + 1, textRect.y + 1, textRect.width, textRect.height), text, labelStyle);
                    SetColor();
                    GUI.Label(textRect, text, labelStyle);

                    textRect.y += textRect.height;
                }

                GUI.EndScrollView();
                Move(0, -1);

                EndUI();
                return;
            }
            else if (SceneView.lastActiveSceneView.orthographic)
            {
                if (Button(wwwTexture)) SelectedTool = SelectedTool;

                Move(1, 0);
                SetColor(HalfGreen);
                if (Button("CourseForge has limited functionality in ISO mode, press TAB to switch back", 14))
                {
                    SceneView.lastActiveSceneView.orthographic = false;
                }
                SetColor();
                Move(-1, 0);

                EndUI();
                return;
            }
            else if (Tools.current != UnityEditor.Tool.View)
            {
                if (Button(wwwTexture)) SelectedTool = SelectedTool;

                Move(1, 0);
                SetColor(HalfGreen);
                if (Button("CourseForge has limited functionality in this view mode, press TAB to switch back", 14))
                {
                    Tools.current = UnityEditor.Tool.View;
                }
                SetColor();
                Move(-1, 0);

                EndUI();
                return;
            }
            else if (CourseBase.TerrainSelected)
            {
                if (Button(wwwTexture)) SelectedTool = SelectedTool;

                Move(1, 0);
                SetColor(HalfGreen);
                if (Button("Terrain Mode - Press TAB to exit", 6))
                {
                    PlatformBase.Editor.ActiveTransform = null;
                }
                SetColor();
                Move(-1, 0);

                EndUI();
                return;
            }
            else if (selected)
            {
                if (ToolButton(Tool.FileTool, wwwTexture)) OnCleanup();

                Move(1, 0);
                if (SelectedTool > Tool.FileTool && Button((ToolName.Length > 8 ? ToolName.Substring(0, 8) + "..." : ToolName), 2))
                {
                    GameObject toolObject = GameObject.Find(ToolName);
                    if (toolObject != null)
                    {
                        EditorGUIUtility.PingObject(toolObject);
                        Selection.activeGameObject = toolObject;
                    }
                }
                Move(-1, 0);
            }
            Move(0, 1);
            #endregion

            #region Top
            if (ToolButton(Tool.SplineTool, freehandTexture))
            {
                OnCleanup();
                if (CourseBase.TerrainLowered) CourseBase.RestoreTerrain();
            }
            Move(0, 1);

            if (ToolButton(Tool.PlantTool, plantTexture))
            {
                OnCleanup();
                if (CourseBase.TerrainLowered) CourseBase.RestoreTerrain();
            }
            Move(0, 1);

            if (Button(terrainTexture))
            {
                SelectedTool = Tool.None;
                OnCleanup();

                if (PlatformBase.IO.IsEditor) PlatformBase.Editor.ActiveTransform = CourseBase.Terrain.transform;
            }
            Move(0, 1);

            if (CourseBase.TerrainLowered) SetColor(Red);
            if (Button(terrainLoweredTexture))
            {
                SelectedTool = Tool.None;
                OnCleanup();

                if (!CourseBase.TerrainLowered)
                {
                    CourseBase.SaveTerrain();
                    CourseBase.LowerTerrain();
                }
                else
                {
                    CourseBase.RestoreTerrain();
                }
            }
            SetColor();
            Move(0, 1);

            if (Button(updateTexture))
            {
                if (CourseBase.TerrainLowered) CourseBase.RestoreTerrain();
                CourseBase.RefreshCourse();
            }
            Move(0, 1);

            if (Button(buildSplinesTexture))
            {
                if (CourseBase.TerrainLowered) CourseBase.RestoreTerrain();
                PopupToolUI.Clear();
                CourseBase.BakeSplines(true);
            }
            Move(0, 1);
            #endregion

            #region Bottom
            MoveToBottom();
            Move(0, -1);

            if (HideLines) SetColor(HalfGrey);
            if (Button(splineEyeTexture))
            {
                HideLines = !HideLines;
            }
            SetColor();
            Move(0, -1);

            if (AutoHide) SetColor(HalfGrey);
            if (Button(autoEyeTexture))
            {
                AutoHide = !AutoHide;
            }
            SetColor();
            Move(0, -1);

            if (HideMeshes) SetColor(HalfGrey);
            if (Button(eyeTexture))
            {
                AutoHide = false;
                HideMeshes = !HideMeshes;

                List<SplineBase> splines = CourseBase.Splines;
                for (int i = 0; i < splines.Count; ++i)
                    splines[i].Renderer.UpdateVisibility(splines[i], !HideMeshes);
            }
            SetColor();
            Move(0, -1);

            if (MainToolUI.NormalMode)
            {
                EditorGUI.BeginDisabledGroup(true);
                LayerPanel = false;
            }

            if (LayerPanel) SetColor(Green);
            if (Button(layersTexture))
            {
                LayerPanel = !LayerPanel;
            }
            SetColor();
            Move(0, -1);

            if (MainToolUI.NormalMode) EditorGUI.EndDisabledGroup();
            #endregion

            EndUI();
        }
        public override void OnCleanup()
        {
            fileTool.OnCleanup();
            splineTool.OnCleanup();
            plantTool.OnCleanup();
            layerTool.OnCleanup();
            infoTool.OnCleanup();
        }
        #endregion

        #region Support Methods
        void OnAutoHide()
        {
            if (AutoHide)
            {
                if (CourseBase.TerrainSelected)
                {
                    if (!HideMeshes)
                    {
                        HideMeshes = true;

                        List<SplineBase> splines = CourseBase.Splines;
                        for (int i = 0; i < splines.Count; ++i)
                            splines[i].Renderer.UpdateVisibility(splines[i], false);
                    }
                }
                else
                {
                    if (HideMeshes)
                    {
                        HideMeshes = false;

                        List<SplineBase> splines = CourseBase.Splines;
                        for (int i = 0; i < splines.Count; ++i)
                            splines[i].Renderer.UpdateVisibility(splines[i], true);
                    }
                }
            }
        }
        void OnAutoTerrain()
        {
            if (CourseBase.TerrainSelected && CourseBase.TerrainLowered)
            {
                CourseBase.RestoreTerrain();
            }
        }
        #endregion
    }
}