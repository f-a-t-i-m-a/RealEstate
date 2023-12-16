using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using ImageResizer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.Common.Util.Identity;
using JahanJooy.Common.Util.Owin;
using JahanJooy.Common.Util.Validation;
using JahanJooy.Common.Util.Web.Multipart;
using JahanJooy.Common.Util.Web.Result;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Api.Web.Identity;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Base;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Domain.Enums.UserActivity;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.Attachments;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Enums;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Log4Net;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.Shared;
using JahanJooy.RealEstateAgency.Util.Models.Users;
using JahanJooy.RealEstateAgency.Util.Notification;
using JahanJooy.RealEstateAgency.Util.Owin;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using JahanJooy.RealEstateAgency.Util.Utils;
using JahanJooy.RealEstateAgency.Util.Utils.ContactMethods;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using Nest;
using LicenseType = JahanJooy.RealEstateAgency.Domain.Enums.User.LicenseType;
using Types = JahanJooy.RealEstateAgency.Domain.Enums.Elastic.Types;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("users")]
    public class UserController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public ElasticManager ElasticManager { get; set; }

        [ComponentPlug]
        public IComposer Composer { get; set; }

        [ComponentPlug]
        public EmailNotificationUtils EmailNotificationUtils { get; set; }

        [ComponentPlug]
        public SmsNotificationUtils SmsNotificationService { get; set; }

        [ComponentPlug]
        public PhotoGridStore PhotoGridStore { get; set; }

        [ComponentPlug]
        public ApplicationUserManager UserManager { get; set; }

        [ComponentPlug]
        public UserUtil UserUtil { get; set; }

        [ComponentPlug]
        public LocalContactMethodUtil LocalContactMethodUtil { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpPost, Route("save")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Create, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult SaveUser(NewApplicationUserInput input)
        {
            var user = Mapper.Map<ApplicationUser>(input);
            var preprationResult = PrepareUserForSave(user, input);
            if (!preprationResult.IsValid)
                return ValidationResult(preprationResult);

            var validationResult = ValidateUserForSave(user);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            validationResult = ValidatePassword(user, input.Password);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            var result = UserManager.CreateAsync(user).Result;

            var errors = new List<ValidationError>();
            if (result.Errors.Any())
                errors.AddRange(
                    result.Errors.Select(
                        error =>
                            new ValidationError("User", UserValidationError.UnexpectedError, new[] {error})));
            if (errors.Any())
                return new ValidationErrorResult(ApiValidationResult.Failure(errors), this);

            var passwordResult = UserManager.AddPasswordAsync(user.Id, input.Password);
            if (passwordResult.Result.Errors.Any())
                errors.AddRange(
                    result.Errors.Select(
                        error =>
                            new ValidationError("User.Password", UserValidationError.UnexpectedError,
                                new[] {error})));
            if (errors.Any())
                return new ValidationErrorResult(ApiValidationResult.Failure(errors), this);

            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(user.Id));

            return Ok(user);
        }

        [HttpGet, Route("get/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public ApplicationUserDetails GetUser(string id)
        {
            return UserUtil.GetUserDetail(id);
        }

        [HttpGet, Route("myprofile")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public ApplicationUserDetails GetMyProfile()
        {
            return UserUtil.GetMyProfile();
        }

        [HttpGet, Route("all")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        public List<ApplicationUserSummary> GetAllUsers()
        {
            var filter = new BsonDocument();
            List<ApplicationUserSummary> users =
                DbManager.ApplicationUser.Find(filter)
                    .Project(p => Mapper.Map<ApplicationUserSummary>(p))
                    .ToListAsync()
                    .Result;

            return users;
        }

        [HttpPost, Route("search")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [SkipUserActivityLogging]
        public SearchApplicationUserOutput Search(SearchApplicationUserInput searchInput)
        {
            if (searchInput.PageSize == 0)
                searchInput.PageSize = DefaultPageSize;
            else if (searchInput.PageSize < 0 || searchInput.PageSize > MaxPageSize)
                searchInput.PageSize = MaxPageSize;

            var response = GetResultFromElastic(searchInput);
            var ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            var result = new PagedListOutput<ApplicationUserSummary>();

            if (ids.Count != 0)
            {
                var filter = Builders<ApplicationUser>.Filter.In("_id", ids);
                List<ApplicationUserSummary> users = DbManager.ApplicationUser.Find(filter)
                    .Project(p => Mapper.Map<ApplicationUserSummary>(p))
                    .ToListAsync().Result;

                List<ApplicationUserSummary> sortedUsers = new List<ApplicationUserSummary>();
                ids.ForEach(id => sortedUsers.Add(users.SingleOrDefault(pr => pr.Id == id.ToString())));
                sortedUsers.RemoveAll(s => s == null);

                result = new PagedListOutput<ApplicationUserSummary>
                {
                    PageItems = sortedUsers,
                    PageNumber = (searchInput.StartIndex/searchInput.PageSize) + 1,
                    TotalNumberOfItems = (int) response.Total,
                    TotalNumberOfPages = (int) Math.Ceiling((decimal) response.Total/searchInput.PageSize)
                };
            }

            return new SearchApplicationUserOutput
            {
                Users = result
            };
        }

        [HttpPost, Route("delete/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Delete, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult DeleteUser(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<ApplicationUser>.Update
                .Set("DeletionTime", DateTime.Now)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.ApplicationUser.UpdateOneAsync(filter, update);

            if (result.Result.MatchedCount != 1)
                return ValidationResult("User", UserValidationError.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("User", UserValidationError.NotModified);

            return Ok(id);
        }

        [HttpPost, Route("approve")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong,
            ActivitySubType = "SettingApproved")]
        public IHttpActionResult ApproveUser(ApproveApplicationUserInput input)
        {
            var ids = input.Ids.Select(ObjectId.Parse).ToList();
            var filter = Builders<ApplicationUser>.Filter.In("_id", ids);
            var update = Builders<ApplicationUser>.Update
                .Set("Approved", true)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.ApplicationUser.UpdateManyAsync(filter, update);

            if (result.Result.MatchedCount != 1)
                return ValidationResult("User", UserValidationError.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("User", UserValidationError.NotModified);

            ids.ForEach(id =>
            {
                UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
                {
                    ApplicationType = ApplicationType.Haftdong,
                    TargetType = EntityType.ApplicationUser,
                    TargetID = id,
                    ActivityType = UserActivityType.Edit,
                    ActivitySubType = "SettingApproved"
                });
            });

            return Ok(result.Result.ModifiedCount);
        }

        [HttpPost, Route("enable/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult EnableOrDisableUser(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(id));
            var user = DbManager.ApplicationUser.Find(filter).SingleAsync().Result;
            if (user == null)
                return ValidationResult("User", UserValidationError.NotFound);

            UserActivityLogUtils.SetMainActivity(activitySubType: user.IsEnabled ? "Disabling" : "Enabling");

            var update = Builders<ApplicationUser>.Update
                .Set("IsEnabled", !user.IsEnabled)
                .Set("LastIndexingTime", BsonNull.Value);

            var result = DbManager.ApplicationUser.UpdateOneAsync(filter, update);
            if (result.Result.MatchedCount != 1)
                return ValidationResult("User", UserValidationError.NotFound);

            if (result.Result.MatchedCount == 1 && result.Result.ModifiedCount != 1)
                return ValidationResult("User", UserValidationError.NotModified);

            return Ok(result.Result.ModifiedCount);
        }

        [HttpPost, Route("update")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.Administrator})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult UpdateUser(UpdateApplicationUserInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(input.ID));
            var userFilter = Builders<ApplicationUser>.Filter.Eq("_id", ObjectId.Parse(input.ID));
            var newUser = Mapper.Map<ApplicationUser>(input);

            newUser.Contact = LocalContactMethodUtil.MapContactMethods(input.ContactInfos, input.DisplayName);
            var preprationResult = PrepareUserForUpdate(newUser);
            if (!preprationResult.IsValid)
                return ValidationResult(preprationResult);

            var validationResult = ValidateUserForUpdate(newUser);
            if (!validationResult.IsValid)
                return ValidationResult(validationResult);

            var updateUser = Builders<ApplicationUser>.Update
                .Set("UserName", newUser.UserName)
                .Set("DisplayName", newUser.DisplayName)
                .Set("About", newUser.About)
                .Set("WebSiteUrl", newUser.WebSiteUrl)
                .Set("Type", newUser.Type)
                .Set("LicenseType", newUser.LicenseType)
                .Set("LicenseActivationTime", newUser.LicenseActivationTime)
                .Set("Contact", newUser.Contact)
                .Set("LastIndexingTime", BsonNull.Value)
                .Set("ModificationTime", DateTime.Now);

            var result = DbManager.ApplicationUser.UpdateOneAsync(userFilter, updateUser).Result;
            if (result.MatchedCount != 1)
                return ValidationResult("User", UserValidationError.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidationResult("User", UserValidationError.NotModified);
            return Ok(newUser);
        }

        [HttpPost, Route("addimage")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Create, EntityType.PhotoInfo, ApplicationType.Haftdong)]
        public IHttpActionResult AddNewImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = MultipartFormDataUtil.ReadRequestContent(Request.Content).Result;
            var files = provider.FileData;
            var fields = provider.FormData;

            var originalFileName =
                InMemoryMultipartFormDataStreamProvider.UnquoteToken(files[0].Headers.ContentDisposition.FileName);
            var originalFileExtension = Path.GetExtension(originalFileName);
            var contentType = files[0].Headers.ContentType.ToString();

            ObjectId userId = new ObjectId();
            if (fields["userId"] != null)
            {
                userId = ObjectId.Parse(fields["userId"]);
            }
            var filter = Builders<ApplicationUser>.Filter.Eq("_id", userId);
            var user = DbManager.ApplicationUser.Find(filter)
                .SingleOrDefaultAsync()
                .Result;

            if (!UserUtil.AuthorizeForEdit(user))
            {
                return ValidationResult("User", GeneralValidationErrors.AccessDenied);
            }

            var photoId = ObjectId.GenerateNewId();
            var image = new PhotoInfo
            {
                ID = photoId,
                Title = originalFileName,
                OriginalFileName = originalFileName,
                OriginalFileExtension = originalFileExtension,
                ContentType = contentType,
                CreationTime = DateTime.Now
            };

            UpdateDefinition<ApplicationUser> update = Builders<ApplicationUser>.Update.Set("ProfilePicture", image);
            var result = DbManager.ApplicationUser.UpdateOneAsync(filter, update).Result;

            if (result.MatchedCount != 1)
                return ValidationResult("User", UserValidationError.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidationResult("User", UserValidationError.NotModified);

            UserActivityLogUtils.SetMainActivity(
                targetId: photoId,
                parentType: EntityType.ApplicationUser,
                parentId: userId
                );

            PhotoGridStore.StoreAttachmentBytes(AttachmentStoreEntityType.Image, photoId,
                PhotoStoreSize.FullSize, files[0].Stream.ToArray());

            if (RebuildAttachmentSizesSupported(files[0].Headers.ContentType.ToString()))
            {
                var sourceImageBytes = files[0].Stream.ToArray();
                RebuildAttachmentSizes(sourceImageBytes, AttachmentStoreEntityType.Image, photoId);
            }
            return Ok();
        }

        [HttpGet, Route("getThumbnail/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public IHttpActionResult GetThumbnail(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.Thumbnail), "image/jpeg");
        }

        [HttpGet, Route("getSmallSize/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public IHttpActionResult GetSmall(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.SmallSize), "image/jpeg");
        }

        [HttpGet, Route("getMediumSize/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public IHttpActionResult GetMedium(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.MediumSize), "image/jpeg");
        }

        [HttpGet, Route("getFullSize/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public IHttpActionResult GetFullSize(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.FullSize), "image/jpeg");
        }

        [HttpGet, Route("download/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        public IHttpActionResult Download(string id)
        {
            return new BinaryResult(PhotoGridStore.GetAttchmentBytes(AttachmentStoreEntityType.Image,
                ObjectId.Parse(id), PhotoStoreSize.FullSize), MediaTypeNames.Application.Octet);
        }

        [HttpPost, Route("deleteimage/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Delete, EntityType.PhotoInfo, ApplicationType.Haftdong)]
        public IHttpActionResult DeleteImage(string id)
        {
            ObjectId newId = ObjectId.Parse(id);
            var filter = Builders<ApplicationUser>.Filter.Eq("ProfilePicture.ID", newId);

            var user = DbManager.ApplicationUser.Find(filter)
                .SingleOrDefaultAsync()
                .Result;

            if (!UserUtil.AuthorizeForEdit(user))
            {
                return ValidationResult("User", GeneralValidationErrors.AccessDenied);
            }

            var update = Builders<ApplicationUser>.Update.Set("ProfilePicture.DeletionTime", DateTime.Now);

            var result = DbManager.ApplicationUser.UpdateOneAsync(filter, update).Result;
            if (result.MatchedCount != 1)
                return ValidationResult("User", UserValidationError.NotFound);

            if (result.MatchedCount == 1 && result.ModifiedCount != 1)
                return ValidationResult("User", UserValidationError.NotModified);

            UserActivityLogUtils.SetMainActivity(
                targetId: newId,
                parentType: EntityType.ApplicationUser,
                parentId: ObjectId.Parse(user.Id)
                );

            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = ApplicationType.Haftdong,
                TargetType = EntityType.ApplicationUser,
                TargetID = ObjectId.Parse(user.Id),
                ActivityType = UserActivityType.Edit,
                ActivitySubType = "InDeletingPhoto"
            });

            return Ok();
        }

        [HttpPost, Route("addnewcontactmethod")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public async Task<IHttpActionResult> CompleteRegistrationAddNewContactMethod(NewContactInfoInput input)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(User.Identity.GetUserId()));

            var result = await UserUtil.CompleteRegistrationAddNewContactMethodAsync(User.Identity.GetUserId(), input, ApplicationType.Haftdong);
            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = ApplicationType.Haftdong,
                TargetType = EntityType.ApplicationUser,
                TargetID = ObjectId.Parse(User.Identity.GetUserId()),
                ActivityType = UserActivityType.Edit,
                ActivitySubType = "InAddingUserContactMethodVerification"
            });

            return Ok(Mapper.Map<ContactInfoSummary>(result.Result));
        }

        [HttpPost, Route("verifycontactmethod")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public async Task<IHttpActionResult> CompleteRegistrationVerifyContactMethod(
            UserApplicationVerifyContactMethodInput input)
        {
            var result = await UserUtil.CompleteRegistrationVerifyContactMethod(input);

            if (!result.IsValid)
                return ValidationResult(result);

            UserActivityLogUtils.ReportAdditionalActivity(new UserActivity
            {
                ApplicationType = ApplicationType.Haftdong,
                TargetType = EntityType.ApplicationUser,
                TargetID = ObjectId.Parse(input.UserID),
                ActivityType = UserActivityType.Edit,
                ActivitySubType = "VerifyContactMethod"
            });

            return Ok();
        }

        [HttpPost, Route("getanothersecret")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult GetAnotherSecret(NewSecretForUserApplicationInput input)
        {
            var result = UserUtil.GetAnotherSecret(input, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok();
        }

        [HttpPost, Route("deletecontactmethod/{id}")]
        [ApplicationAuthorize(BuiltInRoles = new[] {BuiltInRole.RealEstateAgent})]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult DeleteContactMethod(string id)
        {
            UserActivityLogUtils.SetMainActivity(targetId: ObjectId.Parse(id));
            var result = UserUtil.DeleteContactMethod(id, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok(result.Result);
           
        }

        [AllowAnonymous]
        [HttpPost, Route("ForgotPasswordBySms")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult ForgotPasswordBySms(AccountForgotPasswordInput input)
        {
            var result = UserUtil.StartPasswordRecovery(input, ContactMethodType.Phone, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost, Route("ForgotPasswordByEmail")]
        [UserActivity(UserActivityType.Edit, EntityType.ApplicationUser, ApplicationType.Haftdong)]
        public IHttpActionResult ForgotPasswordByEmail(AccountForgotPasswordInput input)
        {
            var result = UserUtil.StartPasswordRecovery(input, ContactMethodType.Email, ApplicationType.Haftdong);

            if (!result.IsValid)
                return ValidationResult(result);

            return Ok();
        }

        #endregion

        #region Private helper methods 

        private ValidationResult PrepareUserForSave(ApplicationUser user, NewApplicationUserInput input)
        {
            user.LastIndexingTime = null;
            user.CreationTime = DateTime.Now;
            user.CreatedByID = ObjectId.Parse(OwinRequestScopeContext.Current.GetUserId());
            user.IsEnabled = true;
            user.IsOperator = false;
            user.IsVerified = true;
            user.Roles.Add(BuiltInRole.RealEstateAgent.ToString());

            if (user.LicenseType == null)
                user.LicenseType = LicenseType.Trial;

            if (user.LicenseActivationTime == null)
                user.LicenseActivationTime = DateTime.Now.Date.AddDays(14);

            user.Contact = LocalContactMethodUtil.MapContactMethods(input.ContactInfos, input.DisplayName);
            var contactResult = LocalContactMethodUtil.PrepareContactMethods(user.Contact, true, false, false);
            if (!contactResult.IsValid)
                return contactResult;

            return Common.Util.Validation.ValidationResult.Success;
        }

        private ValidationResult PrepareUserForUpdate(ApplicationUser user)
        {
            var contactResult = LocalContactMethodUtil.PrepareContactMethods(user.Contact, true, false, false);
            if (!contactResult.IsValid)
                return contactResult;

            return Common.Util.Validation.ValidationResult.Success;
        }

        private ISearchResponse<ApplicationUserIE> GetResultFromElastic(SearchApplicationUserInput searchInput)
        {
            var currentUserId = OwinRequestScopeContext.Current.GetUserId();
            var response = ElasticManager.Client.Search<ApplicationUserIE>(u => u
                .Index(ElasticManager.Index)
                .Type(Types.UserType)
                .Query(q =>
                {
                    QueryContainer query = null;
                    if (!JJOwinRequestScopeContextExtensions.IsAdministrator())
                        query &= q.Term(pr => pr.CreatedByID, currentUserId);
                    query &= q.Term(ur => ur.DeletionTime, 0);
                    if (searchInput.InActive == true)
                        query &= q.Range(ur => ur.Field(o => o.LicenseActivationTime)
                            .LessThan(DateTime.Now.Date.Ticks));
                    if (searchInput.InActive == false || searchInput.InActive == null)
                        query &= q.Range(ur => ur.Field(o => o.LicenseActivationTime)
                            .GreaterThanOrEquals(DateTime.Now.Date.Ticks));
                    if (searchInput.Type != null)
                        query &= q.Term(ur => ur.Type, searchInput.Type.ToString().ToLower());
                    if (!string.IsNullOrEmpty(searchInput.UserName))
                        query &= q.MatchPhrasePrefix(m => m.Field(o => o.UserName).Query(searchInput.UserName));
                    if (!string.IsNullOrEmpty(searchInput.DisplayName))
                        query &= q.MatchPhrasePrefix(m => m.Field(o => o.DisplayName).Query(searchInput.DisplayName));
                    if (!string.IsNullOrEmpty(searchInput.ContactValues))
                        query &=
                            q.MatchPhrasePrefix(m => m.Field(o => o.ContactValues).Query(searchInput.ContactValues));
                    return query;
                })
                .From(searchInput.StartIndex)
                .Take(searchInput.PageSize)
                .Sort(s =>
                {
                    SortDescriptor<ApplicationUserIE> sort = new SortDescriptor<ApplicationUserIE>();
                    if (searchInput.SortColumn.HasValue && searchInput.SortDirection.HasValue)
                    {
                        switch (searchInput.SortColumn.Value)
                        {
                            case ApplicationUserSortColumn.CreationTime:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(uie => uie.CreationTime)
                                    : s.Descending(uie => uie.CreationTime);
                                break;
                            case ApplicationUserSortColumn.LicenseActivationTime:
                                sort = searchInput.SortDirection.Value.Equals(SortDirectionType.Asc)
                                    ? s.Ascending(uie => uie.LicenseActivationTime)
                                    : s.Descending(uie => uie.LicenseActivationTime);
                                break;
                        }
                    }
                    else
                    {
                        sort = s.Descending(uie => uie.CreationTime);
                    }
                    return sort;
                })
                );

            if (!response.IsValid)
            {
                ApplicationStaticLogs.ElasticLog.ErrorFormat(
                    "An error occured while searching users, debug information: {0}",
                    response.DebugInformation);
            }

            return response;
        }

        private ValidationResult ValidateUserForSave(ApplicationUser user)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(user.UserName))
                errors.Add(new ValidationError("User.UserName", GeneralValidationErrors.ValueNotSpecified));
            else
            {
                if (user.UserName.Length > 50 || user.UserName.Length < 4)
                    errors.Add(new ValidationError("User.UserName",
                        GeneralValidationErrors.ValueDoesNotHaveAppropriateLength));
                else
                {
                    if (CheckUserNameExists(user.UserName))
                        errors.Add(new ValidationError(AuthenticationValidationErrors.UserNameIsAlreadyTaken));
                }
            }

            if (string.IsNullOrWhiteSpace(user.DisplayName))
                errors.Add(new ValidationError("User.DisplayName", GeneralValidationErrors.ValueNotSpecified));


            return new ValidationResult {Errors = errors};
        }

        private ValidationResult ValidateUserForUpdate(ApplicationUser user)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(user.UserName))
                errors.Add(new ValidationError("User.UserName", GeneralValidationErrors.ValueNotSpecified));
            else
            {
                if (user.UserName.Length > 50 || user.UserName.Length < 4)
                    errors.Add(new ValidationError("User.UserName",
                        GeneralValidationErrors.ValueDoesNotHaveAppropriateLength));
            }

            if (string.IsNullOrWhiteSpace(user.DisplayName))
                errors.Add(new ValidationError("User.DisplayName", GeneralValidationErrors.ValueNotSpecified));


            return new ValidationResult {Errors = errors};
        }

        private bool CheckUserNameExists(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            userName = userName.Trim().ToLowerInvariant();

            var response = ElasticManager.Client.Search<ApplicationUserIE>(u => u
                .Index(ElasticManager.Index)
                .Type(Types.UserType)
                .Query(q => q.Term(ur => ur.UserName, userName)
                )
                );
            var ids = response.Documents.Select(d => ObjectId.Parse(d.ID)).ToList();
            return (ids.Count > 0);
        }

        private ValidationResult ValidatePassword(ApplicationUser user, string password)
        {
            var errors = new List<ValidationError>();
            if (string.IsNullOrWhiteSpace(password))
                errors.Add(new ValidationError("User.Password", GeneralValidationErrors.ValueNotSpecified));
            else
            {
                if (password.Length < 6)
                    errors.Add(new ValidationError("User.Password",
                        GeneralValidationErrors.ValueDoesNotHaveAppropriateLength));
                var result = ValidatePasswordInclusion(password, user.UserName,
                    AuthenticationValidationErrors.PasswordCantContainUserName);
                if (!result.IsValid)
                    errors.AddRange(result.Errors);

                result = ValidatePasswordInclusion(password, user.DisplayName,
                    AuthenticationValidationErrors.PasswordCantContainDisplayName);
                if (!result.IsValid)
                    errors.AddRange(result.Errors);

                result = ValidatePasswordInclusion(password, user.DisplayName,
                    AuthenticationValidationErrors.PasswordCantContainFullName);
                if (!result.IsValid)
                    errors.AddRange(result.Errors);
            }
            if (errors.Any())
                return new ValidationResult {Errors = errors};
            else
                return Common.Util.Validation.ValidationResult.Success;
        }

        private ValidationResult ValidatePasswordInclusion(string password, string propertyValue, string errorKey)
        {
            var errors = new List<ValidationError>();
            if (!string.IsNullOrWhiteSpace(propertyValue))
            {
                if (password.ToLowerInvariant().Contains(propertyValue.ToLowerInvariant()))
                    errors.Add(new ValidationError(errorKey));
                else if (propertyValue.ToLowerInvariant().Contains(password.ToLowerInvariant()))
                    errors.Add(new ValidationError(errorKey));
            }
            return new ValidationResult {Errors = errors};
        }

        private bool RebuildAttachmentSizesSupported(string contentType)
        {
            var supportedTypes = new[] {"image/jpeg", "image/png", "image/gif", "image/bmp"};
            return supportedTypes.Contains(contentType.ToLower());
        }

        private void RebuildAttachmentSizes(byte[] sourceImageBytes, AttachmentStoreEntityType type, ObjectId photoId)
        {
            using (var ms = new MemoryStream())
            {
                ImageBuilder.Current.Build(sourceImageBytes, ms,
                    new ResizeSettings("width=80&height=80&crop=auto&format=jpg&quality=80"));

                ms.Seek(0, SeekOrigin.Begin);
                PhotoGridStore.StoreAttachmentBytes(type, photoId, PhotoStoreSize.Thumbnail, ms.ToArray());
            }

            using (var ms = new MemoryStream())
            {
                ImageBuilder.Current.Build(sourceImageBytes, ms,
                    new ResizeSettings("maxwidth=800&maxheight=2000&format=jpg&quality=80"));

                ms.Seek(0, SeekOrigin.Begin);
                PhotoGridStore.StoreAttachmentBytes(type, photoId, PhotoStoreSize.MediumSize, ms.ToArray());
            }

            using (var ms = new MemoryStream())
            {
                ImageBuilder.Current.Build(sourceImageBytes, ms,
                    new ResizeSettings("maxwidth=160&maxheight=160&format=jpg&quality=80"));

                ms.Seek(0, SeekOrigin.Begin);
                PhotoGridStore.StoreAttachmentBytes(type, photoId, PhotoStoreSize.SmallSize, ms.ToArray());
            }
        }

        #endregion
    }
}