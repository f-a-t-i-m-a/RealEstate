using System;
using System.Collections.Generic;
using System.IO;
using Compositional.Composer;
using JahanJooy.Common.Util.Validation;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
    public interface IDirectoryService
	{
		ValidatedResult<Agency> SaveAgency(AgencyContent content, AgencyBranchContent mainBranchContent, IEnumerable<AgencyBranchContent> branchContents, bool? approved);
		ValidatedResult<Agency> UpdateAgency(long agencyId, AgencyContent content);
		ValidatedResult<Agency> UpdateAgency(long agencyId, AgencyContent agencyContent, AgencyBranchContent mainBranchContent);
		ValidationResult DeleteAgency(long id);
        void SetApproved(long agencyID, bool? approved);

        ValidatedResult<AgencyBranch> SaveAgencyBranch(long agencyId, AgencyBranchContent agencyBranchContent);
		ValidatedResult<AgencyBranch> UpdateAgencyBranch(long agencyBranchId, AgencyBranchContent agencyBranchContent);
        ValidationResult DeleteAgencyBranch(long id);

        Agency SetLogo(long agencyID, Stream contents);

        byte[] GetAgencyLogoBytes(Guid logoStoreItemId);

        byte[] GetThumbnailLogoBytes(Guid logoStoreItemId);

        Agency SetThumbnailLogo(long agencyID, Stream contents);
	}
}