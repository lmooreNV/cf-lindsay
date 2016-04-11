using UnityEngine;
using System.Reflection;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Course forge terrain class, we need this class to apply fresnel shaders
    /// to our terrain and to apply our shader system globals
    /// </summary>
    [ExecuteInEditMode]
    [Obfuscation(Exclude = true)]
    [AddComponentMenu("GameObject/Hidden/Perfect Parallel/Course Forge/Terrain")]
    public class TerrainControllerBase : MonoBehaviour
    {
        #region Public Fields
        public Texture terrainTexture = null;
        public float fresnelIntensity = 0.1f;
        #endregion
        #region Private Fields
        Terrain terrain = null;
        #endregion

        #region Properties
        /// <summary>
        /// Terrain object
        /// </summary>
        public Terrain Terrain
        {
            get
            {
                return terrain;
            }
            set
            {
                terrain = value;
            }
        }
        #endregion

        #region Base Methods
        void OnEnable()
        {
            terrain = GetComponent<Terrain>();
        }
        void LateUpdate()
        {
            if (terrain && terrain.terrainData)
            {
                Shader.SetGlobalVector("TerrainInfo", new Vector4(terrain.terrainData.size.x, terrain.terrainData.size.y, terrain.terrainData.size.z, 0.0f));
            }
            if (terrainTexture)
            {
                Shader.SetGlobalTexture("TerrainTexture", terrainTexture);
            }
        }
        #endregion
    }
}