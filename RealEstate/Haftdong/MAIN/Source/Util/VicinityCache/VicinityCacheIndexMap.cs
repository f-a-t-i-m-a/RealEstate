using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstateAgency.Domain.Vicinities;
using JahanJooy.RealEstateAgency.Util.Resources;
using Lucene.Net.Documents;

namespace JahanJooy.RealEstateAgency.Util.VicinityCache
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    public class VicinityCacheIndexMap : ObjectIndexMapperBase<Vicinity>
    {
        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

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

        private readonly Field _idField;
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
            // Initialize fields
            _idField = new Field(FieldNames.ID, string.Empty, Field.Store.YES, Field.Index.NO);
            _nameField = new Field(FieldNames.Name, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
            _alternativeNamesField = new Field(FieldNames.AlternativeNames, string.Empty, Field.Store.NO,
                Field.Index.ANALYZED);
            _additionalSearchTextField = new Field(FieldNames.AdditionalSearchText, string.Empty, Field.Store.NO,
                Field.Index.ANALYZED);
            _enabledField = new Field(FieldNames.Enabled, string.Empty, Field.Store.NO,
                Field.Index.NOT_ANALYZED_NO_NORMS);
            _typeField = new Field(FieldNames.Type, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);
            _canContainPropertyRecordsField = new Field(FieldNames.CanContainPropertyRecords, string.Empty,
                Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);

            _parentsIdsField = new Field(FieldNames.ParentsIDs, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
            _localizedTypeField = new Field(FieldNames.LocalizedType, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
            _searchTextField = new Field(FieldNames.SearchText, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
            _parentsSearchTextField = new Field(FieldNames.ParentsSearchText, string.Empty, Field.Store.NO,
                Field.Index.ANALYZED);

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
            var parentsIdsValue = string.Join(" ", vicinity.GetParents().Select(v => v.ID.ToString()));
            var localizedTypeValue = GetLocalizedTypeValue(vicinity);
            var searchTextValue = GetSearchTextValue(vicinity);

            _idField.SetValue(vicinity.ID.ToString());
            _nameField.SetValue(vicinity.Name ?? string.Empty);
            _alternativeNamesField.SetValue(vicinity.AlternativeNames ?? string.Empty);
            _additionalSearchTextField.SetValue(vicinity.AdditionalSearchText ?? string.Empty);

            var currentVicinity = vicinity;
            var enabled = true;
            while (currentVicinity != null)
            {
                enabled &= currentVicinity.Enabled;
                currentVicinity = currentVicinity.Parent;
            }
            _enabledField.SetValue(enabled.ToString());

            _typeField.SetValue(vicinity.Type.ToString());
            _canContainPropertyRecordsField.SetValue(vicinity.CanContainPropertyRecords.ToString());

            _parentsIdsField.SetValue(parentsIdsValue);
            _localizedTypeField.SetValue(localizedTypeValue);
            _searchTextField.SetValue(searchTextValue);
            currentVicinity = vicinity;
            var parentList = new List<string>();
            while (currentVicinity.Parent != null)
            {
                parentList.Add(GetSearchTextValue(currentVicinity.Parent));
                currentVicinity = currentVicinity.Parent;
            }
            _parentsSearchTextField.SetValue(string.Join(" ", parentList));
        }

        private static string GetLocalizedTypeValue(Vicinity vicinity)
        {
            return vicinity.ShowTypeInTitle ? vicinity.Type.Label(StaticEnumResources.ResourceManager) : string.Empty;
        }

        private static string GetSearchTextValue(Vicinity vicinity)
        {
            return string.Join(" ", vicinity.Name, vicinity.AlternativeNames, vicinity.AdditionalSearchText,
                GetLocalizedTypeValue(vicinity));
        }

        #endregion
    }
}