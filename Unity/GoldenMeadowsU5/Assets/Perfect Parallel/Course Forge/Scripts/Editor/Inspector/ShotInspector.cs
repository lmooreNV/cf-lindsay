using UnityEditor;

namespace PerfectParallel.CourseForge.Inspectors
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ShotBase), true)]
	public class ShotInspector : Editor
	{
		#region Methods
		public override void OnInspectorGUI()
		{
		}
		#endregion
	}
}