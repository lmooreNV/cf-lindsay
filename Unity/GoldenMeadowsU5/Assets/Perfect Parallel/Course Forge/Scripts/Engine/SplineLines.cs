using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Poly2Tri;
using ClipperLib;

namespace PerfectParallel.CourseForge
{
	/// <summary>
	/// Class do all of the line-based
	/// logics for Spline class
	/// </summary>
	[Serializable]
    public class SplineLines
	{
		/// <summary>
		/// Line container
		/// </summary>
		[Serializable]
        public class Line
		{
			#region Fields
			public List<Vector3> points = new List<Vector3>();
            public List<PolygonPoint> polyPoints = new List<PolygonPoint>();
            public List<Vector3> normals = new List<Vector3>();
            public List<Vector4> tangents = new List<Vector4>();
			public List<Color> colors = new List<Color>();
			#endregion

			#region Methods
			/// <summary>
			/// Clear the class
			/// </summary>
			public void Clear()
			{
				points.Clear();
				polyPoints.Clear();
				normals.Clear();
                tangents.Clear();
				colors.Clear();
			}
			/// <summary>
			/// Add point/normal/color to line
			/// </summary>
			/// <param name="point"></param>
			/// <param name="normal"></param>
			/// <param name="color"></param>
			public void Add(Vector3 point, Vector3 normal, Color color)
			{
                Vector4 tangent = -Vector3.Cross(normal, Vector3.up);
                if (normal == Vector3.up) tangent = -Vector3.right;
                tangent.w = -1;

				points.Add(point);
				polyPoints.Add(new PolygonPoint(point.x, point.z));
				normals.Add(normal);
                tangents.Add(tangent);
				colors.Add(color);
			}
			/// <summary>
			/// Add another line data to this
			/// </summary>
			/// <param name="line"></param>
			public void Add(Line line)
			{
				points.AddRange(line.points);
				polyPoints.AddRange(line.polyPoints);
				normals.AddRange(line.normals);
                tangents.AddRange(line.tangents);
				colors.AddRange(line.colors);
			}
			/// <summary>
			/// Add another line data with different color
			/// </summary>
			/// <param name="line"></param>
			/// <param name="color"></param>
			public void Add(Line line, Color color)
			{
				points.AddRange(line.points);
				polyPoints.AddRange(line.polyPoints);
                normals.AddRange(line.normals);
                tangents.AddRange(line.tangents);
				for (int i = 0; i < line.colors.Count; ++i) colors.Add(color);
			}
			/// <summary>
			/// Remove points from line
			/// </summary>
			/// <param name="index"></param>
			/// <param name="count"></param>
			public void RemoveRange(int index, int count)
			{
				points.RemoveRange(index, count);
				polyPoints.RemoveRange(index, count);
                normals.RemoveRange(index, count);
                tangents.RemoveRange(index, count);
				colors.RemoveRange(index, count);
			}
			/// <summary>
			/// Update the points before baking mesh
			/// </summary>
			public void Update()
			{
				polyPoints.Clear();
				for (int i = 0; i < points.Count; ++i) polyPoints.Add(new PolygonPoint(points[i].x, points[i].z));
			}
			#endregion
		}

		#region Fields
		[SerializeField]
		Vector3[] editorPoints = new Vector3[0];
		[SerializeField]
		Color[] editorColors = new Color[0];
		[SerializeField]
		Vector3[] editorLinesPoints = new Vector3[0];
		[SerializeField]
		Vector3[] editorBoxesPoints = new Vector3[0];
		[SerializeField]
		Color[] editorLinesColors = new Color[0];
		[SerializeField]
		Vector3 editorMin = Vector3.zero;
		[SerializeField]
		Vector3 editorMax = Vector3.zero;

		[SerializeField]
		Line outerLine = new Line();
		[SerializeField]
		Line extraOuterLine = new Line();
		[SerializeField]
		Line extraInnerLine = new Line();
		[SerializeField]
		Line shrinkLine = new Line();
		[SerializeField]
		Line innerLine = new Line();

		[SerializeField]
		SplinePolygon transitionPoly = new SplinePolygon();
		[SerializeField]
		SplinePolygon extraPoly = new SplinePolygon();
		[SerializeField]
		SplinePolygon extraTransitionPoly = new SplinePolygon();
		[SerializeField]
		SplinePolygon shrinkPoly = new SplinePolygon();
		[SerializeField]
		SplinePolygon corePoly = new SplinePolygon();

