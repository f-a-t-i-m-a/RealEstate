using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using JahanJooy.Common.Util.Web.Attributes;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Enums;
using JahanJooy.RealEstate.Domain.SavedSearch;
using JahanJooy.RealEstate.Web.Resources.Models;
using JahanJooy.RealEstate.Web.Resources.Models.Properties;

namespace JahanJooy.RealEstate.Web.Models.Properties
{
	public class PropertiesSaveSearchModel
	{
		//
		// Output for rendering the page

		public PropertySearch Search { get; set; }
		public SavedPropertySearch SavedSearch { get; set; }
		public IEnumerable<PropertiesSearchOptionModel> SelectedOptions { get; set; }
		
		public IEnumerable<UserContactMethod> UserEmailContactMethods { get; set; }
		public IEnumerable<UserContactMethod> UserSmsContactMethods { get; set; }

		//
		// Input from page

		public string SearchString { get; set; }

		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_Title")]
		[StringLength(100, ErrorMessageResourceType = typeof(GeneralValidationErrorResources), ErrorMessageResourceName = "StringLength")]
		public string Title { get; set; }
	
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SendNotificationEmails")]
		public bool SendNotificationEmails { get; set; }

		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SendPromotionalSmsMessages")]
		public bool SendPromotionalSmsMessages { get; set; }

		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SendPaidSmsMessages")]
		public bool SendPaidSmsMessages { get; set; }

