using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Base class for the Course Forge interfaces,
    /// methods to draw UI, to get information from mouse clicking events,
    /// base virtual methods for tools
    /// </summary>
    public class ToolUI
    {
        #region Protected Fields
        protected static Texture2D wwwTexture = null;
        protected static Texture2D helpTexture = null;
        protected static Texture2D freehandTexture = null;
        protected static Texture2D terrainTexture = null;
        protected static Texture2D updateTexture = null;
        protected static Texture2D pointTexture = null;
        protected static Texture2D rectangleTexture = null;
        protected static Texture2D buildSplinesTexture = null;
        protected static Texture2D eyeTexture = null;
        protected static Texture2D autoEyeTexture = null;
        protected static Texture2D layersTexture = null;
        protected static Texture2D terrainLoweredTexture = null;
        protected static Texture2D blackTexture = null;
        protected static Texture2D exportTexture = null;
        protected static Texture2D importTexture = null;
        protected static Texture2D buildTexture = null;
        protected static Texture2D plantTexture = null;
        protected static Texture2D teeTexture = null;
        protected static Texture2D cupTexture = null;
        protected static Texture2D splineEyeTexture = null;
        protected static Texture2D shotsTexture = null;
        protected static Texture2D measureTexture = null;
        protected static Texture2D flyByTexture = null;
        protected static Texture2D courseInfoTexture = null;
        protected static Texture2D lockTexture = null;
        protected static Texture2D unlockTexture = null;
        protected static Texture2D holeTexture = null;
        protected static Texture2D playTexture = null;
        protected static Texture2D pauseTexture = null;
        protected static Texture2D brushTexture = null;
        protected static Texture2D pencilTexture = null;
        protected static Texture2D xTexture = null;
        protected static Texture2D androidTexture = null;
        protected static string eulaText = "";
        protected static Vector2 eulaScrollPos = Vector2.zero;
        #endregion

        #region Color Fields
        protected static Color White = Color.white;
        protected static Color Black = Color.black;
        protected static Color Green = Color.green;
        protected static Color Yellow = Color.yellow;
        protected static Color Red = Color.red;
        protected static Color HalfGreen = Color.Lerp(Color.green, Color.white, 0.5f);
        protected static Color HalfGrey = Color.Lerp(Color.black, Color.white, 0.5f);
        #endregion

        #region Private Fields
        static DateTime lastTime = default(DateTime);
        static DateTime lastRealTime = default(DateTime);
        static Vector3 mouseWorldPos = Vector3.zero;
        static Vector3 mouseWorldPosTerrain = Vector3.zero;
        static Vector3 startLine = Vector3.zero;
        static Vector3 endLine = Vector3.zero;
        static bool doubleClick = false;
        static float deltaTime = 0;

        static int boxSize = 48;
        static int boxOffset = 6;
        static int topBoxHeight = 38;
        static int rx = 0;
        static int ry = 0;
        static int boxWidth = 0;
        static int boxHeight = 0;
        const float boxTransparency = 0.25f;
        #endregion

        #region Properties
        /// <summary>
        /// Name of the tool
        /// </summary>
        public virtual string ToolName
        {
            get
            {
                return "Tool Name";
            }
        }
        /// <summary>
        /// Is left mouse button is down
        /// </summary>
        public static bool LeftMouseDown
        {
            get
            {
                return Event.current.type == EventType.MouseDown && Event.current.button == 0;
            }
        }
        /// <summary>
        /// Is right mouse button is down
        /// </summary>
        public static bool RightMouseDown
        {
            get
            {
                return Event.current.type == EventType.MouseDown && Event.current.button == 1;
            }
        }
        /// <summary>
        /// Is left mouse button in drag mode
        /// </summary>
        public static bool LeftMouseDrag
        {
            get
            {
                return Event.current.type == EventType.MouseDrag && Event.current.button == 0;
            }
        }
        /// <summary>
        /// Is left mouse button is up
        /// </summary>
        public static bool LeftMouseUp
        {
            get
            {
                return Event.current.type == EventType.MouseUp && Event.current.button == 0;
            }
        }
        /// <summary>
        /// Is left mouse button double-clicked
        /// </summary>
        public static bool LeftDoubleClick
        {
            get
            {
                return doubleClick;
            }
        }
        /// <summary>
        /// Current frame delta time
        /// </summary>
        public static float DeltaTime
        {
            get
            {
                return deltaTime;
            }
        }
        /// <summary>
        /// Box Size
        /// </summary>
        public static float BoxSize
        {
            get
            {
                return boxSize;
            }
        }
        /// <summary>
        /// Top Box Size
        /// </summary>
        public static float TopBoxHeight
        {
            get
            {
                return topBoxHeight;
            }
        }

        protected static bool AutoHide
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/AutoHide", false);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/AutoHide", value);
            }
        }
        protected static bool HideMeshes
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/HideMeshes", false);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/HideMeshes", value);
            }
        }
        protected static bool HideLines
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/HideLines", false);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/HideLines", value);
            }
        }
        protected static bool LayerPanel
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/LayerPanel", false);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/LayerPanel", value);
            }
        }
        protected static bool InfoPanel
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/InfoPanel", false);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/InfoPanel", value);
            }
        }
        protected static bool AndroidPanel
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/AndroidPanel", false);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/AndroidPanel", value);
            }
        }
        protected static bool WarningPanel
        {
            get
            {
                return EditorPrefs.GetBool("Course Forge/WarningPanel", true);
            }
            set
            {
                EditorPrefs.SetBool("Course Forge/WarningPanel", value);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Executes on each scene redraw
        /// </summary>
        /// <param name="selected">tool selected</param>
        public virtual void OnScene(bool selected)
        {
        }
        /// <summary>
        /// Executes on each UI redraw
        /// </summary>
        /// <param name="selected"></param>
        public virtual void OnUI(bool selected)
        {
        }
        /// <summary>
        /// Executes on tool cleanup
        /// </summary>
        public virtual void OnCleanup()
        {
        }
        /// <summary>
        /// Initialize the tool
        /// </summary>
        public static void Initialize()
        {
            wwwTexture = Resources.Load("www", typeof(Texture)) as Texture2D;
            helpTexture = Resources.Load("help", typeof(Texture)) as Texture2D;
            freehandTexture = Resources.Load("freehand", typeof(Texture)) as Texture2D;
            terrainTexture = Resources.Load("terrain", typeof(Texture)) as Texture2D;
            updateTexture = Resources.Load("update", typeof(Texture)) as Texture2D;
            pointTexture = Resources.Load("point", typeof(Texture)) as Texture2D;
            rectangleTexture = Resources.Load("rectangle", typeof(Texture)) as Texture2D;
            buildSplinesTexture = Resources.Load("group", typeof(Texture)) as Texture2D;
            eyeTexture = Resources.Load("eye", typeof(Texture)) as Texture2D;
            autoEyeTexture = Resources.Load("autoEye", typeof(Texture)) as Texture2D;
            layersTexture = Resources.Load("layers", typeof(Texture)) as Texture2D;
            blackTexture = Resources.Load("black", typeof(Texture)) as Texture2D;
            exportTexture = Resources.Load("export", typeof(Texture)) as Texture2D;
            importTexture = Resources.Load("import", typeof(Texture)) as Texture2D;
            buildTexture = Resources.Load("build", typeof(Texture)) as Texture2D;
            plantTexture = Resources.Load("plant", typeof(Texture)) as Texture2D;
            teeTexture = Resources.Load("tee", typeof(Texture)) as Texture2D;
            cupTexture = Resources.Load("cup", typeof(Texture)) as Texture2D;
            splineEyeTexture = Resources.Load("splineEye", typeof(Texture)) as Texture2D;
            shotsTexture = Resources.Load("shots", typeof(Texture)) as Texture2D;
            measureTexture = Resources.Load("measure", typeof(Texture)) as Texture2D;
            flyByTexture = Resources.Load("flyBy", typeof(Texture)) as Texture2D;
            courseInfoTexture = Resources.Load("settings", typeof(Texture)) as Texture2D;
            lockTexture = Resources.Load("lock", typeof(Texture)) as Texture2D;
            unlockTexture = Resources.Load("unlock", typeof(Texture)) as Texture2D;
            holeTexture = Resources.Load("hole", typeof(Texture)) as Texture2D;
            playTexture = Resources.Load("play", typeof(Texture)) as Texture2D;
            pauseTexture = Resources.Load("pause", typeof(Texture)) as Texture2D;
            terrainLoweredTexture = Resources.Load("terrainLowered", typeof(Texture)) as Texture2D;
            brushTexture = Resources.Load("brush", typeof(Texture)) as Texture2D;
            pencilTexture = Resources.Load("pencil", typeof(Texture)) as Texture2D;
            xTexture = Resources.Load("close", typeof(Texture)) as Texture2D;
            androidTexture = Resources.Load("android", typeof(Texture)) as Texture2D;
            eulaText = (Resources.Load("eula", typeof(TextAsset)) as TextAsset).text;
        }
        /// <summary>
        /// Updates the tool fields
        /// </summary>
        public static void Update()
        {
            deltaTime = ((float)(DateTime.Now - lastRealTime).TotalMilliseconds) / 1000.0f;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            startLine = ray.origin;
            endLine = startLine + ray.direction * 300.0f;

            RaycastHit? meshHit = CourseBase.CameraMeshHit;
            if (meshHit != null)
            {
                mouseWorldPos = meshHit.Value.point;
            }
            RaycastHit? terrainHit = CourseBase.CameraTerrainHit;
            if (terrainHit != null)
            {
                mouseWorldPosTerrain = terrainHit.Value.point;
            }

            if (LeftMouseDown)
            {
                DateTime Now = DateTime.Now;
                double delta = (Now - lastTime).TotalMilliseconds / 1000.0f;
                if (delta < 0.3) doubleClick = true;
                else doubleClick = false;
                lastTime = Now;
            }
            else
            {
                doubleClick = false;
            }

            lastRealTime = DateTime.Now;
        }
        #endregion

        #region Rect Methods
        /// <summary>
        /// Prepares UI
        /// </summary>
        public static void BeginUI()
        {
            MoveToLeft();
            MoveToTop();
            SetBoxSize(boxSize, boxSize);

            Handles.BeginGUI();
        }
        /// <summary>
        /// Frees UI
        /// </summary>
        public static void EndUI()
        {
            Handles.EndGUI();
        }
        /// <summary>
        /// Moves caret to top
        /// </summary>
        public static void MoveToTop()
        {
            ry = 0;
        }
        /// <summary>
        /// Moves caret to bottom
        /// </summary>
        public static void MoveToBottom()
        {
            ry = Screen.height - topBoxHeight;
        }
        /// <summary>
        /// Moves caret to left
        /// </summary>
        public static void MoveToLeft()
        {
            rx = 0;
        }
        /// <summary>
        /// Moves caret to right
        /// </summary>
        public static void MoveToRight()
        {
            rx = Screen.width;
        }
        /// <summary>
        /// Moves caret by x*boxWidth,y*boxHeight 
        /// </summary>
        /// <param name="xBoxes">count of x boxes</param>
        /// <param name="yBoxes">count of y boxes</param>
        public static void Move(float xBoxes, float yBoxes)
        {
            rx += (int)(boxWidth * xBoxes);
            ry += (int)(boxHeight * yBoxes);
        }
        /// <summary>
        /// Moves caret to x*boxWidth,y*boxHeight 
        /// </summary>
        /// <param name="xBoxes">count of x boxes</param>
        /// <param name="yBoxes">count of y boxes</param>
        public static void MoveTo(int xBoxes, int yBoxes)
        {
            rx = boxWidth * xBoxes;
            ry = boxHeight * yBoxes;
        }
        /// <summary>
        /// Moves caret by dx,dy
        /// </summary>
        /// <param name="xBoxes">x pixels</param>
        /// <param name="yBoxes">y pixels</param>
        public static void MovePixels(int dx, int dy)
        {
            rx += dx;
            ry += dy;
        }
        /// <summary>
        /// Moves caret to x,y
        /// </summary>
        /// <param name="xBoxes">x pixels</param>
        /// <param name="yBoxes">y pixels</param>
        public static void MoveToPixels(int x, int y)
        {
            rx = x;
            ry = y;
        }
        /// <summary>
        /// Sets the box width and height
        /// </summary>
        /// <param name="width">box width</param>
        /// <param name="height">box height</param>
        public static void SetBoxSize(int width, int height)
        {
            boxWidth = width;
            boxHeight = height;
        }
        /// <summary>
        /// Creates Rect
        /// </summary>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>rect</returns>
        public static Rect GetRect(float xBoxes = 1, float yBoxes = 1)
        {
            return new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes);
        }
        /// <summary>
        /// Creates Offset Rect
        /// </summary>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>rect</returns>
        public static Rect GetOffsetRect(float xBoxes = 1, float yBoxes = 1)
        {
            return OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
        }
        #endregion

        #region Base UI Methods
        /// <summary>
        /// Sets the color for next widget
        /// </summary>
        /// <param name="color">color of next widget</param>
        public static void SetColor(Color color = default(Color))
        {
            if (color == default(Color)) color = Color.white;
            GUI.color = color;
        }
        /// <summary>
        /// Creates background for widget
        /// </summary>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <param name="transparency">box transparency</param>
        public static void Background(float xBoxes = 1, float yBoxes = 1, float transparency = boxTransparency)
        {
            Color cachedColor = GUI.color;
            GUI.color = cachedColor * transparency;
            GUI.DrawTexture(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), blackTexture);
            GUI.color = cachedColor;
        }
        /// <summary>
        /// Creates button
        /// </summary>
        /// <param name="texture">background texture</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>true if clicked</returns>
        public static bool Button(Texture2D texture, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return GUI.Button(rect, texture);
        }
        /// <summary>
        /// Creates button
        /// </summary>
        /// <param name="text">button text</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>true if clicked</returns>
        public static bool Button(string text, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return GUI.Button(rect, text);
        }
        /// <summary>
        /// Creates button
        /// </summary>
        /// <param name="content">button content</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>true if clicked</returns>
        public static bool Button(GUIContent content, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return GUI.Button(rect, content);
        }
        /// <summary>
        /// Blocks the area from clicks
        /// </summary>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        public static void Touch(float xBoxes = 1, float yBoxes = 1)
        {
            if (GUI.Button(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), "", new GUIStyle()))
                Event.current.Use();
        }
        /// <summary>
        /// Creates label
        /// </summary>
        /// <param name="text">label text</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <param name="style">label style</param>
        public static void Label(string text, float xBoxes = 1, float yBoxes = 1, GUIStyle style = null)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            if (style != null) GUI.Label(rect, text, style);
            else GUI.Label(rect, text);
        }
        /// <summary>
        /// Creates popup
        /// </summary>
        /// <param name="index">selected index</param>
        /// <param name="names">array of names</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>selected index</returns>
        public static int Popup(int index, string[] names, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return EditorGUI.Popup(rect, index, names);
        }
        /// <summary>
        /// Creates text field
        /// </summary>
        /// <param name="text">field text</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        public static string TextField(string text, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return GUI.TextField(rect, text);
        }
        /// <summary>
        /// Creates color field
        /// </summary>
        /// <param name="color">field color</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>field color</returns>
        public static Color ColorField(Color color, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return EditorGUI.ColorField(rect, color);
        }
        /// <summary>
        /// Creates float field
        /// </summary>
        /// <param name="val">field value</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>value</returns>
        public static float FloatField(float val, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return EditorGUI.FloatField(rect, val);
        }
        /// <summary>
        /// Creates int field
        /// </summary>
        /// <param name="val">field value</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>value</returns>
        public static int IntField(int val, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return EditorGUI.IntField(rect, val);
        }
        /// <summary>
        /// Creates float slider
        /// </summary>
        /// <param name="val">field value</param>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>value</returns>
        public static float FloatSlider(float val, float min, float max, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            rect.width /= 2;

            val = GUI.HorizontalSlider(rect, val, min, max);

            rect.x += rect.width * 1.1f;
            rect.width *= 0.9f;

            return Mathf.Clamp(EditorGUI.FloatField(rect, val), min, max);
        }
        /// <summary>
        /// Creates object field
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="obj">field object</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>field object</returns>
        [Obfuscation(Exclude = true)]
        public static T ObjectField<T>(T obj, float xBoxes = 1, float yBoxes = 1) where T : UnityEngine.Object
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return (T)EditorGUI.ObjectField(rect, obj, typeof(T), false);
        }
        /// <summary>
        /// Creates toggle
        /// </summary>
        /// <param name="val">toggle value</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>value</returns>
        public static bool Toggle(bool val, float xBoxes = 1, float yBoxes = 1)
        {
            Rect rect = OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), boxOffset);
            return EditorGUI.Toggle(rect, val);
        }
        /// <summary>
        /// Creates HelpBox
        /// </summary>
        /// <param name="text">message</param>
        /// <param name="messageType">type of the message</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        public static void HelpBox(string text, MessageType messageType, float xBoxes = 1, float yBoxes = 1)
        {
            EditorGUI.HelpBox(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), text, messageType);
        }
        #endregion
        #region Extended UI Methods
        /// <summary>
        /// Layer popup
        /// </summary>
        /// <param name="text"></param>
        /// <param name="layer"></param>
        /// <param name="names"></param>
        /// <param name="xBoxes"></param>
        /// <param name="yBoxes"></param>
        /// <returns></returns>
        public static Layer LayerPopup(Layer layer, List<Layer> layers, float xBoxes = 1, float yBoxes = 1)
        {
            string[] names = new string[layers.Count];
            for (int i = 0; i < names.Length; ++i) names[i] = (i + 1) + " " + layers[i].name;

            int index = Popup(layers.IndexOf(layer), names, xBoxes, yBoxes);
            if (index == -1) return null;

            return layers[index];
        }
        /// <summary>
        /// Layer class popup
        /// </summary>
        /// <param name="text"></param>
        /// <param name="layer"></param>
        /// <param name="names"></param>
        /// <param name="xBoxes"></param>
        /// <returns></returns>
        public static Layer LayerPopup(string text, Layer layer, List<Layer> layers, float xBoxes = 1)
        {
            string[] names = new string[layers.Count];
            for (int i = 0; i < names.Length; ++i) names[i] = (i + 1) + " " + layers[i].name;

            Move(xBoxes - 0.6f, 0);
            if (!MainToolUI.NormalMode && Button("L", 0.6f, 0.6f))
            {
                if (LayerPanel && LibraryToolUI.SelectedLayer == layer)
                {
                    LayerPanel = false;
                }
                else
                {
                    LayerPanel = true;
                    LibraryToolUI.SelectedLayer = layer;
                }
            }
            Move(0.6f - xBoxes, 0);

            int index = Popup(text, layers.IndexOf(layer), names, xBoxes);
            if (index == -1) return null;

            return layers[index];
        }
        /// <summary>
        /// Creates long popup with background
        /// </summary>
        /// <param name="text">popup label</param>
        /// <param name="index">selection index</param>
        /// <param name="names">popup names</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        /// <returns>selection index</returns>
        public static int Popup(string text, int index, string[] names, float xBoxes = 1, float yBoxes = 0.6f)
        {
            Background(xBoxes);

            Label(text, xBoxes, 0.75f);
            Move(0, 0.4f);
            index = Popup(index, names, xBoxes, yBoxes);
            Move(0, -0.4f);
            return index;
        }
        /// <summary>
        /// Creates vector field
        /// </summary>
        /// <param name="text">field label</param>
        /// <param name="vector">vector</param>
        /// <param name="xBoxes">x boxes</param>
        /// <returns>vector</returns>
        public static Vector3 VectorField(string text, Vector3 vector, float xBoxes = 1)
        {
            Background(xBoxes);

            return EditorGUI.Vector3Field(OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight), boxOffset), text, vector);
        }
        /// <summary>
        /// Creates curve field
        /// </summary>
        /// <param name="text">field label</param>
        /// <param name="curve">field curve</param>
        /// <param name="reset">reset curve</param>
        /// <param name="xBoxes">x boxes</param>
        /// <returns>curve</returns>
        public static AnimationCurve CurveField(string text, AnimationCurve curve, AnimationCurve reset, float xBoxes = 1)
        {
            Background(xBoxes);

            Label(text, xBoxes, 0.6f);
            Move(xBoxes - 1, 0);
            if (Button("R", 1.0f, 0.6f) || curve[curve.length - 1].value != 1 || curve[0].value != 0 || curve[curve.length - 1].time != 1 || curve[0].time != 0 || curve.length < 2)
            {
                curve = reset;
            }
            Move(-(xBoxes - 1), 0);

            Move(0, 0.4f);
            curve = EditorGUI.CurveField(OffsetRect(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * 0.6f), boxOffset), curve);
            Move(0, -0.4f);

            return curve;
        }
        /// <summary>
        /// Creates key field
        /// </summary>
        /// <param name="key">field key</param>
        /// <param name="text">field label</param>
        /// <param name="xBoxes">x boxes</param>
        /// <returns>value</returns>
        public static string KeyField(string key, string text, float xBoxes)
        {
            Background(xBoxes);

            GUI.SetNextControlName("Key TextField");
            key = TextField(key, xBoxes - 3);
            if (key == "") Label(text, xBoxes - 3);
            Move(xBoxes - 3, 0);
            GUI.FocusControl("Key TextField");

            if (Button("R"))
            {
                key = "";
            }
            Move(1, 0);

            if (Button("Insert", 2))
            {
                TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.GetControlID(FocusType.Keyboard));
                textEditor.SelectAll();
                textEditor.Paste();
                textEditor.SelectAll();
                key = textEditor.SelectedText;
                textEditor.SelectNone();
            }
            Move(2 - xBoxes, 0);
            return key;
        }
        /// <summary>
        /// Creates length slider
        /// </summary>
        /// <param name="length">field length</param>
        /// <param name="text">field label</param>
        /// <param name="min">min value</param>
        /// <param name="max">max value</param>
        /// <param name="reset">reset value</param>
        /// <param name="xBoxes">x boxes</param>
        /// <returns>length</returns>
        public static float LengthSlider(float length, string text, float min, float max, float reset, float xBoxes)
        {
            Background(xBoxes);

            if (Button("R"))
            {
                length = reset;
            }
            Move(1, 0);
            Label(text + (length == reset ? " (" + reset + " is default)" : ""), xBoxes - 1, 0.6f);
            Move(0, 0.4f);
            length = FloatSlider(length, min, max, xBoxes - 1, 0.6f);
            Move(0, -0.4f);
            Move(-1, 0);

            return length;
        }
        /// <summary>
        /// Creates label without offset
        /// </summary>
        /// <param name="text">label text</param>
        /// <param name="xBoxes">x boxes</param>
        /// <param name="yBoxes">y boxes</param>
        public static void LabelNoOffset(string text, float xBoxes = 1, float yBoxes = 1)
        {
            GUI.Label(new Rect(rx, ry, boxWidth * xBoxes, boxHeight * yBoxes), text);
        }
        #endregion

        #region Support Methods
        static Rect OffsetRect(Rect rect, int size)
        {
            return new Rect(rect.x + size, rect.y + size, Mathf.Max(1, rect.width - size * 2), Mathf.Max(1, rect.height - size * 2));
        }

        /// <summary>
        /// Projects world point to screen Rect
        /// </summary>
        /// <param name="world">point</param>
        /// <param name="width">width in screen-space</param>
        /// <param name="height">height in screen-space</param>
        /// <param name="size">multiplier of the size</param>
        /// <returns>screen Rect</returns>
        public static Rect ProjectToScreenRect(Vector3 world, float width, float height)
        {
            Vector2 position = HandleUtility.WorldToGUIPoint(world);
            return new Rect(position.x - width / 2, position.y - height / 2, width, height);
        }
        /// <summary>
        /// Checks whenever world point is on screen
        /// </summary>
        /// <param name="world">point</param>
        /// <returns>true if on</returns>
        public static bool IsPointOnScreen(Vector3 world)
        {
            if (Camera.current)
            {
                Vector3 screen = Camera.current.WorldToScreenPoint(world);
                if (screen.z < 0) return false;
            }

            Vector2 position = HandleUtility.WorldToGUIPoint(world);
            if (position.x < boxSize || position.y < boxSize) return false;
            return true;
        }
        /// <summary>
        /// Checks if mouse is inside [min;max]
        /// </summary>
        /// <param name="min">min point</param>
        /// <param name="max">max point</param>
        /// <returns>true if inside</returns>
        public static bool IsMouseInside(Vector3 min, Vector3 max)
        {
            if (mouseWorldPos.x > min.x && mouseWorldPos.z > min.z && mouseWorldPos.x < max.x && mouseWorldPos.z < max.z) return true;
            if (mouseWorldPosTerrain.x > min.x && mouseWorldPosTerrain.z > min.z && mouseWorldPosTerrain.x < max.x && mouseWorldPosTerrain.z < max.z) return true;
            return false;
        }
        /// <summary>
        /// Checks if mouse is around world point
        /// </summary>
        /// <param name="world">point</param>
        /// <returns>true if around</returns>
        public static bool IsMouseAround(Vector3 world)
        {
            return HandleUtility.DistancePointLine(world, startLine, endLine) < Utility.squareHalf * 2;
        }
        #endregion
    }
}