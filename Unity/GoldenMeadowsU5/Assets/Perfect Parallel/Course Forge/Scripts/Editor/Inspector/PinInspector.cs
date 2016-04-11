using UnityEditor;
using UnityEngine;

namespace PerfectParallel.CourseForge.Inspectors
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(PinBase), true)]
	public class PinInspector : Editor
	{
		#region Methods
		public override void OnInspectorGUI()
		{
		}
		#endregion
	}
}