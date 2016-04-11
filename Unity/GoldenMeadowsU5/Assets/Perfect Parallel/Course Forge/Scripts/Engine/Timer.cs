using UnityEngine;
using System;

namespace PerfectParallel
{
	/// <summary>
	/// Timer class, can start/stop/resume
	/// move to time
	/// </summary>
	[Serializable]
	public class Timer
	{
		#region Fields
		float elapsed = -1;
		float maxTime = -1;

		bool pause = false;
		#endregion

		#region Methods
		/// <summary>
		/// Start timer
		/// </summary>
		/// <param name="maxTime"></param>
		public void Start(float maxTime = 0)
		{
			this.maxTime = maxTime;
			elapsed = 0;

			pause = false;
		}
		/// <summary>
		/// Stop timer
		/// </summary>
		public void Stop()
		{
			elapsed = -1;

			pause = false;
		}
		/// <summary>
		/// Pause timer
		/// </summary>
		public void Pause()
		{
			pause = true;
		}
		/// <summary>
		/// Resume timer
		/// </summary>
		public void Resume()
		{
			pause = false;
		}
		/// <summary>
		/// Update time with delta tie
		/// </summary>
		/// <param name="dt"></param>
		public void Update(float dt)
		{
			if (!pause)
			{
				elapsed += dt;
				if (elapsed < 0) elapsed = 0;
			}
		}
		/// <summary>
		/// Move to time in range [0..1]
		/// </summary>
		/// <param name="time01"></param>
		public void MoveToTime(float time01)
		{
			elapsed = time01 * maxTime;

			Resume();
			Pause();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Is timer enabled?
		/// </summary>
		public bool IsRunning
		{
			get
			{
				if (elapsed == -1) return false;
				if (pause) return false;
				return elapsed <= maxTime;
			}
		}
		/// <summary>
		/// Elapsed time
		/// </summary>
		public float Elapsed
		{
			get
			{
				if (elapsed == -1) return 0.0f;
				return elapsed;
			}
		}
		/// <summary>
		/// Elapsed time in [0..1] range
		/// </summary>
		public float Elapsed01
		{
			get
			{
				if (elapsed == -1) return 0.0f;
				return elapsed / maxTime;
			}
		}
		/// <summary>
		/// Maximum timer time
		/// </summary>
		public float MaxTime
		{
			get
			{
				return maxTime;
			}
		}
		#endregion
	}
}