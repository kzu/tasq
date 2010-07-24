using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	public class JobManager
	{
		public JobManager()
		{
			this.Jobs = new List<Job>();
		}

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
