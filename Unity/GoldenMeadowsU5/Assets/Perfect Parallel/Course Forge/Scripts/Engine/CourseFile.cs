using System.Reflection;
using System.Collections.Generic;

namespace PerfectParallel.CourseForge
{
	/// <summary>
	/// CourseFile, legacy class 
	/// to load old CFC course files
	/// </summary>
	[Obfuscation(Exclude = true)]
	public class CourseFile
	{
		#region Fields
		public const string currentVersion = "1.03";
		public string version = "1.00";
		public string library = "";
		public List<SplineBase.SplineInfo> infos = new List<SplineBase.SplineInfo>();
		public CourseBase.CourseInfo info = new CourseBase.CourseInfo();
		#endregion

		#region Methods
		/// <summary>
		/// Apply patches to course file
		/// </summary>
		/// <param name="text"></param>
		public void ApplyPatches(string text)
		{
			if (version == "1.00")
			{
				infos = Utility.JsonRead<List<SplineBase.SplineInfo>>(text);
				version = "1.01";
			}
			if (version == "1.01")
			{
				if (library != "" && !library.StartsWith("Assets"))
				{
					library = library.Remove(0, library.IndexOf("Assets"));
				}
			}
			if (version == "1.02")
			{
				if (library != "" && !library.StartsWith("Assets"))
				{
					library = library.Remove(0, library.IndexOf("Assets"));
				}
			}
		}
		#endregion
	}
}