using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tasq
{
	/// <summary>
	/// A trigger that fires at specified intervals.
	/// </summary>
	public class TimerTrigger : ITrigger
	{
		private bool isEnabled;
		private Timer timer;

		/// <summary>
		/// Occurs when the trigger fires.
		/// </summary>
		public event EventHandler Fired = (sender, args) => { };

		/// <summary>
		/// Initializes a new instance of the <see cref="TimerTrigger"/> class.
		/// </summary>
		public TimerTrigger()
		{
			this.timer = new Timer(OnTick);
		}

		/// <summary>
		/// Gets or sets a value indicating whether this trigger is enabled.
		/// </summary>
		/// <value></value>
		public bool IsEnabled
		{
			get { return this.isEnabled; }
			set
			{
				if (value == this.isEnabled)
					return;

				this.isEnabled = value;

				if (value == true)
					this.timer.Change(this.DueTime, this.Interval);
				else
					this.timer.Change(Timeout.Infinite, Timeout.Infinite);
			}
		}

		/// <summary>
		/// Gets or sets the due time for the first fire of the trigger.
		/// </summary>
		public TimeSpan DueTime { get; set; }

		/// <summary>
		/// Gets or sets the interval for subsequent fires of the trigger after the <see cref="DueTime"/>.
		/// </summary>
		public TimeSpan Interval { get; set; }

		private void OnTick(object state)
		{
			this.Fired(this, EventArgs.Empty);
		}
	}
}
