using UnityEngine;
using System;
using System.Collections.Generic;

namespace PerfectParallel
{
	/// <summary>
	/// Vector3 container class
	/// </summary>
	[Serializable]
	public class Vector3Object
	{
		#region Fields
		public float x = 0, y = 0, z = 0;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public Vector3Object()
		{
		}
		/// <summary>
		/// Constructor from x,y,z
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public Vector3Object(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		/// <summary>
		/// Constructor from dictionary
		/// </summary>
		/// <param name="obj"></param>
		public Vector3Object(Dictionary<string, object> obj)
		{
			this.x = Convert.ToSingle(obj["x"]);
			this.y = Convert.ToSingle(obj["y"]);
			this.z = Convert.ToSingle(obj["z"]);
		}
		/// <summary>
		/// Constructor from Unity3D vector
		/// </summary>
		/// <param name="obj"></param>
		public Vector3Object(Vector3 obj)
		{
			Set(obj);
		}
		#endregion

		#region Operators
		public static implicit operator Vector3Object(Vector3 obj)
		{
			return new Vector3Object(obj);
		}
		public static implicit operator Vector3Object(Vector2 obj)
		{
			return new Vector3Object(new Vector3(obj.x, obj.y, 0));
		}
		public static implicit operator Vector3(Vector3Object c)
		{
			return c.ToVector3();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Set from Unity3D vector
		/// </summary>
		/// <param name="obj"></param>
		public void Set(Vector3 obj)
		{
			x = obj.x;
			y = obj.y;
			z = obj.z;
		}
		/// <summary>
		/// Set components
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public void Set(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		/// <summary>
		/// Convert to Unity3D vector
		/// </summary>
		/// <returns></returns>
		public Vector3 ToVector3()
		{
			return new Vector3(x, y, z);
		}
		#endregion
	}
}