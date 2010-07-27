using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// The <see cref="Job"/> status.
	/// </summary>
	public enum Status
	{
		/// <summary>
		/// The job is not running and is not in error.
		/// </summary>
		Idle,
		/// <summary>
		/// The job is currently running.
		/// </summary>
		Running,
		/// <summary>
		/// The last run of the job resulted in an error, therefore 
		/// the job has also been disabled.
		/// </summary>
		Error,
	}
}
