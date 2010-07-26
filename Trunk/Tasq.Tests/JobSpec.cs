using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace Tasq.Tests
{
	[TestClass]
	public class JobSpec
	{
		[TestMethod]
		public void WhenJobIsDisabledAndTriggerFires_ThenJobIsNotRun()
		{
			var job = new Mock<Job> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Disable(ApplyTo.JobOnly);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Never());
		}

		[TestMethod]
		public void WhenTriggerFires_ThenJobIsRun()
		{
			var job = new Mock<Job> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Once());
		}

		[TestMethod]
		public void WhenTriggerRemovedAndThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<Job> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers.Remove(trigger.Object);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Never());
		}

		[TestMethod]
		public void WhenTriggersClearedAndThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<Job> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers.Clear();

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Never());
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void WhenAddingNullTrigger_ThenThrows()
		{
			var job = new Mock<Job> { CallBase = true };

			job.Object.Triggers.Add(null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void WhenSettingNullTriggerByIndex_ThenThrows()
		{
			var job = new Mock<Job> { CallBase = true };
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);
			job.Object.Triggers[0] = null;
		}

		[TestMethod]
		public void WhenTriggerReplacedByIndexThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<Job> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers[0] = Mocks.OneOf<ITrigger>();

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Never());
		}

		[TestMethod]
		public void WhenEnableAppliesToTriggers_ThenTriggerIsEnabled()
		{
			var job = new Mock<Job> { CallBase = true };
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == false);

			job.Object.Triggers.Add(trigger);

			job.Object.Enable(ApplyTo.JobAndTriggers);

			Assert.IsTrue(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenDisableAppliesToTriggers_ThenTriggerIsDisabled()
		{
			var job = new Mock<Job> { CallBase = true };
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == true);

			job.Object.Triggers.Add(trigger);

			job.Object.Disable(ApplyTo.JobAndTriggers);

			Assert.IsFalse(trigger.IsEnabled);
		}
	}
}
