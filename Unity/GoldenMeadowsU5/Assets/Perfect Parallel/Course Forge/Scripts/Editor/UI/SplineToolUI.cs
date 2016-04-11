using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Course Forge spline tool
    /// </summary>
    public class SplineToolUI : ToolUI
    {
        #region Fields
        bool drawing = false;
        static Vector3[] points = new Vector3[0];
        static Vector3[] boxes = new Vector3[0];
        static RaycastHit? firstHit = null;
        static Vector3[] originalPoints = new Vector3[0];

        static SplineBase startSpline = null;
        static int startSplineIndex = -1;
        static int startSplineDirection = -1;
        static Layer startSplineLayer = null;
        static int endSplineIndex = -1;
        #endregion

        #region Properties
        static bool SplineDrawing = false;
        static bool HazardDrawing = false;
        static bool PointByPoint = false;
        static SplineBase.SplineInfo.Flags FlagsDrawing = 0;
        static string LayerName
        {
            get
            {
                return EditorPrefs.GetString("Course Forge/SplineLayerName", "NoLayerName");
            }
            set
            {
                EditorPrefs.SetString("Course Forge/SplineLayerName", value);
            }
        }

        public override string ToolName
        {
            get
            {
                if (CourseBase.ActiveSpline == null) return "Spline Tool";
                else return CourseBase.ActiveSpline.Transform.gameObject.name;
            }
        }
        #endregion

        #region UI Methods
        void SplineField()
        {
            Background();

            if (SplineDrawing) SetColor(Green);
            if (Button(pencilTexture))
            {
                SplineDrawing = !SplineDrawing;
                if (SplineDrawing && HazardDrawing)
                {
                    HazardDrawing = false;
                    if (CourseBase.SplineLayers.Count != 0) LayerName = CourseBase.GetLayer("NoLayerName", CourseBase.SplineLayers).name;
                }
            }
            SetColor();
        }
        void HazardField()
        {
            Background();

            if (HazardDrawing) SetColor(Green);
            if (Button(brushTexture))
            {
                HazardDrawing = !HazardDrawing;
                if (HazardDrawing && SplineDrawing)
                {
                    SplineDrawing = false;
                    if (CourseBase.HazardLayers.Count != 0) LayerName = CourseBase.GetLayer("NoLayerName", CourseBase.HazardLayers).name;
                }
            }
            SetColor();
        }
        void PointByPointField()
        {
            Background();

            if (PointByPoint) SetColor(Green);
            if (Button(pointTexture) && !drawing)
            {
                PointByPoint = !PointByPoint;
            }
            SetColor();
        }
        SplineBase.SplineInfo.Flags HoleFlagsField(SplineBase.SplineInfo.Flags flags)
        {
            Background();

            SetColor();
            if (flags.AND(SplineBase.SplineInfo.Flags.Hole)) SetColor(Green);
            if (Button(holeTexture) && !drawing)
            {
                if (flags.AND(SplineBase.SplineInfo.Flags.Hole)) flags -= SplineBase.SplineInfo.Flags.Hole;
                else flags |= SplineBase.SplineInfo.Flags.Hole;
            }
            SetColor();

            return flags;
        }
        SplineBase.SplineInfo.Flags SquareField(SplineBase.SplineInfo.Flags flags)
        {
            Background();

            SetColor();
            if (flags.AND(SplineBase.SplineInfo.Flags.Square)) SetColor(Green);
            if (Button(rectangleTexture) && !drawing)
            {
                if (flags.AND(SplineBase.SplineInfo.Flags.Square)) flags -= SplineBase.SplineInfo.Flags.Square;
                else flags |= SplineBase.SplineInfo.Flags.Square;
            }
            SetColor();

            return flags;
        }
        bool LockedField(bool locked)
        {
            SetColor(HalfGrey);
            if (locked) SetColor(Green);

            Background();

            if (Button(locked ? lockTexture : unlockTexture))
            {
                locked = !locked;
            }
            SetColor();

            return locked;
        }
        #endregion

        #region Context Methods
        [MenuItem("CONTEXT/SplineTool/Hazard/Start/Start -> Out of Bounds")]
        static void HazardStartOutOfBounds()
        {
            startSplineLayer = CourseBase.HazardLayers.Find(x => CourseBase.HazardType(x.name) == HazardBase.Type.Out_of_Bounds);
        }
        [MenuItem("CONTEXT/SplineTool/Hazard/Start/Start -> Water Hazard")]
        static void HazardStartWaterHazard()
        {
            startSplineLayer = CourseBase.HazardLayers.Find(x => CourseBase.HazardType(x.name) == HazardBase.Type.Water_Hazard);
        }
        [MenuItem("CONTEXT/SplineTool/Hazard/Start/Start -> Lateral Water Hazard")]
        static void HazardStartLateralWaterHazard()
        {
            startSplineLayer = CourseBase.HazardLayers.Find(x => CourseBase.HazardType(x.name) == HazardBase.Type.Lateral_Water_Hazard);
        }
        [MenuItem("CONTEXT/SplineTool/Hazard/Start/Start -> Free Drop")]
        static void HazardStartFreeDrop()
        {
            startSplineLayer = CourseBase.HazardLayers.Find(x => CourseBase.HazardType(x.name) == HazardBase.Type.Free_Drop);
        }

        [MenuItem("CONTEXT/SplineTool/Hazard/Change/Change Direction")]
        static void HazardChangeDirection()
        {
            startSplineDirection *= -1;
        }
        [MenuItem("CONTEXT/SplineTool/Hazard/Change/Clear")]
        static void HazardChangeClear()
        {
            startSpline = null;
            startSplineIndex = -1;
            startSplineDirection = 1;
            startSplineLayer = null;
            endSplineIndex = -1;
        }

        [MenuItem("CONTEXT/SplineTool/Hazard/End/Set -> End")]
        static void HazardEnd()
        {
            if (startSpline != null)
            {
                string[] colorNames = startSpline.Info.colorNames;
                if (startSplineIndex < endSplineIndex)
                {
                    if (startSplineDirection > 0)
                    {
                        for (int i = startSplineIndex; i <= endSplineIndex; ++i) colorNames[i] = startSplineLayer.name;
                    }
                    else
                    {
                        for (int i = 0; i < colorNames.Length; ++i) if (i <= startSplineIndex || i >= endSplineIndex) colorNames[i] = startSplineLayer.name;
                    }
                }
                else
                {
                    if (startSplineDirection > 0)
                    {
                        for (int i = 0; i < colorNames.Length; ++i)
                            if (i >= startSplineIndex || i <= endSplineIndex) colorNames[i] = startSplineLayer.name;
                    }
                    else
                    {
                        for (int i = endSplineIndex; i <= startSplineIndex; ++i) colorNames[i] = startSplineLayer.name;
                    }
                }

                startSpline.Info.colorNames = colorNames;
                startSpline.SetRefresh();
                startSpline.UpdateLine();
                startSpline.FormLines();
                startSpline.UpdateMesh();
                startSpline.UpdateMaterial();
            }

            HazardChangeClear();
        }
        [MenuItem("CONTEXT/SplineTool/Hazard/End/Clear")]
        static void HazardEndClear()
        {
            HazardChangeClear();
        }
        #endregion

        #region Methods
        public static void OnSplineScene(SplineBase spline, bool selected)
        {
            if (spline.IsHazard) LineToolUI.DrawLines(spline.Lines.Lines, spline.Lines.LinesColors);
            else LineToolUI.DrawLines(spline.Lines.Lines, spline.Layer.LayerColor);

            if (spline.Info.pin) return;
            if (!IsMouseInside(spline.Lines.MinPoint, spline.Lines.MaxPoint)) return;

            if (selected)
            {
                LineToolUI.DrawLines(spline.Lines.Boxes, Black);

                if (Event.current.shift)
                {
                    LineToolUI.DrawLines(spline.Lines.InnerSplineData.points.ToArray(), Black);
                    LineToolUI.DrawLines(spline.Lines.OuterSplineData.points.ToArray(), Black);

                    if (spline.Pattern.extra)
                    {
                        LineToolUI.DrawLines(spline.Lines.ExtraInnerLineData.points.ToArray(), Black);
                        LineToolUI.DrawLines(spline.Lines.ExtraOuterLineData.points.ToArray(), Black);
                    }
                    if (spline.Pattern.shrink)
                    {
                        LineToolUI.DrawLines(spline.Lines.ShrinkLineData.points.ToArray(), Black);
                    }
                }

                Vector3[] points = spline.Points;
                for (int i = 0; i < points.Length; ++i)
                {
                    if (IsMouseAround(points[i]))
                    {
                        LineToolUI.DrawBox(points[i], Yellow);
                    }
                    if (spline.ActivePoint == i)
                    {
                        LineToolUI.DrawBox(points[i], Red);
                    }
                }

                if (spline == startSpline && startSplineLayer != null)
                {
                    Vector3 target = Vector3.zero;
                    if (startSplineIndex == 0 && startSplineDirection == -1) target = points[points.Length - 1];
                    if (startSplineIndex == points.Length - 1 && startSplineDirection == 1) target = points[0];
                    else target = points[startSplineIndex + startSplineDirection];

                    LineToolUI.DrawStart(points[startSplineIndex], target, startSplineLayer.hazardColor);
                }
            }
        }
        public static void OnSplineGUI(SplineBase spline, bool selected)
        {
            if (spline.Info.pin) return;
            if (spline != CourseBase.ActiveSpline && !IsMouseInside(spline.Lines.MinPoint, spline.Lines.MaxPoint)) return;

            bool leftDown = LeftMouseDown;
            bool rightDown = RightMouseDown;
            RaycastHit? currentHit = CourseBase.CameraTerrainHit;
            Vector3[] points = spline.Points;

            if (spline.ActivePoint == -1)
            {
                for (int i = 0; i < points.Length; ++i)
                {
                    if (rightDown && IsMouseAround(points[i]))
                    {
                        if (spline.IsHazard)
                        {
                            if (spline == startSpline && startSplineLayer != null)
                            {
                                if (i == startSplineIndex)
                                {
                                    EditorUtility.DisplayPopupMenu(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), "CONTEXT/SplineTool/Hazard/Change/", null);
                                }
                                else
                                {
                                    endSplineIndex = i;
                                    EditorUtility.DisplayPopupMenu(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), "CONTEXT/SplineTool/Hazard/End/", null);
                                }
                            }
                            else
                            {
                                startSpline = spline;
                                startSplineIndex = i;
                                startSplineDirection = 1;
                                startSplineLayer = null;

                                EditorUtility.DisplayPopupMenu(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0), "CONTEXT/SplineTool/Hazard/Start/", null);
                            }
                        }
                        Event.current.Use();
                        break;
                    }
                    if (leftDown && IsMouseAround(points[i]))
                    {
                        CourseBase.ActiveSpline = spline;
                        spline.ActivePoint = i;
                        firstHit = currentHit;
                        originalPoints = (Vector3[])points.Clone();
                        Event.current.Use();
                        break;
                    }
                }
            }
            if (spline.ActivePoint != -1)
            {
                Vector3 delta = Vector3.zero;
                if (firstHit != null && currentHit != null) delta = (currentHit.Value.point - firstHit.Value.point);
                bool changed = delta.magnitude > 0.01f;

                if (LeftMouseUp)
                {
                    if (changed) spline.LineChanged();
                    spline.ActivePoint = -1;

                    Event.current.Use();
                }
                else if (LeftDoubleClick && !spline.Info.locked)
                {
                    if (Event.current.modifiers == EventModifiers.Shift) DeletePoint(spline);
                    else AddPoint(spline);
                }
                else if (LeftMouseDrag && changed && !spline.Info.locked)
                {
                    if (Event.current.modifiers == EventModifiers.Shift)
                    {
                        for (int p = 0; p < points.Length; ++p)
                        {
                            points[p] = originalPoints[p] + delta;
                            points[p].y = CourseBase.TerrainHeight(points[p].x, points[p].z);
                        }
                    }
                    else if (Event.current.modifiers == EventModifiers.Control)
                    {
                        Vector3 center = spline.Lines.MinPoint + (spline.Lines.MaxPoint - spline.Lines.MinPoint) / 2;
                        float scale = Mathf.Max(((currentHit.Value.point - center).magnitude / (firstHit.Value.point - center).magnitude), 0);
                        for (int p = 0; p < points.Length; ++p)
                        {
                            points[p] = center + (originalPoints[p] - center) * scale;
                            points[p].y = CourseBase.TerrainHeight(points[p].x, points[p].z);
                        }
                    }
                    else
                    {
                        int index = spline.ActivePoint;
                        points[index] = originalPoints[index] + delta;
                        points[index].y = CourseBase.TerrainHeight(points[index].x, points[index].z);
                    }
                    spline.UpdateLine();
                }
            }
        }

        public override void OnScene(bool selected)
        {
            List<SplineBase> splines = CourseBase.Curves;
            for (int i = 0; i < splines.Count; ++i)
                OnSplineScene(splines[i], selected);
        }
        public override void OnUI(bool selected)
        {
            if (selected)
            {
                #region Delete
                if (CourseBase.ActiveSpline && Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.KeyDown)
                {
                    CourseBase.ActiveSpline.LineChanged();
                    CourseBase.ActiveSpline.Transform.parent = null;
                    if (!CourseBase.ActiveSpline.IsHazard) CourseBase.ActiveSpline.Transform.DetachChildren();

                    MonoBehaviour.DestroyImmediate(CourseBase.ActiveSpline.Transform.gameObject);

                    if (CourseBase.ActiveSpline.IsHazard) CourseBase.Hazards.Remove(CourseBase.ActiveSpline as HazardBase);
                    else CourseBase.Splines.Remove(CourseBase.ActiveSpline);
                    CourseBase.Curves.Remove(CourseBase.ActiveSpline);

                    CourseBase.ActiveSpline = null;

                    Event.current.Use();
                    return;
                }
                #endregion

                BeginUI();
                Move(3, 0);

                if (CourseBase.ActiveSpline != null)
                {
                    SetColor(Green);
                    Move(3, 0);

                    if (!(CourseBase.ActiveSpline is HazardBase))
                    {
                        CourseBase.ActiveSpline.Info.flags = (int)HoleFlagsField((SplineBase.SplineInfo.Flags)CourseBase.ActiveSpline.Info.flags);
                        SetColor(Green);
                    }
                    Move(1, 0);

                    CourseBase.ActiveSpline.Info.flags = (int)SquareField((SplineBase.SplineInfo.Flags)CourseBase.ActiveSpline.Info.flags);
                    Move(1, 0);
                    SetColor(Green);

                    if (CourseBase.ActiveSpline is HazardBase)
                    {
                        Layer layer = CourseBase.ActiveSpline.Layer;

                        CourseBase.ActiveSpline.Layer = LayerPopup("Layer", CourseBase.ActiveSpline.Layer, CourseBase.HazardLayers, 3);
                        Move(3, 0);

                        if (layer != CourseBase.ActiveSpline.Layer)
                        {
                            for (int i = 0; i < CourseBase.ActiveSpline.Info.colorNames.Length; ++i) CourseBase.ActiveSpline.Info.colorNames[i] = CourseBase.ActiveSpline.Layer.name;
                        }
                    }
                    else
                    {
                        CourseBase.ActiveSpline.Layer = LayerPopup("Layer", CourseBase.ActiveSpline.Layer, CourseBase.SplineLayers, 3);
                        SetColor();
                        Move(3, 0);

                        CourseBase.ActiveSpline.Info.locked = LockedField(CourseBase.ActiveSpline.Info.locked);
                    }
                    Move(1, 0);
                    SetColor();
                }
                else
                {
                    SplineField();
                    Move(1, 0);

                    HazardField();
                    Move(1, 0);

                    if (SplineDrawing)
                    {
                        PointByPointField();
                        Move(1, 0);

                        FlagsDrawing = HoleFlagsField(FlagsDrawing);
                        Move(1, 0);

                        FlagsDrawing = SquareField(FlagsDrawing);
                        Move(1, 0);

                        LayerName = LayerPopup("Layer", CourseBase.GetLayer(LayerName, CourseBase.SplineLayers), CourseBase.SplineLayers, 3).name;
                        Move(3, 0);
                    }

                    if (HazardDrawing)
                    {
                        PointByPointField();
                        Move(1, 0);

                        FlagsDrawing = HoleFlagsField(FlagsDrawing);
                        Move(1, 0);

                        FlagsDrawing = SquareField(FlagsDrawing);
                        Move(1, 0);

                        LayerName = LayerPopup("Layer", CourseBase.GetLayer(LayerName, CourseBase.HazardLayers), CourseBase.HazardLayers, 3).name;
                        Move(3, 0);
                    }
                }

                EndUI();

                List<SplineBase> splines = CourseBase.Curves;
                for (int i = 0; i < splines.Count; ++i)
                    OnSplineGUI(splines[i], selected);
            }

            if (LeftMouseDown) CourseBase.ActiveSpline = null;
            if (selected)
            {
                if (CourseBase.ActiveSpline == null && (SplineDrawing || HazardDrawing)) OnMouse();
                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.Arrow);
                }
            }
        }
        public override void OnCleanup()
        {
            OnCleanupBase();

            FlagsDrawing = 0;

            firstHit = null;
            originalPoints = new Vector3[0];
        }
        #endregion

        #region Support Methods
        void OnCleanupBase()
        {
            if (CourseBase.Terrain) CourseBase.ActiveSpline = null;

            drawing = false;
            points = new Vector3[0];
            boxes = new Vector3[0];

            SplineDrawing = false;
            HazardDrawing = false;
            PointByPoint = false;
            FlagsDrawing = 0;
        }
        void OnMouse()
        {
            if (PointByPoint)
            {
                if (LeftMouseDown)
                {
                    OnMouseDown();
                    Event.current.Use();
                }
                if (LeftDoubleClick)
                {
                    if (points.Length >= 3)
                    {
                        if (HazardDrawing) CourseBase.CreateHazard(points, CourseBase.GetLayer(LayerName, CourseBase.HazardLayers), FlagsDrawing);
                        else CourseBase.CreateSpline(points, CourseBase.GetLayer(LayerName, CourseBase.SplineLayers), FlagsDrawing);
                    }
                    Event.current.Use();
                    OnCleanupBase();
                }
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Event.current.Use();
                    OnCleanupBase();
                }
                LineToolUI.DrawLines(boxes, Black);
            }
            else
            {
                if (LeftMouseDown || LeftMouseDrag)
                {
                    drawing = true;
                    Event.current.Use();
                }
                if (drawing && LeftMouseUp)
                {
                    if (points.Length >= 3)
                    {
                        if (HazardDrawing) CourseBase.CreateHazard(points, CourseBase.GetLayer(LayerName, CourseBase.HazardLayers), FlagsDrawing, true);
                        else CourseBase.CreateSpline(points, CourseBase.GetLayer(LayerName, CourseBase.SplineLayers), FlagsDrawing, true);
                    }
                    Event.current.Use();
                    OnCleanupBase();
                }
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Event.current.Use();
                    OnCleanupBase();
                }

                if (drawing) OnMouseDown();
            }

            LineToolUI.DrawLines(points, Black);
        }
        void OnMouseDown()
        {
            RaycastHit? hit = CourseBase.CameraTerrainHit;
            if (hit != null)
            {
                Vector3 point = hit.Value.point;
                ArrayUtility.Add(ref points, point);

                ArrayUtility.Add(ref boxes, new Vector3(point.x - Utility.squareHalf, point.y, point.z - Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(point.x + Utility.squareHalf, point.y, point.z - Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(point.x - Utility.squareHalf, point.y, point.z - Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(point.x - Utility.squareHalf, point.y, point.z + Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(point.x - Utility.squareHalf, point.y, point.z + Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(point.x + Utility.squareHalf, point.y, point.z + Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(point.x + Utility.squareHalf, point.y, point.z - Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(point.x + Utility.squareHalf, point.y, point.z + Utility.squareHalf));
                ArrayUtility.Add(ref boxes, new Vector3(0, float.PositiveInfinity, 0));
            }
        }

        static void AddPoint(SplineBase spline)
        {
            Vector3[] points = spline.Points;
            string[] colorNames = spline.Info.colorNames;
            int i = spline.ActivePoint;

            if (spline.IsHazard) ArrayUtility.Insert(ref colorNames, i, colorNames[i]);
            if (i != 0)
            {
                ArrayUtility.Insert(ref points, i, Vector3.Lerp(points[i], points[i - 1], 0.5f));
                points[i].y = CourseBase.TerrainHeight(points[i].x, points[i].z);
                points[i - 1].y = CourseBase.TerrainHeight(points[i - 1].x, points[i - 1].z);
            }
            else
            {
                ArrayUtility.Insert(ref points, i, Vector3.Lerp(points[i], points[points.Length - 1], 0.5f));
                points[i].y = CourseBase.TerrainHeight(points[i].x, points[i].z);
                points[points.Length - 1].y = CourseBase.TerrainHeight(points[points.Length - 1].x, points[points.Length - 1].z);
            }

            spline.Points = points;
            spline.Info.colorNames = colorNames;
            spline.UpdateLine();
            spline.LineChanged();
        }
        static void DeletePoint(SplineBase spline)
        {
            Vector3[] points = spline.Points;
            string[] colorNames = spline.Info.colorNames;
            int i = spline.ActivePoint;

            ArrayUtility.RemoveAt(ref points, i);
            if (spline.IsHazard) ArrayUtility.RemoveAt(ref colorNames, i);

            spline.Points = points;
            spline.Info.colorNames = colorNames;
            spline.UpdateLine();
            spline.LineChanged();
        }
        #endregion
    }
}