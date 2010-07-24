using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tasq
{
	internal static class Extensions
	{
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				collection.Add(item);
			}
		}

		public static void Apply<T>(this IEnumerable<T> source, Action<T> action, Action<T> before = null, Action<T> after = null)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}

		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (var item in source)
			{
				action(item);
				yield return item;
			}
		}

		public static void Try<T>(this T source, Action<T> action)
		{
			if (source != null && action != null)
			{
				try
				{
					action(source);
				}
				catch (Exception ex)
				{
					Tracing.TraceSource.TraceWarning(ex, "Failed to execute action on source {0}.", source);
				}
			}
		}
	}
}
