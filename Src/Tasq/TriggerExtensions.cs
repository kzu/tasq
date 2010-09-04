using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// Extensions for triggers.
	/// </summary>
	public static class TriggerExtensions
	{
		/// <summary>
		/// Enables the given <paramref name="target"/> trigger whenever the 
		/// <paramref name="condition"/> trigger fires.
		/// </summary>
		public static ITrigger EnableWhen(this ITrigger target, ITrigger condition)
		{
			return new ConditionalTrigger(target, condition, true);
		}

		/// <summary>
		/// Disables the given <paramref name="target"/> trigger whenever the 
		/// <paramref name="condition"/> trigger fires.
		/// </summary>
		public static ITrigger DisableWhen(this ITrigger target, ITrigger condition)
		{
			return new ConditionalTrigger(target, condition, false);
		}

		/// <summary>
		/// Internal implementation of the conditional triggers.
		/// </summary>
		private class ConditionalTrigger : ITrigger
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
