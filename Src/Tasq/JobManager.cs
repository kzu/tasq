using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// Manages a list of <see cref="Job"/>s.
	/// </summary>
	public class JobManager : IDisposable
	{
		private List<Job> jobs;

		/// <summary>
		/// Initializes a new instance of the <see cref="JobManager"/> class.
		/// </summary>
		public JobManager()
		{
			this.jobs = new List<Job>();
		}

		/// <summary>
		/// Adds the specified job.
		/// </summary>
		public virtual void Add(Job job, bool enableJob = true, ApplyTo applyEnableTo = ApplyTo.JobAndTriggers)
		{
			this.jobs.Add(job);
			if (enableJob)
			{
				job.Enable(applyEnableTo);
			}
		}

		/// <summary>
		/// Gets the jobs managed by this instance.
		/// </summary>
		public virtual IEnumerable<Job> Jobs { get { return this.jobs.AsReadOnly(); } }

		/// <summary>
		/// Disables all jobs, optionally specifying if all job triggers should be disabled too.
		/// </summary>
		/// <remarks>By default disables all jobs but does not forcedly disable triggers.</remarks>
		public virtual void PauseAll(ApplyTo disableAppliesTo = ApplyTo.JobOnly)
		{
			Tracing.TraceSource.TraceInformation("Pausing all jobs.");

			this.jobs
				.Apply(
					job => job.Disable(disableAppliesTo),
					before: job => Tracing.TraceSource.TraceVerbose("Pausing job {0}.", job.Identifier));
		}

		/// <summary>
		/// Resumes all jobs, optionally specifying whether all job triggers should also be forcedly enabled.
		/// </summary>
		/// <remarks>By default enables all jobs but does not forcedly enable triggers.</remarks>
		public virtual void ResumeAll(ApplyTo enableAppliesTo = ApplyTo.JobOnly)
		{
			Tracing.TraceSource.TraceInformation("Resuming all jobs.");

			this.jobs
				.Apply(
					job => job.Enable(enableAppliesTo),
					before: job => Tracing.TraceSource.TraceVerbose("Resuming job {0}.", job.Identifier));
		}

		/// <summary>
		/// Disposes the manager and all disposable jobs.
		/// </summary>
		public virtual void Dispose()
		{
			this.jobs
				.Select(job => job as IDisposable)
				.Where(disposable => disposable != null)
				.Apply(disposable => disposable.Try(d => d.Dispose()));
		}
	}
}
