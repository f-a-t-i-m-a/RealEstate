using Compositional.Composer;
using JahanJooy.Common.Util.Search;
using JahanJooy.RealEstate.Core.Impl.Index.Base;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Component]
	public class PropertyRequestIndex : EntityIndexBase<PropertyRequest>, IPropertyRequestIndex
	{
		private const string IndexIDString = "PropertyRequest";

		#region Initialization

		static PropertyRequestIndex()
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
			get { return PropertyRequestIndexMap.FieldNames.ID; }
		}

		protected override long GetEntityID(PropertyRequest entity)
		{
			return entity.ID;
		}

		#endregion
	}
}