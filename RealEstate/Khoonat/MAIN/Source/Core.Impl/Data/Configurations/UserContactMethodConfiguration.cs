using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations
{
	public class UserContactMethodConfiguration : EntityTypeConfiguration<UserContactMethod>
	{
		public UserContactMethodConfiguration()
		{
			Property(u => u.ContactMethodText)
				.IsRequired()
				.HasMaxLength(150);

			HasRequired(ucm => ucm.User)
				.WithMany(u => u.ContactMethods)
				.HasForeignKey(ucm => ucm.UserID)
				.WillCascadeOnDelete(true);
		}
	}
}