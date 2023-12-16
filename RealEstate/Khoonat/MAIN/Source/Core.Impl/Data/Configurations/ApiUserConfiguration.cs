using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations
{
	public class ApiUserConfiguration : EntityTypeConfiguration<ApiUser>
	{
		public ApiUserConfiguration()
		{
			Property(au => au.Key).HasMaxLength(50);
		}
	}
}