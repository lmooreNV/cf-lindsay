using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Poly2Tri;

namespace PerfectParallel.CourseForge
{
	/// <summary>
	/// Spline polygond, uses calulated line data 
	/// to produce meshes
	/// </summary>
	public class SplinePolygon
    {
        #region Fields
        Polygon polygon = null;
		SplineLines.Line data = new SplineLines.Line();

		bool simple = false;
		SplineLines.Line outer = new SplineLines.Line();
		SplineLines.Line inner = new SplineLines.Line();

		List<int> triangles = new List<int>();
		List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector4> tangents = new List<Vector4>();
		List<Color> colors = new List<Color>();
		#endregion

		#region Properties
		/// <summary>
		/// Polygon calculated vertices
		/// </summary>
		public List<Vector3> Vertices
		{
			get
			{
				return vertices;
			}
		}
        /// <summary>
        /// Polygon calculated normals
        /// </summary>
        public List<Vector3> Normals
        {
            get
            {
                return normals;
            }
        }
        /// <summary>
        /// Polygon calculated tangents
        /// </summary>
        public List<Vector4> Tangents
        {
            get
            {
                return tangents;
            }
        }
		/// <summary>
		/// Polygon calculated colors
		/// </summary>
		public List<Color> Colors
		{
			get
			{
				return colors;
			}
		}
		/// <summary>
		/// Polygon calculated indices
		/// </summary>
		public List<int> Triangles
		{
			get
			{
				return triangles;
			}
		}
		/// <summary>
		/// Is polygon in use?
		/// </summary>
		public bool IsInUse
		{
			get
			{
				return simple || data.points.Count != 0;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Clear the class
		/// </summary>
		public void Clear()
		{
			polygon = null;
			data.Clear();

			simple = false;
			outer.Clear();
			inner.Clear();

			triangles.Clear();
			vertices.Clear();
			normals.Clear();
            tangents.Clear();
			colors.Clear();
		}
		/// <summary>
		/// Add polygon from line data
		/// </summary>
		/// <param name="line"></param>
		public void AddPolygon(SplineLines.Line line)
		{
			polygon = new Polygon((IList<PolygonPoint>)line.polyPoints);
			AddData(line);
		}
		/// <summary>
		/// Add simple transition polygon
		/// </summary>
		/// <param name="outer"></param>
		/// <param name="inner"></param>
		public void AddSimple(SplineLines.Line outer, SplineLines.Line inner)
		{
			this.simple = true;
			this.outer.Add(outer);
			this.inner.Add(inner);
		}
		/// <summary>
		/// Add inner steiner points (for coly polygons)
		/// </summary>
		/// <param name="points"></param>
		public void AddSteiner(SplineLines.Line points)
		{
			for (int i = 0; i < points.polyPoints.Count; ++i)
				polygon.AddSteinerPoint(points.polyPoints[i]);

			this.data.Add(points);
		}
		/// <summary>
		/// Add hole into current polygon
		/// </summary>
		/// <param name="line"></param>
		public void AddChildAsHole(SplineLines.Line line)
		{
			polygon.AddHole(new Polygon((IList<PolygonPoint>)line.polyPoints));
			this.data.Add(line, this.data.colors[0]);
		}
		/// <summary>
		/// Update data before creating meshes (for non-serialized fields)
		/// </summary>
		public void Update()
		{
			data.Update();

			outer.Update();
			inner.Update();
		}
		/// <summary>
		/// Triangulate polgon, create vertices
		/// </summary>
		public void Triangulate()
		{
			if (simple)
			{
				AddTriangles(inner, outer);
			}
			else
			{
				vertices = data.points;
				normals = data.normals;
                tangents = data.tangents;
				colors = data.colors;

				P2T.Triangulate(polygon);
				IList<DelaunayTriangle> polyTris = polygon.Triangles;

				triangles.Clear();
				for (int i = 0; i < polyTris.Count; ++i)
				{
					triangles.Add(data.polyPoints.FindIndex(x => x.VertexCode == polyTris[i].Points[2].VertexCode));
					triangles.Add(data.polyPoints.FindIndex(x => x.VertexCode == polyTris[i].Points[1].VertexCode));
					triangles.Add(data.polyPoints.FindIndex(x => x.VertexCode == polyTris[i].Points[0].VertexCode));
				}
			}
		}
		/// <summary>
		/// Creates re-indexed indices with index offset
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public List<int> IndexedTriangles(int index)
		{
			List<int> newTriangles = new List<int>(triangles);
			for (int i = 0; i < newTriangles.Count; ++i) newTriangles[i] += index;
			return newTriangles;
		}
		#endregion

		#region Support Methods
		void AddData(SplineLines.Line data)
		{
			this.data.Add(data);
		}
		void AddTriangles(SplineLines.Line innerLine, SplineLines.Line outerLine)
		{
			int count = outerLine.points.Count;

			if (IsClockwise(outerLine.points))
			{
				outerLine.points.Reverse();
                innerLine.points.Reverse();
                outerLine.normals.Reverse();
                innerLine.normals.Reverse();
                outerLine.tangents.Reverse();
                innerLine.tangents.Reverse();
				outerLine.colors.Reverse();
				innerLine.colors.Reverse();

				vertices.AddRange(outerLine.points);
				vertices.AddRange(innerLine.points);
				normals.AddRange(outerLine.normals);
				normals.AddRange(innerLine.normals);
                tangents.AddRange(outerLine.tangents);
                tangents.AddRange(innerLine.tangents);
				colors.AddRange(outerLine.colors);
				colors.AddRange(innerLine.colors);

				triangles = new List<int>();
				for (int i = 1; i < count; ++i)
				{
					triangles.Add(i + count - 1);
					triangles.Add(i + count);
					triangles.Add(i - 1);
					triangles.Add(i - 1);
					triangles.Add(i + count);
					triangles.Add(i);
				}
				triangles.Add(count + count - 1);
				triangles.Add(0 + count);
				triangles.Add(count - 1);
				triangles.Add(count - 1);
				triangles.Add(0 + count);
				triangles.Add(0);
			}
			else
			{
				vertices.AddRange(outerLine.points);
                vertices.AddRange(innerLine.points);
                normals.AddRange(outerLine.normals);
                normals.AddRange(innerLine.normals);
                tangents.AddRange(outerLine.tangents);
                tangents.AddRange(innerLine.tangents);
				colors.AddRange(outerLine.colors);
				colors.AddRange(innerLine.colors);

				triangles = new List<int>();
				for (int i = 1; i < count; ++i)
				{
					triangles.Add(i + count - 1);
					triangles.Add(i + count);
					triangles.Add(i - 1);
					triangles.Add(i - 1);
					triangles.Add(i + count);
					triangles.Add(i);
				}
				triangles.Add(count + count - 1);
				triangles.Add(0 + count);
				triangles.Add(count - 1);
				triangles.Add(count - 1);
				triangles.Add(0 + count);
				triangles.Add(0);
			}
		}
		bool IsClockwise(List<Vector3> points)
		{
			float sum = 0;
			for (int i = 0; i < points.Count; i++)
			{
				Vector3 v1 = points[i];
				Vector3 v2 = points[(i + 1) % points.Count];
				sum += (v2.x - v1.x) * (v2.z + v1.z);
			}
			return sum > 0;
		}
		#endregion
	}
}