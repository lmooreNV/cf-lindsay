using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace PerfectParallel
{
	/// <summary>
	/// Color object class, a container to save into JSON
	/// </summary>
	[Serializable]
	public class ColorObject
	{
		#region Fields
		public float r = 0, g = 0, b = 0, a = 0;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public ColorObject()
		{
		}
		/// <summary>
		/// Constructor from color components
		/// </summary>
		/// <param name="r">red</param>
		/// <param name="g">green</param>
		/// <param name="b">blue</param>
		/// <param name="a">alpha</param>
		public ColorObject(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
		/// <summary>
		/// Constructor from Dictionary
		/// </summary>
		/// <param name="dictionary"></param>
		public ColorObject(Dictionary<string, object> dictionary)
		{
			this.r = Convert.ToSingle(dictionary["r"]);
			this.g = Convert.ToSingle(dictionary["g"]);
			this.b = Convert.ToSingle(dictionary["b"]);
			this.a = Convert.ToSingle(dictionary["a"]);
		}
		/// <summary>
		/// Constructor from Unity3D object
		/// </summary>
		/// <param name="color"></param>
		public ColorObject(Color color)
		{
			Set(color);
		}
		#endregion

		#region Operators
		public static implicit operator ColorObject(Color obj)
		{
			return new ColorObject(obj);
		}
		public static implicit operator Color(ColorObject c)
		{
			return c.ToColor();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Set from Unity3D color
		/// </summary>
		/// <param name="obj"></param>
		public void Set(Color obj)
		{
			r = obj.r;
			g = obj.g;
			b = obj.b;
			a = obj.a;
		}
		/// <summary>
		/// Set color components
		/// </summary>
		/// <param name="r">red</param>
		/// <param name="g">green</param>
		/// <param name="b">blue</param>
		/// <param name="a">alpha</param>
		public void Set(float r, float g, float b, float a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}
		/// <summary>
		/// Convert to Unity3D color
		/// </summary>
		/// <returns></returns>
		public Color ToColor()
		{
			return new Color(r, g, b, a);
		}
		#endregion
	}
}