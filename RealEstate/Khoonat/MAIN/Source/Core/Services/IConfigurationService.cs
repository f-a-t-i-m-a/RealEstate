using System.Collections.Generic;
using Compositional.Composer;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Services
{
    [Contract]
    public interface IConfigurationService
    {
        List<ConfigurationDataItem> LoadAllItems();
        ConfigurationDataItem LoadItem(string identifier);

        void UpdateItem(string identifier, string value);
    }
}