using System.Globalization;
using System.Linq;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Util.Resources;
using Lucene.Net.Documents;

namespace JahanJooy.RealEstate.Core.Impl.Cache
{
	public class VicinityCacheIndexMap : ObjectIndexMapperBase<Vicinity>
	{
		public static class FieldNames
		{
			public const string ID = "ID";
			public const string Name = "Name";
			public const string AlternativeNames = "AlternativeNames";
			public const string AdditionalSearchText = "AdditionalSearchText";
			public const string Enabled = "Enabled";
			public const string Type = "Type";
			public const string CanContainPropertyRecords = "CanContainPropertyRecords";

			public const string ParentsIDs = "ParentsIDs";
			public const string LocalizedType = "LocalizedType";
			public const string SearchText = "SearchText";
			public const string ParentsSearchText = "ParentsSearchText";
		}

		private readonly NumericField _idField;
		private readonly Field _nameField;
		private readonly Field _alternativeNamesField;
		private readonly Field _additionalSearchTextField;
		private readonly Field _enabledField;
		private readonly Field _typeField;
		private readonly Field _canContainPropertyRecordsField;
		
		private readonly Field _parentsIdsField;
		private readonly Field _localizedTypeField;
		private readonly Field _searchTextField;
		private readonly Field _parentsSearchTextField;

		public VicinityCacheIndexMap()
		{
			//
			// Initialize fields

			_idField = new NumericField(FieldNames.ID, Field.Store.YES, true);
			_nameField = new Field(FieldNames.Name, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_alternativeNamesField = new Field(FieldNames.AlternativeNames, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_additionalSearchTextField = new Field(FieldNames.AdditionalSearchText, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_enabledField = new Field(FieldNames.Enabled, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);
			_typeField = new Field(FieldNames.Type, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);
			_canContainPropertyRecordsField = new Field(FieldNames.CanContainPropertyRecords, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);

			_parentsIdsField = new Field(FieldNames.ParentsIDs, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_localizedTypeField = new Field(FieldNames.LocalizedType, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_searchTextField = new Field(FieldNames.SearchText, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_parentsSearchTextField = new Field(FieldNames.ParentsSearchText, string.Empty, Field.Store.NO, Field.Index.ANALYZED);

			//
			// Add fields to the document

			Document.Add(_idField);
			Document.Add(_nameField);
			Document.Add(_alternativeNamesField);
			Document.Add(_additionalSearchTextField);
			Document.Add(_enabledField);
			Document.Add(_typeField);
			Document.Add(_canContainPropertyRecordsField);

			Document.Add(_parentsIdsField);
			Document.Add(_localizedTypeField);
			Document.Add(_searchTextField);
			Document.Add(_parentsSearchTextField);
		}

		#region Vicinity content mapping

		protected override void SetFieldValues(Vicinity vicinity)
		{
			var parentsIdsValue = string.Join(" ", vicinity.GetParents().Select(v => v.ID.ToString(CultureInfo.InvariantCulture)));
			var localizedTypeValue = GetLocalizedTypeValue(vicinity);
			var searchTextValue = GetSearchTextValue(vicinity);

			_idField.SetLongValue(vicinity.ID);
			_nameField.SetValue(vicinity.Name ?? string.Empty);
			_alternativeNamesField.SetValue(vicinity.AlternativeNames ?? string.Empty);
			_additionalSearchTextField.SetValue(vicinity.AdditionalSearchText ?? string.Empty);
			_enabledField.SetValue(vicinity.GetParentsInclusive().All(v => v.Enabled).ToString());
			_typeField.SetValue(vicinity.Type.ToString());
			_canContainPropertyRecordsField.SetValue(vicinity.CanContainPropertyRecords.ToString());

			_parentsIdsField.SetValue(parentsIdsValue);
			_localizedTypeField.SetValue(localizedTypeValue);
			_searchTextField.SetValue(searchTextValue);
			_parentsSearchTextField.SetValue(string.Join(" ", vicinity.GetParents().Select(GetSearchTextValue)));
		}

		private static string GetLocalizedTypeValue(Vicinity vicinity)
		{
			return vicinity.ShowTypeInTitle ? vicinity.Type.Label(DomainEnumResources.ResourceManager) : string.Empty;
		}

		private static string GetSearchTextValue(Vicinity vicinity)
		{
			return string.Join(" ", vicinity.Name, vicinity.AlternativeNames, vicinity.AdditionalSearchText, GetLocalizedTypeValue(vicinity));
		}

		#endregion
	}
}