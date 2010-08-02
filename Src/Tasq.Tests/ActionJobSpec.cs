using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tasq.Tests
{
	[TestClass]
	public class ActionJobSpec
	{
		[TestMethod]
		public void WhenTriggerFired_ThenActionIsRun()
		{
			var run = false;
			var trigger = new Mock<ITrigger>();
			var job = new ActionJob(() => run = true, trigger.Object);
			job.Enable(ApplyTo.JobAndTriggers);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			Assert.IsTrue(run);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void WhenActionIsNull_ThenThrows()
		{
			var trigger = new Mock<ITrigger>();
			var job = new ActionJob(null, trigger.Object);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void WhenActionIsSetToNull_ThenThrows()
		{
			var trigger = new Mock<ITrigger>();
			var job = new ActionJob(() => { }, trigger.Object);

			job.Action = null;
		}
	}
}
