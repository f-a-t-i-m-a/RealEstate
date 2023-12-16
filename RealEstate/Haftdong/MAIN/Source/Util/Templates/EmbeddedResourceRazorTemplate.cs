using System;
using System.IO;
using System.Reflection;
using JahanJooy.RealEstateAgency.Util.Templates;

namespace JahanJooy.RealEstateAgency.Util.Templates
{
	public class EmbeddedResourceRazorTemplate : IRazorTemplate
	{
		private readonly Assembly _assembly;
		private readonly string _resourceName;

		public EmbeddedResourceRazorTemplate(Assembly assembly, string resourceName)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			if (string.IsNullOrEmpty(resourceName))
				throw new ArgumentNullException("resourceName");

			_assembly = assembly;
			_resourceName = resourceName;
		}

		public TextReader GetReader()
		{
			var manifestResourceStream = _assembly.GetManifestResourceStream(_resourceName);
			if (manifestResourceStream == null)
				throw new InvalidOperationException("Manifest resource " + _resourceName + " not found in assembly " + _assembly.FullName);

			return new StreamReader(manifestResourceStream);
		}
	}
}