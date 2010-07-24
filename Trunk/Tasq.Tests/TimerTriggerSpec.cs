using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Threading;

namespace Tasq.Tests
{
	[TestClass]
	public class TimerTriggerSpec
	{
		[TestMethod]
		public void WhenAddedToJob_ThenTriggersRunAtDueTime()
		{
			var job = new Mock<Job>();
			var trigger = new TimerTrigger
			{
				DueTime = TimeSpan.FromMilliseconds(300)
			};

			job.Object.Triggers.Add(trigger);

			Thread.Sleep(400);

			job.Protected().Verify("Run", Times.Once());
		}

		[TestMethod]
		public void WhenTriggerIsDisabled_ThenDoesNotCauseRun()
		{
			var job = new Mock<Job>();
			var trigger = new TimerTrigger
			{
				DueTime = TimeSpan.FromMilliseconds(200),
			};

			job.Object.Triggers.Add(trigger);

			trigger.IsEnabled = false;

			Thread.Sleep(300);

			job.Protected().Verify("Run", Times.Never());
		}
	}
}
