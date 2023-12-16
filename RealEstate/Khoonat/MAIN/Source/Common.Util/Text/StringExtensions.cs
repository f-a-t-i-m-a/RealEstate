using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security.AntiXss;

namespace JahanJooy.Common.Util.Text
{
	public static class StringExtensions
	{
		private static readonly Regex WhitespaceRegex = new Regex("\\s");

		public static string FormatIfNotNull(this string format, params object[] formatParams)
		{
			if (formatParams.Any(fp => fp == null))
				return null;

			return String.Format(format, formatParams);
		}

		public static HtmlString FormatAndEncodeIfNotNull(this string format, params object[] formatParams)
		{
			if (formatParams.Any(fp => fp == null))
				return null;

			return new HtmlString(String.Format(AntiXssEncoder.HtmlEncode(format, false),
			                                    formatParams.Select(fp => fp is HtmlString ? fp.ToString() : AntiXssEncoder.HtmlEncode(fp.ToString(), false)).Cast<object>().ToArray()));
		}

		public static string FormatIfNotEmpty(this string format, params string[] formatParams)
		{
			if (formatParams.Any(String.IsNullOrWhiteSpace))
				return null;

			return String.Format(format, formatParams.Cast<object>().ToArray());
		}

		public static HtmlString FormatAndEncodeIfNotEmpty(this string format, params string[] formatParams)
		{
			if (formatParams.Any(String.IsNullOrWhiteSpace))
				return null;

			return new HtmlString(String.Format(AntiXssEncoder.HtmlEncode(format, false),
			                                    formatParams.Select(p => AntiXssEncoder.HtmlEncode(p, false)).Cast<object>().ToArray()));
		}

		public static HtmlString FormatAndEncodeIfNotEmpty(this string format, params object[] formatParams)
		{
			if (formatParams.Any(fp => fp == null))
				return null;

			var formatParamStrings = formatParams.Select(fp => fp is HtmlString ? fp.ToString() : AntiXssEncoder.HtmlEncode(fp.ToString(), false)).ToArray();
			var result = AntiXssEncoder.HtmlEncode(format, false).FormatIfNotEmpty(formatParamStrings);

			return result == null ? null : new HtmlString(result);
		}

		public static string If(this string str, bool condition)
		{
			if (condition)
				return str;

			return null;
		}

		public static string If(this string str, bool? condition)
		{
			if (condition.HasValue && condition.Value)
				return str;

			return null;
		}

		public static TResult IfNotEmpty<TResult>(this string str, Func<string, TResult> f, TResult emptyValue)
		{
			return String.IsNullOrWhiteSpace(str) ? emptyValue : f(str);
		}

		public static TResult IfNotEmpty<TResult>(this string str, Func<string, TResult> f)
		{
			return String.IsNullOrWhiteSpace(str) ? default(TResult) : f(str);
		}

		public static string Truncate(this string str, int maxLength)
		{
			if (str == null)
				return null;

			if (str.Length <= maxLength)
				return str;

			return str.Substring(0, maxLength - 3) + "...";
		}

		public static string RemoveWhitespace(this string str)
		{
			if (str == null)
				return null;

			return WhitespaceRegex.Replace(str, "");
		}

		public static string[] SplitByWhitespace(this string str)
		{
			if (str == null)
				return null;

			return WhitespaceRegex.Split(str);
		}

		public static bool IsNullOrWhitespace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}

		public static bool HasText(this string str)
		{
			return !string.IsNullOrWhiteSpace(str);
		}

		public static IEnumerable<string> WhereNotNullOrEmpty(this IEnumerable<string> source)
		{
			return source == null ? null : source.Where(s => !String.IsNullOrEmpty(s));
		}

		public static IEnumerable<string> WhereNotNullOrWhitespace(this IEnumerable<string> source)
		{
			return source == null ? null : source.Where(s => !String.IsNullOrWhiteSpace(s));
		}
	}
}