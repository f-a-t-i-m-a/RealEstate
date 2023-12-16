using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Domain.Property;
using Lucene.Net.Documents;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Contract]
	[Component]
	[ComponentCache(null)]
	public class PropertyListingIndexMap : ObjectIndexMapperBase<PropertyListing>
	{
		public static class FieldNames
		{
			public const string ID = "ID";
		}

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<PropertyListingContent> PropertyListingContentSerializer { get; set; }

		private readonly NumericField _idField;

		public PropertyListingIndexMap()
		{
			//
			// Initialize fields

			_idField = new NumericField(AgencyIndexMap.FieldNames.ID, Field.Store.YES, true);

			//
			// Add fields to the document

			Document.Add(_idField);
		}

		#region Vicinity content mapping

		protected override void SetFieldValues(PropertyListing request)
		{
			PropertyListingContentSerializer.DeserializeIfNeeded(request);
			_idField.SetLongValue(request.ID);
		}

		#endregion
	}
}