using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tasq.Tests
{
	[TestClass]
	public class JobManagerSpec
	{
		[TestMethod]
		public void WhenPausingAll_ThenDisablesJobsButDoesNotModifyTrigger()
		{
			var manager = new JobManager();
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == true);
			var job = new ActionJob(() => { }, trigger);
			manager.Add(job);

			manager.PauseAll();

			Assert.IsFalse(job.IsEnabled);
			Assert.IsTrue(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenResumingAll_ThenEnablesJobButDoesNotModifyTrigger()
		{
			var manager = new JobManager();
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == false);
			var job = new ActionJob(() => { }, trigger);
			manager.Add(job);

			trigger.IsEnabled = false;

			manager.ResumeAll();

			Assert.IsTrue(job.IsEnabled);
			Assert.IsFalse(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenDisposing_ThenDisposesDisposableJob()
		{
			var manager = new JobManager();
			var disposableJob = new Mock<Job>().As<IDisposable>();

			manager.Add((Job)disposableJob.Object);
			manager.Add(Mocks.OneOf<Job>());

			manager.Dispose();

			disposableJob.Verify(x => x.Dispose(), Times.Once());
		}

		[TestMethod]
		public void WhenAddingJob_ThenCanSkipEnablingJob()
		{
			var manager = new JobManager();
			var job = new Mock<Job> { CallBase = true };

			manager.Add(job.Object, false);

			job.Verify(x => x.Enable(It.IsAny<ApplyTo>()), Times.Never());
		}

		[TestMethod]
		public void WhenAddingJob_ThenCanEnableTriggers()
		{
			var manager = new JobManager();
			var job = new Mock<Job> { CallBase = true };
			var trigger = Mocks.OneOf<ITrigger>(x => x.IsEnabled == false);
			job.Object.Triggers.Add(trigger);

			manager.Add(job.Object, true, ApplyTo.JobAndTriggers);

			job.Verify(x => x.Enable(It.IsAny<ApplyTo>()), Times.Once());
			Assert.IsTrue(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenAddingJob_ThenCanEnableJobOnly()
		{
			var manager = new JobManager();
			var job = new Mock<Job> { CallBase = true };
			var trigger = Mocks.OneOf<ITrigger>(x => x.IsEnabled == false);

			manager.Add(job.Object, true, ApplyTo.JobOnly);

			job.Verify(x => x.Enable(It.IsAny<ApplyTo>()), Times.Once());
			Assert.IsFalse(trigger.IsEnabled);
		}
	}
}
