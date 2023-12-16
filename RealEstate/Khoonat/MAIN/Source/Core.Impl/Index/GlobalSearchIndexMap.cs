using System;
using System.Globalization;
using Compositional.Composer;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using JahanJooy.RealEstate.Domain.Enums;
using Lucene.Net.Documents;
using ServiceStack;

namespace JahanJooy.RealEstate.Core.Impl.Index
{
	[Contract]
	[Component]
	[ComponentCache(null)]
	public class GlobalSearchIndexMap : ObjectIndexMapperBase<GlobalSearchIndexItem>, ITwoWayObjectIndexMapper<GlobalSearchIndexItem, GlobalSearchResultItem>
	{
		public static class FieldNames
		{
			public const string Type = "Type";
			public const string SubType = "SubType";
			public const string ID = "ID";
			public const string Title = "Title";

			public const string VicinityId = "VicinityId";
			public const string GeographicLocationLat = "GeographicLocationLat";
			public const string GeographicLocationLng = "GeographicLocationLng";
			public const string GeographicLocationType = "GeographicLocationType";

			public const string VicinityIdHierarchy = "VicinityIdHierarchy";
			public const string Text = "Text";
			public const string Tags = "Tags";

			public const string PrincipalsAllowedToView = "PrincipalsAllowedToView";
			public const string Archived = "Archived";
			public const string Deleted = "Deleted";
		}

		[ComponentPlug]
		public IVicinityCache VicinityCache { get; set; }


		private readonly Field _typeField;
		private readonly Field _subTypeField;
		private readonly NumericField _idField;
		private readonly Field _titleField;

		private readonly Field _vicinityIdField;
		private readonly NumericField _geographicLocationLatField;
		private readonly NumericField _geographicLocationLngField;
		private readonly Field _geographicLocationTypeField;

		private readonly Field _vicinityIdHierarchyField;
		private readonly Field _textField;
		private readonly Field _tagsField;

		private readonly Field _principalsAllowedToViewField;
		private readonly Field _archivedField;
		private readonly Field _deletedField;

		public GlobalSearchIndexMap()
		{
			//
			// Initialize fields

			_typeField = new Field(FieldNames.Type, string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
			_subTypeField = new Field(FieldNames.SubType, string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
			_idField = new NumericField(FieldNames.ID, Field.Store.YES, true);
			_titleField = new Field(FieldNames.Title, string.Empty, Field.Store.YES, Field.Index.ANALYZED);

			_vicinityIdField = new Field(FieldNames.VicinityId, string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);
			_geographicLocationLatField = new NumericField(FieldNames.GeographicLocationLat, Field.Store.YES, true);
			_geographicLocationLngField = new NumericField(FieldNames.GeographicLocationLng, Field.Store.YES, true);
			_geographicLocationTypeField = new Field(FieldNames.GeographicLocationType, string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS);

			_vicinityIdHierarchyField = new Field(FieldNames.VicinityIdHierarchy, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_textField = new Field(FieldNames.Text, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_tagsField = new Field(FieldNames.Tags, string.Empty, Field.Store.NO, Field.Index.ANALYZED);

			_principalsAllowedToViewField = new Field(FieldNames.PrincipalsAllowedToView, string.Empty, Field.Store.NO, Field.Index.ANALYZED);
			_archivedField = new Field(FieldNames.Archived, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);
			_deletedField = new Field(FieldNames.Deleted, string.Empty, Field.Store.NO, Field.Index.NOT_ANALYZED_NO_NORMS);

			//
			// Add fields to the document

			Document.Add(_typeField);
			Document.Add(_subTypeField);
			Document.Add(_idField);
            Document.Add(_titleField);

            Document.Add(_vicinityIdField);
            Document.Add(_geographicLocationLatField);
            Document.Add(_geographicLocationLngField);
            Document.Add(_geographicLocationTypeField);

            Document.Add(_vicinityIdHierarchyField);
            Document.Add(_textField);
            Document.Add(_tagsField);

            Document.Add(_principalsAllowedToViewField);
            Document.Add(_archivedField);
            Document.Add(_deletedField);
		}

		#region Object content mapping

		protected override void SetFieldValues(GlobalSearchIndexItem item)
		{
			var vicinityIdHierarchy = item.VicinityID
				.IfHasValue(vid => VicinityCache[vid])
				.IfNotNull(v => v.GetParentIDsInclusive().Join(" "), string.Empty);

			_typeField.SetValue(item.Type.ToString());
			_subTypeField.SetValue(item.SubType ?? string.Empty);
			_idField.SetLongValue(item.ID);
			_titleField.SetValue(item.Title ?? string.Empty);

			_vicinityIdField.SetValue(item.VicinityID.IfHasValue(vid => vid.ToString(CultureInfo.InvariantCulture), string.Empty));
			_geographicLocationLatField.SetDoubleValue(item.GeographicLocation.IfNotNull(ll => ll.Lat));
			_geographicLocationLngField.SetDoubleValue(item.GeographicLocation.IfNotNull(ll => ll.Lng));
			_geographicLocationTypeField.SetValue(item.GeographicLocationType.IfHasValue(t => t.ToString()) ?? string.Empty);

			_vicinityIdHierarchyField.SetValue(vicinityIdHierarchy);
			_textField.SetValue(item.Text);
			_tagsField.SetValue(item.Tags.IfNotNull(tags => tags.Join(" "), string.Empty));

			_principalsAllowedToViewField.SetValue(item.PrincipalsAllowedToView.IfNotNull(ps => ps.Join(" "), string.Empty));
			_archivedField.SetValue(item.Archived.ToString());
			_deletedField.SetValue(item.Deleted.ToString());
		}

		public GlobalSearchResultItem GetObject(Document document)
		{
			if (document == null)
				return null;

			var result = new GlobalSearchResultItem();
			result.SubType = document.Get(FieldNames.SubType);
			result.Title = document.Get(FieldNames.Title);

			GlobalSearchRecordType type;
			if (Enum.TryParse(document.Get(FieldNames.Type), out type))
				result.Type = type;

			long id;
			if (long.TryParse(document.Get(FieldNames.ID), out id))
				result.ID = id;

			long vicinityId;
			if (long.TryParse(document.Get(FieldNames.VicinityId), out vicinityId))
				result.VicinityID = vicinityId;

			double geoLocationLat;
			double geoLocationLng;

			if (double.TryParse(document.Get(FieldNames.GeographicLocationLat), out geoLocationLat) &&
			    double.TryParse(document.Get(FieldNames.GeographicLocationLng), out geoLocationLng))
			{
				if (Math.Abs(geoLocationLat) > 0.001 && Math.Abs(geoLocationLng) > 0.001)
					result.GeographicLocation = new LatLng {Lat = geoLocationLat, Lng = geoLocationLng};
			}

			GeographicLocationSpecificationType geographicLocationType;
			if (Enum.TryParse(document.Get(FieldNames.GeographicLocationType), out geographicLocationType))
				result.GeographicLocationType = geographicLocationType;

			return result;
		}

		#endregion
	}
}