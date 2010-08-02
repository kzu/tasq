using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using Moq;

namespace Tasq.Tests
{
	[TestClass]
	public class TaskJobSpec
	{
		[TestMethod]
		public void WhenTaskThrows_ThenJobIsDisabled()
		{
			var task = new Task(Throws);
			var trigger = new Mock<ITrigger>();
			var job = new TaskJob(task);

			task.ContinueWith(t => Assert.Fail("Should never get here as the task failed"), TaskContinuationOptions.OnlyOnRanToCompletion);

			job.Triggers.Add(trigger.Object);
			job.Enable(ApplyTo.JobAndTriggers);

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			Thread.Sleep(300);
			Assert.AreEqual(TaskStatus.Faulted, task.Status);
			Assert.IsFalse(job.IsEnabled);
		}

		[TestMethod]
		public void WhenTaskThrowsAndExceptionHandled_ThenJobRemainsEnabled()
		{
			var task = new Task(Throws);
			var trigger = new Mock<ITrigger>();
			var job = new TaskJob(task);

			task.ContinueWith(t => Assert.Fail("Should never get here as the task failed"), TaskContinuationOptions.OnlyOnRanToCompletion);

			job.Triggers.Add(trigger.Object);
			job.Enable(ApplyTo.JobAndTriggers);

			job.UnhandledException += (sender, args) => args.Handled = true;

			trigger.Raise(x => x.Fired += null, EventArgs.Empty);

			Thread.Sleep(300);
			Assert.AreEqual(TaskStatus.Faulted, task.Status);
			Assert.IsTrue(job.IsEnabled);
		}

		private void Throws()
		{
			Thread.Sleep(100);
			throw new InvalidOperationException("Fake exception");
		}
	}
}
