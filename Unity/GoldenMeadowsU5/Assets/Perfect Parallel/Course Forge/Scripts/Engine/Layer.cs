using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using JsonFx.Json;

namespace PerfectParallel.CourseForge
{
	/// <summary>
	/// Library layer class, contains color, tile, material
	/// shape parameters
	/// </summary>
	[Serializable]
	public class Layer
	{
		public enum FillType
		{
			Grid_Fill,
			Radial_Fill
		}

        /// <summary>
		/// Tile info class, contains texture and tiling
		/// </summary>
		[Serializable]
		public class TileInfo
		{
			#region Public Fields
			public string textureName = "";
			public float tile = 0;
            public float bump = 1;
			#endregion
			#region Private Fields
			[SerializeField]
			protected Texture texture = null;
			#endregion

			#region Constuctors
			/// <summary>
			/// Constructor default
			/// </summary>
			public TileInfo()
			{
			}
			/// <summary>
			/// Tile info constructor from tile
			/// </summary>
			/// <param name="tile"></param>
			public TileInfo(float tile)
			{
				this.tile = tile;
			}
			/// <summary>
			/// Tile info constructor from another Tile info
			/// </summary>
			/// <param name="info"></param>
			public TileInfo(TileInfo info)
			{
				this.textureName = info.textureName;
				this.tile = info.tile;
                this.bump = info.bump;

				this.texture = info.texture;
			}
			#endregion

			#region Properties
			/// <summary>
			/// Tile current texture
            /// </summary>
            [JsonFx.Json.JsonIgnore]
			public Texture TileTexture
			{
				get
				{
					return texture;
				}
				set
				{
					if (texture != value)
					{
						texture = value;

						if (texture != null)
						{
							if (PlatformBase.IO.IsEditor) textureName = PlatformBase.IO.GetFileName(PlatformBase.Editor.ObjectToPath(texture));
						}
						else
						{
							textureName = "";
						}
					}
				}
			}
			/// <summary>
			/// Is tile has texture?
            /// </summary>
            [JsonFx.Json.JsonIgnore]
			public bool HasTexture
			{
				get
				{
					return textureName != "";
				}
			}
			#endregion
		}

		#region Constructors
		public Layer()
		{
		}
		public Layer(Layer layer)
		{
			this.physicMaterial = layer.physicMaterial;
			this.hazardPost = layer.hazardPost;

			this.physicMaterialGUID = layer.physicMaterialGUID;
			this.hazardPost = layer.hazardPost;
			this.name = layer.name;
			this.r = layer.r;
			this.g = layer.g;
			this.b = layer.b;
			this.a = layer.a;
			this.mainColor = new ColorObject(layer.mainColor.ToColor());
			this.specularColor = new ColorObject(layer.specularColor.ToColor());
            this.hazardColor = new ColorObject(layer.hazardColor.ToColor());
            this.diffuse = new Layer.TileInfo(layer.diffuse);
            this.detail = new Layer.TileInfo(layer.detail);
            this.normal = new Layer.TileInfo(layer.normal);
            this.detailNormal = new Layer.TileInfo(layer.detailNormal);
			this.fresnelIntensity = layer.fresnelIntensity;

			this.metersPerOnePoint = layer.metersPerOnePoint;
			this.metersPerOnePost = layer.metersPerOnePost;
			this.pointsPerEdge = layer.pointsPerEdge;
			this.resolution = layer.resolution;
			this.pattern = new SplineBase.SplinePattern(layer.pattern);
		}
		#endregion

		#region Fields
		[SerializeField]
		protected PhysicMaterial physicMaterial = null;
		[SerializeField]
		protected GameObject hazardPost = null;

		public string physicMaterialGUID = "";
		public string hazardPostGUID = "";
		public string name = "Unknown";
		public float r = 1, g = 1, b = 1, a = 1;
		public ColorObject mainColor = new ColorObject(Color.white);
		public ColorObject specularColor = new ColorObject(Color.white);
        public ColorObject hazardColor = new ColorObject(Color.white);
        public TileInfo diffuse = new TileInfo(0);
        public TileInfo detail = new TileInfo(100);
        public TileInfo normal = new TileInfo(0);
        public TileInfo detailNormal = new TileInfo(0);
		public float fresnelIntensity = 0.25f;

		public float metersPerOnePoint = 8;
		public float metersPerOnePost = 8;
		public int pointsPerEdge = 4;
		public float resolution = 5;
		public FillType fillType = FillType.Grid_Fill;
		public SplineBase.SplinePattern pattern = new SplineBase.SplinePattern();
		#endregion

		#region Properties
		/// <summary>
		/// Layer color
		/// </summary>
		[JsonIgnore]
		public Color LayerColor
		{
			get
			{
				return new Color(r, g, b, a);
			}
			set
			{
				r = value.r;
				g = value.g;
				b = value.b;
				a = value.a;
			}
		}
		/// <summary>
		/// Layer physics material
		/// </summary>
		[JsonIgnore]
		public PhysicMaterial PhysicsMaterial
		{
			get
			{
				return physicMaterial;
			}
			set
			{
				if (physicMaterial != value)
				{
					physicMaterial = value;

                    if (physicMaterial != null)
                    {
                        if (PlatformBase.IO.IsEditor) physicMaterialGUID = PlatformBase.Editor.ObjectToGUID(physicMaterial);
                    }
                    else physicMaterialGUID = "";
				}
			}
		}
		/// <summary>
		/// Hazard post gameObject
		/// </summary>
		[JsonIgnore]
		public GameObject HazardPost
		{
			get
			{
				return hazardPost;
			}
			set
			{
				if (hazardPost != value)
				{
					hazardPost = value;

                    if (hazardPost != null)
                    {
                         if (PlatformBase.IO.IsEditor) hazardPostGUID = PlatformBase.Editor.ObjectToGUID(hazardPost);
                    }
                    else hazardPostGUID = "";
				}
			}
		}

		/// <summary>
		/// Is layer bunker?
		/// </summary>
		[JsonIgnore]
		public bool IsBunker
		{
			get
			{
				return name.ToLower().Contains("bunker");
			}
		}
		#endregion
	}
}