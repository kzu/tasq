using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using Moq;

namespace Tasq.Tests
{
	[TestClass]
	public class NetworkAvailabilityTriggerSpec
	{
		[TestMethod]
		public void WhenTriggerIsNotEnabled_ThenDoesNotFireForNetworkStatusChanges()
		{
			var network = new Mock<NetworkAvailabilityTrigger.INetworkStatus>();
			var trigger = new NetworkAvailabilityTrigger(NetworkAvailabilityKind.Unavailable, network.Object);

			bool fired = false;
			trigger.Fired += (sender, e) => fired = true;

			network.Raise(x => x.IsAvailableChanged += null, EventArgs.Empty);

			Assert.IsFalse(fired);
		}

		[TestMethod]
		public void WhenTriggerIsEnabledThenDisabled_ThenDoesNotFireForNetworkStatusChanges()
		{
			var network = new Mock<NetworkAvailabilityTrigger.INetworkStatus>();
			network.SetupGet(x => x.IsAvailable).Returns(true);
			var trigger = new NetworkAvailabilityTrigger(NetworkAvailabilityKind.Available, network.Object);

			trigger.IsEnabled = true;
			trigger.IsEnabled = false;

			bool fired = false;
			trigger.Fired += (sender, e) => fired = true;

			network.Raise(x => x.IsAvailableChanged += null, EventArgs.Empty);

			Assert.IsFalse(fired);
		}

		[TestMethod]
		public void WhenTriggerIsEnabledAndNetworkStatusMatchesAvailabilityKindAvailable_ThenFiresInmediately()
		{
			var network = new Mock<NetworkAvailabilityTrigger.INetworkStatus>();
			network.SetupGet(x => x.IsAvailable).Returns(true);
			var trigger = new NetworkAvailabilityTrigger(NetworkAvailabilityKind.Available, network.Object);

			bool fired = false;
			trigger.Fired += (sender, e) => fired = true;

			trigger.IsEnabled = true;

			Assert.IsTrue(fired);
		}

		[TestMethod]
		public void WhenTriggerIsEnabledAndNetworkStatusMatchesAvailabilityKindUnavailable_ThenFiresInmediately()
		{
			var network = new Mock<NetworkAvailabilityTrigger.INetworkStatus>();
			network.SetupGet(x => x.IsAvailable).Returns(false);
			var trigger = new NetworkAvailabilityTrigger(NetworkAvailabilityKind.Unavailable, network.Object);

			bool fired = false;
			trigger.Fired += (sender, e) => fired = true;

			trigger.IsEnabled = true;

			Assert.IsTrue(fired);
		}

		[TestMethod]
		public void WhenTriggerIsEnabledAndNetworkStatusMatchesAvailabilityKindBoth_ThenFiresInmediately()
		{
			var network = new Mock<NetworkAvailabilityTrigger.INetworkStatus>();
			network.SetupGet(x => x.IsAvailable).Returns(false);
			var trigger = new NetworkAvailabilityTrigger(NetworkAvailabilityKind.Both, network.Object);

			bool fired = false;
			trigger.Fired += (sender, e) => fired = true;

			trigger.IsEnabled = true;

			Assert.IsTrue(fired);
		}

		[TestMethod]
		public void WhenNetworkStatusBecomesAvailable_ThenFires()
		{
			var network = new Mock<NetworkAvailabilityTrigger.INetworkStatus>();
			network.SetupGet(x => x.IsAvailable).Returns(false);
			var trigger = new NetworkAvailabilityTrigger(NetworkAvailabilityKind.Available, network.Object);

			bool fired = false;
			trigger.Fired += (sender, e) => fired = true;

			trigger.IsEnabled = true;

			Assert.IsFalse(fired);

			network.SetupGet(x => x.IsAvailable).Returns(true);
			network.Raise(x => x.IsAvailableChanged += null, EventArgs.Empty);

			Assert.IsTrue(fired);
		}

		[TestMethod]
		public void WhenNetworkStatusBecomesUnavailable_ThenFires()
		{
			var network = new Mock<NetworkAvailabilityTrigger.INetworkStatus>();
			network.SetupGet(x => x.IsAvailable).Returns(true);
			var trigger = new NetworkAvailabilityTrigger(NetworkAvailabilityKind.Unavailable, network.Object);

			bool fired = false;
			trigger.Fired += (sender, e) => fired = true;

			trigger.IsEnabled = true;

			Assert.IsFalse(fired);

			network.SetupGet(x => x.IsAvailable).Returns(false);
			network.Raise(x => x.IsAvailableChanged += null, EventArgs.Empty);

			Assert.IsTrue(fired);
		}
	}
}
