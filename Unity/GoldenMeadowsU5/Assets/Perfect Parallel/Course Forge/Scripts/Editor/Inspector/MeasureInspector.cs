using UnityEditor;

namespace PerfectParallel.CourseForge.Inspectors
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(MeasureBase), true)]
	public class MeasureInspector : Editor
	{
		#region Methods
		public override void OnInspectorGUI()
		{
		}
		#endregion
	}
}