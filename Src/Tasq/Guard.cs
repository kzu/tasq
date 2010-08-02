using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Tasq
{
	/// <summary>
	/// Common guard class for argument validation.
	/// </summary>
	[DebuggerStepThrough]
	internal static class Guard
	{
		/// <summary>
		/// Ensures the given <paramref name="value"/> is not null.
		/// Throws <see cref="ArgumentNullException"/> otherwise.
		/// </summary>
		public static void NotNull<T>(Expression<Func<T>> reference, T value)
		{
			if (value == null)
				throw new ArgumentNullException(GetParameterName(reference), "Parameter cannot be null.");
		}

		/// <summary>
		/// Ensures the given string <paramref name="value"/> is not null or empty.
		/// Throws <see cref="ArgumentNullException"/> in the first case, or 
		/// <see cref="ArgumentException"/> in the latter.
		/// </summary>
		public static void NotNullOrEmpty(Expression<Func<string>> reference, string value)
		{
			NotNull<string>(reference, value);
			if (value.Length == 0)
				throw new ArgumentException(GetParameterName(reference), "Parameter cannot be empty.");
		}

		private static string GetParameterName(Expression reference)
		{
			var lambda = reference as LambdaExpression;
			var member = lambda.Body as MemberExpression;

			return member.Member.Name;
		}
	}

}
