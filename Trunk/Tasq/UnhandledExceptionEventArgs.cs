using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	/// <summary>
	/// Arguments for the <see cref="Job.UnhandledException"/> event.
	/// </summary>
	public class UnhandledExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UnhandledExceptionEventArgs"/> class.
		/// </summary>
		/// <param name="exception">The exception that was not handled on the job run.</param>
		public UnhandledExceptionEventArgs(Exception exception)
		{
			this.Exception = exception;
		}

		/// <summary>
		/// Gets the exception throw on the job run.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception has been handled and the job can 
		/// remain enabled.
		/// </summary>
		public bool Handled { get; set; }
	}
}
