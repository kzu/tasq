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
		public void WhenTriggerIsEnabled_ThenDoesFire()
		{
			var fired = false;
			var trigger = new TimerTrigger
			{
				DueTime = TimeSpan.FromMilliseconds(200),
			};

			trigger.Fired += (sender, args) => fired = true;
			trigger.IsEnabled = true;

			Thread.Sleep(300);

			Assert.IsTrue(fired);
		}

		[TestMethod]
		public void WhenTriggerIsDisabled_ThenDoesFire()
		{
			var fired = false;
			var trigger = new TimerTrigger
			{
				DueTime = TimeSpan.FromMilliseconds(200),
			};

			trigger.Fired += (sender, args) => fired = true;
			trigger.IsEnabled = false;

			Thread.Sleep(300);

			Assert.IsFalse(fired);
		}

		[TestMethod]
		public void WhenTriggerIsEnabled_ThenFiresAtEveryInterval()
		{
			var trigger = new TimerTrigger
			{
				DueTime = TimeSpan.FromMilliseconds(100),
				Interval = TimeSpan.FromMilliseconds(100),
			};

			var fires = 0;
			trigger.Fired += (sender, args) => fires++;

			trigger.IsEnabled = true;

			Thread.Sleep(400);

			Assert.IsTrue(fires >= 2);
		}

	}
}
