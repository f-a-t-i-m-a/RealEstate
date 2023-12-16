using System.Data.Entity.Spatial;
using JahanJooy.Common.Util.DomainModel;
using JahanJooy.RealEstate.Domain.Enums;

namespace JahanJooy.RealEstate.Domain.Directory
{
	public class AgencyBranchContent : IEntityContent
	{
		public string BranchName { get; set; }
		public string BranchManagerName { get; set; }

		public long VicinityID { get; set; }
		public DbGeography GeographicLocation { get; set; }
		public GeographicLocationSpecificationType? GeographicLocationType { get; set; }
		public string FullAddress { get; set; }

		public string Phone1 { get; set; }
		public string Phone2 { get; set; }
		public string CellPhone1 { get; set; }
		public string CellPhone2 { get; set; }
		public string Fax { get; set; }
		public string Email { get; set; }
		public string SmsNumber { get; set; }
		public string Description { get; set; }

		public string ActivityRegion { get; set; }
		public DbGeography GeoActivityRegion { get; set; }
	}
}