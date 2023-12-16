using System;
using JahanJooy.Common.Util.DomainModel;

namespace JahanJooy.RealEstate.Domain.Directory
{
	public class AgencyContent : IEntityContent
	{
		public string Name { get; set; }
		public string ManagerName { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
        public Guid? LogoStoreItemID { get; set; }
	}
}