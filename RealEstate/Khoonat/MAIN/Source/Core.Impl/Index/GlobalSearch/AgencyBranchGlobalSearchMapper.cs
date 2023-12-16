using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.DomainServiceContracts;
using JahanJooy.Common.Util.Spatial;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Index;
using JahanJooy.RealEstate.Core.Services.Dto.GlobalSearch;
using JahanJooy.RealEstate.Domain.Directory;

namespace JahanJooy.RealEstate.Core.Impl.Index.GlobalSearch
{
	[Component]
	public class AgencyBranchGlobalSearchMapper : IGlobalSearchEntityMapper<AgencyBranch>
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyBranchContent> BranchContentSerializer { get; set; }

		[ComponentPlug]
		public IEntityContentSerializer<AgencyContent> AgencyContentSerializer { get; set; }

		public GlobalSearchIndexItem Map(AgencyBranch branch)
		{
			if (branch == null)
				return null;

			AgencyContentSerializer.DeserializeIfNeeded(branch.Agency);
			BranchContentSerializer.DeserializeIfNeeded(branch);

			return new GlobalSearchIndexItem
			{
				Type = GlobalSearchRecordType.AgencyBranch,
				SubType = string.Empty,
				ID = branch.ID,
				Title = branch.Agency.Content.Name,
				VicinityID = branch.Content.VicinityID,
				GeographicLocation = branch.Content.GeographicLocation.ToLatLng(),
				GeographicLocationType = branch.Content.GeographicLocationType,
				Text = string.Empty, // TODO
				Tags = new List<string>(), // TODO
				PrincipalsAllowedToView = new List<string>(), // TODO
				Archived = false,
				Deleted = branch.DeleteTime.HasValue
			};
		}

		public AgencyBranch Retrieve(GlobalSearchResultItem item)
		{
			if (item == null || item.Type != GlobalSearchRecordType.AgencyBranch || item.ID <= 0)
				return null;

			return DbManager.Db.AgencyBranches.Include(b => b.Agency).SingleOrDefault(l => l.ID == item.ID);
		}
	}
}