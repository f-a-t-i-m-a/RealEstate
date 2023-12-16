using System.Configuration;

namespace Compositional.Composer.Web.Configuration
{
	public class SetupCompositionXmlsCollection : ConfigurationElementCollection
	{
		#region Overrides of ConfigurationElementCollection

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new SetupCompositionXml();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((SetupCompositionXml) element).Key;
		}

		#endregion
	}
}