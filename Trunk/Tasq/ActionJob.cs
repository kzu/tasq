using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// A <see cref="Job"/> that executes an <see cref="Action"/> when the trigger/s 
	/// fire.
	/// </summary>
	public class ActionJob : Job
	{
		private Action action;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionJob"/> class, automatically enabling the job 
		/// and all triggers.
		/// </summary>
		public ActionJob(Action action, params ITrigger[] triggers)
			: this(action, true, ApplyTo.JobAndTriggers, triggers)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionJob"/> class.
		/// </summary>
		public ActionJob(Action action, bool isEnabled = true, ApplyTo isEnabledAppliesTo = ApplyTo.JobAndTriggers, params ITrigger[] triggers)
		{
			Guard.NotNull(() => action, action);

			this.action = action;
			this.Triggers.AddRange(triggers);
			if (isEnabled)
				this.Enable(isEnabledAppliesTo);
		}

		/// <summary>
		/// Gets or sets the action to execute when the trigger/s fire.
		/// </summary>
		public virtual Action Action
		{
			get { return this.action; }
			set
			{
				Guard.NotNull(() => value, value);
				this.action = value;
			}
		}

		/// <summary>
		/// Executes the <see cref="Action"/>.
		/// </summary>
		protected override void Run()
		{
			this.Action.Try(action => action());
		}
	}
}
