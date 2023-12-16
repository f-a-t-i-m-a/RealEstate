using System.Collections.Generic;

namespace JahanJooy.RealEstate.Core.Search
{
	public interface IPropertySearchQueryProcessor
	{
		/// <summary>
		/// Retrieves list of strings that can be handled by this processor if seen in a query segment.
		/// </summary>
		/// <returns>List of strings that can be handled by this processor if seen in a query segment</returns>
		IEnumerable<string> GetParsableKeywords();

		/// <summary>
		/// Tries to parse one or more segments of a query related to this processor.
		/// </summary>
		/// <param name="segments">List of segments</param>
		/// <param name="index">Index of the segment in the segments list which should be parsed</param>
		/// <param name="result">Parse result where the parsed segments should be applied</param>
		/// <returns>Number of segments that could be parsed and is applied to the result</returns>
		int ParseQuerySegment(List<string> segments, int index, PropertySearch result);

		/// <summary>
		/// Possibly generates query segments related to this processor
		/// </summary>
		/// <param name="propertySearch">The source from which query should be generated</param>
		/// <param name="segments">Where the generated segments should be placed</param>
		/// <returns>True if anything is added to the result segments, false otherwise.</returns>
		bool GenerateQuerySegment(PropertySearch propertySearch, List<string> segments);
	}
}