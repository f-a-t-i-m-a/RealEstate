﻿namespace JahanJooy.Common.Util.Web.Routing
{
	/// <summary>
	/// Represents a route segment that may as well be greedy (catch-all).
	/// 
	/// Copied from:
	/// http://erraticdev.blogspot.de/2011/01/custom-aspnet-mvc-route-class-with.html
	/// 
	/// </summary>
	public class GreedyRouteSegment
	{
		/// <summary>
		/// Gets or sets segment path or token name.
		/// </summary>
		/// <value>Route segment path or token name.</value>
		public string Name { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether this segment is greedy.
		/// </summary>
		/// <value><c>true</c> if this segment is greedy; otherwise, <c>false</c>.</value>
		public bool IsGreedy { get; set; }


		/// <summary>
		/// Gets or sets a value indicating whether this segment is a token.
		/// </summary>
		/// <value><c>true</c> if this segment is a token; otherwise, <c>false</c>.</value>
		public bool IsToken { get; set; }
	}
}