		[Numeric(ErrorMessageResourceType = typeof(PropertiesSaveSearchResources), ErrorMessageResourceName = "Validation_DaysToKeepSendingNotificatons_Numeric")]
		[BindingExceptionMessage(ErrorMessageResourceType = typeof(PropertiesSaveSearchResources), ErrorMessageResourceName = "Validation_DaysToKeepSendingNotificatons_Numeric")]
		[Range(1, 200, ErrorMessageResourceType = typeof(PropertiesSaveSearchResources), ErrorMessageResourceName = "Validation_DaysToKeepSendingNotificatons_Range")]
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_DaysToKeepSendingNotificatons")]
		public int? DaysToKeepSendingNotificatons { get; set; }

		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_EmailNotificationTargetID")]
		public long? EmailNotificationTargetID { get; set; }

		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsNotificationTargetID")]
		public long? SmsNotificationTargetID { get; set; }

		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartSiteName")]
		public bool SmsPartSiteName { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartListingUrl")]
		public bool SmsPartListingUrl { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartListingCode")]
		public bool SmsPartListingCode { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartPropertyType")]
		public bool SmsPartPropertyType { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartNumberOfRooms")]
		public bool SmsPartNumberOfRooms { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartShortGeographicRegion")]
		public bool SmsPartShortGeographicRegion { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartLongGeographicRegion")]
		public bool SmsPartLongGeographicRegion { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartUserEnteredAddress")]
		public bool SmsPartUserEnteredAddress { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartArea")]
		public bool SmsPartArea { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartPriceOrRent")]
		public bool SmsPartPriceOrRent { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartPerAreaPrice")]
		public bool SmsPartPerAreaPrice { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartContactName")]
		public bool SmsPartContactName { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartContactPhone")]
		public bool SmsPartContactPhone { get; set; }
		[Display(ResourceType = typeof(PropertiesSaveSearchResources), Name = "Label_SmsPartContactSecondPhone")]
		public bool SmsPartContactSecondPhone { get; set; }

		#region Conversion methods

		public static PropertiesSaveSearchModel FromDomain(SavedPropertySearch domain)
		{
			var model = new PropertiesSaveSearchModel();
			model.UpdateModel(domain);
			return model;
		}

		public SavedPropertySearch ToDomain(IVicinityCache vicinityCache)
		{
			var domain = new SavedPropertySearch();
            UpdateDomain(domain, vicinityCache);
			return domain;
		}

		public void UpdateDomain(SavedPropertySearch domain, IVicinityCache vicinityCache)
		{
			domain.Title = Title;

			if (Search != null)
			{
				domain.PropertyType = Search.PropertyType;
				domain.IntentionOfOwner = Search.IntentionOfOwner;

                if (Search.VicinityIDs != null && Search.VicinityIDs.Any())
                    domain.GeographicRegions = Search.VicinityIDs.Select(vid => new SavedPropertySearchGeographicRegion { VicinityID = vid }).ToList();

				domain.AdditionalFilters = PropertySearchQueryUtil.GenerateQuery(Search);
			}

			domain.SendNotificationEmails = SendNotificationEmails;
			domain.SendPromotionalSmsMessages = SendPromotionalSmsMessages;
			domain.SendPaidSmsMessages = SendPaidSmsMessages;

			if (DaysToKeepSendingNotificatons.HasValue)
				domain.SendNotificationsUntil = DateTime.Now.Date.AddDays(DaysToKeepSendingNotificatons.Value);

			domain.EmailNotificationTargetID = EmailNotificationTargetID;
			domain.SmsNotificationTargetID = SmsNotificationTargetID;

			domain.EmailNotificationType = SavedPropertySearchEmailNotificationType.Individual;
			domain.SmsNotificationParts =
				(SmsPartSiteName ? SavedPropertySearchSmsNotificationPart.SiteName : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartListingUrl ? SavedPropertySearchSmsNotificationPart.ListingUrl : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartListingCode ? SavedPropertySearchSmsNotificationPart.ListingCode : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartPropertyType ? SavedPropertySearchSmsNotificationPart.PropertyType : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartNumberOfRooms ? SavedPropertySearchSmsNotificationPart.NumberOfRooms : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartShortGeographicRegion ? SavedPropertySearchSmsNotificationPart.ShortGeographicRegion : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartLongGeographicRegion ? SavedPropertySearchSmsNotificationPart.LongGeographicRegion : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartUserEnteredAddress ? SavedPropertySearchSmsNotificationPart.UserEnteredAddress : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartArea ? SavedPropertySearchSmsNotificationPart.Area : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartPriceOrRent ? SavedPropertySearchSmsNotificationPart.PriceOrRent : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartPerAreaPrice ? SavedPropertySearchSmsNotificationPart.PerAreaPrice : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartContactName ? SavedPropertySearchSmsNotificationPart.ContactName : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartContactPhone ? SavedPropertySearchSmsNotificationPart.ContactPhone : SavedPropertySearchSmsNotificationPart.None) |
				(SmsPartContactSecondPhone ? SavedPropertySearchSmsNotificationPart.ContactSecondPhone : SavedPropertySearchSmsNotificationPart.None);
		}

		public void UpdateModel(SavedPropertySearch domain)
		{
			Title = domain.Title;
			Search = PropertySearchUtil.BuildPropertySearch(domain);
			SavedSearch = domain;

			SendNotificationEmails = domain.SendNotificationEmails;
			SendPromotionalSmsMessages = domain.SendPromotionalSmsMessages;
			SendPaidSmsMessages = domain.SendPaidSmsMessages;

			EmailNotificationTargetID = domain.EmailNotificationTargetID;
			SmsNotificationTargetID = domain.SmsNotificationTargetID;

		    if (domain.SmsNotificationParts.HasValue)
		    {
                SmsPartSiteName = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.SiteName);
                SmsPartListingUrl = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ListingUrl);
                SmsPartListingCode = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ListingCode);
                SmsPartPropertyType = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.PropertyType);
                SmsPartNumberOfRooms = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.NumberOfRooms);
                SmsPartShortGeographicRegion = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ShortGeographicRegion);
                SmsPartLongGeographicRegion = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.LongGeographicRegion);
                SmsPartUserEnteredAddress = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.UserEnteredAddress);
                SmsPartArea = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.Area);
                SmsPartPriceOrRent = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.PriceOrRent);
                SmsPartPerAreaPrice = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.PerAreaPrice);
                SmsPartContactName = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ContactName);
                SmsPartContactPhone = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ContactPhone);
                SmsPartContactSecondPhone = domain.SmsNotificationParts.Value.HasFlag(SavedPropertySearchSmsNotificationPart.ContactSecondPhone);
            }
		}

		#endregion
	}
}