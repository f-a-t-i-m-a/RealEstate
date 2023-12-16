using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Domain.Directory;
using Lucene.Net.Documents;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Contract]
	[Component]
	[ComponentCache(null)]
	public class AgencyIndexMap : ObjectIndexMapperBase<Agency>
	{
		public static class FieldNames
		{
			public const string ID = "ID";
			public const string Name = "Name";
			public const string ManagerName = "ManagerName";
			public const string VicinityIds = "VicinityIds";
            public const string Deleted = "Deleted";
		}

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyContent> AgencyContentSerializer { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyBranchContent> AgencyBranchContentSerializer { get; set; }

		private readonly NumericField _idField;
		private readonly Field _nameField;
		private readonly Field _managerNameField;
		private readonly Field _vicinityIdsField;
        private readonly Field _deletedField;

		public AgencyIndexMap()
		{
			//
			// Initialize fields

			_idField = new NumericField(FieldNames.ID, Field.Store.YES, true);
			_nameField = new Field(FieldNames.Name, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_managerNameField = new Field(FieldNames.ManagerName, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_vicinityIdsField = new Field(FieldNames.VicinityIds, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
            _deletedField = new Field(FieldNames.Deleted, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);

			//
			// Add fields to the document

			Document.Add(_idField);
			Document.Add(_nameField);
			Document.Add(_managerNameField);
			Document.Add(_vicinityIdsField);
            Document.Add(_deletedField);
		}

		#region Vicinity content mapping

		protected override void SetFieldValues(Agency agency)
		{
			AgencyContentSerializer.DeserializeIfNeeded(agency);
			agency.AgencyBranches.SafeForEach(branch => AgencyBranchContentSerializer.DeserializeIfNeeded(branch));

			var vicinityIdsValue = agency.AgencyBranches.EmptyIfNull()
					.Select(b => VicinityCache[b.Content.VicinityID])
					.WhereNotNull()
					.SelectMany(v => v.GetParentIDsInclusive())
					.Distinct()
					.Join(" ");

			_idField.SetLongValue(agency.ID);
			_nameField.SetValue(agency.Content.Name ?? string.Empty);
			_managerNameField.SetValue(agency.Content.ManagerName ?? string.Empty);
			_vicinityIdsField.SetValue(vicinityIdsValue ?? string.Empty);
            _deletedField.SetValue(agency.DeleteTime.HasValue.ToString() );
		}

		#endregion
	}
}