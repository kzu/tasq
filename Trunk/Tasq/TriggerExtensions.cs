using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	public static class TriggerExtensions
	{
		public static ITrigger EnableWhen(this ITrigger target, ITrigger condition)
		{
			return new ConditionalTrigger(target, condition, true);
		}

		public static ITrigger DisableWhen(this ITrigger target, ITrigger condition)
		{
			return new ConditionalTrigger(target, condition, false);
		}

		public class ConditionalTrigger : ITrigger
		{
			public event EventHandler Fired = (sender, args) => { };

			public ConditionalTrigger(ITrigger target, ITrigger condition, bool targetStatusOnConditionFire)
			{
				this.Target = target;
				this.Condition = condition;

				target.IsEnabled = false;

				this.Condition.Fired += (sender, args) => this.Target.IsEnabled = targetStatusOnConditionFire;
				this.Target.Fired += (sender, args) => this.Fired(this, EventArgs.Empty);
			}

			public bool IsEnabled
			{
				get { return this.Condition.IsEnabled; }
				set
				{
					this.Condition.IsEnabled = value;
					if (!value)
						this.Target.IsEnabled = false;
				}
			}

			public ITrigger Target { get; private set; }
			public ITrigger Condition { get; private set; }
		}
	}
}
