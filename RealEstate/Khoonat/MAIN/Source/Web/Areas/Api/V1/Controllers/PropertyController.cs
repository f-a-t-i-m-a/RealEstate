using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.RealEstate.Core.Cache;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Search;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Dto;
using JahanJooy.RealEstate.Core.Services.Dto.Property;
using JahanJooy.RealEstate.Domain.Property;
using JahanJooy.RealEstate.Util;
using JahanJooy.RealEstate.Util.Presentation;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Application;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Property.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared;
using JahanJooy.RealEstate.Web.Areas.Api.V1.Models.Shared.Spatial;
using JahanJooy.RealEstate.Web.Helpers.Property;
using JahanJooy.RealEstate.Web.Resources.Views.Properties;
using ServiceStack;

namespace JahanJooy.RealEstate.Web.Areas.Api.V1.Controllers
{
    public class PropertyController : ApiControllerBase
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IPropertyService PropertyService { get; set; }

        [ComponentPlug]
        public IVicinityCache VicinityCache { get; set; }

        [ComponentPlug]
        public IPropertyPhotoService PropertyPhotoService { get; set; }

        [HttpPost]
        public ActionResult Search(ApiPropertySearchInputModel input)
        {
            if (!ModelState.IsValid)
                return ValidationErrorFromModelState();

            var searchResult = PropertyService.RunSearch(input.ToPropertySearch(), input.Pagination.StartIndex,
                input.Pagination.PageSize, true, true);

            var output = new ApiPropertySearchOutputModel();
            output.TotalNumberOfRecords = searchResult.TotalNumberOfRecords;
            output.IndexOfFirstResult = searchResult.IndexOfFirstResult;
            output.IndexOfLastResult = searchResult.IndexOfLastResult;

            if (searchResult.PageResults != null)
            {
                output.PropertyListings = searchResult.PageResults.Select(summary => ConvertPropertyListing(summary, input.ReturnEssentialInfoOnly)).ToArray();

                if (input.IncludeCoverPhotoThumbnailBytes)
                {
                    PopulatePhotoFiles(output);
                }

                output.SponsoredPropertyListingSummaryModel =
                    searchResult.SponsoredResults.Select(ConvertSponsoredPropertyListing).ToArray();
            }

            return Json(output);
        }

        [HttpPost]
        public ActionResult GetContactInfo(ApiPropertyGetContactInfoInputModel input)
        {
            // Load the listing properties to authorize
            PropertyListing listing = DbManager.Db.PropertyListingsDbSet.SingleOrDefault(pl => pl.ID == input.ListingID);
            if (!PropertyControllerHelper.CanViewListing(listing))
                return Error(ApiErrorCode.PermissionDeniedForEntity);

            // Re-load the listing from the service to count the number of contact info retrievals
            var listingWithContactInfo = PropertyService.LoadForContactInfo(input.ListingID);
            var output = ApiPropertyGetContactInfoOutputModel.FromDomain(listingWithContactInfo);
            return Json(output);
        }

        [HttpPost]
        public ActionResult GetPhotos(ApiPropertyGetPhotosInputModel input)
        {
            // Load the listing properties to authorize
            PropertyListing listing = DbManager.Db.PropertyListingsDbSet.SingleOrDefault(pl => pl.ID == input.ListingID);
            if (!PropertyControllerHelper.CanViewListing(listing))
                return Error(ApiErrorCode.PermissionDeniedForEntity);

            var query = DbManager.Db.PropertyListingPhotos
                .Where(plp => plp.PropertyListingID == listing.ID && plp.Approved.HasValue && plp.Approved.Value)
                .OrderBy(plp => plp.Order);

            var photos = PagedList<PropertyListingPhoto>.BuildUsingStartIndex(query.Count(), input.Pagination.PageSize,
                input.Pagination.StartIndex);
            photos.FillFrom(query);

            var output = new ApiPropertyGetPhotosOutputModel
            {
                PhotoListings =
                    photos.Select(
                        photo =>
                            ConvertPhotoListing(photo, input.IncludeThumbnailFile, input.IncludeMediumSizeFile, false))
                        .ToArray()
            };

            return Json(output);
        }

