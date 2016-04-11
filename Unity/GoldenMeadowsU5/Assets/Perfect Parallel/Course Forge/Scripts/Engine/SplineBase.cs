using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
    /// <summary>
    /// Spline class
    /// </summary>
    [AddComponentMenu("Perfect Parallel/Course Forge/Spline/Spline")]
    public class SplineBase : MonoBehaviour
    {
        /// <summary>
        /// Info class
        /// </summary>
        [Serializable]
        public class SplineInfo
        {
            public enum Flags
            {
                Hole = 1,
                Square = 2,
            }

            #region Fields
            public int activePoint = -1;

            public int layerIndex = -1;
            public string layerName = "NoLayerName";
            public int flags = 0;
            public bool pin = false;
            public int InstanceID = -1;
            public Vector3Object[] points = new Vector3Object[0];
            public int[] colorIndex = new int[0];
            public string[] colorNames = new string[0];
            public bool locked = false;
            #endregion
        }

        /// <summary>
        /// Spline shape container
        /// </summary>
        [Serializable]
        public class SplinePattern
        {
            #region Fields
            public bool transition = true;
            public float transitionLength = 0.1f;
            public float transitionDepth = 0.0f;

            public bool extra = false;
            public int extraLayerIndex = -1;
            public string extraLayerName = "NoLayerName";
            public float extraLength = 0.1f;
            public float extraDepth = 0.0f;

            public bool extraTransition = true;
            public float extraTransitionLength = 0.1f;
            public float extraTransitionDepth = 0.0f;

            public bool shrink = false;
            public float shrinkLength = 0.1f;
            public float shrinkDepth = 0.0f;
            #endregion

            #region Constuctors
            /// <summary>
            /// Constructor default
            /// </summary>
            public SplinePattern()
            {
            }
            /// <summary>
            /// Constructor from another instance
            /// </summary>
            /// <param name="pattern"></param>
            public SplinePattern(SplinePattern pattern)
            {
                this.transition = pattern.transition;
                this.transitionLength = pattern.transitionLength;
                this.transitionDepth = pattern.transitionDepth;

                this.extra = pattern.extra;
                this.extraLayerIndex = pattern.extraLayerIndex;
                this.extraLayerName = pattern.extraLayerName;
                this.extraLength = pattern.extraLength;
                this.extraDepth = pattern.extraDepth;

                this.extraTransition = pattern.extraTransition;
                this.extraTransitionLength = pattern.extraTransitionLength;
                this.extraTransitionDepth = pattern.extraTransitionDepth;

                this.shrink = pattern.shrink;
                this.shrinkLength = pattern.shrinkLength;
                this.shrinkDepth = pattern.shrinkDepth;
            }
            #endregion
        }

        /// <summary>
        /// Binary heightmap container
        /// </summary>
        [Serializable]
        public class BinaryHeightmap
        {
            #region Fields
            public int fileSize = 0;
            public int gridSizeX = 0;
            public int gridSizeY = 0;
            public double cellSizeX = 0;
            public double cellSizeY = 0;
            public double spreadZ = 0;
            public double nullValue = 0;
            public double gridModelCount = 0;
            public double polyCount = 0;
            public List<int> polys = new List<int>();

            public List<Vector2> line = new List<Vector2>();
            public Texture2D texture = null;
            public Vector3 offset = new Vector3(0, 2, 0);
            public float rotation = 0;
            #endregion

            #region Properties
            /// <summary>
            /// Size along XZ axis
            /// </summary>
            public Vector2 SizeXY
            {
                get
                {
                    return new Vector2((float)cellSizeX * (float)gridSizeX, (float)cellSizeY * (float)gridSizeY);
                }
            }
            /// <summary>
            /// Heightmap size
            /// </summary>
            public Vector3 Size
            {
                get
                {
                    return new Vector3((float)cellSizeX * (float)gridSizeX, (float)spreadZ, (float)cellSizeY * (float)gridSizeY);
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Read from a file
            /// </summary>
            /// <param name="path"></param>
            public void Read(string path)
            {
                /*
				FileStream file = new FileStream(path, FileMode.Open);
				BinaryReader br = new BinaryReader(file);

				br.ReadBytes(4);
				fileSize = br.ReadInt32();
				gridSizeX = br.ReadInt32();
				gridSizeY = br.ReadInt32();
				cellSizeX = br.ReadDouble();
				cellSizeY = br.ReadDouble();
				spreadZ = br.ReadDouble();
				nullValue = br.ReadDouble();
				gridModelCount = br.ReadInt32();
				polyCount = br.ReadInt32();

				polys.Clear();
				for (int i = 0; i < polyCount; ++i)
					polys.Add(br.ReadInt32());

           
				//   0 1 2 3
				//  ________
				//0| 0 1 2 3
				//1| 4 5 6 7 
				//2| 8 9 0 0
				//3| 0 0 0 0 
				//4| 0 0 0 19 
            
            

				for (int i = 0; i < polyCount; ++i)
				{
					int iy = polys[i] / gridSizeX;
					int ix = polys[i] % gridSizeX;

					float fx = (float)ix / (float)gridSizeX;
					float fy = (float)iy / (float)gridSizeY;
					Vector2 pos01 = new Vector2(fx, fy);

					line.Add(pos01);
				}

				byte[] pngData = br.ReadBytes(fileSize);

				texture = new Texture2D(4096, 4096);
				texture.LoadImage(pngData);
				*/
            }
            #endregion
        }

        #region Fields
        [SerializeField]
        SplineInfo splineInfo = new SplineInfo();
        [SerializeField]
        SplineRenderer splineRenderer = new SplineRenderer();
        [SerializeField]
        SplineLines splineLines = new SplineLines();
        [SerializeField]
        BinaryHeightmap splineHeightmap = new BinaryHeightmap();
        #endregion

        #region Spline properties
        /// <summary>
        /// Spline points
        /// </summary>
        public Vector3[] Points
        {
            get
            {
                return splineLines.Points;
            }
            set
            {
                splineLines.Points = value;
            }
        }

        /// <summary>
        /// Spline layer object
        /// </summary>
        public virtual Layer Layer
        {
            get
            {
                return CourseBase.GetLayer(splineInfo.layerName, CourseBase.SplineLayers);
            }
            set
            {
                splineInfo.layerName = value.name;
            }
        }
        /// <summary>
        /// Extra layer
        /// </summary>
        public Layer ExtraLayer
        {
            get
            {
                return CourseBase.GetLayer(Pattern.extraLayerName, CourseBase.SplineLayers);
            }
        }
        /// <summary>
        /// Bitmask of flags
        /// </summary>
        public SplineInfo.Flags Flags
        {
            get
            {
                return (SplineInfo.Flags)splineInfo.flags;
            }
        }

        /// <summary>
        /// Is spline is a hole-spline?
        /// </summary>
        public bool IsHole
        {
            get
            {
                return (Flags & SplineInfo.Flags.Hole) != 0;
            }
        }
        /// <summary>
        /// Is mesh squared? (i.e. with sharp edges)
        /// </summary>
        public bool IsSquare
        {
            get
            {
                return (Flags & SplineInfo.Flags.Square) != 0;
            }
        }
        /// <summary>
        /// Is hazard?
        /// </summary>
        public virtual bool IsHazard
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// Is pin layer?
        /// </summary>
        public bool IsPin
        {
            get
            {
                return Layer == CourseBase.GetPinLayer();
            }
        }
        /// <summary>
        /// Is pin lip layer?
        /// </summary>
        public bool IsPinLip
        {
            get
            {
                return Layer == CourseBase.GetPinLipLayer();
            }
        }
        #endregion

        #region Transform Properties
        /// <summary>
        /// This gameObject transform
        /// </summary>
        public Transform Transform
        {
            get
            {
                return transform;
            }
        }
        /// <summary>
        /// Spline parent object
        /// </summary>
        public virtual SplineBase Parent
        {
            get
            {
                if (transform.parent) return transform.parent.GetComponent<SplineBase>();
                return null;
            }
        }

        /// <summary>
        /// Does spline have parent?
        /// </summary>
        public virtual bool HasParent
        {
            get
            {
                return transform.parent != null;
            }
        }
        /// <summary>
        /// Does spline have childs?
        /// </summary>
        public virtual bool HasChilds
        {
            get
            {
                return transform.childCount != 0;
            }
        }

        /// <summary>
        /// Spline starting offset on up axis
        /// </summary>
        public float Offset
        {
            get
            {
                if (HasParent)
                {
                    SplineBase parent = Parent;
                    float depth = parent.Depth;
                    return parent.Offset + depth;
                }
                else
                {
                    return CourseBase.Info.offset;
                }
            }
        }
        /// <summary>
        /// Death of this spline based on the pattern
        /// </summary>
        public float Depth
        {
            get
            {
                SplineBase.SplinePattern pattern = Pattern;
                float depth = 0;
                if (pattern.transition) depth += pattern.transitionDepth;
                if (pattern.extra)
                {
                    depth += pattern.extraDepth;
                    if (pattern.extraTransition) depth += pattern.extraTransitionDepth;
                }
                if (pattern.shrink)
                {
                    depth += pattern.shrinkDepth;
                }
                return depth;
            }
        }
        /// <summary>
        /// Minimum distance to child splines
        /// </summary>
        public float MinDistanceToChilds
        {
            get
            {
                List<Vector3> splineLine = splineLines.InnerSplineData.points;

                float min = 0;
                for (int i = 0; i < Transform.childCount; ++i)
                {
                    SplineBase child = Transform.GetChild(i).GetComponent<SplineBase>();
                    List<Vector3> childLine = child.Lines.OuterSplineData.points;

                    for (int j = 0; j < childLine.Count; ++j)
                    {
                        float distance = childLine[j].MinDistance(splineLine);
                        if (distance < min) min = distance;
                    }
                }

                return min;
            }
        }
        #endregion

        #region Accessors Properties
        /// <summary>
        /// Spline selected point
        /// </summary>
        public int ActivePoint
        {
            get
            {
                return splineInfo.activePoint;
            }
            set
            {
                splineInfo.activePoint = value;
            }
        }
        /// <summary>
        /// Spline info object
        /// </summary>
        public SplineInfo Info
        {
            get
            {
                return splineInfo;
            }
        }
        /// <summary>
        /// Spline renderer
        /// </summary>
        public SplineRenderer Renderer
        {
            get
            {
                return splineRenderer;
            }
        }
        /// <summary>
        /// Spline lines object
        /// </summary>
        public SplineLines Lines
        {
            get
            {
                return splineLines;
            }
        }
        /// <summary>
        /// Spline shape pattern
        /// </summary>
        public SplineBase.SplinePattern Pattern
        {
            get
            {
                return Layer.pattern;
            }
        }
        /// <summary>
        /// Binary heightmap object
        /// </summary>
        public BinaryHeightmap Heightmap
        {
            get
            {
                return splineHeightmap;
            }
            set
            {
                splineHeightmap = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Spline line changed - mark for update
        /// </summary>
        public void LineChanged()
        {
            if (HasParent) Parent.SetRefresh();
            SetRefreshRecursive(this);
        }
        /// <summary>
        /// Set spline refresh flag
        /// </summary>
        public void SetRefresh()
        {
            if (splineInfo.locked) return;

            splineLines.NeedMeshRefresh = true;
            splineRenderer.NeedMaterialRefresh = true;

            if (PlatformBase.IO.IsEditor) PlatformBase.Editor.SetDirty(this);
        }
        /// <summary>
        /// Update spline line data
        /// </summary>
        public void UpdateLine()
        {
            splineLines.UpdateLines(this);

            if (splineLines.Points.Length != splineInfo.points.Length)
            {
                splineInfo.points = new Vector3Object[splineLines.Points.Length];

                for (int i = 0; i < splineInfo.points.Length; ++i)
                    splineInfo.points[i] = new Vector3Object(splineLines.Points[i]);
            }
            else
            {
                for (int i = 0; i < splineLines.Points.Length; ++i)
                    splineInfo.points[i].Set(splineLines.Points[i]);
            }
        }
        /// <summary>
        /// Form spline lines
        /// </summary>
        public void FormLines()
        {
            splineLines.FormLines(this);
        }
        /// <summary>
        /// Update spline mesh
        /// </summary>
        public void UpdateMesh()
        {
            splineLines.UpdateMesh(this);
        }
        /// <summary>
        /// Update spline material
        /// </summary>
        public virtual void UpdateMaterial()
        {
            splineRenderer.UpdateMesh(this);
            splineRenderer.UpdateMaterial(this);
        }

        /// <summary>
        /// Is point in bounds?
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool InBounds(Vector3 point)
        {
            float x = point.x;
            float y = point.z;
            Vector3 splineMin = Lines.MinPoint;
            Vector3 splineMax = Lines.MaxPoint;
            if (x < splineMin.x || x > splineMax.x || y < splineMin.z || y > splineMax.z) return false;
            return true;
        }
        #endregion

        #region Support Methods
        static void SetRefreshRecursive(SplineBase spline)
        {
            if (spline == null)
            {
                Debug.LogException(new NullReferenceException("Null spline"));
                return;
            }

            spline.SetRefresh();

            for (int i = 0; i < spline.Transform.childCount; ++i)
            {
                SplineBase child = spline.Transform.GetChild(i).GetComponent<SplineBase>();
                if (child) SetRefreshRecursive(child);
            }
        }
        #endregion
    }
}