using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// Interface that defines a trigger that fires when certain condition 
	/// is met, causing the owning <see cref="Job"/> to run.
	/// </summary>
	public interface ITrigger
	{
		/// <summary>
		/// Occurs when the trigger fires.
		/// </summary>
		event EventHandler Fired;

		/// <summary>
		/// Gets or sets a value indicating whether this trigger is enabled.
		/// </summary>
		bool IsEnabled { get; set; }
	}
}
