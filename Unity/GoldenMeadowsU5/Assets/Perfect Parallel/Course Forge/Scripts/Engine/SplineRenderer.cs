using UnityEngine;
using System;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Spline helper class to create meshes,
    /// collider and renderer
    /// </summary>
    [Serializable]
    public class SplineRenderer
    {
        #region Fields
        [SerializeField]
        Mesh mesh = null;
        [SerializeField]
        Mesh meshPhysics = null;

        [SerializeField]
        MeshFilter meshFilter = null;
        [SerializeField]
        MeshCollider meshCollider = null;

        [SerializeField]
        MeshRenderer meshRenderer = null;
        [SerializeField]
        List<Material> materials = new List<Material>();

        [SerializeField]
        bool needMaterialRefresh = false;
        #endregion

        #region Properties
        /// <summary>
        /// Is material need update?
        /// </summary>
        public bool NeedMaterialRefresh
        {
            get
            {
                return needMaterialRefresh;
            }
            set
            {
                needMaterialRefresh = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update mesh from the line data
        /// </summary>
        /// <param name="spline"></param>
        public void UpdateMesh(SplineBase spline)
        {
            if (mesh)
            {
                MonoBehaviour.DestroyImmediate(mesh);
                mesh = null;
            }
            if (meshPhysics)
            {
                MonoBehaviour.DestroyImmediate(meshPhysics);
                meshPhysics = null;
            }
            if (meshFilter)
            {
                MonoBehaviour.DestroyImmediate(meshFilter);
                meshFilter = null;
            }
            if (meshCollider && meshCollider.material)
            {
                MonoBehaviour.DestroyImmediate(meshCollider.material);
                meshCollider.material = null;
            }
            if (meshCollider && meshCollider.sharedMaterial)
            {
                meshCollider.sharedMaterial = null;
            }
            if (meshCollider)
            {
                MonoBehaviour.DestroyImmediate(meshCollider);
                meshCollider = null;
            }

            if (spline.IsHazard) return;

            SplineBase.SplinePattern pattern = spline.Pattern;
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();
            List<Color> colors = new List<Color>();

            mesh = new Mesh();
            mesh.Clear();
            mesh.name = "Spline Mesh";

            int submesh = 0;
            if (pattern.extra)
            {
                if (pattern.transition) AddPoly(spline.Lines.TransitionPoly, mesh, submesh++, vertices, normals, tangents, colors);
                AddPoly(spline.Lines.ExtraPoly, mesh, submesh++, vertices, normals, tangents, colors);
                if (pattern.extraTransition) AddPoly(spline.Lines.ExtraTransitionPoly, mesh, submesh++, vertices, normals, tangents, colors);
            }
            else
            {
                if (pattern.transition) AddPoly(spline.Lines.TransitionPoly, mesh, submesh++, vertices, normals, tangents, colors);

            }
            if (pattern.shrink)
            {
                AddPoly(spline.Lines.ShrinkPoly, mesh, submesh++, vertices, normals, tangents, colors);
            }
            AddPoly(spline.Lines.CorePoly, mesh, submesh++, vertices, normals, tangents, colors);

            meshFilter = spline.Transform.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            meshCollider = spline.Transform.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.sharedMaterial = spline.Layer.PhysicsMaterial;
        }
        /// <summary>
        /// Update mesh material
        /// </summary>
        /// <param name="spline"></param>
        public void UpdateMaterial(SplineBase spline)
        {
            if (meshRenderer)
            {
                MonoBehaviour.DestroyImmediate(meshRenderer);
                meshRenderer = null;
            }

            if (meshRenderer && meshRenderer.sharedMaterials != null)
            {
                meshRenderer.sharedMaterials = null;
            }
            for (int i = 0; i < materials.Count; ++i)
            {
                MonoBehaviour.DestroyImmediate(materials[i]);
                materials[i] = null;
            }

            if (spline.IsHazard) return;

            SplineBase parent = null;
            if (spline.HasParent) parent = spline.Parent;

            Layer cLayer = spline.Layer;
            Layer eLayer = spline.Layer;
            Layer tLayer = spline.Layer;

            if (spline.Pattern.extra)
            {
                eLayer = spline.ExtraLayer;
                if (spline.HasParent) tLayer = parent.Layer;
                else tLayer = eLayer;
            }
            else
            {
                if (spline.HasParent) tLayer = parent.Layer;
            }

            materials.Clear();
            SplineBase.SplinePattern pattern = spline.Pattern;
            if (pattern.extra)
            {
                if (pattern.transition) AddMaterialTransition(materials, tLayer, eLayer);
                AddMaterialCore(materials, eLayer);
                if (pattern.extraTransition) AddMaterialTransition(materials, cLayer, eLayer);
            }
            else
            {
                if (pattern.transition) AddMaterialTransition(materials, tLayer, cLayer);
            }
            if (pattern.shrink)
            {
                AddMaterialCore(materials, cLayer);
            }
            AddMaterialCore(materials, cLayer);

            meshRenderer = spline.Transform.gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = materials.ToArray();
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            needMaterialRefresh = false;
        }
        /// <summary>
        /// Update mesh visibility
        /// </summary>
        /// <param name="spline"></param>
        /// <param name="visible"></param>
        public void UpdateVisibility(SplineBase spline, bool visible)
        {
            if (visible)
            {
                if (meshRenderer) meshRenderer.enabled = true;
            }
            else
            {
                if (meshRenderer) meshRenderer.enabled = false;
            }
        }
        #endregion

        #region Support Methods
        void AddPoly(SplinePolygon poly, Mesh mesh, int submesh, List<Vector3> vertices, List<Vector3> normals, List<Vector4> tangents, List<Color> colors)
        {
            vertices.AddRange(poly.Vertices);
            normals.AddRange(poly.Normals);
            tangents.AddRange(poly.Tangents);
            colors.AddRange(poly.Colors);

            mesh.vertices = vertices.ToArray();
            mesh.normals = normals.ToArray();
            mesh.tangents = tangents.ToArray();
            mesh.colors = colors.ToArray();
            mesh.subMeshCount = submesh + 1;
            mesh.SetTriangles(poly.IndexedTriangles(vertices.Count - poly.Vertices.Count).ToArray(), submesh);

            Terrain terrain = CourseBase.Terrain;
            Vector2[] uv = new Vector2[mesh.vertices.Length];
            Vector2[] uv2 = new Vector2[mesh.vertices.Length];
            for (int i = 0; i < uv.Length; i++) uv[i] = new Vector2(1, 1);
            for (int i = 0; i < uv2.Length; i++) uv2[i] = new Vector2(vertices[i].x / terrain.terrainData.size.x, vertices[i].z / terrain.terrainData.size.z);
            mesh.uv = uv;
            mesh.uv2 = uv2;
            mesh.RecalculateBounds();
        }
        void AddMaterialCore(List<Material> materials, Layer coreLayer)
        {
            Material material = new Material(Shader.Find("Perfect Parallel/Overlay-Core"));

            material.SetColor("CMain", coreLayer.mainColor);
            material.SetColor("CSpecular", coreLayer.specularColor);
            if (coreLayer.diffuse.TileTexture) material.SetTexture("CDiffuse", coreLayer.diffuse.TileTexture);
            if (coreLayer.detail.TileTexture) material.SetTexture("CDetail", coreLayer.detail.TileTexture);
            if (coreLayer.normal.TileTexture) material.SetTexture("CNormal", coreLayer.normal.TileTexture);
            if (coreLayer.detailNormal.TileTexture) material.SetTexture("CDetailNormal", coreLayer.detailNormal.TileTexture);
            material.SetFloat("CDiffuseTile", coreLayer.diffuse.tile);
            material.SetFloat("CDetailTile", coreLayer.detail.tile);
            material.SetFloat("CNormalTile", coreLayer.normal.tile);
            material.SetFloat("CNormalBump", coreLayer.normal.bump);
            material.SetFloat("CDetailNormalTile", coreLayer.detailNormal.tile);
            material.SetFloat("CDetailNormalBump", coreLayer.detailNormal.bump);
            material.SetFloat("CFresnelIntensity", coreLayer.fresnelIntensity);

            materials.Add(material);
        }
        void AddMaterialTransition(List<Material> materials, Layer outerLayer, Layer innerLayer)
        {
            Material material = new Material(Shader.Find("Perfect Parallel/Overlay-Transition"));

            material.SetColor("OMain", outerLayer.mainColor);
            material.SetColor("OSpecular", outerLayer.specularColor);
            if (outerLayer.diffuse.TileTexture) material.SetTexture("ODiffuse", outerLayer.diffuse.TileTexture);
            if (outerLayer.detail.TileTexture) material.SetTexture("ODetail", outerLayer.detail.TileTexture);
            if (outerLayer.normal.TileTexture) material.SetTexture("ONormal", outerLayer.normal.TileTexture);
            if (outerLayer.detailNormal.TileTexture) material.SetTexture("ODetailNormal", outerLayer.detailNormal.TileTexture);
            material.SetFloat("ODiffuseTile", outerLayer.diffuse.tile);
            material.SetFloat("ODetailTile", outerLayer.detail.tile);
            material.SetFloat("ONormalTile", outerLayer.normal.tile);
            material.SetFloat("ONormalBump", outerLayer.normal.bump);
            material.SetFloat("ODetailNormalTile", outerLayer.detailNormal.tile);
            material.SetFloat("ODetailNormalBump", outerLayer.detailNormal.bump);
            material.SetFloat("OFresnelIntensity", outerLayer.fresnelIntensity);

            material.SetColor("IMain", innerLayer.mainColor);
            material.SetColor("ISpecular", innerLayer.specularColor);
            if (innerLayer.diffuse.TileTexture) material.SetTexture("IDiffuse", innerLayer.diffuse.TileTexture);
            if (innerLayer.detail.TileTexture) material.SetTexture("IDetail", innerLayer.detail.TileTexture);
            if (innerLayer.normal.TileTexture) material.SetTexture("INormal", innerLayer.normal.TileTexture);
            if (innerLayer.detailNormal.TileTexture) material.SetTexture("IDetailNormal", innerLayer.detailNormal.TileTexture);
            material.SetFloat("IDiffuseTile", innerLayer.diffuse.tile);
            material.SetFloat("IDetailTile", innerLayer.detail.tile);
            material.SetFloat("INormalTile", innerLayer.normal.tile);
            material.SetFloat("INormalBump", innerLayer.normal.bump);
            material.SetFloat("IDetailNormalTile", innerLayer.detailNormal.tile);
            material.SetFloat("IDetailNormalBump", innerLayer.detailNormal.bump);
            material.SetFloat("IFresnelIntensity", innerLayer.fresnelIntensity);

            materials.Add(material);
        }
        #endregion
    }
}