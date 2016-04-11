using UnityEditor;

namespace PerfectParallel.CourseForge.Inspectors
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TeeBase), true)]
	public class TeeInspector : Editor
	{
		#region Methods
		public override void OnInspectorGUI()
		{
		}
		#endregion
	}
}