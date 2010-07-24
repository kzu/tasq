using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	public class ActionJob : Job
	{
		private Action action;

		public ActionJob(Action action, params ITrigger[] triggers)
		{
			Guard.NotNull(() => action, action);

			this.action = action;
			this.Triggers.AddRange(triggers);
		}

		public Action Action
		{
			get { return this.action; }
			set
			{
				Guard.NotNull(() => value, value);
				this.action = value;
			}
		}

		protected override void Run()
		{
			this.Action.Try(action => action());
		}
	}
}
