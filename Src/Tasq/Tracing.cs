using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Tasq
{
	/// <summary>
	/// Provides access to the library trace functionality.
	/// </summary>
	public static class Tracing
	{
		static Tracing()
		{
			TraceSource = new TraceSource(typeof(Tracing).Namespace, SourceLevels.Warning);
		}

		/// <summary>
		/// Gets the <see cref="TraceSource"/> used across the library for tracing. 
		/// </summary>
		/// <remarks>
		/// The name of the source for configuring via the application configuration file matches the 
		/// root namespace, <c>FeedSync</c>. Runtime access to this source allows dynamic 
		/// configuration, filtering of events, etc.
		/// <para>
		/// Default tracing level for the library is set to warning.
		/// </para>
		/// </remarks>
		public static TraceSource TraceSource { get; private set; }

		internal static void TraceError(this TraceSource source, Exception exception, string message, params object[] args)
		{
			var logmessage = string.Format(message, args);

			source.TraceEvent(TraceEventType.Error, 0, logmessage + Environment.NewLine + exception.ToString());
		}

		internal static void TraceError(this TraceSource source, string message, params object[] args)
		{
			source.TraceEvent(TraceEventType.Error, 0, message, args);
		}

		internal static void TraceWarning(this TraceSource source, string message, params object[] args)
		{
			source.TraceEvent(TraceEventType.Warning, 0, message, args);
		}

		internal static void TraceWarning(this TraceSource source, Exception exception, string message, params object[] args)
		{
			var logmessage = string.Format(message, args);

			source.TraceEvent(TraceEventType.Warning, 0, logmessage + Environment.NewLine + exception.ToString());
		}

		internal static void TraceVerbose(this TraceSource source, string message, params object[] args)
		{
			source.TraceEvent(TraceEventType.Verbose, 0, message, args);
		}
	}
}
