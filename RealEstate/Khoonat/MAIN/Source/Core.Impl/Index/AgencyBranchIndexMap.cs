using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Domain.Directory;
using Lucene.Net.Documents;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Contract]
	[Component]
	[ComponentCache(null)]
	public class AgencyBranchIndexMap : ObjectIndexMapperBase<AgencyBranch>
	{
		public static class FieldNames
		{
			public const string ID = "ID";
			public const string AgencyID = "AgencyID";
			public const string BranchName = "BranchName";
			public const string BranchManagerName = "BranchManagerName";
			public const string VicinityIds = "VicinityIds";
			public const string GeographicLocationLat = "GeographicLocationLat";
			public const string GeographicLocationLng = "GeographicLocationLng";
			public const string GeographicLocationType = "GeographicLocationType";
		}

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyContent> AgencyContentSerializer { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyBranchContent> AgencyBranchContentSerializer { get; set; }

		private readonly NumericField _idField;
		private readonly NumericField _agencyIdField;
		private readonly Field _branchNameField;
		private readonly Field _branchManagerNameField;
		private readonly Field _vicinityIdsField;
		private readonly NumericField _geographicLocationLatField;
		private readonly NumericField _geographicLocationLngField;
		private readonly Field _geographicLocationTypeField;

		public AgencyBranchIndexMap()
		{
			//
			// Initialize fields

			_idField = new NumericField(FieldNames.ID, Field.Store.YES, true);
			_agencyIdField = new NumericField(FieldNames.AgencyID, Field.Store.NO, true);
			_branchNameField = new Field(FieldNames.BranchName, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_branchManagerNameField = new Field(FieldNames.BranchManagerName, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_vicinityIdsField = new Field(FieldNames.VicinityIds, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_geographicLocationLatField = new NumericField(FieldNames.GeographicLocationLat, Field.Store.NO, true);
			_geographicLocationLngField = new NumericField(FieldNames.GeographicLocationLng, Field.Store.NO, true);
			_geographicLocationTypeField = new Field(FieldNames.GeographicLocationType, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);

			//
			// Add fields to the document

			Document.Add(_idField);
			Document.Add(_agencyIdField);
			Document.Add(_branchNameField);
			Document.Add(_branchManagerNameField);
			Document.Add(_vicinityIdsField);
			Document.Add(_geographicLocationLatField);
			Document.Add(_geographicLocationLngField);
			Document.Add(_geographicLocationTypeField);
		}

		#region Vicinity content mapping

		protected override void SetFieldValues(AgencyBranch branch)
		{
			AgencyBranchContentSerializer.DeserializeIfNeeded(branch);
			if (branch.Agency != null)
				AgencyContentSerializer.DeserializeIfNeeded(branch.Agency);

			var vicinityIdsValue = VicinityCache[branch.Content.VicinityID].IfNotNull(v => v.GetParentIDsInclusive().Join(" "));
			var latLng = branch.Content.GeographicLocation.IfNotNull(g => g.ToLatLng());

			_idField.SetLongValue(branch.ID);
			_agencyIdField.SetLongValue(branch.AgencyID);
			_branchNameField.SetValue(branch.Content.BranchName ?? string.Empty);
			_branchManagerNameField.SetValue(branch.Content.BranchManagerName ?? string.Empty);
			_vicinityIdsField.SetValue(vicinityIdsValue ?? string.Empty);
			_geographicLocationLatField.SetDoubleValue(latLng.IfNotNull(ll => ll.Lat));
			_geographicLocationLngField.SetDoubleValue(latLng.IfNotNull(ll => ll.Lng));
			_geographicLocationTypeField.SetValue(branch.Content.GeographicLocationType.IfHasValue(t => t.ToString()) ?? string.Empty);
		}

		#endregion
	}
}