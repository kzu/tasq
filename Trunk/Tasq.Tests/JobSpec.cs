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
		public void WhenTriggerFires_ThenJobIsRun()
		{
			var job = new Mock<Job>();
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Once());
		}

		[TestMethod]
		public void WhenTriggerRemovedAndThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<Job>();
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers.Remove(trigger.Object);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Never());
		}

		[TestMethod]
		public void WhenTriggersClearedAndThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<Job>();
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
			var job = new Mock<Job>();

			job.Object.Triggers.Add(null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void WhenSettingNullTriggerByIndex_ThenThrows()
		{
			var job = new Mock<Job>();
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);
			job.Object.Triggers[0] = null;
		}

		[TestMethod]
		public void WhenTriggerReplacedByIndexThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<Job>();
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers[0] = Mocks.OneOf<ITrigger>();

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Protected().Verify("Run", Times.Never());
		}

		[TestMethod]
		public void WhenTriggerIsAdded_ThenItIsEnabled()
		{
			var job = new Mock<Job>();
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == false);

			job.Object.Triggers.Add(trigger);

			Assert.IsTrue(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenTriggerIsSetByIndex_ThenItIsEnabled()
		{
			var job = new Mock<Job>();
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == false);

			job.Object.Triggers.Add(Mocks.OneOf<ITrigger>());
			job.Object.Triggers[0] = trigger;

			Assert.IsTrue(trigger.IsEnabled);
		}
	}
}
