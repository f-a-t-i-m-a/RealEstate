using System.Data.Entity.ModelConfiguration;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Impl.Data.Configurations
{
    public class ConfigurationDataItemConfiguration : EntityTypeConfiguration<ConfigurationDataItem>
    {
        public ConfigurationDataItemConfiguration()
        {
            Property(i => i.Identifier).IsRequired().HasMaxLength(100);
            Property(i => i.Value).HasMaxLength(2000);
        }
    }
}