        [HttpPost]
        public ActionResult GetPhotoFiles(ApiPropertyGetPhotoFilesInputModel input)
        {
            var photoIDs = input.PhotoIDs;

            if (photoIDs == null || !photoIDs.Any())
                return Error(ApiErrorCode.InputQueryIsEmpty);

            if (photoIDs.Length > 20)
                return Error(ApiErrorCode.InputQueryTooLarge);

            var photos = DbManager.Db.PropertyListingPhotos.Where(plp =>
                photoIDs.Contains(plp.ID) &&
                plp.Approved.HasValue &&
                plp.Approved.Value &&
                !plp.DeleteTime.HasValue &&
                plp.PropertyListing.PublishDate.HasValue &&
                !plp.PropertyListing.DeleteDate.HasValue).ToList();

            var output = new ApiPropertyGetPhotoFilesOutputModel
            {
                PhotoListings = photos.Select(photo => ConvertPhotoListing(photo,
                    input.Size == ApiPropertyGetPhotoFilesInputModel.PhotoSizeSpec.Thumbnail,
                    input.Size == ApiPropertyGetPhotoFilesInputModel.PhotoSizeSpec.Medium,
                    input.Size == ApiPropertyGetPhotoFilesInputModel.PhotoSizeSpec.Full)).ToArray(),
                RequestedSize = input.Size
            };

            return Json(output);
        }

        [HttpPost]
        public ActionResult GetDetails(ApiPropertyGetDetailsInputModel input)
        {
            var listing = PropertyService.LoadForVisit(input.PropertyListingID);
            if (listing == null)
                return Error(ApiErrorCode.EntityNotFound);

            // TODO: Organize permission checking and access control
            bool isOwner = PropertyControllerHelper.DetermineIfUserIsOwner(listing, input.EditPassword);
            if (!isOwner && !User.IsOperator)
            {
                if (!listing.PublishDate.HasValue || !listing.Approved.HasValue || !listing.Approved.Value)
                    return Error(ApiErrorCode.PermissionDeniedForEntity);
            }

            var output = Mapper.Map<ApiPropertyGetDetailsOutputModel>(listing);

            if (listing.CreatorUserID.HasValue)
            {
                var creatorUser = DbManager.Db.Users.SingleOrDefault(u => u.ID == listing.CreatorUserID.Value);
                output.CreatorUser = Mapper.Map<ApiOutputUserModel>(creatorUser);
            }

            if (listing.OwnerUserID.HasValue)
            {
                var ownerUser = DbManager.Db.Users.SingleOrDefault(u => u.ID == listing.OwnerUserID.Value);
                output.OwnerUser = Mapper.Map<ApiOutputUserModel>(ownerUser);
            }

            // Retrieving contact info should be done using another service call
            output.Details.ContactInfo = null;

            return Json(output);
        }

        [HttpPost]
        public ActionResult CreateNew(ApiPropertyCreateNewInputModel input)
        {
            if (!ModelState.IsValid)
                return ValidationErrorFromModelState();

            if (input.PropertyListingDetails == null)
                return InputIsEmptyError("PropertyListingDetails");

            var listing = Mapper.Map<PropertyListing>(input.PropertyListingDetails);
            var validationResult = input.PropertyListingDetails.UserAccess != null &&
                                   input.PropertyListingDetails.UserAccess.IsPublic
                ? PropertyService.ValidateForPublish(PropertyListingDetails.MakeDetails(listing))
                : PropertyService.ValidateForSave(listing);

            if (!validationResult.IsValid)
                return ValidationError(validationResult);

            var result = PropertyService.Save(listing);
            if (!result.IsValid)
                return ValidationError(result);

            if (input.PropertyListingDetails.UserAccess != null && input.PropertyListingDetails.UserAccess.IsPublic)
            {
                var publishResult = PropertyService.Publish(listing.ID,
                    input.PropertyListingDetails.UserAccess.DaysBeforeAutoArchive ?? 90);
                if (!publishResult.IsValid)
                    return ValidationError(publishResult);
            }

            var output = Mapper.Map<ApiPropertyCreateNewOutputModel>(listing);
            return Json(output);
        }

        [HttpPost]
        public ActionResult Update(ApiPropertyUpdateInputModel input)
        {
            if (input.PropertyListingDetails == null)
                return InputIsEmptyError("PropertyListingDetails");

            var propertyListing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.ID == input.PropertyListingID);
            if (propertyListing == null)
                return Error(ApiErrorCode.EntityNotFound);

            bool isOwner = PropertyControllerHelper.DetermineIfUserIsOwner(propertyListing, input.EditPassword);
            if (!isOwner && !User.IsOperator)
                return Error(ApiErrorCode.PermissionDeniedForEntity);

            var result = PropertyService.Update(input.PropertyListingID,
                listing =>
                {
                    Mapper.Map(input.PropertyListingDetails, listing);
                    return EntityUpdateResult.Default;
                });

            if (!result.IsValid)
                return ValidationError(result);

