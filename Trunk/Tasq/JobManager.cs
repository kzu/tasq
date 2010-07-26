using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// Manages a list of <see cref="Job"/>s.
	/// </summary>
	public class JobManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		public JobManager()
		{
			this.Jobs = new List<Job>();
		}

		/// <summary>
		/// Gets the jobs managed by this instance.
		/// </summary>
		public IList<Job> Jobs { get; private set; }

		/// <summary>
		/// Disables all triggers in all jobs.
		/// </summary>
		public void PauseAll()
		{
			Tracing.TraceSource.TraceInformation("Pausing all triggers.");

			this.Jobs
				.ForEach(job => Tracing.TraceSource.TraceVerbose("Pausing triggers on job {0}.", job.Identifier))
				.SelectMany(job => job.Triggers)
				.Apply(trigger => trigger.IsEnabled = false,
						after: trigger => Tracing.TraceSource.TraceVerbose("Paused trigger {0}.", trigger));
		}

		/// <summary>
		/// Resumes all triggers in all jobs, by setting their <see cref="ITrigger.IsEnabled"/> to true.
		/// </summary>
		public void ResumeAll()
		{
			Tracing.TraceSource.TraceInformation("Resuming all triggers.");

			this.Jobs
				.ForEach(job => Tracing.TraceSource.TraceVerbose("Resuming triggers on job {0}.", job.Identifier))
				.SelectMany(job => job.Triggers)
				.Apply(trigger => trigger.IsEnabled = true,
						after: trigger => Tracing.TraceSource.TraceVerbose("Resumed trigger {0}.", trigger));
		}
	}
}
