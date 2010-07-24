using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tasq
{
	public class TimerTrigger : ITrigger
	{
		private bool isEnabled;
		private Timer timer;

		public event EventHandler Fired = (sender, args) => { };

		public TimerTrigger()
		{
			this.timer = new Timer(OnTick);
		}

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

		public TimeSpan DueTime { get; set; }
		public TimeSpan Interval { get; set; }

		private void OnTick(object state)
		{
			this.Fired(this, EventArgs.Empty);
		}
	}
}
