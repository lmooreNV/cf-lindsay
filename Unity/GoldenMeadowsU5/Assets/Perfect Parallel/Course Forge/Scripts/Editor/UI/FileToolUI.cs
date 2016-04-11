using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Course Forge file tool, can build course, export heightmap
    /// </summary>
    public class FileToolUI : ToolUI
    {
        #region Properties
        float ExportTerrainStep
        {
            get
            {
                return EditorPrefs.GetFloat("Course Forge/ExportTerrainStep", 0.5f);
            }
            set
            {
                EditorPrefs.SetFloat("Course Forge/ExportTerrainStep", value);
            }
        }
        string BuildTarget
        {
            get
            {
                return EditorPrefs.GetString("Course Forge/ExportBuildTarget", UnityEditor.BuildTarget.WebPlayerStreamed.ToString());
            }
            set
            {
                EditorPrefs.SetString("Course Forge/ExportBuildTarget", value);
                CourseBase.Info.platform = value;
            }
        }
        #endregion

        #region Methods
        public override void OnUI(bool selected)
        {
            if (selected == false) return;

            BeginUI();
            Move(1, 0);

            Background(4);

            if (MainToolUI.InfoPanel) SetColor(Green);
            if (Button(courseInfoTexture))
            {
                MainToolUI.InfoPanel = !MainToolUI.InfoPanel;
            }
            SetColor();
            Move(1, 0);

            if (Button(helpTexture))
            {
                Application.OpenURL("http://perfectparallel.com/documentation");
            }

            #region Version
            Move(0, 0.47f);

            SetColor(Black);
            MovePixels(1, 1);
            Label(CourseBase.version, 1, 1, EditorStyles.boldLabel);
            MovePixels(-1, -1);

            SetColor();
            Label(CourseBase.version, 1, 1, EditorStyles.boldLabel);

            Move(0, -0.47f);
            #endregion

            SetColor();
            Move(1, 0);

            if (Button(androidTexture))
            {
                MainToolUI.AndroidPanel = !MainToolUI.AndroidPanel;
            }
            Move(1, 0);

            EditorGUI.BeginDisabledGroup(!PlatformBase.IO.IsEditor);
            if (PopupToolUI.Count() != 0)
            {
                SetColor(Yellow);
            }
            if (Button(new GUIContent(buildTexture, (PopupToolUI.Count() != 0 ? "Click to build it anyway" : null))))
            {
                if (PopupToolUI.Count() == 0)
                {
                    CheckCourse();
                    if (PopupToolUI.Count() == 0) CourseBase.BuildCourse((int)Utility.FromName<UnityEditor.BuildTarget>(BuildTarget));
                }
                else
                {
                    CourseBase.BuildCourse((int)Utility.FromName<UnityEditor.BuildTarget>(BuildTarget));
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(MainToolUI.NormalMode);
            if (!MainToolUI.NormalMode || BuildTarget != UnityEditor.BuildTarget.WebPlayerStreamed.ToString())
            {
                Move(1.3f, 0.35f);
                Background(2.0f, 0.35f);
                Background(2.0f, 0.35f);
                BuildTarget = EditorGUI.EnumPopup(GetRect(2.0f, 1.0f), Utility.FromName<UnityEditor.BuildTarget>(BuildTarget)).ToString();

                Color color = GUI.color;
                SetColor(Black);
                Move(-0.45f, -0.15f);
                MovePixels(1, 1);
                Label("►", 3, 3, EditorStyles.largeLabel);
                MovePixels(-1, -1);
                SetColor();
                GUI.color = color;

                Move(-0.01f, 0.01f);
                Label("►", 3, 3, EditorStyles.largeLabel);
                Move(-0.05f, -0.6f);
            }
            EditorGUI.EndDisabledGroup();

            SetColor();
            Move(1, 0);

            Move(3, 0);

            EditorGUI.BeginDisabledGroup(!PlatformBase.IO.IsEditor);
            if (!MainToolUI.CommunityMode)
            {
                Background(2);

                if (Button(importTexture))
                {
                    string path = PlatformBase.Editor.OpenFileDialog("Load .geojson", PlatformBase.IO.CoursesPath, "geojson");
                    if (!string.IsNullOrEmpty(path)) GeoJSON.ImportSplines(path);
                }
                Move(1, 0);

                if (Button(exportTexture))
                {
                    string path = PlatformBase.Editor.SaveFileDialog("Save .geojson", PlatformBase.IO.CoursesPath, "", "geojson");
                    if (!string.IsNullOrEmpty(path)) GeoJSON.ExportSplines(path);
                }
                Move(1, 0);

                Background(4);

                Label("Export Terrain Step", 2.7f);
                Move(0, 0.35f);
                ExportTerrainStep = FloatSlider(ExportTerrainStep, 0.01f, 10.0f, 3.0f, 0.6f);
                Move(0, -0.35f);
                Move(3, 0);

                if (Button(terrainTexture))
                {
                    string path = PlatformBase.Editor.SaveFileDialog("Save .bil", PlatformBase.IO.CoursesPath, "", "bil");
                    if (!string.IsNullOrEmpty(path)) GeoJSON.ExportTerrain(path, ExportTerrainStep);
                }
                Move(1, 0);
            }
            EditorGUI.EndDisabledGroup();

            EndUI();
        }
        #endregion

        #region Support Methods
        void CheckCourse()
        {
            PopupToolUI.Clear();

            if (CourseBase.Holes.Any(x => x.pins.Count == 0 || x.tees.Count == 0 || x.flyBys.Count == 0)) PopupToolUI.Add("Course doesn't have 18 pins/tees/flybys.");
            if (CourseBase.Info.Splash == null || CourseBase.Info.Cameo == null) PopupToolUI.Add("Course doesn't have Course Images.");
            if (CourseBase.Hazards.Any(x => x.Layer.name == Utility.GetName(HazardBase.Type.Out_of_Bounds)) == false) PopupToolUI.Add("Course doesn't have 'Out Of Bounds' hazard.");
        }
        #endregion
    }
}
