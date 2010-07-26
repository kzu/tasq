using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Tasq
{
	/// <summary>
	/// Base class that represents jobs. Triggers can be added or removed from the 
	/// <see cref="Triggers"/> collection freely at any time.
	/// </summary>
	/// <remarks>
	/// The collection of triggers for a job tracks add/remove operations for triggers, 
	/// and automatically attaches/detaches from their <see cref="ITrigger.Fire"/> 
	/// event.
	/// </remarks>
	public abstract class Job : IDisposable
	{
		protected Job()
		{
			this.Identifier = Guid.NewGuid().ToString();
			this.Triggers = new TriggerCollection(this);
		}

		/// <summary>
		/// Gets or sets the identifier, which defaults to a new <see cref="Guid"/> if not specified.
		/// </summary>
		public virtual string Identifier { get; set; }

		/// <summary>
		/// Gets or sets the triggers that will cause this job to run.
		/// </summary>
		public virtual IList<ITrigger> Triggers { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this job is enabled.
		/// </summary>
		/// <remarks>
		/// While the job is disabled, its triggers continue to fire as configured, but the 
		/// job does not respond to them. This allows separate configuration for triggers.
		/// </remarks>
		public virtual bool IsEnabled { get; private set; }

		/// <summary>
		/// Enables the job, optionally specifying whether all the triggers should be forcedly enabled too.
		/// </summary>
		public virtual void Enable(ApplyTo enableAppliesTo = ApplyTo.JobAndTriggers)
		{
			this.IsEnabled = true;
			if (enableAppliesTo == ApplyTo.JobAndTriggers)
				this.Triggers.Apply(trigger => trigger.IsEnabled = true,
					before: trigger => Tracing.TraceSource.TraceVerbose("Enabling trigger {0}.", trigger));
		}

		/// <summary>
		/// Disables the job, optionally specifying whether all the triggers should be forcedly disabled too.
		/// </summary>
		public virtual void Disable(ApplyTo disableAppliesTo = ApplyTo.JobAndTriggers)
		{
			this.IsEnabled = false;
			if (disableAppliesTo == ApplyTo.JobAndTriggers)
				this.Triggers.Apply(trigger => trigger.IsEnabled = false,
					before: trigger => Tracing.TraceSource.TraceVerbose("Disabling trigger {0}.", trigger));
		}

		/// <summary>
		/// Disposes this instance and all triggers that are disposable too.
		/// </summary>
		public virtual void Dispose()
		{
			this.Triggers
				.Select(trigger => trigger as IDisposable)
				.Apply(disposable => disposable.Try(d => d.Dispose()));
		}

		/// <summary>
		/// Executes the job behavior, as specified by a derived class.
		/// </summary>
		protected abstract void Run();

		private void OnTriggerFired(object sender, EventArgs e)
		{
			if (this.IsEnabled)
			{
				Tracing.TraceSource.TraceInformation("Trigger {0} fired. Running job {1}.", sender, this);
				Run();
			}
			else
			{
				Tracing.TraceSource.TraceInformation("Trigger {0} fired but job {1} is disabled.", sender, this);
			}
		}

		public override string ToString()
		{
			return this.GetType().FullName + " (" + this.Identifier + ")";
		}

		private class TriggerCollection : Collection<ITrigger>
		{
			private Job job;

			public TriggerCollection(Job job)
			{
				this.job = job;
			}

			protected override void ClearItems()
			{
				this.Items.Apply(trigger => trigger.Try(t => t.Fired -= this.job.OnTriggerFired));

				base.ClearItems();
			}

			protected override void InsertItem(int index, ITrigger item)
			{
				Guard.NotNull(() => item, item);

				base.InsertItem(index, item);

				item.Fired += this.job.OnTriggerFired;

				Tracing.TraceSource.TraceVerbose("Added trigger: {0} (Job {1}).", item, this.job);
			}

			protected override void RemoveItem(int index)
			{
				var trigger = this.Items[index];
				if (trigger != null)
				{
					trigger.Fired -= this.job.OnTriggerFired;
					Tracing.TraceSource.TraceVerbose("Removed trigger: {0} (Job {1}).", trigger, this.job);
				}

				base.RemoveItem(index);
			}

			protected override void SetItem(int index, ITrigger item)
			{
				Guard.NotNull(() => item, item);

				var trigger = this.Items[index];
				if (trigger != null)
				{
					trigger.Fired -= this.job.OnTriggerFired;
					Tracing.TraceSource.TraceVerbose("Removed replaced trigger: {0} (Job {1}).", trigger, this.job);
				}

				base.SetItem(index, item);

				item.Fired += this.job.OnTriggerFired;

				Tracing.TraceSource.TraceVerbose("Added trigger: {0} (Job {1}).", item, this.job);
			}
		}
	}
}
