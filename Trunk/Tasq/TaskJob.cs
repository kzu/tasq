using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasq
{
	/// <summary>
	/// A job that runs an asynchronous <see cref="Task"/> when triggers fire.
	/// </summary>
	public class TaskJob : Job
	{
		private Task task;
		private TaskScheduler scheduler;

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskJob"/> class.
		/// </summary>
		/// <param name="task">The task that has been configured to run.</param>
		public TaskJob(Task task, TaskScheduler scheduler = null)
		{
			this.task = task;
			this.scheduler = scheduler != null ? scheduler : TaskScheduler.Current;
		}

		/// <summary>
		/// Performs the task execution. The job is disabled while the asynchronous task is being executed, 
		/// to avoid re-entrancy when its not need
		/// </summary>
		protected override void OnRun()
		{
			this.Disable(ApplyTo.JobOnly);

			this.task.ContinueWith(t => HandleTaskException(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
			this.task.ContinueWith(t => this.Enable(ApplyTo.JobOnly), TaskContinuationOptions.OnlyOnRanToCompletion);

			this.task.Start(this.scheduler);
		}

		private void HandleTaskException(AggregateException exception)
		{
			OnUnhandledException(exception);
			if (this.Status != Status.Error)
				this.Enable(ApplyTo.JobOnly);
		}
	}
}
