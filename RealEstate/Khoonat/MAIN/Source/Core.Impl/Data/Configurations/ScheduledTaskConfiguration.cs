using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations
{
	public class ScheduledTaskConfiguration : EntityTypeConfiguration<ScheduledTask>
	{
		public ScheduledTaskConfiguration()
		{
			Property(st => st.TaskKey).IsRequired().HasMaxLength(100);
			Property(st => st.TaskProgress).HasMaxLength(2000);

			Property(st => st.LastExecutionResult).HasMaxLength(2000);
			Property(st => st.LastErrorMessage).HasMaxLength(2000);
		}
	}
}