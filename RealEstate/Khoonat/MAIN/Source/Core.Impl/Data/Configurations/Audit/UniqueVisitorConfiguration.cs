using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Audit
{
	public class UniqueVisitorConfiguration : EntityTypeConfiguration<UniqueVisitor>
	{
		public UniqueVisitorConfiguration()
		{
			Property(v => v.UniqueIdentifier)
				.IsRequired()
				.HasMaxLength(40);
		}
	}
}