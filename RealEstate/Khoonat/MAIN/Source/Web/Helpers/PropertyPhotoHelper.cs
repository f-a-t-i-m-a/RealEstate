using System.Globalization;
using System.Web;
using System.Web.Security.AntiXss;
using Compositional.Composer;
using JahanJooy.Common.Util.Text;
using JahanJooy.Common.Util.Web.Extensions;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util.Presentation.Property;
using JahanJooy.RealEstate.Util.Resources;
using JahanJooy.Common.Util;
using JahanJooy.RealEstate.Web.Resources.Helpers.Property;

namespace JahanJooy.RealEstate.Web.Helpers
{
	[Contract]
	[Component]
	public class PropertyPhotoHelper
	{
        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

		[ComponentPlug]
		public PropertyPresentationHelper PropertyPresentationHelper { get; set; }

		public static IHtmlString BuildPhotoTitle(PropertyListingPhoto photo)
		{
			if (string.IsNullOrWhiteSpace(photo.Title) && !photo.Subject.HasValue)
				return null;

			var result = StringUtils.JoinNonEmpty(" - ",
			                                      photo.Subject.Label(DomainEnumResources.ResourceManager),
			                                      photo.Title.IfNotNull(title => AntiXssEncoder.HtmlEncode(title, true)));

			return new HtmlString(result);
		}

		public string BuildFullSizePageTitle(PropertyListingPhoto photo, PropertyListing listing)
		{
			string listingTitle = PropertyPhotoHelperResources.PropertyListingCodeFormat.FormatIfNotEmpty(listing.Code.ToString(CultureInfo.InvariantCulture)) + ": " +
							listing.IntentionOfOwner.Label(DomainEnumResources.ResourceManager) + " " +
							listing.PropertyType.Label(DomainEnumResources.ResourceManager) + " ";
		    if (listing.VicinityID.HasValue)
		        listingTitle += " -";
            if (listing. VicinityID.HasValue && VicinityCache[listing.VicinityID.Value] != null)
                listingTitle += " " + VicinityCache[listing.VicinityID.Value].Name;

			if (photo.Subject.HasValue)
				return string.Format(PropertyPhotoHelperResources.FullSizeTitleWithSubjectFormat, photo.Subject.Label(DomainEnumResources.ResourceManager), listingTitle);

			return string.Format(PropertyPhotoHelperResources.FullSizeTitleWithoutSubjectFormat, listingTitle);
		}

		public string BuildFullSizePageDescription(PropertyListingPhoto photo, PropertyListing listing)
		{
			string result = BuildFullSizePageTitle(photo, listing);
			if (!string.IsNullOrWhiteSpace(photo.Title))
				result += " - " + AntiXssEncoder.HtmlEncode(photo.Title, false);

			return result;
		}
	}
}