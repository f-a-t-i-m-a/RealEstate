using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using JahanJooy.Common.Util;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Services.Dto.Billing;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Domain.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Models.SponsoredProperty
{
    [Bind(Exclude = "PropertyListing,Tarrif,BillingMethodSelectList")]
    public class SponsoredPropertyEditSponsorshipModel
    {
        // Outputs to the view

        public PropertyListing PropertyListing { get; set; }
        public PropertyListingSummary ListingSummary { get; set; }
        public SponsorshipTarrif Tarrif { get; set; }
        public IEnumerable<SelectListItem> BillingMethodSelectList { get; set; }

        // Inputs from the user

        public long PropertyListingID { get; set; }

        [Display(ResourceType = typeof (SponsoredPropertyModelResources), Name = "Label_ShowInAllPages")]
        public bool ShowInAllPages { get; set; }

        [Display(ResourceType = typeof (SponsoredPropertyModelResources), Name = "Label_ShowOnMap")]
        public bool ShowOnMap { get; set; }

        [Display(ResourceType = typeof (SponsoredPropertyModelResources), Name = "Label_DaysBeforeExpire")]
        public DaysBeforeExpirationOption? DaysBeforeExpiration { get; set; }

        [Required(ErrorMessageResourceType = typeof (SponsoredPropertyModelResources),
            ErrorMessageResourceName = "Validation_Title_Required")]
        [Display(ResourceType = typeof (SponsoredPropertyModelResources), Name = "Label_Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof (SponsoredPropertyModelResources), Name = "Label_BillingMethod")]
        public SponsoredEntityBillingMethod BillingMethod { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (SponsoredPropertyModelResources),
            ErrorMessageResourceName = "Validation_MaxPayPerImpression_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (SponsoredPropertyModelResources),
            ErrorMessageResourceName = "Validation_MaxPayPerImpression_Numeric")]
        [Range(0, 9999, ErrorMessageResourceType = typeof (SponsoredPropertyModelResources),
            ErrorMessageResourceName = "Validation_MaxPayPerImpression_Range")]
        [Display(ResourceType = typeof (SponsoredPropertyModelResources), Name = "Label_MaxPayPerImpression")]
        public decimal? MaxPayPerImpression { get; set; }

        [Numeric(ErrorMessageResourceType = typeof (SponsoredPropertyModelResources),
            ErrorMessageResourceName = "Validation_MaxPayPerClick_Numeric")]
        [BindingExceptionMessage(ErrorMessageResourceType = typeof (SponsoredPropertyModelResources),
            ErrorMessageResourceName = "Validation_MaxPayPerClick_Numeric")]
        [Range(0, 99999, ErrorMessageResourceType = typeof (SponsoredPropertyModelResources),
            ErrorMessageResourceName = "Validation_MaxPayPerClick_Range")]
        [Display(ResourceType = typeof (SponsoredPropertyModelResources), Name = "Label_MaxPayPerClick")]
        public decimal? MaxPayPerClick { get; set; }

        public DateTime? ExpirationTime { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(SponsoredPropertyModelResources), ErrorMessageResourceName = "Validation_CustomCaption_Length")]
        [Display(ResourceType = typeof(SponsoredPropertyModelResources), Name = "Label_CustomCaption")]
        public string CustomCaption { get; set; }
        [Display(ResourceType = typeof(SponsoredPropertyModelResources), Name = "Label_IgnoreSearchQuery")]
        public bool IgnoreSearchQuery { get; set; }

        public decimal PerImpressionMaximumBid { get; set; }
        public decimal PerClickMaximumBid { get; set; }


        public enum DaysBeforeExpirationOption
        {
            TwoDays = 2,
            FourDays = 4,
            OneWeek = 7,
            TwoWeeks = 14,
            ThreeWeeks = 21,
            OneMonth = 30,
            TwoMonths = 60,
            ThreeMonths = 90,
            FourMonths = 120,
            FiveMonths = 150
        }

        public SponsoredPropertyListing ToDomain()
        {
            DateTime? expirationTime = DaysBeforeExpiration.IfHasValue(d => (DateTime?)DateTime.Now.AddDays((double)d));
            
            var result = new SponsoredPropertyListing
            {
                SponsoredEntity = new SponsoredEntity
                {
                    ExpirationTime = expirationTime,
                    Title = Title,
                    BillingMethod = BillingMethod,
                    MaxPayPerImpression = MaxPayPerImpression.GetValueOrDefault(),
                    MaxPayPerClick = MaxPayPerClick.GetValueOrDefault(),
                    EntityType = SponsoredEntityType.PropertyListing,
                    CreationTime = DateTime.Now,
                    Enabled = true,
                    
                },
                ListingID = PropertyListingID,
                ShowInAllPages = ShowInAllPages,
                ShowOnMap = ShowOnMap,
                CustomCaption = CustomCaption,
                IgnoreSearchQuery = IgnoreSearchQuery, 
                Approved = null
            };

            return result;
        }
    }
}