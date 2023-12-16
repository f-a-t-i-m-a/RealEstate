using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain.Billing;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations.Billing
{
    public abstract class BillingSourceEntityConfiguration<TEntityType> : EntityTypeConfiguration<TEntityType>
        where TEntityType : class, IBillingSourceEntity
    {
        protected BillingSourceEntityConfiguration()
        {
            HasOptional(e => e.TargetUser)
                .WithMany()
                .HasForeignKey(e => e.TargetUserID)
                .WillCascadeOnDelete(false);

            HasOptional(e => e.ForwardTransaction)
                .WithMany()
                .HasForeignKey(e => e.ForwardTransactionID)
                .WillCascadeOnDelete(false);

            HasOptional(e => e.ReverseTransaction)
                .WithMany()
                .HasForeignKey(e => e.ReverseTransactionID)
                .WillCascadeOnDelete(false);
        }
    }
}