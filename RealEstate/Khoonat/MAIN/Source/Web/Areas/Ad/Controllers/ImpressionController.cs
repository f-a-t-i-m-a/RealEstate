using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Compositional.Composer;
using JahanJooy.RealEstate.Core;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services.Ad;
using JahanJooy.RealEstate.Domain.Ad;
using JahanJooy.RealEstate.Web.Application.Base;
using JahanJooy.RealEstate.Web.Models.Property;

namespace JahanJooy.RealEstate.Web.Areas.Ad.Controllers
{
    public class ImpressionController : CustomControllerBase
    {
        #region Injected dependencies

        [ComponentPlug]
        public ISponsoredEntityService SponsoredEntityService { get; set; }

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        #endregion

        #region Impression action methods

        [HttpGet]
        public ActionResult Click(long id, Guid guid)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                return Error(ErrorResult.AccessDenied);

            var sponsoredEntityImpression = SponsoredEntityService.CreateClick(id, guid);
            if (sponsoredEntityImpression == null)
                return Error(ErrorResult.EntityNotFound);


            if (sponsoredEntityImpression.SponsoredEntity.EntityType == SponsoredEntityType.PropertyListing)
            {
                var listingId = DbManager.Db.SponsoredPropertyListings
                    .Where(sl => sl.SponsoredEntityID == sponsoredEntityImpression.SponsoredEntityID)
                    .Select(sl => sl.ListingID)
                    .Single();

                return RedirectToAction("ViewDetails", "Property", new PropertyViewDetailsParamsModel { ID = listingId });
            }
            
            return Error(ErrorResult.EntityNotFound);
        }

        #endregion
    }
}
