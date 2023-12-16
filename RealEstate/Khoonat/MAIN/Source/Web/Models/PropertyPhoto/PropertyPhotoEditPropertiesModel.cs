using System.ComponentModel.DataAnnotations;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.PropertyPhoto;

namespace JahanJooy.RealEstate.Web.Models.PropertyPhoto
{
	public class PropertyPhotoEditPropertiesModel
	{
		public PropertyListingPhoto Photo { get; set; }

		[Display(ResourceType = typeof(PropertyPhotoResources), Name = "Label_Subject")]
		public PropertyListingPhotoSubject? Subject { get; set; }

		[Display(ResourceType = typeof(PropertyPhotoResources), Name = "Label_Title")]
		[StringLength(100, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string Title { get; set; }

		[Display(ResourceType = typeof(PropertyPhotoResources), Name = "Label_Description")]
		[StringLength(2000, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string Description { get; set; }
	}
}