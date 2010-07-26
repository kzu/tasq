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
		/// Initializes a new instance of the <see cref="ActionJob"/> class.
		/// </summary>
		public ActionJob(Action action, params ITrigger[] triggers)
		{
			Guard.NotNull(() => action, action);

			this.action = action;
			this.Triggers.AddRange(triggers);
		}

		/// <summary>
		/// Gets or sets the action to execute when the trigger/s fire.
		/// </summary>
		public Action Action
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
