using Compositional.Composer;

namespace JahanJooy.Common.Util.Configuration
{
	[Contract]
	[Component]
	[IgnoredOnAssemblyRegistration]
	public class ApplicationRevisionInfo
	{
		[ConfigurationPoint]
		public string VersionString { get; set; }

		[ConfigurationPoint(false)]
		public string ApiVersionString { get; set; }
	}
}