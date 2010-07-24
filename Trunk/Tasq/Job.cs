using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Tasq
{
	/// <summary>
	/// Base class that represents jobs. Triggers can be added or removed from the 
	/// <see cref="Triggers"/> collection freely at any time. As triggers 
	/// are added, they are automatically enabled. 
	/// </summary>
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
		public string Identifier { get; set; }

		/// <summary>
		/// Gets or sets the triggers that will cause this job to run.
		/// </summary>
		public IList<ITrigger> Triggers { get; private set; }

		/// <summary>
		/// Executes the job behavior, as specified by a derived class.
		/// </summary>
		protected abstract void Run();

		private void OnTriggerFired(object sender, EventArgs e)
		{
			Run();
		}

		/// <summary>
		/// Disposes this instance and all triggers that are disposable too.
		/// </summary>
		public void Dispose()
		{
			this.Triggers
				.Select(trigger => trigger as IDisposable)
				.Apply(disposable => disposable.Try(d => d.Dispose()));
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
				item.IsEnabled = true;
			}

			protected override void RemoveItem(int index)
			{
				var trigger = this.Items[index];
				trigger.Try(t => t.Fired -= this.job.OnTriggerFired);

				base.RemoveItem(index);
			}

			protected override void SetItem(int index, ITrigger item)
			{
				Guard.NotNull(() => item, item);

				var trigger = this.Items[index];
				trigger.Try(t => t.Fired -= this.job.OnTriggerFired);

				base.SetItem(index, item);

				item.Fired += this.job.OnTriggerFired;
				item.IsEnabled = true;
			}
		}
	}
}
