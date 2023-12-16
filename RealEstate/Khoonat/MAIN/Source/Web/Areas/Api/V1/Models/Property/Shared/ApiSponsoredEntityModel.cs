using System;
using AutoMapper;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Ad;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared
{
	public class ApiSponsoredEntityModel
    {
        public long ID { get; set; }
        public SponsoredEntityType EntityType { get; set; }

        public long BilledUserID { get; set; }
        public User BilledUser { get; set; }

        public DateTime CreationTime { get; set; }
//		public DateTime? LastUnblockTime { get; set; }
//		public DateTime? LastImpressionTime { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? DeleteTime { get; set; }

        public string Title { get; set; }
        public bool Enabled { get; set; }
        public SponsoredEntityBillingMethod BillingMethod { get; set; }
        public decimal MaxPayPerImpression { get; set; }
        public decimal MaxPayPerClick { get; set; }
         
        public bool BlockedForLowCredit { get; set; }
        //        public decimal EstimatedClicksPerImpression { get; set; }
        //        public decimal ProjectedMaxPayPerImpression { get; set; }
        //		public double SelectionProbabilityWeight { get; set; }
        //        public DateTime? NextRecalcDue { get; set; }

        //        public ICollection<SponsoredEntityImpression> Impressions { get; set; }
        //        public ICollection<SponsoredEntityImpressionBilling> ImpressionBillings { get; set; }
        //        public ICollection<SponsoredEntityClick> Clicks { get; set; }
        //        public ICollection<SponsoredEntityClickBilling> ClickBillings { get; set; }

	    public static void ConfigureMapper()
	    {
	        Mapper.CreateMap<SponsoredEntity, ApiSponsoredEntityModel>();
	    }
    }
}