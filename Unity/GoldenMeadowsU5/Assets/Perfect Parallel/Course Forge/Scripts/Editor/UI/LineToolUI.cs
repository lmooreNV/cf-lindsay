using UnityEngine;
using UnityEditor;

namespace PerfectParallel.CourseForge.UI
{
	/// <summary>
	/// Course Forge line tool, can draw handles, lines, boxes
	/// </summary>
	public class LineToolUI
	{
		#region Fields
		static Vector3[] points2 = new Vector3[] { Vector3.zero, Vector3.zero };
		static Vector3[] points3 = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
		static Vector3[] points4 = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		static Vector3[] points6 = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		static Vector3[] points8 = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
		#endregion

		#region Methods
		/// <summary>
		/// Draws line between points
		/// </summary>
		/// <param name="p1">point 1</param>
		/// <param name="p2">point 2</param>
		/// <param name="color">color of the line</param>
		public static void DrawLine(Vector3 p1, Vector3 p2, Color color)
		{
			Handles.color = color;
			Handles.DrawLine(p1, p2);
		}
		/// <summary>
		/// Draws lines based on points array
		/// </summary>
		/// <param name="points">array of the points</param>
        /// <param name="color">color of the line</param>
        public static void DrawLines(Vector3[] points, Color color)
        {
            Handles.color = color;
            Handles.DrawPolyLine(points);
        }
        /// <summary>
        /// Draws lines based on points and color arrays
        /// </summary>
        /// <param name="points"></param>
        /// <param name="colors"></param>
        public static void DrawLines(Vector3[] points, Color[] colors)
        {
            for (int i = 1; i < points.Length; ++i)
            {
                Handles.color = colors[i];
                Handles.DrawLine(points[i - 1], points[i]);
            }
        }
		/// <summary>
		/// Draws ground box
		/// </summary>
		/// <param name="point">center of pin</param>
		/// <param name="color">color of the line</param>
		/// <param name="width">width of the box</param>
		/// <param name="height">height of the box</param>
		/// <param name="forward">forward vector</param>
		public static void DrawBox(Vector3 point, Color color, float width = -1, float height = -1, Vector3 forward = default(Vector3))
		{
			if (width == -1) width = Utility.squareHalf * 3;
			if (height == -1) height = Utility.squareHalf * 3;

			points8[0].Set(- width / 2, 0, - height / 2);
			points8[1].Set(  width / 2, 0, - height / 2);
			points8[2].Set(- width / 2, 0, - height / 2);
			points8[3].Set(- width / 2, 0,   height / 2);
			points8[4].Set(- width / 2, 0,   height / 2);
			points8[5].Set(width / 2, 0, height / 2);
			points8[6].Set(  width / 2, 0, - height / 2);
			points8[7].Set(  width / 2, 0,   height / 2);

			if (forward.magnitude != 0)
			{
				Quaternion rotation = Quaternion.FromToRotation(Vector3.right, forward);
				for (int i = 0; i < 8; ++i) points8[i] = rotation * points8[i];
			}

			for (int i = 0; i < 8; ++i) points8[i] += point;

			Handles.color = color;
			Handles.DrawPolyLine(points8);
		}
		/// <summary>
		/// Draws pin
		/// </summary>
		/// <param name="point">center of pin</param>
		/// <param name="color">color of the line</param>
		public static void DrawPin(Vector3 point, Color color)
		{
			float d = 1.5f;
			float size = Utility.squareHalf * d * 4;

			points4[0] = (point);
			points4[1] = (point + Vector3.up * size * 2);
			points4[2] = (point + Vector3.up * size * 2 * 0.75f + Vector3.forward * size);
			points4[3] = (point + Vector3.up * size * 2 * 0.5f);

			Handles.color = color;
			Handles.DrawPolyLine(points4);
		}
        /// <summary>
        /// Draws hazard start arrow
        /// </summary>
        /// <param name="point"></param>
        /// <param name="target"></param>
        /// <param name="color"></param>
        public static void DrawStart(Vector3 point, Vector3 target, Color color)
        {
            float d = 1.5f;
            float size = Utility.squareHalf * d * 4;

            points8[0] = Vector3.zero;
            points8[1] = Vector3.up * size;
            points8[2] = Vector3.up * size / 2 + Vector3.forward * size;
            points8[3] = Vector3.zero;
            points8[4] = Vector3.up * size / 2;
            points8[5] = Vector3.up * size / 2 + Vector3.forward * size;
            points8[6] = Vector3.up * size / 2;
            points8[7] = Vector3.up * size / 2;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, (target - point).normalized);
            for (int i = 0; i < 8; ++i) points8[i] = point + rotation * points8[i];

            color.a = 1;
            Handles.color = color;
            Handles.DrawPolyLine(points8);
        }
		/// <summary>
		/// Draws shot
		/// </summary>
		/// <param name="point">center of shot</param>
		/// <param name="color">color of the line</param>
		public static void DrawShot(Vector3 point, Color color)
		{
			float d = 1.5f;
			float size = Utility.squareHalf * d * 4;

			Handles.color = color;
			points3[0] = point + Vector3.up * size * 2;
			points3[1] = point;
			points3[2] = point + Vector3.up * size / 2 - Vector3.right * size / 2;
			Handles.DrawPolyLine(points3);
			points2[0] = point;
			points2[1] = point + Vector3.up * size / 2 + Vector3.right * size / 2;
			Handles.DrawPolyLine(points2);
		}
		/// <summary>
		/// Draws tee
		/// </summary>
		/// <param name="point">center of tee</param>
		/// <param name="color">color of the line</param>
		public static void DrawTee(Vector3 point, Color color)
		{
			float d = 1.5f;
			float size = Utility.squareHalf * d * 4;

			Handles.color = color;
			points6[0] = point;
			points6[1] = point;
			points6[2] = point + Vector3.up * size / 2;
			points6[3] = point + Vector3.up * size + Vector3.right * size / 4;
			points6[4] = point + Vector3.up * size - Vector3.right * size / 4;
			points6[5] = point + Vector3.up * size / 2;
			Handles.DrawPolyLine(points6);
		}
		/// <summary>
		/// Draws measure
		/// </summary>
		/// <param name="point">center of measure</param>
		/// <param name="direction">block orientation</param>
		/// <param name="color">color of the line</param>
		public static void DrawMeasure(Vector3 point, Vector3 direction, Color color)
		{
			float d = 1.5f;
			float size = Utility.squareHalf * d * 2;

			Handles.color = color;
			Handles.CubeCap(0, point, Quaternion.FromToRotation(Vector3.forward, direction), size);
		}
		#endregion
	}
}