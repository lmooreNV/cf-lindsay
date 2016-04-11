using UnityEditor;

namespace PerfectParallel.CourseForge.Inspectors
{
	/// <summary>
	/// Course class inspector
	/// </summary>
	[CustomEditor(typeof(CourseBase), true)]
	public class CourseInspector : Editor
	{
		#region Methods
		public override void OnInspectorGUI()
        {
		}
		#endregion
	}
}