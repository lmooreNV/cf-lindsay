using UnityEditor;
using UnityEngine;

namespace PerfectParallel.CourseForge.Inspectors
{
	[CustomEditor(typeof(TerrainControllerBase), true)]
	public class TerrainControllerInspector : Editor
	{
		#region Methods
		public override void OnInspectorGUI()
		{
			TerrainControllerBase main = target as TerrainControllerBase;
			main.terrainTexture = (Texture)EditorGUILayout.ObjectField("Terrain Overlay", main.terrainTexture, typeof(Texture), true);
			main.fresnelIntensity = EditorGUILayout.FloatField("Fresnel Intensity", main.fresnelIntensity);
			if (GUILayout.Button("Reset heightmap"))
			{
				CourseBase.TerrainLowered = false;
			}
		}
		#endregion
	}
}