using System;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
	/// <summary>
	/// Library file class-container
	/// has list of the layers
	/// </summary>
	[Serializable]
	public class LibraryFile
	{
		#region Fields
		public const string currentVersion = "1.01";
		public string version = "1.00";
		public List<Layer> layers = new List<Layer>();
		#endregion

		#region Methods
		/// <summary>
		/// Apply patches to library
		/// </summary>
		/// <param name="text"></param>
		public void ApplyPatches(string text)
		{
			if (version == "1.00")
			{
				layers = Utility.JsonRead<List<Layer>>(text);
				version = "1.01";
			}
		}
		#endregion
	}
}