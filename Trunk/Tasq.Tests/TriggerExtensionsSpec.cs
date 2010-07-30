using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tasq.Tests
{
	public class TriggerExtensionsSpec
	{
		[TestClass]
		public abstract class GivenAConditionalTrigger
		{
			protected abstract ITrigger CreateConditionalTrigger(ITrigger target, ITrigger condition);

			[TestMethod]
			public void WhenTargetIsDisabled_ThenSequenceIsDisabled()
			{
				var target = new Mock<ITrigger>();
				target.SetupProperty(x => x.IsEnabled, false);
				var condition = new Mock<ITrigger>();
				condition.SetupProperty(x => x.IsEnabled, false);

				var trigger = CreateConditionalTrigger(target.Object, condition.Object);

				Assert.IsFalse(trigger.IsEnabled);
			}

			[TestMethod]
			public void WhenSequenceIsDisabled_ThenBothAreDisabled()
			{
				var target = new Mock<ITrigger>();
				target.SetupProperty(x => x.IsEnabled, true);
				var condition = new Mock<ITrigger>();
				condition.SetupProperty(x => x.IsEnabled, true);

				var trigger = CreateConditionalTrigger(target.Object, condition.Object);

				trigger.IsEnabled = false;

				Assert.IsFalse(target.Object.IsEnabled);
				Assert.IsFalse(condition.Object.IsEnabled);
			}

			[TestMethod]
			public void WhenConditionIsEnabled_ThenConditionIsEnabledAndTargetIsDisabled()
			{
				var target = new Mock<ITrigger>();
				target.SetupProperty(x => x.IsEnabled, false);
				var condition = new Mock<ITrigger>();
				condition.SetupProperty(x => x.IsEnabled, true);

				var trigger = CreateConditionalTrigger(target.Object, condition.Object);

				trigger.IsEnabled = true;

				Assert.IsFalse(target.Object.IsEnabled);
				Assert.IsTrue(condition.Object.IsEnabled);
			}
		}

		[TestClass]
		public class GivenAnEnabledWhenTrigger : GivenAConditionalTrigger
		{
			protected override ITrigger CreateConditionalTrigger(ITrigger target, ITrigger condition)
			{
				return target.EnableWhen(condition);
			}

			[TestMethod]
			public void WhenConditionFires_ThenTargetIsEnabled()
			{
				var target = new Mock<ITrigger>();
				target.SetupProperty(x => x.IsEnabled, true);
				var condition = new Mock<ITrigger>();
				condition.SetupProperty(x => x.IsEnabled, false);

				var trigger = target.Object.EnableWhen(condition.Object);

				condition.Raise(x => x.Fired += null, EventArgs.Empty);

				Assert.IsTrue(target.Object.IsEnabled);
			}

			[TestMethod]
			public void WhenConditionFiresThenTargetFires_ThenTriggerFires()
			{
				var target = new Mock<ITrigger>();
				target.SetupProperty(x => x.IsEnabled, false);
				var condition = new Mock<ITrigger>();
				condition.SetupProperty(x => x.IsEnabled, false);

				var trigger = target.Object.EnableWhen(condition.Object);
				var fired = false;
				trigger.Fired += (sender, args) => fired = true;
				trigger.IsEnabled = true;

				target.Raise(x => x.Fired += null, EventArgs.Empty);
				condition.Raise(x => x.Fired += null, EventArgs.Empty);

				Assert.IsTrue(fired);
			}
		}

		[TestClass]
		public class GivenAnDisabledWhenTrigger : GivenAConditionalTrigger
		{
			protected override ITrigger CreateConditionalTrigger(ITrigger target, ITrigger condition)
			{
				return target.DisableWhen(condition);
			}

			[TestMethod]
			public void WhenConditionFires_ThenTargetIsDisabled()
			{
				var target = new Mock<ITrigger>();
				target.SetupProperty(x => x.IsEnabled, true);
				var condition = new Mock<ITrigger>();
				condition.SetupProperty(x => x.IsEnabled, false);

				var trigger = target.Object.DisableWhen(condition.Object);

				condition.Raise(x => x.Fired += null, EventArgs.Empty);

				Assert.IsFalse(target.Object.IsEnabled);
			}
		}
	}
}
