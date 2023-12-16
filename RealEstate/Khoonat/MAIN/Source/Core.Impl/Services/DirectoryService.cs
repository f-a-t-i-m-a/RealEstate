using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using Compositional.Composer;
using ImageResizer;
using JahanJooy.Common.Util.Collections;
using JahanJooy.Common.Util.Components;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Streams;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Core.Services.Enums;
using JahanJooy.RealEstate.Domain.Audit;
using JahanJooy.RealEstate.Domain.Directory;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class DirectoryService : IDirectoryService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyPhotoService));

        private const string LogoStoreId = "agencyLogo";
        private const string LogoThumbnailStoreId = "agencyLogo-Thumbnail";

        #region Image resource constants
        private const string ResourceKeyImageCouldNotBeRetrieved = "JahanJooy.RealEstate.Core.Impl.Resources.ErrorImages.ImageCouldNotBeRetrieved.png";
        private const string ResourceKeyThumbnailCouldNotBeRetrieved = "JahanJooy.RealEstate.Core.Impl.Resources.ErrorImages.ThumbnailCouldNotBeRetrieved.png";
        #endregion

        static DirectoryService()
	    {
	        BinaryStorageComponent.RegisterStoreConfigurationKeys(LogoStoreId);
            BinaryStorageComponent.RegisterStoreConfigurationKeys(LogoThumbnailStoreId);
	    }
        private bool _initialized = false;

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyContent> AgencyContentSerializer { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyBranchContent> AgencyBranchContentSerializer { get; set; }

        [ComponentPlug]
        public BinaryStorageComponent BinaryStorage { get; set; }

        #region Implementation of Agency methods in IDirectoryService

        public ValidatedResult<Agency> SaveAgency(AgencyContent content, AgencyBranchContent mainBranchContent, IEnumerable<AgencyBranchContent> branchContents, bool? approved)
        {
            if (ServiceContext.CurrentSession == null || ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

			if (content == null)
				throw new ArgumentNullException("content");
			if (mainBranchContent == null)
				throw new ArgumentNullException("mainBranchContent");

            var validationResult = ValidateForSave(content);
            if (!validationResult.IsValid)
                return validationResult.ToFailedValidatedResult<Agency>();

			validationResult = ValidateForSave(mainBranchContent);
			if (!validationResult.IsValid)
				return validationResult.ToFailedValidatedResult<Agency>();

			// The branchContents collection may contain entities.
            // We can support creating the agency and the branch together.

            if (branchContents.SafeAny())
            {
                foreach (var branchContent in branchContents)
                {
                    validationResult = ValidateForSave(branchContent);
                    if (!validationResult.IsValid)
                        return validationResult.ToFailedValidatedResult<Agency>();
                }
            }

            var newAgency = new Agency
                            {
								Content = content,
								ContentString = AgencyContentSerializer.Serialize(content),
								AgencyBranches = new Collection<AgencyBranch>(),
                                Approved = approved
                            };

			newAgency.AgencyBranches.Add(new AgencyBranch
			                             {
				                             Content = mainBranchContent,
				                             ContentString = AgencyBranchContentSerializer.Serialize(mainBranchContent),
				                             IsMainBranch = true
			                             });

            if (branchContents.SafeAny())
            {
                newAgency.AgencyBranches = new List<AgencyBranch>();
				foreach (var branchContent in branchContents)
                {
                    newAgency.AgencyBranches.Add(new AgencyBranch
                                                 {
	                                                 Content = branchContent,
													 ContentString = AgencyBranchContentSerializer.Serialize(branchContent)
                                                 });
                }
            }

            DbManager.Db.AgenciesDbSet.Add(newAgency);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.Agency, newAgency.ID, ActivityAction.Create);

            return ValidatedResult<Agency>.Success(newAgency);
        }

		public ValidatedResult<Agency> UpdateAgency(long agencyId, AgencyContent agencyContent)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

			var validationResult = ValidateForSave(agencyContent);
            if (!validationResult.IsValid)
                return validationResult.ToFailedValidatedResult<Agency>();

            Agency agency = DbManager.Db.AgenciesDbSet.Find(agencyId);
            if (agency == null)
                throw new InvalidOperationException("Agency not found");

	        agency.Content = agencyContent;
	        agency.ContentString = AgencyContentSerializer.Serialize(agencyContent);

            ActivityLogService.ReportActivity(TargetEntityType.Agency, agency.ID, ActivityAction.Edit);

            return ValidatedResult<Agency>.Success(agency);
        }

		public ValidatedResult<Agency> UpdateAgency(long agencyId, AgencyContent agencyContent, AgencyBranchContent mainBranchContent)
	    {
			var result = UpdateAgency(agencyId, agencyContent);
		    if (!result.IsValid)
			    return result;

		    var mainBranch = DbManager.Db.AgencyBranchesDbSet.FirstOrDefault(ab => ab.AgencyID == agencyId && ab.IsMainBranch);
		    if (mainBranch != null)
		    {
			    var updateResult = UpdateAgencyBranch(mainBranch.ID, mainBranchContent);
			    if (!updateResult.IsValid)
				    return updateResult.ToFailedValidatedResult<Agency>();

		        return result;
		    }

		    var saveResult = SaveAgencyBranch(agencyId, mainBranchContent);
		    if (!saveResult.IsValid)
				return saveResult.ToFailedValidatedResult<Agency>();

			saveResult.Result.IsMainBranch = true;
			return result;
		}

		public ValidationResult DeleteAgency(long id)
		{
			Agency originalAgency =
				DbManager.Db.AgenciesDbSet.Include(a => a.AgencyBranches).FirstOrDefault(a => a.ID == id);

			if (originalAgency == null)
				throw new InvalidOperationException("Agency not found");

			originalAgency.DeleteTime = DateTime.Now;

			if (originalAgency.AgencyBranches != null)
			{
				foreach (var branch in originalAgency.AgencyBranches)
				{
					branch.DeleteTime = DateTime.Now;
				}
			}

			ActivityLogService.ReportActivity(TargetEntityType.Agency, id, ActivityAction.Delete);

			return ValidationResult.Success;
		}

        public Agency SetLogo(long agencyID, Stream contents)
        {
            EnsureInitialized();
            bool hasThumbnail = false;
            var agency = DbManager.Db.AgenciesDbSet
                .SingleOrDefault(a => a.ID == agencyID);
            if (agency == null) 
                throw new ArgumentException("No such Agency exists.");
            if (agency.Content != null && agency.Content.LogoStoreItemID.HasValue)
            {
                hasThumbnail = true;
            }
            AgencyContentSerializer.DeserializeIfNeeded(agency);

            agency.Content.LogoStoreItemID = Guid.NewGuid();

            AgencyContentSerializer.Serialize(agency);
           
            byte[] untouchedBytes;
            using (var ms = new MemoryStream())
            {
                try
                {
                    ImageBuilder.Current.Build(contents, ms, new ResizeSettings("maxwidth=600&maxheight=600&format=jpg&quality=90&autorotate=true"));
                }
                catch (Exception e)
                {
                    Log.Error("Could not process uploaded image", e);
                    return null;
                }

                untouchedBytes = ms.ToArray();
            }

            BinaryStorage.StoreBytes(LogoStoreId, agency.Content.LogoStoreItemID.Value, untouchedBytes);
            if (!hasThumbnail)
                RebuildLogoSizes(agency, untouchedBytes);

            ActivityLogService.ReportActivity(TargetEntityType.Agency, agency.ID, ActivityAction.EditDetail);
            return agency;
        }

        public byte[] GetAgencyLogoBytes(Guid logoStoreItemId)
        {
            EnsureInitialized();

            try
            {
                return BinaryStorage.RetrieveBytes(LogoStoreId, logoStoreItemId);
            }
            catch (Exception e)
            {
                Log.Error("Could not load image ID " + logoStoreItemId + " from store " + LogoStoreId, e);
                return ManifestResourceUtil.GetManifestResourceBytes(Assembly.GetExecutingAssembly(), ResourceKeyImageCouldNotBeRetrieved);
            }
        }

        public byte[] GetThumbnailLogoBytes(Guid logoStoreItemId)
        {
            EnsureInitialized();

            try
            {
                return BinaryStorage.RetrieveBytes(LogoThumbnailStoreId, logoStoreItemId);
            }
            catch (Exception e)
            {
                Log.Error("Could not load image ID " + logoStoreItemId + " from store " + LogoThumbnailStoreId, e);
                return ManifestResourceUtil.GetManifestResourceBytes(Assembly.GetExecutingAssembly(), ResourceKeyThumbnailCouldNotBeRetrieved);
            }
        }

        public Agency SetThumbnailLogo(long agencyID, Stream contents)
        {
            EnsureInitialized();

            var agency = DbManager.Db.Agencies.SingleOrDefault(l => l.ID == agencyID);
            if (agency == null)
                throw new ArgumentException("No such Agency exists.");

            byte[] untouchedBytes;
            using (var ms = new MemoryStream())
            {
                try
                {
                    ImageBuilder.Current.Build(contents, ms,new ResizeSettings("width=80&height=80&crop=auto&format=jpg&quality=80"));
                }
                catch (Exception e)
                {
                    Log.Error("Could not process uploaded image", e);
                    return null;
                }

                untouchedBytes = ms.ToArray();
            }

            AgencyContentSerializer.DeserializeIfNeeded(agency);
            AgencyContentSerializer.Serialize(agency);

            if (agency.Content.LogoStoreItemID.HasValue)
            {
                using (var ms = new MemoryStream())
                {
                    ImageBuilder.Current.Build(untouchedBytes, ms,
                        new ResizeSettings("width=80&height=80&crop=auto&format=jpg&quality=80"));
                    BinaryStorage.StoreBytes(LogoThumbnailStoreId, agency.Content.LogoStoreItemID.Value, ms.ToArray());
                }
            }
            return agency;
        }


		#endregion

        #region Implementation of AgencyBranch methods in IDirectoryService

        public ValidatedResult<AgencyBranch> SaveAgencyBranch(long agencyId, AgencyBranchContent agencyBranchContent)
        {
            if (ServiceContext.CurrentSession == null || ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

			var validationResult = ValidateForSave(agencyBranchContent);
            if (!validationResult.IsValid)
                return ValidatedResult<AgencyBranch>.Failure(validationResult.Errors);

            var newAgencyBranch = new AgencyBranch
                                  {
									  AgencyID = agencyId,
	                                  Content = agencyBranchContent,
									  ContentString = AgencyBranchContentSerializer.Serialize(agencyBranchContent)
                                  }; 

            DbManager.Db.AgencyBranchesDbSet.Add(newAgencyBranch);
            DbManager.SaveDefaultDbChanges();

            ActivityLogService.ReportActivity(TargetEntityType.AgencyBranch, newAgencyBranch.ID, ActivityAction.Create);

            return ValidatedResult<AgencyBranch>.Success(newAgencyBranch);
        }

		public ValidatedResult<AgencyBranch> UpdateAgencyBranch(long agencyBranchId, AgencyBranchContent agencyBranchContent)
        {
            if (ServiceContext.CurrentSession.IsCrawler)
                throw new InvalidOperationException("Not a user-interactive session.");

            AgencyBranch agencyBranch = DbManager.Db.AgencyBranchesDbSet.Find(agencyBranchId);
            if (agencyBranch == null)
                throw new InvalidOperationException("AgencyBranch not found");

	        agencyBranch.Content = agencyBranchContent;
	        agencyBranch.ContentString = AgencyBranchContentSerializer.Serialize(agencyBranchContent);

            ActivityLogService.ReportActivity(TargetEntityType.AgencyBranch, agencyBranch.ID, ActivityAction.Edit);

            return ValidatedResult<AgencyBranch>.Success(agencyBranch);
        }

        public ValidationResult DeleteAgencyBranch(long id)
        {
            AgencyBranch originalAgencyBranch = DbManager.Db.AgencyBranchesDbSet.Find(id);

            if (originalAgencyBranch == null)
                throw new InvalidOperationException("AgencyBranch not found");

            originalAgencyBranch.DeleteTime = DateTime.Now;

            ActivityLogService.ReportActivity(TargetEntityType.AgencyBranch, id, ActivityAction.Delete);

            return ValidationResult.Success;
        }

        public void SetApproved(long agencyID, bool? approved)
        {
            var agency = DbManager.Db.AgenciesDbSet.SingleOrDefault(a => a.ID == agencyID);
            if (agency == null)
                throw new ArgumentException("agency not found.");

            agency.Approved = approved;

        }

        #endregion

        #region Private methds

        private ValidationResult ValidateForSave(AgencyContent agency)
        {
            if (string.IsNullOrWhiteSpace(agency.Name))
                return ValidationResult.Failure("DisplayName", GeneralValidationErrors.ValueNotSpecified);

            return ValidationResult.Success;
        }

        private ValidationResult ValidateForSave(AgencyBranchContent agencyBranch)
        {
            if (string.IsNullOrWhiteSpace(agencyBranch.BranchName))
            {
                return ValidationResult.Failure("BranchName", GeneralValidationErrors.ValueNotSpecified);
            }

            if (agencyBranch.VicinityID == 0)
            {
                return ValidationResult.Failure("VicinityID", GeneralValidationErrors.ValueNotSpecified);
            }

            return ValidationResult.Success;
        }

        private void EnsureInitialized()
        {
            if (_initialized)
                return;

            BinaryStorage.RegisterStoreId(LogoStoreId);
            BinaryStorage.RegisterStoreId(LogoThumbnailStoreId);

            _initialized = true;
        }

        private void RebuildLogoSizes(Agency agency, byte[] sourceImageBytes)
        {
            EnsureInitialized();
            if (agency.Content.LogoStoreItemID.HasValue)
            {
                using (var ms = new MemoryStream())
                {
                    ImageBuilder.Current.Build(sourceImageBytes, ms,
                        new ResizeSettings("width=80&height=80&crop=auto&format=jpg&quality=80"));
                    BinaryStorage.StoreBytes(LogoThumbnailStoreId, agency.Content.LogoStoreItemID.Value, ms.ToArray());
                }
            }
        }

        #endregion
    }
}