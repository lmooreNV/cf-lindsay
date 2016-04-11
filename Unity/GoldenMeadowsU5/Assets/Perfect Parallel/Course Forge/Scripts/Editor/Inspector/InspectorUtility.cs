using UnityEditor;

namespace PerfectParallel.CourseForge.Inspectors
{
	/// <summary>
	/// Utility class to draw Inspectors
	/// </summary>
	public class InspectorUtility
	{
		#region Methods
		/// <summary>
		/// Draws default inspector with name of the component
		/// </summary>
		/// <param name="editor">current editor</param>
		public static void DrawDefaultInspector(Editor editor)
		{
			EditorGUILayout.HelpBox("CF " + editor.targets[0].GetType().Name + " Component", MessageType.Info);
		}
		#endregion
	}
}