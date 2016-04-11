using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge.Inspectors
{
	[CustomEditor(typeof(SplineBase), true)]
	public class SplineInspector : Editor
	{
		#region Methods
		public override void OnInspectorGUI()
        {
            SplineBase spline = (target as SplineBase);
            if (GUILayout.Button("Copy material parameters to layer"))
            {
                Layer layer = spline.Layer;
                Material[] sharedMaterials = spline.GetComponent<Renderer>().sharedMaterials;
                for (int i = 0; i < sharedMaterials.Length; ++i)
                {
                    if (!sharedMaterials[i].HasProperty("CMain")) continue;
                    layer.mainColor = sharedMaterials[i].GetColor("CMain");
                    layer.specularColor = sharedMaterials[i].GetColor("CSpecular");
                    layer.fresnelIntensity = sharedMaterials[i].GetFloat("CFresnelIntensity");
                    layer.diffuse.tile = sharedMaterials[i].GetFloat("CDiffuseTile");
                    layer.detail.tile = sharedMaterials[i].GetFloat("CDetailTile");
                    layer.normal.tile = sharedMaterials[i].GetFloat("CNormalTile");
                    layer.detailNormal.tile = sharedMaterials[i].GetFloat("CDetailNormalTile");
                    layer.normal.bump = sharedMaterials[i].GetFloat("CNormalBump");
                    layer.detailNormal.bump = sharedMaterials[i].GetFloat("CDetailNormalBump");
                }
            }
		}
		#endregion
	}
}