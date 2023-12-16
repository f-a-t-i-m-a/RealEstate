using System.ComponentModel.DataAnnotations;

namespace JahanJooy.RealEstate.Web.Models.Property
{
    public class PropertySharePropertyListingModel
    {
        [DataType(DataType.EmailAddress)]
        [Common.Util.Web.Attributes.EmailAddress(true, ErrorMessageResourceType = typeof(PropertyModelResources),
            ErrorMessageResourceName = "Validation_Email_Email")]
        [StringLength(120, ErrorMessageResourceType = typeof(PropertyModelResources),
            ErrorMessageResourceName = "StringLength")]
        [Required(ErrorMessageResourceType = typeof(PropertyModelResources),
           ErrorMessageResourceName = "Validation_Required_Email")]
        public string Email { get; set; }
         
        [Required(ErrorMessageResourceType = typeof(PropertyModelResources),
            ErrorMessageResourceName = "Validation_Required_ReceiverName")]
        public string ReceiverName { get; set; }
        public string Description { get; set; }
        public string SenderName { get; set; }
        public long PropertyListingID { get; set; }

    }
}