            return Success();
        }

        [HttpPost]
        public ActionResult AddPhoto(ApiPropertyAddPhotoInputModel input)
        {
            if (!ModelState.IsValid)
                return ValidationErrorFromModelState();
            if (input.Photos.IsNullOrEmptyEnumerable())
                return InputIsEmptyError("Photos");
            if (input.Photos.Any(p => p.File == null) || input.Photos.Any(p => p.File.Base64Content.IsNullOrEmpty()))
                return Error(ApiErrorCode.InputFileIsEmpty);

            var propertyListing = DbManager.Db.PropertyListings.SingleOrDefault(l => l.ID == input.PropertyListingID);
            if (propertyListing == null)
                return Error(ApiErrorCode.EntityNotFound);

            bool isOwner = PropertyControllerHelper.DetermineIfUserIsOwner(propertyListing, input.EditPassword);
            if (!isOwner && !User.IsOperator)
                return Error(ApiErrorCode.PermissionDeniedForEntity);

            var outputItems = new List<ApiPropertyAddPhotoOutputModel.OutputItem>();
            foreach (var inputPhoto in input.Photos)
            {
                var outputItem = new ApiPropertyAddPhotoOutputModel.OutputItem();

                try
                {
                    var stream = new MemoryStream(Convert.FromBase64String(inputPhoto.File.Base64Content));
                    var photo = PropertyPhotoService.AddPhoto(input.PropertyListingID, stream);
                    outputItem.PhotoID = photo.ID;

                    Mapper.Map(inputPhoto, photo);
                    PropertyPhotoService.UpdatePhotoProperties(photo);
                }
                catch (Exception e)
                {
                    outputItem.Error = e.Message;
                }

                outputItems.Add(outputItem);
            }

            return Json(new ApiPropertyAddPhotoOutputModel {Results = outputItems.ToArray()});
        }

        [HttpPost]
        public ActionResult ListSearchTags(ApiPropertyListSearchTagsInputModel input)
        {
            var result = new ApiPropertyListSearchTagsOutputModel();

            result.EstateTags = BuildSearchTagDetails(PropertySearchOptions.EstateOptions.Values).ToArray();
            result.BuildingTags = BuildSearchTagDetails(PropertySearchOptions.BuildingOptions.Values).ToArray();
            result.UnitTags = BuildSearchTagDetails(PropertySearchOptions.UnitOptions.Values).ToArray();
            result.OtherTags = BuildSearchTagDetails(PropertySearchOptions.OtherOptions.Values).ToArray();

            return Json(result);
        }

        #region Private helper methods

        private ApiPropertySummaryModel ConvertPropertyListing(PropertyListingSummary summary, bool essentialOnly)
        {
            var result = new ApiPropertySummaryModel
            {
                ID = summary.ID,
                Code = summary.Code,
                PropertyType = summary.PropertyType,
                IntentionOfOwner = summary.IntentionOfOwner,
                IsAgencyActivityAllowed = summary.IsAgencyActivityAllowed,
                IsAgencyListing = summary.IsAgencyListing,
                Vicinity = ApiOutputVicinityModel.FromVicinityID(summary.VicinityID, VicinityCache, !essentialOnly),
                UsageType = summary.UsageType,
                NumberOfRooms = summary.NumberOfRooms,
                FloorNumber = summary.FloorNumber,
                HasElevator = summary.HasElevator,
                NumberOfParkings = summary.NumberOfParkings,
                StorageRoomArea = summary.StorageRoomArea,
                HasBeenReconstructed = summary.HasBeenReconstructed,
                EstateArea = summary.EstateArea,
                UnitArea = summary.UnitArea,
                Price = summary.Price,
                PricePerEstateArea = summary.PricePerEstateArea,
                PricePerUnitArea = summary.PricePerUnitArea,
                HasTransferableLoan = summary.HasTransferableLoan,
                Mortgage = summary.Mortgage,
                Rent = summary.Rent,
                MortgageAndRentConvertible = summary.MortgageAndRentConvertible,
                PublishDate = summary.PublishDate,
                NumberOfPhotos = summary.NumberOfPhotos,
                CoverPhotoId = summary.CoverPhotoId,
                CreationDate = summary.CreationDate,
                ModificationDate = summary.ModificationDate,
                Visits = summary.Visits,
                ContactInfoRetrievals = summary.ContactInfoRetrievals,
                Searches = summary.Searches,
                IsFavoritedByUser = summary.IsFavoritedByUser,
                TimesFavorited = summary.TimesFavorited
            };

            if (!essentialOnly)
            {
                result.GeographicLocation = ApiGeoPoint.FromDbGeography(summary.GeographicLocation);
                result.GeographicLocationType = summary.GeographicLocationType;
                result.EstateDirection = summary.EstateDirection;
                result.EstateSurfaceType = summary.EstateSurfaceType;
                result.TotalNumberOfUnitsInBuilding = summary.TotalNumberOfUnitsInBuilding;
                result.TotalNumberOfFloorsInBuilding = summary.TotalNumberOfFloorsInBuilding;
                result.PublishEndDate = summary.PublishEndDate;
                result.OwnerUserID = summary.OwnerUserID;
            }

            return result;
        }

        private ApiSponsoredPropertyListingSummaryModel ConvertSponsoredPropertyListing(
            SponsoredPropertyListingSummary summary)
        {
            var result = new ApiSponsoredPropertyListingSummaryModel
            {
                ApiPropertyListingSummaryModel = Mapper.Map<ApiPropertySummaryModel>(summary.PropertyListingSummary),
                ApiSponsoredEntityModel = Mapper.Map<ApiSponsoredEntityModel>(summary.SponsoredEntity)
            };

            result.ApiPropertyListingSummaryModel.GeographicLocation =
                ApiGeoPoint.FromDbGeography(summary.PropertyListingSummary.GeographicLocation);
            result.ApiPropertyListingSummaryModel.Vicinity =
                ApiOutputVicinityModel.FromVicinityID(summary.PropertyListingSummary.VicinityID,
                    VicinityCache);

            return result;
        }

        private ApiPropertyPhotoOutputModel ConvertPhotoListing(PropertyListingPhoto photo, bool includeThumbnailFile,
            bool includeMediumSizeFile, bool includeFillSizeFile)
        {
            var output = new ApiPropertyPhotoOutputModel
            {
                ID = photo.ID,
                Subject = photo.Subject,
                SubjectStr = photo.Subject.ToString(),
                Title = photo.Title,
                Description = photo.Description,
                Order = photo.Order,
                ThumbnailLength = photo.ThumbnailLength,
                MediumSizeLength = photo.MediumSizeLength,
                FullSizeLength = photo.FullSizeLength,
                CreationDate = photo.CreationTime,
                CreationTime = photo.CreationTime,
            };

            if (includeThumbnailFile)
            {
                output.ThumbnailFile = new ApiFileContentModel
                {
                    ContentType = "image/jpeg",
                    Base64Content = Convert.ToBase64String(PropertyPhotoService.GetThumbnailBytes(photo.StoreItemID))
                };
            }

            if (includeMediumSizeFile)
            {
                output.MediumSizeFile = new ApiFileContentModel
                {
                    ContentType = "image/jpeg",
                    Base64Content = Convert.ToBase64String(PropertyPhotoService.GetMediumSizeBytes(photo.StoreItemID))
                };
            }

            if (includeFillSizeFile)
            {
                output.FillSizeFile = new ApiFileContentModel
                {
                    ContentType = "image/jpeg",
                    Base64Content = Convert.ToBase64String(PropertyPhotoService.GetFullSizeBytes(photo.StoreItemID))
                };
            }

            return output;
        }

        private IEnumerable<ApiPropertyListSearchTagsOutputModel.TagDetail> BuildSearchTagDetails(
            IEnumerable<PropertySearchOption> searchOptions)
        {
            return searchOptions.Select(o => new ApiPropertyListSearchTagsOutputModel.TagDetail
            {
                Tag = o.Key,
                DisplayTextKey = o.Label,
                LocalizedDisplayText =
                    PropertiesBrowseResources.ResourceManager.GetString("option_" + o.Label.ToLower()) ?? o.Label
            });
        }

        private void PopulatePhotoFiles(ApiPropertySearchOutputModel output)
        {
            var coverPhotoIds =
                output.PropertyListings.Where(pl => pl.CoverPhotoId.HasValue)
                    .Select(pl => pl.CoverPhotoId.Value)
                    .ToList();
            var coverPhotos =
                DbManager.Db.PropertyListingPhotos.Where(plp => coverPhotoIds.Contains(plp.ID))
                    .ToList()
                    .ToDictionary(p => p.ID);

            foreach (var outputItem in output.PropertyListings)
            {
                if (!outputItem.CoverPhotoId.HasValue)
                    continue;

                PropertyListingPhoto coverPhoto;
                if (!coverPhotos.TryGetValue(outputItem.CoverPhotoId.Value, out coverPhoto))
                    continue;

                outputItem.CoverPhotoFile = new ApiFileContentModel
                {
                    ContentType = "image/jpeg",
                    Base64Content =
                        Convert.ToBase64String(PropertyPhotoService.GetThumbnailBytes(coverPhoto.StoreItemID))
                };
            }
        }

        #endregion
    }
}