using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// The scope of an operation on the job.
	/// </summary>
	public enum ApplyTo
	{
		/// <summary>
		/// The operation (i.e. enabling) applies only to the job.
		/// </summary>
		JobOnly,
		/// <summary>
		/// The operation (i.e. enabling) applies to the job and its triggers.
		/// </summary>
		JobAndTriggers,
	}
}