		[SerializeField]
		bool needMeshRefresh = false;
		#endregion

		#region Properties
		/// <summary>
		/// Editor control points
		/// </summary>
		public Vector3[] Points
		{
			get
			{
				return editorPoints;
			}
			set
			{
				editorPoints = value;
			}
		}
		/// <summary>
		/// Points colors
		/// </summary>
		public Color[] Colors
		{
			get
			{
				return editorColors;
			}
		}
		/// <summary>
		/// Editor smooth lines
		/// </summary>
		public Vector3[] Lines
		{
			get
			{
				return editorLinesPoints;
			}
		}
		/// <summary>
		/// Lines colors
		/// </summary>
		public Color[] LinesColors
		{
			get
			{
				return editorLinesColors;
			}
		}
		/// <summary>
		/// Editor boxes
		/// </summary>
		public Vector3[] Boxes
		{
			get
			{
				return editorBoxesPoints;
			}
		}
		/// <summary>
		/// Spline bounds min point
		/// </summary>
		public Vector3 MinPoint
		{
			get
			{
				return editorMin;
			}
		}
		/// <summary>
		/// Spline bounds max point
		/// </summary>
		public Vector3 MaxPoint
		{
			get
			{
				return editorMax;
			}
		}
		/// <summary>
		/// Spline bounds min point (scaled by 101%)
		/// </summary>
		public Vector3 RealMinPoint
		{
			get
			{
				return editorMin * 0.95f;
			}
		}
		/// <summary>
		/// Spline bounds max point (scaled by 101%)
		/// </summary>
		public Vector3 RealMaxPoint
		{
			get
			{
                return editorMax * 1.05f;
			}
		}

		/// <summary>
		/// Most outer spline line data
        /// </summary>
        public Line OuterSplineData
        {
            get
            {
                return outerLine;
            }
        }
        /// <summary>
        /// Extra outer spline line data
        /// </summary>
        public Line ExtraOuterLineData
        {
            get
            {
                return extraOuterLine;
            }
        }
        /// <summary>
        /// Extra inner spline line data
        /// </summary>
        public Line ExtraInnerLineData
        {
            get
            {
                return extraInnerLine;
            }
        }
        /// <summary>
        /// Shrink spline line data
        /// </summary>
        public Line ShrinkLineData
        {
            get
            {
                return shrinkLine;
            }
        }
		/// <summary>
		/// Most inner spline line data
		/// </summary>
		public Line InnerSplineData
		{
			get
			{
				return innerLine;
			}
		}

