using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Course Forge planting tool
    /// </summary>
    public class PlantToolUI : ToolUI
    {
        #region Fields
        static int holeIndex = 0;
        static bool teePlanting = false;
        static bool shotPlanting = false;
        static bool pinPlanting = false;
        static bool measurePlanting = false;
        static bool flyByPlanting = false;

        static TeeBase.Info.Type teeType = TeeBase.Info.Type.Championship;
        static PinBase.Info.Difficulty pinDifficulty = PinBase.Info.Difficulty.Medium;
        static TeeBase.Info.Par teePar = TeeBase.Info.Par._4;
        static int teeStrokeIndex = 0;
        static float teeWidth = 0;
        static float teeHeight = 0;
        static FlyByBase.Info.Type flyByType = FlyByBase.Info.Type.A;
        static float metersPerNode = 30.0f;

        static bool play = false;
        static bool pause = false;
        static Timer timer = new Timer();

        static bool drawing = false;
        static Vector3[] points = new Vector3[0];
        static RaycastHit? firstHit = null;
        static Vector3 originalPoint = Vector3.zero;
        static PlantBase.BaseInfo activeInfo = null;
        #endregion

        #region Properties
        public override string ToolName
        {
            get
            {
                if (CourseBase.ActivePlant == null) return "Plant Tool";
                else return CourseBase.ActivePlant.Transform.gameObject.name;
            }
        }
        #endregion

        #region UI Methods
        void TeeField()
        {
            Background();

            if (teePlanting) SetColor(Green);
            if (Button(teeTexture))
            {
                teePlanting = !teePlanting;
                if (CourseBase.FreeTees(holeIndex).Length == 0)
                    teePlanting = false;

                shotPlanting = false;
                pinPlanting = false;
                measurePlanting = false;
                flyByPlanting = false;
            }
            SetColor();
        }
        void ShotField()
        {
            Background();

            if (shotPlanting) SetColor(Green);
            if (Button(shotsTexture))
            {
                teePlanting = false;
                shotPlanting = !shotPlanting;
                pinPlanting = false;
                measurePlanting = false;
                flyByPlanting = false;
            }
            SetColor();
        }
        void PinField()
        {
            Background();

            if (pinPlanting) SetColor(Green);
            if (Button(cupTexture))
            {
                teePlanting = false;
                shotPlanting = false;
                pinPlanting = !pinPlanting;
                measurePlanting = false;
                flyByPlanting = false;
            }
            SetColor();
        }
        void MeasureField()
        {
            Background();

            if (measurePlanting) SetColor(Green);
            if (Button(measureTexture))
            {
                teePlanting = false;
                shotPlanting = false;
                pinPlanting = false;
                measurePlanting = !measurePlanting;
                flyByPlanting = false;
            }
            SetColor();
        }
        void FlyByField()
        {
            Background();

            if (flyByPlanting) SetColor(Green);
            if (Button(flyByTexture))
            {
                teePlanting = false;
                shotPlanting = false;
                pinPlanting = false;
                measurePlanting = false;
                flyByPlanting = !flyByPlanting;

                if (flyByPlanting && CourseBase.FreeFlyBys(holeIndex).Length == 0) flyByPlanting = false;
            }
            SetColor();
        }
        int StrokeIndexField(int holeIndex, TeeBase.Info.Type teeType, int index, bool add, float xBoxes)
        {
            string[] names = CourseBase.FreeTeesStrokeIndex(holeIndex, teeType);
            string indexName = index.ToString();
            if (!ArrayUtility.Contains(names, indexName))
            {
                if (add)
                {
                    ArrayUtility.Add(ref names, indexName);

                    string[] newNames = CourseBase.HoleNames;
                    for (int i = 0; i < newNames.Length; ++i)
                    {
                        if (!ArrayUtility.Contains(names, newNames[i]))
                        {
                            ArrayUtility.RemoveAt(ref newNames, i);
                            i = -1;
                            continue;
                        }
                    }
                    names = newNames;
                }
                else
                {
                    indexName = names[0];
                }
            }

            int namesIndex = ArrayUtility.IndexOf(names, indexName);
            namesIndex = Popup("StrokeIndex", namesIndex, names, xBoxes);
            if (namesIndex >= 0) int.TryParse(names[namesIndex], out index);

            return index;
        }
        TeeBase.Info.Type TeeTypeField(TeeBase.Info.Type index, int holeIndex, bool add, float xBoxes)
        {
            string[] names = CourseBase.FreeTees(holeIndex);
            string indexName = index.ToString();
            if (!ArrayUtility.Contains(names, indexName))
            {
                if (add)
                {
                    ArrayUtility.Add(ref names, indexName);

                    string[] newNames = System.Enum.GetNames(typeof(TeeBase.Info.Type));
                    for (int i = 0; i < newNames.Length; ++i)
                    {
                        if (!ArrayUtility.Contains(names, newNames[i]))
                        {
                            ArrayUtility.RemoveAt(ref newNames, i);
                            i = -1;
                            continue;
                        }
                    }
                    names = newNames;
                }
                else
                {
                    string[] newNames = System.Enum.GetNames(typeof(TeeBase.Info.Type));
                    for (int i = 0; i < newNames.Length; ++i)
                    {
                        if (!ArrayUtility.Contains(names, newNames[i]))
                        {
                            ArrayUtility.RemoveAt(ref newNames, i);
                            i = -1;
                            continue;
                        }
                    }
                    names = newNames;
                    indexName = names[0];
                }
            }

            int namesIndex = ArrayUtility.IndexOf(names, indexName);
            namesIndex = Popup("Type", namesIndex, names, xBoxes);
            index = (TeeBase.Info.Type)System.Enum.Parse(typeof(TeeBase.Info.Type), names[namesIndex]);

            return index;
            //return Iloveyou;
        }
        TeeBase.Info.Par ParField(TeeBase.Info.Par par, float xBoxes)
        {
            string[] names = System.Enum.GetNames(typeof(TeeBase.Info.Par));
            for (int i = 0; i < names.Length; ++i)
                names[i] = names[i].Replace("_", "");

            return (TeeBase.Info.Par)Popup("Par", (int)par, names, xBoxes);
        }
        PinBase.Info.Difficulty DifficultyField(PinBase.Info.Difficulty index, float xBoxes)
        {
            return (PinBase.Info.Difficulty)Popup("Difficulty", (int)index, System.Enum.GetNames(typeof(PinBase.Info.Difficulty)), xBoxes);
        }
        FlyByBase.Info.Node.Target TargetField(FlyByBase.Info.Node.Target target, FlyByBase.Info.Node node, float xBoxes)
        {
            target = (FlyByBase.Info.Node.Target)Popup("Target", (int)target, System.Enum.GetNames(typeof(FlyByBase.Info.Node.Target)), xBoxes);

            Move(1, 0);
            if (target == FlyByBase.Info.Node.Target.GameObject)
                node.CameraTarget = (Transform)ObjectField(node.CameraTarget, 1, 0.6f);
            Move(-1, 0);

            return target;
        }
        FlyByBase.Info.Type FlyByTypeField(FlyByBase.Info.Type index, int holeIndex, bool add, float xBoxes)
        {
            string[] names = CourseBase.FreeFlyBys(holeIndex);
            string indexName = index.ToString();
            if (!ArrayUtility.Contains(names, indexName))
            {
                if (add)
                {
                    ArrayUtility.Add(ref names, indexName);

                    string[] newNames = System.Enum.GetNames(typeof(FlyByBase.Info.Type));
                    for (int i = 0; i < newNames.Length; ++i)
                    {
                        if (!ArrayUtility.Contains(names, newNames[i]))
                        {
                            ArrayUtility.RemoveAt(ref newNames, i);
                            i = -1;
                            continue;
                        }
                    }
                    names = newNames;
                }
                else
                {
                    string[] newNames = System.Enum.GetNames(typeof(FlyByBase.Info.Type));
                    for (int i = 0; i < newNames.Length; ++i)
                    {
                        if (!ArrayUtility.Contains(names, newNames[i]))
                        {
                            ArrayUtility.RemoveAt(ref newNames, i);
                            i = -1;
                            continue;
                        }
                    }
                    names = newNames;
                    indexName = names[0];
                }
            }

            int namesIndex = ArrayUtility.IndexOf(names, indexName);
            namesIndex = Popup("Label", namesIndex, names, xBoxes);
            return (FlyByBase.Info.Type)System.Enum.Parse(typeof(FlyByBase.Info.Type), names[namesIndex]);
        }
        float VelocityField(string text, float velocity, float reset, float xBoxes)
        {
            Background(xBoxes);

            Label(text, xBoxes, 0.6f);
            Move(xBoxes - 1, 0);
            if (Button("R", 1.0f, 0.6f))
            {
                velocity = reset;
            }
            Move(-(xBoxes - 1), 0);

            Move(0, 0.4f);
            velocity = FloatSlider(velocity, 0, 30, xBoxes, 0.6f);
            Move(0, -0.4f);

            return velocity;
        }
        void VideoSlider(ref float time, float xBoxes)
        {
            SetColor(Green);
            Background(xBoxes);
            SetColor();

            if (play) SetColor(Green);
            if (Button(playTexture, xBoxes / 3, 0.6f))
            {
                play = !play;
                if (play)
                {
                    timer.Start((CourseBase.ActivePlant as FlyByBase).Time);
                }
                else
                {
                    timer.Stop();
                    pause = false;
                }
            }
            SetColor();
            Move(xBoxes / 3, 0);

            if (pause) SetColor(Green);
            if (Button(pauseTexture, xBoxes / 3, 0.6f) && play)
            {
                pause = !pause;

                if (!pause) timer.Resume();
                else timer.Pause();
            }
            SetColor();
            Move(xBoxes / 3, 0);


            if (Button("R", xBoxes / 3, 0.6f) && play)
            {
                time = 0;
                timer.Stop();
                pause = false;
                play = false;
            }
            Move(-xBoxes * 2 / 3, 0);
            Move(0, 0.4f);

            time = FloatSlider(time, 0, timer.MaxTime, 3, 0.6f);
            Move(0, -0.4f);

            if (play && !pause && !timer.IsRunning)
            {
                play = false;
                timer.Stop();
            }
        }

        void OnSceneTees(bool selected)
        {
            List<TeeBase> tees = CourseBase.Tees;
            for (int i = 0; i < tees.Count; ++i)
            {
                Color color = new Color(0.15f, 0.75f, 0.15f);

                if (CourseBase.ActivePlant is TeeBase && tees[i] == (TeeBase)CourseBase.ActivePlant)
                {
                    color = White;
                }

                Vector3 forward = Vector3.right;
                if (CourseBase.Holes[tees[i].HoleIndex].pins.Count != 0) forward = (CourseBase.AimPoint(tees[i].Position, tees[i], CourseBase.Holes[tees[i].HoleIndex].shots, CourseBase.Holes[tees[i].HoleIndex].pins[0]) - tees[i].Position).normalized;
                forward.y = 0;

                LineToolUI.DrawTee(tees[i].Position, color);

                if (selected)
                {
                    LineToolUI.DrawBox(tees[i].Position, Black, tees[i].Height, tees[i].Width + Utility.markerOffset * 2, forward);

                    if (tees[i].PlantInfo == activeInfo)
                    {
                        LineToolUI.DrawBox(tees[i].Position, Red);
                        LineToolUI.DrawBox(tees[i].Position, Red, tees[i].Height, tees[i].Width, forward);
                    }
                    else if (IsMouseAround(tees[i].Position))
                    {
                        LineToolUI.DrawBox(tees[i].Position, Yellow);
                        LineToolUI.DrawBox(tees[i].Position, Yellow, tees[i].Height, tees[i].Width, forward);
                    }

                    if (IsPointOnScreen(tees[i].Position))
                    {
                        string text = CourseBase.GetTeeName(tees[i].HoleIndex, tees[i].Type, tees[i].Par, tees[i].StrokeIndex);
                        tees[i].gameObject.name = text;

                        Rect rect = ProjectToScreenRect(tees[i].Position, 16, 16);
                        rect.width = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                        rect.x -= rect.width / 4;

                        BeginUI();
                        MoveToPixels((int)rect.x, (int)rect.y);
                        SetBoxSize((int)rect.width, (int)rect.height);
                        Background(1, 1, 0.5f);
                        LabelNoOffset(text);
                        EndUI();
                    }
                }
            }
        }
        void OnSceneShots(bool selected)
        {
            List<ShotBase> shots = CourseBase.Shots;
            for (int i = 0; i < shots.Count; ++i)
            {
                Color color = new Color(0.75f, 0.75f, 0.15f);

                if (CourseBase.ActivePlant is ShotBase && shots[i] == (ShotBase)CourseBase.ActivePlant)
                {
                    color = Color.white;
                }

                LineToolUI.DrawShot(shots[i].Position, color);

                if (selected)
                {
                    if (shots[i].PlantInfo == activeInfo)
                    {
                        LineToolUI.DrawBox(shots[i].Position, Red);
                    }
                    else if (IsMouseAround(shots[i].Position))
                    {
                        LineToolUI.DrawBox(shots[i].Position, Yellow);
                    }

                    if (IsPointOnScreen(shots[i].Position))
                    {
                        string text = CourseBase.GetShotName(shots[i].HoleIndex, shots[i].OrderIndex);
                        shots[i].gameObject.name = text;

                        Rect rect = ProjectToScreenRect(shots[i].Position, 16, 16);
                        rect.width = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                        rect.x -= rect.width / 4;

                        BeginUI();
                        MoveToPixels((int)rect.x, (int)rect.y);
                        SetBoxSize((int)rect.width, (int)rect.height);
                        Background(1, 1, 0.5f);
                        LabelNoOffset(text);
                        EndUI();
                    }
                }
            }
        }
        void OnScenePins(bool selected)
        {
            List<PinBase> pins = CourseBase.Pins;
            for (int i = 0; i < pins.Count; ++i)
            {
                Color color = new Color(0.75f, 0.15f, 0.15f);

                if (CourseBase.ActivePlant is PinBase && pins[i] == (PinBase)CourseBase.ActivePlant)
                {
                    color = White;
                }

                LineToolUI.DrawPin(pins[i].Position, color);

                if (selected)
                {
                    if (pins[i].PlantInfo == activeInfo)
                    {
                        LineToolUI.DrawBox(pins[i].Position, Red);

                        RaycastHit hit = CourseBase.TerrainHit(pins[i].Position.x, pins[i].Position.z).Value;

                        Vector3 right = Vector3.Cross(Vector3.Cross(Vector3.up, hit.normal).normalized, hit.normal).normalized;
                        float angle = Vector3.Angle(new Vector3(hit.normal.x, 0, hit.normal.z).normalized, right);
                        if (angle > 10) angle = 10;

                        LineToolUI.DrawLine(new Vector3(pins[i].Position.x, pins[i].Position.y + 0.02f, pins[i].Position.z), new Vector3(pins[i].Position.x, pins[i].Position.y + 0.02f, pins[i].Position.z) + right * Mathf.Lerp(0, 1, angle / 10), Black);
                    }
                    else if (IsMouseAround(pins[i].Position))
                    {
                        LineToolUI.DrawBox(pins[i].Position, Yellow);
                    }

                    if (IsPointOnScreen(pins[i].Position))
                    {
                        string text = CourseBase.GetPinName(pins[i].HoleIndex, pins[i].OrderIndex, pins[i].Difficulty);
                        pins[i].gameObject.name = text;

                        Rect rect = ProjectToScreenRect(pins[i].Position + Vector3.up * 1.8f, 16, 16);
                        rect.width = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                        rect.x -= rect.width / 4;

                        BeginUI();
                        MoveToPixels((int)rect.x, (int)rect.y);
                        SetBoxSize((int)rect.width, (int)rect.height);
                        Background(1, 1, 0.5f);
                        LabelNoOffset(text);
                        EndUI();
                    }
                }
            }
        }
        void OnSceneMeasures(bool selected)
        {
            List<MeasureBase> measures = CourseBase.Measures;
            for (int i = 0; i < measures.Count; ++i)
            {
                Color color = new Color(0.15f, 0.75f, 0.15f);

                if (CourseBase.ActivePlant is MeasureBase && measures[i] == (MeasureBase)CourseBase.ActivePlant)
                {
                    color = White;
                }

                Vector3 center = measures[i].StartPosition + (measures[i].EndPosition - measures[i].StartPosition) / 2;
                Vector3 direction = (measures[i].EndPosition - measures[i].StartPosition).normalized;
                LineToolUI.DrawMeasure(measures[i].StartPosition, direction, color);
                LineToolUI.DrawMeasure(measures[i].EndPosition, direction, color);
                LineToolUI.DrawLine(measures[i].StartPosition, measures[i].EndPosition, color);

                if (measures[i].StartInfo == activeInfo)
                {
                    LineToolUI.DrawBox(measures[i].StartPosition, Red);
                }
                else if (IsMouseAround(measures[i].StartPosition))
                {
                    LineToolUI.DrawBox(measures[i].StartPosition, Yellow);
                }

                if (measures[i].EndInfo == activeInfo)
                {
                    LineToolUI.DrawBox(measures[i].EndPosition, Red);
                }
                else if (IsMouseAround(measures[i].EndPosition))
                {
                    LineToolUI.DrawBox(measures[i].EndPosition, Yellow);
                }

                if (IsPointOnScreen(center))
                {
                    Vector2 endXZ = new Vector2(measures[i].EndPosition.x, measures[i].EndPosition.z);
                    Vector2 startXZ = new Vector2(measures[i].StartPosition.x, measures[i].StartPosition.z);
                    float xz = (endXZ - startXZ).magnitude;
                    float y = Mathf.Abs(measures[i].EndPosition.y - measures[i].StartPosition.y);

                    measures[i].gameObject.name = CourseBase.GetMeasureName(measures[i].HoleIndex, measures[i].OrderIndex);
                    string text = Utility.DistanceHeight(xz, y, MainToolUI.CurrentUnits);

                    Rect rect = ProjectToScreenRect(center, 16, 16);
                    rect.width = GUI.skin.label.CalcSize(new GUIContent(text)).x;
                    rect.height = GUI.skin.label.CalcSize(new GUIContent(text)).y;
                    rect.x -= rect.width / 4;
                    rect.y -= rect.height / 4;

                    BeginUI();
                    MoveToPixels((int)rect.x, (int)rect.y);
                    SetBoxSize((int)rect.width, (int)rect.height);
                    Background(1, 1, 0.5f);
                    LabelNoOffset(text);
                    EndUI();
                }
            }
        }
        void OnSceneFlyBys(bool selected)
        {
            List<FlyByBase> flyBys = CourseBase.FlyBys;
            for (int i = 0; i < flyBys.Count; ++i)
            {
                Color color = new Color(0.75f, 0.0f, 0.15f);

                if (CourseBase.ActivePlant is FlyByBase && flyBys[i] == (FlyByBase)CourseBase.ActivePlant)
                {
                    color = White;
                }

                Handles.color = color;
                Handles.DrawPolyLine(flyBys[i].Points);
                Handles.color = White;

                if (selected)
                {
                    string text = CourseBase.GetFlyByName(flyBys[i].HoleIndex, flyBys[i].OrderIndex, flyBys[i].Type);
                    flyBys[i].gameObject.name = text;

                    for (int n = 0; n < flyBys[i].Nodes.Count; ++n)
                    {
                        Color parentColor = new Color(1, 0, 0, 0.5f);
                        Color childColor = new Color(0, 1, 0, 0.5f);
                        if (flyBys[i].Nodes[n] == activeInfo)
                        {
                            parentColor = new Color(1, 1, 1, 0.5f);
                            childColor = Color.Lerp(childColor, parentColor, 0.5f);
                        }
                        else if (IsMouseAround(flyBys[i].Nodes[n].position.ToVector3()) || (flyBys[i].Nodes[n].target == FlyByBase.Info.Node.Target.Probe && IsMouseAround(flyBys[i].Nodes[n].probe.position.ToVector3())))
                        {
                            parentColor = new Color(1, 1, 0, 0.5f);
                            childColor = Color.Lerp(childColor, parentColor, 0.5f);
                        }

                        Vector3 position = flyBys[i].Nodes[n].position.ToVector3();
                        Vector3 targetPosition = flyBys[i].Nodes[n].TargetPosition(flyBys[i].HoleIndex, flyBys[i].Nodes, Vector3.zero, Vector3.zero, Vector3.zero);
                        float size = Utility.squareHalf * 4;
                        float arrowSize = size / 2;

                        Handles.color = parentColor;
                        Handles.SphereCap(0, position, Quaternion.identity, size);

                        if (CourseBase.ActivePlant == flyBys[i])
                        {
                            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, (targetPosition - position).normalized);
                            Vector3 arrowEnd = targetPosition - (targetPosition - position).normalized * (size / 2);
                            Vector3 arrowStart = arrowEnd - (targetPosition - position).normalized * (arrowSize / 2);

                            Handles.color = Color.Lerp(parentColor, childColor, 0.5f);
                            Handles.DrawLine(position, arrowEnd);
                            Handles.ConeCap(0, arrowStart, rotation, arrowSize);
                            Handles.color = childColor;
                            Handles.SphereCap(0, targetPosition, Quaternion.identity, size);
                        }

                        Handles.color = Color.white;
                    }
                }
            }
        }
        #endregion

        #region Methods
        public override void OnScene(bool selected)
        {
            if (timer.IsRunning) timer.Update(DeltaTime);

            OnScenePins(selected);
            OnSceneTees(selected);
            OnSceneShots(selected);
            OnSceneMeasures(selected);
            if (selected) OnSceneFlyBys(selected);
        }
        public override void OnUI(bool selected)
        {
            List<MeasureBase> measures = CourseBase.Measures;
            List<PinBase> pins = CourseBase.Pins;
            List<ShotBase> shots = CourseBase.Shots;
            List<TeeBase> tees = CourseBase.Tees;
            List<FlyByBase> flyBys = CourseBase.FlyBys;

            if (selected)
            {
                #region Delete
                if (CourseBase.ActivePlant != null && Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.KeyDown)
                {
                    Event.current.Use();
                    MonoBehaviour.DestroyImmediate(CourseBase.ActivePlant.Transform.gameObject);
                    return;
                }
                #endregion

                if (CourseBase.ActivePlant != null)
                {
                    BeginUI();
                    Move(3, 0);
                    SetColor(Green);

                    if (!(CourseBase.ActivePlant is MeasureBase))
                    {
                        SetColor(Red);
                        Popup("Hole", CourseBase.ActivePlant.HoleIndex, CourseBase.HoleNames, 1);
                        SetColor();
                        Move(1, 0);
                    }

                    if (CourseBase.ActivePlant is TeeBase)
                    {
                        TeeBase tee = (CourseBase.ActivePlant as TeeBase);
                        tee.Type = TeeTypeField(tee.Type, tee.HoleIndex, true, 2);
                        Move(2, 0);

                        tee.Par = ParField(tee.Par, 2);
                        Move(2, 0);

                        tee.StrokeIndex = StrokeIndexField(tee.HoleIndex, tee.Type, tee.StrokeIndex, true, 2);
                        Move(2, 0);

                        tee.TeeInfo.width = LengthSlider(tee.TeeInfo.width, "Width", 0.0f, 10, 0, 4);
                        Move(4, 0);

                        tee.TeeInfo.height = LengthSlider(tee.TeeInfo.height, "Height", 0.0f, 10, 0, 4);
                        Move(4, 0);

                        if (tee.TestLeft && tee.TestRight)
                        {
                            if (Button("Remove Marker", 4))
                            {
                                if (tee.TestLeft != null) MonoBehaviour.DestroyImmediate(tee.TestLeft);
                                if (tee.TestRight != null) MonoBehaviour.DestroyImmediate(tee.TestRight);
                            }
                        }
                        else
                        {
                            if (Button("Test Tee Marker", 4))
                            {
                                Vector3 forward = Vector3.right;
                                if (CourseBase.Holes[tee.HoleIndex].pins.Count != 0) forward = (CourseBase.AimPoint(tee.Position, tee, CourseBase.Holes[tee.HoleIndex].shots, CourseBase.Holes[tee.HoleIndex].pins[0]) - tee.Position).normalized;
                                forward.y = 0;
                                Vector3 right = Vector3.Cross(forward, Vector3.up);

                                Vector3 teePosition = tee.Position + forward * tee.Height / 2 * Random.insideUnitCircle.x + right * tee.Width / 2 * Random.insideUnitCircle.x;
                                teePosition.y = CourseBase.MeshHeight(teePosition.x, teePosition.z);

                                GameObject courseTeeMarker = CourseBase.GetTeeMarker(tee.Type);
                                if (courseTeeMarker)
                                {
                                    tee.TestLeft = (GameObject)GameObject.Instantiate(courseTeeMarker, teePosition - right * Utility.markerOffset, Quaternion.identity);
                                    tee.TestRight = (GameObject)GameObject.Instantiate(courseTeeMarker, teePosition + right * Utility.markerOffset, Quaternion.identity);
                                    tee.TestLeft.transform.forward = forward;
                                    tee.TestRight.transform.forward = forward;
                                }
                            }
                        }
                        Move(4, 0);
                    }
                    else if (CourseBase.ActivePlant is ShotBase)
                    {
                    }
                    else if (CourseBase.ActivePlant is PinBase)
                    {
                        (CourseBase.ActivePlant as PinBase).Difficulty = DifficultyField((CourseBase.ActivePlant as PinBase).Difficulty, 2);
                        Move(2, 0);
                    }
                    else if (CourseBase.ActivePlant is MeasureBase)
                    {
                    }
                    else if (CourseBase.ActivePlant is FlyByBase)
                    {
                        FlyByBase flyby = (CourseBase.ActivePlant as FlyByBase);
                        FlyByBase.Info.Node node = (activeInfo as FlyByBase.Info.Node);

                        flyby.Type = FlyByTypeField(flyby.Type, flyby.HoleIndex, true, 1);
                        Move(1, 0);

                        if (node != null) node.target = TargetField(node.target, node, 2);
                        Move(2, 0);

                        node.velocity = VelocityField("Velocity", node.velocity, 15.0f, 3);
                        Move(3, 0);

                        node.RotationCurve = CurveField("Rotation Step", node.RotationCurve, AnimationCurve.Linear(0, 0, 1, 1), 3);
                        Move(3, 0);

                        FlyByBase flyBy = CourseBase.ActivePlant as FlyByBase;
                        float originalTime = timer.Elapsed;
                        float time = originalTime;

                        VideoSlider(ref time, 3);
                        Move(3, 0);

                        if (node == (CourseBase.ActivePlant as FlyByBase).Nodes[0] || node == (CourseBase.ActivePlant as FlyByBase).Nodes[1] || node == (CourseBase.ActivePlant as FlyByBase).Nodes[2])
                        {
                            SetColor(Green);
                            node.position.Set(VectorField("Position", node.position.ToVector3(), 5));
                            Move(5, 0);
                            SetColor();
                        }

                        if (play)
                        {
                            if (time == timer.MaxTime)
                            {
                                time = 0;
                                timer.Stop();
                                pause = false;
                                play = false;
                            }
                            else if (time != originalTime)
                            {
                                timer.MoveToTime(time / timer.MaxTime);
                                if (!pause) timer.Resume();
                            }

                            HandleUtility.Repaint();

                            Camera.main.transform.position = (CourseBase.ActivePlant as FlyByBase).GetPosition(timer.Elapsed);
                            Camera.main.transform.LookAt((CourseBase.ActivePlant as FlyByBase).GetTargetPosition(timer.Elapsed, Vector3.zero, Vector3.zero, Vector3.zero));
                        }
                        else
                        {
                            if (time != originalTime)
                            {
                                play = true;
                                pause = true;

                                timer.Start((CourseBase.ActivePlant as FlyByBase).Time);
                                timer.MoveToTime(time / timer.MaxTime);

                                HandleUtility.Repaint();
                            }

                            Camera.main.transform.position = node.position;
                            Camera.main.transform.LookAt(node.TargetPosition(flyBy.HoleIndex, flyBy.Nodes, Vector3.zero, Vector3.zero, Vector3.zero));
                        }

                        if (Event.current.type == EventType.Repaint && Camera.main)
                        {
                            Selection.activeTransform = Camera.main.transform;
                        }
                    }

                    SetColor();
                    EndUI();
                }
                else
                {
                    BeginUI();
                    Move(3, 0);

                    holeIndex = Popup("Hole", holeIndex, CourseBase.HoleNames, 1);
                    Move(1, 0);

                    TeeField();
                    Move(1, 0);

                    if (teePlanting && CourseBase.FreeTees(holeIndex).Length == 0) teePlanting = false;
                    if (teePlanting)
                    {
                        teeType = TeeTypeField(teeType, holeIndex, false, 2);
                        Move(2, 0);

                        teePar = ParField(teePar, 2);
                        Move(2, 0);

                        teeStrokeIndex = StrokeIndexField(holeIndex, teeType, teeStrokeIndex, false, 2);
                        Move(2, 0);

                        teeWidth = LengthSlider(teeWidth, "Width", 0.0f, 10, 0, 4);
                        Move(4, 0);

                        teeHeight = LengthSlider(teeHeight, "Height", 0.0f, 10, 0, 4);
                        Move(4, 0);
                    }

                    ShotField();
                    Move(1, 0);

                    PinField();
                    Move(1, 0);

                    if (pinPlanting)
                    {
                        pinDifficulty = DifficultyField(pinDifficulty, 2);
                        Move(2, 0);
                    }

                    MeasureField();
                    Move(1, 0);

                    FlyByField();
                    Move(1, 0);

                    if (flyByPlanting)
                    {
                        flyByType = FlyByTypeField(flyByType, holeIndex, false, 2);
                        Move(2, 0);
                    }

                    SetColor();
                    EndUI();
                }
            }

            #region UnClick
            if (activeInfo != null && LeftMouseUp && !(activeInfo is FlyByBase.Info.Node))
            {
                Event.current.Use();
                activeInfo = null;
                return;
            }
            #endregion

            if (selected)
            {
                for (int i = 0; i < pins.Count; ++i)
                    OnPlantUI(pins[i], pins[i].PlantInfo);
                for (int i = 0; i < shots.Count; ++i)
                    OnPlantUI(shots[i], shots[i].PlantInfo);
                for (int i = 0; i < tees.Count; ++i)
                    OnPlantUI(tees[i], tees[i].PlantInfo);

                for (int i = 0; i < flyBys.Count; ++i)
                    for (int n = 0; n < flyBys[i].Nodes.Count; ++n)
                        OnFlyByUI(flyBys[i], flyBys[i].Nodes[n]);
            }
            for (int i = 0; i < measures.Count; ++i)
            {
                OnPlantUI(measures[i], measures[i].StartInfo);
                OnPlantUI(measures[i], measures[i].EndInfo);
            }

            if (selected)
            {
                OnMouse();

                if (Event.current.type == EventType.Repaint)
                {
                    EditorGUIUtility.AddCursorRect(new Rect(0, 0, Screen.width, Screen.height), MouseCursor.Arrow);
                }
            }

            if (CourseBase.ActivePlant != null && LeftMouseDown)
            {
                Event.current.Use();
                CourseBase.ActivePlant = null;
                activeInfo = null;
            }
        }
        public override void OnCleanup()
        {
            holeIndex = 0;
            teePlanting = false;
            shotPlanting = false;
            pinPlanting = false;
            measurePlanting = false;
            flyByPlanting = false;

            teePlanting = false;
            teeType = TeeBase.Info.Type.Championship;
            pinDifficulty = PinBase.Info.Difficulty.Medium;
            teePar = TeeBase.Info.Par._4;
            teeStrokeIndex = 0;
            teeWidth = 0;
            teeHeight = 0;
            flyByType = FlyByBase.Info.Type.A;
            metersPerNode = 30.0f;

            play = false;
            pause = false;
            timer = new Timer();

            drawing = false;
            points = new Vector3[0];
            firstHit = null;
            originalPoint = Vector3.zero;
            activeInfo = null;
            if (CourseBase.Terrain) CourseBase.ActivePlant = null;
        }
        #endregion

        #region Support Methods
        void OnMouse()
        {
            if (teePlanting)
            {
                if (LeftMouseDown)
                {
                    if (CourseBase.CameraTerrainHit != null)
                    {
                        CourseBase.CreateTee(holeIndex, teeType, teePar, teeStrokeIndex, teeWidth, teeHeight);
                        teePlanting = false;
                    }
                    Event.current.Use();
                }
            }

            if (shotPlanting)
            {
                if (LeftMouseDown)
                {
                    if (CourseBase.CameraTerrainHit != null)
                    {
                        CourseBase.CreateShot(holeIndex);
                        shotPlanting = false;
                    }
                    Event.current.Use();
                }
            }

            if (pinPlanting)
            {
                if (LeftMouseDown)
                {
                    if (CourseBase.CameraTerrainHit != null)
                    {
                        CourseBase.CreatePin(holeIndex, pinDifficulty, CourseBase.CameraTerrainHit.Value.point);
                        pinPlanting = false;
                    }
                    Event.current.Use();
                }
            }

            if (measurePlanting)
            {
                if (LeftMouseDown)
                {
                    if (CourseBase.CameraTerrainHit != null)
                    {
                        firstHit = CourseBase.CameraTerrainHit;
                        originalPoint = firstHit.Value.point;
                    }
                    Event.current.Use();
                }
                if (LeftMouseUp)
                {
                    if (CourseBase.CameraTerrainHit != null)
                    {
                        CourseBase.CreateMeasure(holeIndex, originalPoint, CourseBase.CameraTerrainHit.Value.point, Units.Imperial);
                        firstHit = null;
                        originalPoint = Vector3.zero;
                    }
                    measurePlanting = false;
                    Event.current.Use();
                }
            }

            if (flyByPlanting)
            {
                if (LeftMouseDown)
                {
                    drawing = true;
                    Event.current.Use();
                }
                if (drawing && LeftMouseUp)
                {
                    if (points.Length >= 2) points = points.SmoothLine(metersPerNode, false);
                    if (points.Length >= 2)
                    {
                        CourseBase.CreateFlyBy(holeIndex, points, flyByType);
                        Event.current.Use();
                    }
                    OnCleanup();
                }
                if (drawing) OnMouseDown();

                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Event.current.Use();
                    OnCleanup();
                }
                LineToolUI.DrawLines(points, Black);
            }
        }
        void OnMouseDown()
        {
            RaycastHit? hit = CourseBase.CameraTerrainHit;
            if (hit != null)
            {
                Vector3 point = hit.Value.point;
                ArrayUtility.Add(ref points, point);
            }
        }

        void OnPlantUI(IPlant plant, PlantBase.BaseInfo info)
        {
            if (activeInfo != info && LeftMouseDown && IsMouseAround(info.position.ToVector3()))
            {
                Event.current.Use();

                CourseBase.ActivePlant = plant;
                activeInfo = info;
                firstHit = CourseBase.CameraTerrainHit;
                originalPoint = info.position.ToVector3();

                if (MainToolUI.SelectedTool != MainToolUI.Tool.PlantTool) MainToolUI.SelectedTool = MainToolUI.Tool.PlantTool;
                return;
            }
            if (info == activeInfo)
            {
                RaycastHit? hit = CourseBase.CameraTerrainHit;
                if (firstHit != null && hit != null)
                {
                    PlatformBase.Editor.RecordObject(plant.Transform.gameObject, "Plant Info Position Change");

                    Vector3 position = (originalPoint + hit.Value.point - firstHit.Value.point);
                    position.y = CourseBase.TerrainHeight(position.x, position.z);

                    plant.Position = position;
                    info.position.Set(position);
                }
            }
        }
        void OnFlyByUI(IPlant plant, PlantBase.BaseInfo info)
        {
            FlyByBase flyBy = (plant as FlyByBase);
            FlyByBase.Info.Node infoNode = (info as FlyByBase.Info.Node);
            if (LeftMouseDown && IsMouseAround(info.position) || (infoNode.target == FlyByBase.Info.Node.Target.Probe && IsMouseAround(infoNode.probe.position)))
            {
                if (LeftDoubleClick)
                {
                    if (Event.current.modifiers == EventModifiers.Shift)
                    {
                        if (flyBy.Nodes.Count > 3)
                        {
                            if (activeInfo == info)
                            {
                                CourseBase.ActivePlant = null;
                                activeInfo = null;
                            }

                            flyBy.Nodes.Remove((FlyByBase.Info.Node)infoNode);
                            flyBy.UpdateFlyBy();
                            Event.current.Use();
                        }
                    }
                    else
                    {
                        FlyByBase.Info.Node prev = info as FlyByBase.Info.Node;
                        FlyByBase.Info.Node next = (info as FlyByBase.Info.Node).GetNext(flyBy.Nodes);
                        FlyByBase.Info.Node node = new FlyByBase.Info.Node();
                        node.position = prev.position + (next.position.ToVector3() - prev.position) / 2;

                        Vector3 right = Vector3.right;
                        float distance = 3.0f;
                        right = Vector3.Cross((next.position.ToVector3() - prev.position).normalized, Vector3.up);
                        distance = (next.position.ToVector3() - prev.position).magnitude / 2;

                        node.probe.position = node.position.ToVector3() + right * distance;

                        flyBy.Nodes.Insert(flyBy.Nodes.IndexOf((FlyByBase.Info.Node)prev) + 1, (FlyByBase.Info.Node)node);
                        flyBy.UpdateFlyBy();
                        Event.current.Use();
                    }

                    return;
                }
                else if (activeInfo != info)
                {
                    CourseBase.ActivePlant = plant;
                    activeInfo = info;
                    Event.current.Use();
                }
            }
            if (info == activeInfo)
            {
                Vector3 delta = Vector3.zero;
                FlyByBase.Info.Node node = (info as FlyByBase.Info.Node);

                if (node.target == FlyByBase.Info.Node.Target.Probe)
                {
                    delta = node.probe.position;
                    node.probe.position = Handles.PositionHandle(node.probe.position, Quaternion.identity);
                    delta -= node.probe.position;

                    if (Event.current.modifiers == EventModifiers.Shift)
                        for (int i = 0; i < flyBy.Nodes.Count; ++i)
                        {
                            if (flyBy.Nodes[i].probe == info) continue;
                            flyBy.Nodes[i].probe.position = flyBy.Nodes[i].probe.position - delta;
                        }
                }

                delta = info.position;
                info.position = Handles.PositionHandle(info.position.ToVector3(), Quaternion.identity);
                delta -= info.position;

                if (Event.current.modifiers == EventModifiers.Shift)
                    for (int i = 0; i < flyBy.Nodes.Count; ++i)
                    {
                        if (flyBy.Nodes[i] == info) continue;
                        flyBy.Nodes[i].position = flyBy.Nodes[i].position.ToVector3() - delta;
                    }

                flyBy.UpdateFlyBy();
            }
        }
        #endregion
    }
}
