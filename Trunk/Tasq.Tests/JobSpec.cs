using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace Tasq.Tests
{
	[TestClass]
	public class JobSpec
	{
		[TestMethod]
		public void WhenJobIsDisabledAndTriggerFires_ThenJobIsNotRun()
		{
			var job = new Mock<TestJob> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Disable(ApplyTo.JobOnly);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Verify(x => x.DoRun(), Times.Never());
		}

		[TestMethod]
		public void WhenTriggerFires_ThenJobIsRun()
		{
			var job = new Mock<TestJob> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Verify(x => x.DoRun(), Times.Once());
		}

		[TestMethod]
		public void WhenTriggerRemovedAndThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<TestJob> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers.Remove(trigger.Object);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Verify(x => x.DoRun(), Times.Never());
		}

		[TestMethod]
		public void WhenTriggersClearedAndThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<TestJob> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers.Clear();

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Verify(x => x.DoRun(), Times.Never());
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void WhenAddingNullTrigger_ThenThrows()
		{
			var job = new Mock<TestJob> { CallBase = true };

			job.Object.Triggers.Add(null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[TestMethod]
		public void WhenSettingNullTriggerByIndex_ThenThrows()
		{
			var job = new Mock<TestJob> { CallBase = true };
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);
			job.Object.Triggers[0] = null;
		}

		[TestMethod]
		public void WhenTriggerReplacedByIndexThenFires_ThenJobIsNotRun()
		{
			var job = new Mock<TestJob> { CallBase = true };
			job.Object.Enable(ApplyTo.JobAndTriggers);
			var trigger = new Mock<ITrigger>();

			job.Object.Triggers.Add(trigger.Object);

			job.Object.Triggers[0] = Mocks.OneOf<ITrigger>();

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Verify(x => x.DoRun(), Times.Never());
		}

		[TestMethod]
		public void WhenEnableAppliesToTriggers_ThenTriggerIsEnabled()
		{
			var job = new Mock<TestJob> { CallBase = true };
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == false);

			job.Object.Triggers.Add(trigger);

			job.Object.Enable(ApplyTo.JobAndTriggers);

			Assert.IsTrue(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenDisableAppliesToTriggers_ThenTriggerIsDisabled()
		{
			var job = new Mock<TestJob> { CallBase = true };
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == true);

			job.Object.Triggers.Add(trigger);

			job.Object.Disable(ApplyTo.JobAndTriggers);

			Assert.IsFalse(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenRunThrows_ThenJobIsDisabled()
		{
			var trigger = new Mock<ITrigger>();
			var job = new Mock<TestJob>(trigger.Object) { CallBase = true };
			var exception = new InvalidOperationException();

			job.Setup(x => x.DoRun()).Throws(exception);
			job.Object.Enable();

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			Assert.IsFalse(job.Object.IsEnabled);
			Assert.AreEqual(Status.Error, job.Object.Status);
			Assert.AreSame(exception, job.Object.LastError);
		}

		[TestMethod]
		public void WhenExceptionIsHandled_ThenJobRemainsEnabled()
		{
			var trigger = new Mock<ITrigger>();
			var job = new Mock<TestJob>(trigger.Object) { CallBase = true };
			var exception = new InvalidOperationException();

			job.Setup(x => x.DoRun()).Throws(exception);
			job.Object.Enable();
			job.Object.UnhandledException += (sender, args) => args.Handled = true;

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			Assert.IsTrue(job.Object.IsEnabled);
			Assert.AreEqual(Status.Idle, job.Object.Status);
			Assert.IsNull(job.Object.LastError);
		}

		[TestMethod]
		public void WhenJobIsRunning_ThenStatusIsRunning()
		{
			var trigger = new Mock<ITrigger>();
			var job = new Mock<TestJob>(trigger.Object) { CallBase = true };

			job.Setup(x => x.DoRun())
				.Callback(() => Assert.AreEqual(Status.Running, job.Object.Status));
			job.Object.Enable();

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			job.Verify(x => x.DoRun());
		}

		[TestMethod]
		public void WhenJobIsRunning_ThenOtherTriggerEventsDoNotCauseRun()
		{
			var job = new Mock<TestJob> { CallBase = true };
			job.Object.Triggers.Add(new TimerTrigger { DueTime = TimeSpan.FromMilliseconds(200), Interval = TimeSpan.FromMilliseconds(500) });
			job.Object.Triggers.Add(new TimerTrigger { DueTime = TimeSpan.FromMilliseconds(300), Interval = TimeSpan.FromMilliseconds(500) });
			job.Object.Triggers.Add(new TimerTrigger { DueTime = TimeSpan.FromMilliseconds(400), Interval = TimeSpan.FromMilliseconds(500) });

			job.Setup(x => x.DoRun()).Callback(() => Thread.Sleep(1000));
			job.Object.Enable(ApplyTo.JobAndTriggers);

			Thread.Sleep(1100);
			job.Object.Disable(ApplyTo.JobOnly);

			job.Verify(x => x.DoRun(), Times.Once());
		}

		public abstract class TestJob : Job
		{
			public TestJob()
			{
			}

			public TestJob(ITrigger trigger)
			{
				base.Triggers.Add(trigger);
			}

			protected override void OnRun()
			{
				DoRun();
			}

			public abstract void DoRun();
		}
	}
}