		/// <summary>
		/// Outer transition polygon
		/// </summary>
		public SplinePolygon TransitionPoly
		{
			get
			{
				return transitionPoly;
			}
		}
		/// <summary>
		/// Fringe polygon
		/// </summary>
		public SplinePolygon ExtraPoly
		{
			get
			{
				return extraPoly;
			}
		}
		/// <summary>
		/// Fringe transition polygon
		/// </summary>
		public SplinePolygon ExtraTransitionPoly
		{
			get
			{
				return extraTransitionPoly;
			}
		}
		/// <summary>
		/// Shrink polygon
		/// </summary>
		public SplinePolygon ShrinkPoly
		{
			get
			{
				return shrinkPoly;
			}
		}
		/// <summary>
		/// Core polygon
		/// </summary>
		public SplinePolygon CorePoly
		{
			get
			{
				return corePoly;
			}
		}
		/// <summary>
		/// Is line data needed update?
		/// </summary>
		public bool NeedMeshRefresh
		{
			get
			{
				return needMeshRefresh;
			}
			set
			{
				needMeshRefresh = value;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Update the lines
		/// </summary>
		/// <param name="spline"></param>
		public void UpdateLines(SplineBase spline)
		{
			if (spline.IsSquare)
			{
				List<Vector3> temp = editorPoints.ToList();
				temp.Add(temp[0]);
				editorLinesPoints = temp.ToArray();
			}
			else
			{
				editorLinesPoints = editorPoints.CatmullRom(spline.Layer.pointsPerEdge, true).ToArray();
			}
			if (spline.IsHazard)
			{
				editorLinesColors = new Color[editorLinesPoints.Length];

                string[] colorNames = spline.Info.colorNames;
                editorColors = new Color[colorNames.Length];
                for (int i = 0; i < colorNames.Length; ++i) editorColors[i] = CourseBase.GetLayer(colorNames[i], CourseBase.HazardLayers).hazardColor;

				int step = (int)(editorLinesPoints.Length / editorPoints.Length);

				int index = 0;
				int counter = 0;
				for (int i = 0; i < editorLinesPoints.Length; ++i)
				{
                    try
                    {
                        editorLinesColors[i] = editorColors[index];
                        counter++;
                        if (counter == step)
                        {
                            counter = 0;
                            if (index != editorPoints.Length - 1) index++;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(editorColors.Length + "[" + index + "]");
                        Debug.LogException(e);
                    }
				}
			}

			editorBoxesPoints = new Vector3[editorPoints.Length * 9];
			for (int i = 0; i < editorPoints.Length; ++i)
			{
				editorBoxesPoints[i * 9 + 0].Set(editorPoints[i].x - Utility.squareHalf, editorPoints[i].y, editorPoints[i].z - Utility.squareHalf);
				editorBoxesPoints[i * 9 + 1].Set(editorPoints[i].x + Utility.squareHalf, editorPoints[i].y, editorPoints[i].z - Utility.squareHalf);
				editorBoxesPoints[i * 9 + 2].Set(editorPoints[i].x - Utility.squareHalf, editorPoints[i].y, editorPoints[i].z - Utility.squareHalf);
				editorBoxesPoints[i * 9 + 3].Set(editorPoints[i].x - Utility.squareHalf, editorPoints[i].y, editorPoints[i].z + Utility.squareHalf);
				editorBoxesPoints[i * 9 + 4].Set(editorPoints[i].x - Utility.squareHalf, editorPoints[i].y, editorPoints[i].z + Utility.squareHalf);
				editorBoxesPoints[i * 9 + 5].Set(editorPoints[i].x + Utility.squareHalf, editorPoints[i].y, editorPoints[i].z + Utility.squareHalf);
				editorBoxesPoints[i * 9 + 6].Set(editorPoints[i].x + Utility.squareHalf, editorPoints[i].y, editorPoints[i].z - Utility.squareHalf);
				editorBoxesPoints[i * 9 + 7].Set(editorPoints[i].x + Utility.squareHalf, editorPoints[i].y, editorPoints[i].z + Utility.squareHalf);
				editorBoxesPoints[i * 9 + 8].Set(0, float.PositiveInfinity, 0);
			}

			editorMin = editorLinesPoints[0];
			editorMax = editorLinesPoints[0];
			for (int i = 0; i < editorLinesPoints.Length; ++i)
			{
				if (editorLinesPoints[i].x < editorMin.x) editorMin.x = editorLinesPoints[i].x;
				if (editorLinesPoints[i].y < editorMin.y) editorMin.y = editorLinesPoints[i].y;
				if (editorLinesPoints[i].z < editorMin.z) editorMin.z = editorLinesPoints[i].z;

				if (editorLinesPoints[i].x > editorMax.x) editorMax.x = editorLinesPoints[i].x;
				if (editorLinesPoints[i].y > editorMax.y) editorMax.y = editorLinesPoints[i].y;
				if (editorLinesPoints[i].z > editorMax.z) editorMax.z = editorLinesPoints[i].z;
			}
			editorMin /= 1.01f;
			editorMax *= 1.01f;
		}
		/// <summary>
		/// Update line data, fill the containers
		/// </summary>
		/// <param name="spline"></param>
		public void FormLines(SplineBase spline)
		{
			outerLine.Clear();
			extraOuterLine.Clear();
			extraInnerLine.Clear();
			shrinkLine.Clear();
			innerLine.Clear();

			bool isHazard = spline.IsHazard;
			bool hasParent = spline.HasParent;
			SplineBase.SplinePattern pattern = spline.Pattern;

			Color outerColor = new Color(0, 0, 0, hasParent ? 1 : 0);
			Color extraInnerColor = new Color(1, 0, 0, 1);
			Color extraOuterColor = new Color(1, 0, 0, 1);
			Color shrinkColor = new Color(pattern.extra ? 0 : 1, 0, 0, spline.IsHole ? 0.01f : 1);
			Color innerColor = new Color(pattern.extra ? 0 : 1, 0, 0, spline.IsHole ? 0.01f : 1);

			float baseOffset = spline.Offset;
			Vector3[] points = editorLinesPoints;

			if (isHazard)
			{
				baseOffset += 0.01f;
			}

			for (int i = 0; i < points.Length - 1; ++i)
			{
				float offset = baseOffset;

				if (isHazard)
				{
					outerColor = editorLinesColors[i];
					innerColor = editorLinesColors[i];
				}

				Vector2 point = new Vector2(points[i].x, points[i].z);
				Vector2 normalVector = GetNormalAtIndex(points, i);

				Vector2 outerPoint2 = point + (pattern.transition ? (pattern.transitionLength * (isHazard ? 0.5f : 1.0f)) * normalVector : Vector2.zero);
				Vector2 extraOuterPoint2 = outerPoint2 - (pattern.extra ? (pattern.transition ? pattern.transitionLength * normalVector : Vector2.zero) : Vector2.zero);
				Vector2 extraInnerPoint2 = extraOuterPoint2 - (pattern.extra ? pattern.extraLength * normalVector : Vector2.zero);
				Vector2 shrinkPoint2 = extraInnerPoint2 - (pattern.extra ? (pattern.extraTransition ? pattern.extraTransitionLength * normalVector : Vector2.zero) : (pattern.transition ? pattern.transitionLength * normalVector : Vector2.zero));
				Vector2 innerPoint2 = shrinkPoint2 - (pattern.shrink ? pattern.shrinkLength * normalVector : Vector2.zero);

				if (!isHazard && !hasParent) offset *= 0.001f;
				Vector3 outerPoint3 = GetPosition(outerPoint2, offset);
				if (!isHazard && !hasParent) offset *= 1000;
				if (pattern.transition) offset += pattern.transitionDepth;

				Vector3 extraOuterPoint3 = GetPosition(extraOuterPoint2, offset); if (pattern.extra) offset += pattern.extraDepth;
				Vector3 extraInnerPoint3 = GetPosition(extraInnerPoint2, offset); if (pattern.extra && pattern.extraTransition) offset += pattern.extraTransitionDepth;
				Vector3 shrinkPoint3 = GetPosition(shrinkPoint2, offset); if (pattern.shrink) offset += pattern.shrinkDepth;
				Vector3 innerPoint3 = GetPosition(innerPoint2, offset);

                outerLine.Add(outerPoint3, GetNormal(outerPoint3, Vector3.zero, Vector3.zero, outerPoint2), outerColor);
				if (pattern.extra)
				{
                    extraOuterLine.Add(extraOuterPoint3, GetNormal(extraOuterPoint3, (pattern.transition ? outerPoint3 : Vector3.zero), extraInnerPoint3, extraOuterPoint2, spline.Layer.IsBunker), extraOuterColor);
                    extraInnerLine.Add(extraInnerPoint3, GetNormal(extraInnerPoint3, extraOuterPoint3, (pattern.extraTransition ? (pattern.shrink ? shrinkPoint3 : innerPoint3) : Vector3.zero), extraInnerPoint2, spline.Layer.IsBunker), extraInnerColor);
				}
				if (pattern.shrink)
				{
                    shrinkLine.Add(shrinkPoint3, GetNormal(shrinkPoint3, (pattern.extra && pattern.extraTransition ? extraInnerPoint3 : (pattern.transition ? outerPoint3 : Vector3.zero)), innerPoint3, shrinkPoint2), shrinkColor);
				}
                innerLine.Add(innerPoint3, GetNormal(innerPoint3, Vector3.zero, Vector3.zero, innerPoint2), innerColor);
			}
		}
		/// <summary>
		/// Uppdate mesh data, create mesh
		/// </summary>
		/// <param name="spline"></param>
		public void UpdateMesh(SplineBase spline)
		{
			outerLine.Update();
			extraOuterLine.Update();
			extraInnerLine.Update();
			shrinkLine.Update();
			innerLine.Update();

			transitionPoly.Clear();
			extraPoly.Clear();
			extraTransitionPoly.Clear();
			shrinkPoly.Clear();
			corePoly.Clear();

			SplineBase.SplinePattern pattern = spline.Pattern;
			if (pattern.extra)
			{
				if (pattern.transition) transitionPoly.AddSimple(outerLine, extraOuterLine);

				extraPoly.AddSimple(extraOuterLine, extraInnerLine);

				if (pattern.shrink)
				{
					if (pattern.extraTransition) extraTransitionPoly.AddSimple(extraInnerLine, shrinkLine);
					shrinkPoly.AddSimple(shrinkLine, innerLine);
				}
				else
				{
					if (pattern.extraTransition) extraTransitionPoly.AddSimple(extraInnerLine, innerLine);
                }
			}
			else
			{
				if (pattern.shrink)
				{
					if (pattern.transition) transitionPoly.AddSimple(outerLine, shrinkLine);
					shrinkPoly.AddSimple(shrinkLine, innerLine);
				}
				else
				{
					if (pattern.transition) transitionPoly.AddSimple(outerLine, innerLine);
				}
			}

			if (!spline.IsHazard)
			{
				corePoly.AddPolygon(innerLine);

				Layer layer = spline.Layer;
				if (layer.fillType == Layer.FillType.Grid_Fill)
				{
					corePoly.AddSteiner(GridFill(spline, innerLine));

					for (int i = 0; i < spline.Transform.childCount; ++i)
					{
						SplineBase child = spline.Transform.GetChild(i).GetComponent<SplineBase>();
						child.Lines.outerLine.Update();
						corePoly.AddChildAsHole(child.Lines.outerLine);
					}
				}
				if (layer.fillType == Layer.FillType.Radial_Fill)
				{
					List<Line> xy = RadialFill(spline);
					for (int i = 0; i < xy.Count; ++i) corePoly.AddSteiner(xy[i]);

					for (int i = 0; i < spline.Transform.childCount; ++i)
					{
						SplineBase child = spline.Transform.GetChild(i).GetComponent<SplineBase>();
						child.Lines.outerLine.Update();
						corePoly.AddChildAsHole(child.Lines.outerLine);
					}
				}
			}

			if (transitionPoly.IsInUse) transitionPoly.Triangulate();
			if (extraPoly.IsInUse) extraPoly.Triangulate();
			if (extraTransitionPoly.IsInUse) extraTransitionPoly.Triangulate();
			if (shrinkPoly.IsInUse) shrinkPoly.Triangulate();
			if (corePoly.IsInUse) corePoly.Triangulate();

			needMeshRefresh = false;
		}
		#endregion

		#region Support Methods
		Vector3 GetPosition(Vector2 position, float offset)
		{
			float height = CourseBase.TerrainHeight(position.x, position.y);
            return new Vector3(position.x, height + offset, position.y);
		}
        Vector3 GetNormal(Vector3 middle, Vector3 left, Vector3 right, Vector2 position, bool blendNormals = false)
		{
			Vector3 n = Vector3.zero;
			Vector3 nl = Vector3.zero;
			Vector3 nr = Vector3.zero;
            Vector3 nm = CourseBase.TerrainNormal(middle);

			if (right != Vector3.zero)
			{
				nr = (middle - right).normalized;
				nr = Quaternion.AngleAxis(90, Vector3.Cross(new Vector3(nr.x, 0, nr.z).normalized, Vector3.up)) * nr;
			}
			if (left != Vector3.zero)
			{
				nl = (left - middle).normalized;
				nl = Quaternion.AngleAxis(90, Vector3.Cross(new Vector3(nl.x, 0, nl.z).normalized, Vector3.up)) * nl;
			}
			if (right != Vector3.zero && left != Vector3.zero)
            {
                /*
                if (blendNormals) n = new Vector3(nr.x + nl.x, nr.y * nl.y, nr.z + nl.z);
                else n = Vector3.Lerp(nl, nr, 0.5f);

                n = new Vector3(n.x + nm.x, n.y * nm.y, n.z + nm.z);
                n.Normalize();
                */

                n = Vector3.Lerp(nl, nr, 0.5f);
                n = Vector3.Lerp(nm, n, 0.75f);
			}
			else
			{
				n = CourseBase.TerrainNormal(middle);
			}

			return n;
		}

		Vector2 GetNormalAtIndex(Vector3[] points, int index)
		{
			Vector2 point = new Vector2(points[index].x, points[index].z);
			Vector2 rightPoint = Vector2.zero;
			Vector2 leftPoint = Vector2.zero;

			if (index == 0)
			{
				leftPoint.x = points[points.Length - 2].x;
				leftPoint.y = points[points.Length - 2].z;
			}
			else
			{
				leftPoint.x = points[index - 1].x;
				leftPoint.y = points[index - 1].z;
			}
			if (index == points.Length - 1)
			{
				rightPoint.x = points[1].x;
				rightPoint.y = points[1].z;
			}
			else
			{
				rightPoint.x = points[index + 1].x;
				rightPoint.y = points[index + 1].z;
			}

			Vector2 rightNormal = rightPoint.RotateAround(point, -90.0f);
			Vector2 leftNormal = leftPoint.RotateAround(point, 90.0f);
			Vector2 averageNormal = ((rightNormal - point).normalized + (leftNormal - point).normalized) / 2.0f;
			Vector2 normalVector = averageNormal.normalized;

			if ((point + ((Utility.squareHalf) * normalVector)).IsInside(points))
				normalVector *= -1;

			return normalVector;
		}
		Vector2 GetNormalAtIndex(List<Vector2> points, int index)
		{
			Vector2 point = new Vector2(points[index].x, points[index].y);
			Vector2 rightPoint = Vector2.zero;
			Vector2 leftPoint = Vector2.zero;

			if (index == 0)
			{
				leftPoint.x = points[points.Count - 2].x;
				leftPoint.y = points[points.Count - 2].y;
			}
			else
			{
				leftPoint.x = points[index - 1].x;
				leftPoint.y = points[index - 1].y;
			}
			if (index == points.Count - 1)
			{
				rightPoint.x = points[1].x;
				rightPoint.y = points[1].y;
			}
			else
			{
				rightPoint.x = points[index + 1].x;
				rightPoint.y = points[index + 1].y;
			}

			Vector2 rightNormal = rightPoint.RotateAround(point, -90.0f);
			Vector2 leftNormal = leftPoint.RotateAround(point, 90.0f);
			Vector2 averageNormal = ((rightNormal - point).normalized + (leftNormal - point).normalized) / 2.0f;
			Vector2 normalVector = averageNormal.normalized;

			if ((point + ((Utility.squareHalf) * normalVector)).IsInside(points))
				normalVector *= -1;

			return normalVector;
		}

		Line GridFill(SplineBase spline, Line line)
		{
			float offset = spline.Offset + spline.Depth;
			Layer layer = spline.Layer;
			float resolution = layer.resolution;
			SplineBase.SplinePattern pattern = spline.Pattern;
			Color color = new Color(pattern.extra ? 0 : 1, 0, 0, spline.IsHole ? 0.01f : 1);

			Line xy = new Line();
			for (float x = editorMin.x; x < editorMax.x; x += resolution)
			{
				for (float z = editorMin.z; z < editorMax.z; z += resolution)
				{
					Vector3 point = new Vector3(x, 0, z);
					if (!point.IsInside(line.points)) continue;

                    float y = offset;
                    xy.Add(GetPosition(new Vector2(x, z), y), GetNormal(point, Vector3.zero, Vector3.zero, new Vector2(x, z)), color);
				}
			}
			return xy;
		}
		List<Line> RadialFill(SplineBase spline)
		{
			Layer layer = spline.Layer;
			float resolution = layer.resolution;
			float offset = spline.Offset + spline.Depth;
			float minDistanceToChilds = (RealMaxPoint - RealMinPoint).magnitude;
			List<Line> xy = new List<Line>();
			Color color = innerLine.colors[0];

			List<List<Vector2>> previous = new List<List<Vector2>>();
			previous.Add(innerLine.points.ToVector2());

			float distance = 0;
			while (distance + resolution < minDistanceToChilds)
			{
				distance += resolution;

				List<List<Vector2>> current = new List<List<Vector2>>();
				for (int t = 0; t < 10; ++t)
				{
					try
					{
						current = previous.MakeShell(-resolution, 1000 + t);
						break;
					}
					catch
					{
					}
				}

				for (int i = 0; i < current.Count; ++i)
				{
					if (current[i].Count < 3)
					{
						current.RemoveAt(i);
						i = -1;
						continue;
					}
				}

				for (int i = 0; i < current.Count; ++i)
				{
					Line line = new Line();
					for (int j = 0; j < current[i].Count; ++j)
					{
						Vector3 position = GetPosition(current[i][j], offset);
						Vector3 normal = GetNormal(position, Vector3.zero, Vector3.zero, current[i][j]);
						line.Add(position, normal, color);
					}
					xy.Add(line);
				}
				previous = current;
			}

			for (int i = 0; i < previous.Count; ++i)
			{
				Vector2 center = previous[i][0];
				for (int j = 1; j < previous[i].Count; ++j)
				{
					center += previous[i][j];
				}
				center /= previous[i].Count;
				{
					Line line = new Line();
					Vector3 position = GetPosition(center, offset);
					Vector3 normal = GetNormal(position, Vector3.zero, Vector3.zero, center);
					line.Add(position, normal, color);
					xy.Add(line);
				}
			}
			return xy;
		}
		#endregion
	}
}