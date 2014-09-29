using System;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
	public static class StringExtensions
	{
		private static readonly Regex whitespace = new Regex(@"[ \t]{2,}", RegexOptions.Compiled);
		private static readonly Regex whitespaceWithBreaks = new Regex(@"\s{2,}", RegexOptions.Compiled);

		/// <summary>
		/// Removes all leading and trailing whitespace and replaces consecutive whitespace characters with a single space.
		/// </summary>
		public static string TrimWhitespace(this string value)
		{
			return TrimWhitespace(value, " ", false);
		}

		/// <summary>
		/// Removes all leading and trailing whitespace and replaces consecutive whitespace characters with the replacement string specified.
		/// </summary>
		/// <param name="replacement">The string which replaces consecutive whitespace</param>
		public static string TrimWhitespace(this string value, string replacement)
		{
			return TrimWhitespace(value, replacement, false);
		}

		/// <summary>
		/// Removes all leading and trailing whitespace and replaces consecutive whitespace characters with a single space.
		/// </summary>
		/// <param name="treatLinebreaksAsWhitespace">If true, treats carriage returns (CR, \r) and line feeds (LF, \n) as whitspace</param>
		public static string TrimWhitespace(this string value, bool treatLinebreaksAsWhitespace)
		{
			return TrimWhitespace(value, " ", treatLinebreaksAsWhitespace);
		}

		/// <summary>
		/// Removes all leading and trailing whitespace and replaces consecutive whitespace characters with the replacement string specified.
		/// </summary>
		/// <param name="replacement">The string which replaces consecutive whitespace</param>
		/// <param name="treatLinebreaksAsWhitespace">If true, treats carriage returns (CR, \r) and line feeds (LF, \n) as whitspace</param>
		public static string TrimWhitespace(this string value, string replacement, bool treatLinebreaksAsWhitespace)
		{
			if (value == null)
				return null;

			if (treatLinebreaksAsWhitespace)
				return whitespaceWithBreaks.Replace(value, replacement).Trim();
			else
				return whitespace.Replace(value, replacement).Trim();
		}

		/// <summary>
		/// Removes all leading and trailing white-space characters. Returns null if the string is null.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string TrimNullSafe(this string value)
		{
			if (value == null)
				return null;

			return value.Trim();
		}

		/// <summary>
		/// Truncates the string to the specified length. If the string is shorter than or equal to the length, the string is returned as is. If the string is null, a null string is returned.
		/// </summary>
		/// <param name="length">The length to truncate the string at</param>
		public static string Truncate(this string value, int length)
		{
			if (value == null || value.Length <= length)
				return value;
			else
				return value.Substring(0, length);
		}

		/// <summary>
		/// Replaces the format item in a specified string with the string representation of a corresponding object in a specified array. 
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
		public static string FormatString(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		/// <summary>
		/// Replaces the format items in a specified string with the string representations of corresponding objects in a specified array. A parameter supplies culture-specific formatting information.
		/// </summary>
		/// <param name="format">A composite format string.</param>
		/// <param name="provider">An object that supplies culture-specific formatting information.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A copy of format in which the format items have been replaced by the string representation of the corresponding objects in args.</returns>
		public static string FormatString(this string format, IFormatProvider provider, params object[] args)
		{
			return string.Format(provider, format, args);
		}
	}
}