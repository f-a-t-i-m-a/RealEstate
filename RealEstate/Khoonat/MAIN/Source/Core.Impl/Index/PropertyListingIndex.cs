using Compositional.Composer;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Impl.Index.Base;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Component]
	public class PropertyListingIndex : EntityIndexBase<PropertyListing>, IPropertyListingIndex
	{
		private const string IndexIDString = "PropertyListing";

		#region Initialization

		static PropertyListingIndex()
		{
			LuceneIndexManager.RegisterConfigurationKeys(IndexIDString);
		}

		[OnCompositionComplete]
		public void OnCompositionComplete()
		{
			IndexManager.InitializeIndex(IndexIDString);
		}

		#endregion

		#region Overrides

		public override string IndexID
		{
			get { return IndexIDString; }
		}

		protected override string IdentityFieldName
		{
			get { return PropertyListingIndexMap.FieldNames.ID; }
		}

		protected override long GetEntityID(PropertyListing entity)
		{
			return entity.ID;
		}

		#endregion

	}
}