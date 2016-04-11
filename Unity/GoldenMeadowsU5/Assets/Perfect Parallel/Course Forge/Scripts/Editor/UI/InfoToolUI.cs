using UnityEngine;
using UnityEditor;

namespace PerfectParallel.CourseForge.UI
{
    /// <summary>
    /// Course Forge info tool, controls the settings of the course
    /// </summary>
    public class InfoToolUI : ToolUI
    {

        #region Fields
        int count = 0;
        #endregion

        #region Methods
        public override void OnUI(bool selected)
        {
            BeginUI();

            MoveToTop();
            Move(1, 1);
            MovePixels(1, 1);
            Background(5, 5.55f, 0.5f);

            #region Course Info
            Label("| Info", 3.5f);

            Move(3, 0);
            if (GUI.Button(GetOffsetRect(4, 1), MainToolUI.CommunityMode ? "[Community]" : "[Commerical]", GUI.skin.label))
            {
                count++;
                if (count == 7)
                {
                    count = 0;

                    TextEditor text = new TextEditor();
                    text.Paste();
                    EditorPrefs.SetString("Course Forge/CommunityMode", text.content.text);
                }
            }
            Move(-3, 0);

            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(5, 5.05f, 0.75f);
            {
                Label("  Author", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.author = TextField(CourseBase.Info.author, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Course Name", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.name = TextField(CourseBase.Info.name, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            #endregion

            MoveToTop();
            Move(5, 1);
            MovePixels(1, 1);
            Background(5, 5.55f, 0.5f);

            #region Apperance
            Label("| Course Images", 2.5f);
            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(5, 2.5f, 0.75f);
            {
                Label("  Splash", 2.5f);
                Move(2.5f, 0);
                Label("Cameo", 2.5f);
                Move(-2.5f, 0);
                Move(0, 0.3f);

                Move(0.2f, 0);
                CourseBase.Info.Splash = ObjectField(CourseBase.Info.Splash, 2.3f, 2.3f);
                Move(2.3f, 0);
                CourseBase.Info.Cameo = ObjectField(CourseBase.Info.Cameo, 2.3f, 2.3f);
                Move(-2.5f, 0);
                Move(0, 2.1f);
            }
            #endregion

            #region Location
            Label("| Location", 3.5f);
            Move(0, 0.5f);
            MovePixels(1, 1);
            Background(5, 2.15f, 0.75f);
            {
                Label("  Geo X/West", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.geoX = FloatField(CourseBase.Info.geoX, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Geo Y/North", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.geoY = FloatField(CourseBase.Info.geoY, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Geo Z/Height", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.geoZ = FloatField(CourseBase.Info.geoZ, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  UTM Zone", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.utmZone = IntField(CourseBase.Info.utmZone, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Mesh", 2.5f);
                Move(2.2f, 0);
                CourseBase.Info.offset = FloatSlider(CourseBase.Info.offset, 0, 2.0f, 2.9f, 0.6f);
                Move(-2.2f, 0);
            }
            Move(0, 0.5f);
            MovePixels(-1, 0);
            #endregion

            MoveToTop();
            Move(5, 1);
            MovePixels(2, 1);
            Background(5, 7.8f, 0.5f);

            #region Tee Markers
            Label("| Tee Markers", 3.5f);
            Move(0, 0.5f);
            MovePixels(0, 1);
            Background(5, 8.5f, 0.75f);
            {
                Label("  Championship", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.championshipTeeMarker = ObjectField<GameObject>(CourseBase.Info.championshipTeeMarker, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Width", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.championshipWidth = FloatSlider(CourseBase.Info.championshipWidth, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Height", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.championshipHeight = FloatSlider(CourseBase.Info.championshipHeight, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Tournament", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.tournamentTeeMarker = ObjectField<GameObject>(CourseBase.Info.tournamentTeeMarker, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Width", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.tournamentWidth = FloatSlider(CourseBase.Info.tournamentWidth, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Height", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.tournamentHeight = FloatSlider(CourseBase.Info.tournamentHeight, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Back", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.backTeeMarker = ObjectField<GameObject>(CourseBase.Info.backTeeMarker, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Width", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.backWidth = FloatSlider(CourseBase.Info.backWidth, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Height", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.backHeight = FloatSlider(CourseBase.Info.backHeight, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Member", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.memberTeeMarker = ObjectField<GameObject>(CourseBase.Info.memberTeeMarker, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Width", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.memberWidth = FloatSlider(CourseBase.Info.memberWidth, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Height", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.memberHeight = FloatSlider(CourseBase.Info.memberHeight, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Forward", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.forwardTeeMarker = ObjectField<GameObject>(CourseBase.Info.forwardTeeMarker, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Width", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.forwardWidth = FloatSlider(CourseBase.Info.forwardWidth, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Height", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.forwardHeight = FloatSlider(CourseBase.Info.forwardHeight, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Ladies", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.ladiesTeeMarker = ObjectField<GameObject>(CourseBase.Info.ladiesTeeMarker, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Width", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.ladiesWidth = FloatSlider(CourseBase.Info.ladiesWidth, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Height", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.ladiesHeight = FloatSlider(CourseBase.Info.ladiesHeight, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("  Challenge", 2.5f);
                Move(2.5f, 0);
                CourseBase.Info.challengeTeeMarker = ObjectField<GameObject>(CourseBase.Info.challengeTeeMarker, 2.5f, 0.6f);
                Move(-2.5f, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Width", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.challengeWidth = FloatSlider(CourseBase.Info.challengeWidth, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            {
                Label("        Height", 2.5f);
                MovePixels(-3, 0);
                Move(2.5f, 0);
                CourseBase.Info.challengeHeight = FloatSlider(CourseBase.Info.challengeHeight, 0.0f, 10, 2.65f, 0.6f);
                Move(-2.5f, 0);
                MovePixels(3, 0);
            }
            Move(0, 0.4f);
            #endregion

            MoveToTop();
            MoveToLeft();
            Move(1, 1);
            Touch(10, 5.15f);
            Move(10, 1);
            Touch(5, 7.8f);

            EndUI();
        }
        #endregion
    }
}