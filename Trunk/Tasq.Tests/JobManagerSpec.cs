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
		public void WhenPausingAll_ThenDisablesJobTrigger()
		{
			var manager = new JobManager();
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == true);
			var job = new ActionJob(() => { }, trigger);
			manager.Jobs.Add(job);

			manager.PauseAll();

			Assert.IsFalse(trigger.IsEnabled);
		}

		[TestMethod]
		public void WhenResumingAll_ThenEnablesJobTrigger()
		{
			var manager = new JobManager();
			var trigger = Mocks.OneOf<ITrigger>(t => t.IsEnabled == false);
			var job = new ActionJob(() => { }, trigger);
			manager.Jobs.Add(job);

			manager.ResumeAll();

			Assert.IsTrue(trigger.IsEnabled);
		}
	}
}
