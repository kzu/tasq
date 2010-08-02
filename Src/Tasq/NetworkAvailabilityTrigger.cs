using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.ComponentModel;

namespace Tasq
{
	/// <summary>
	/// Kind of network availability that a <see cref="NetworkAvailabilityTrigger"/> 
	/// will monitor.
	/// </summary>
	public enum NetworkAvailabilityKind
	{
		/// <summary>
		/// Monitor whenever the network becomes available.
		/// </summary>
		Available,
		/// <summary>
		/// Monitor whenever the network becomes unavailable.
		/// </summary>
		Unavailable,
		/// <summary>
		/// Monitor whenever the network becomes available or unavailable.
		/// </summary>
		Both,
	}

	/// <summary>
	/// A trigger that fires when the current network availability matches the given 
	/// <see cref="NetworkAvailabilityKind"/>.
	/// </summary>
	public class NetworkAvailabilityTrigger : ITrigger, IDisposable
	{
		private INetworkStatus networkStatus;
		private bool isEnabled;

		/// <summary>
		/// Occurs when the trigger fires.
		/// </summary>
		public event EventHandler Fired = (sender, args) => { };

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkAvailabilityTrigger"/> class.
		/// </summary>
		public NetworkAvailabilityTrigger(NetworkAvailabilityKind availabilityKind)
			: this(availabilityKind, new NetworkStatus())
		{
		}

		internal NetworkAvailabilityTrigger(NetworkAvailabilityKind availabilityKind, INetworkStatus networkStatus)
		{
			this.AvailabilityKind = availabilityKind;
			this.networkStatus = networkStatus;
		}

		void OnAvailabilityChanged(object sender, EventArgs e)
		{
			Fired(this, EventArgs.Empty);
		}

		/// <summary>
		/// Gets or sets the type of network availability that the trigger will monitor for changes.
		/// </summary>
		public NetworkAvailabilityKind AvailabilityKind { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this trigger is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get { return this.isEnabled; }
			set
			{
				if (value == this.isEnabled)
					return;

				this.isEnabled = value;
				if (this.isEnabled)
				{
					DoFire();
					this.networkStatus.IsAvailableChanged += OnAvailabilityChanged;
				}
				else
				{
					this.networkStatus.IsAvailableChanged -= OnAvailabilityChanged;
				}
			}
		}

		/// <summary>
		/// Disposes the trigger and stops listening for network status changes.
		/// </summary>
		public void Dispose()
		{
			this.networkStatus.IsAvailableChanged -= OnAvailabilityChanged;
		}

		private void DoFire()
		{
			if (this.networkStatus.IsAvailable)
			{
				if (this.AvailabilityKind == NetworkAvailabilityKind.Available ||
					this.AvailabilityKind == NetworkAvailabilityKind.Both)
					Fired(this, EventArgs.Empty);
			}
			else
			{
				if (this.AvailabilityKind == NetworkAvailabilityKind.Unavailable ||
					this.AvailabilityKind == NetworkAvailabilityKind.Both)
					Fired(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Interfaces for testing.
		/// </summary>
		internal interface INetworkStatus
		{
			event EventHandler IsAvailableChanged;
			bool IsAvailable { get; }
		}

		internal class NetworkStatus : INetworkStatus
		{
			public event EventHandler IsAvailableChanged = (sender, args) => { };

			public NetworkStatus()
			{
				NetworkChange.NetworkAvailabilityChanged += (sender, args) => IsAvailableChanged(this, EventArgs.Empty);
			}

			public bool IsAvailable
			{
				get { return NetworkInterface.GetIsNetworkAvailable(); }
			}
		}
	}
}
