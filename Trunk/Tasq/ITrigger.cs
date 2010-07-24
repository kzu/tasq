using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	public interface ITrigger
	{
		event EventHandler Fired;
		bool IsEnabled { get; set; }
	}
}
