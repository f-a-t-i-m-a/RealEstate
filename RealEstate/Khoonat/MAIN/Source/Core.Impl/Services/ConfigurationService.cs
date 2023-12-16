using System.Collections.Generic;
using System.Linq;
using Compositional.Composer;
using JahanJooy.Common.Util.Configuration;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;
using JahanJooy.RealEstate.Domain;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
    [Component]
    public class ConfigurationService : IConfigurationService
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        [ComponentPlug]
        public IApplicationSettings ApplicationSettings { get; set; }

        [ComponentPlug]
        public IActivityLogService ActivityLogService { get; set; }

        #region IConfigurationService implementation

        public List<ConfigurationDataItem> LoadAllItems()
        {
            return DbManager.Db.ConfigurationDataItems.ToList();
        }

        public ConfigurationDataItem LoadItem(string identifier)
        {
            return DbManager.Db.ConfigurationDataItems.SingleOrDefault(i => i.Identifier == identifier);
        }

        public void UpdateItem(string identifier, string value)
        {
            var item = DbManager.Db.ConfigurationDataItemsDbSet.SingleOrDefault(i => i.Identifier == identifier);

            if (item != null)
            {
                if (value == null)
                {
                    DbManager.Db.ConfigurationDataItemsDbSet.Remove(item);
                    ActivityLogService.ReportActivity(TargetEntityType.ConfigurationDataItem, item.ID, ActivityAction.Delete);
                }
                else
                {
                    item.Value = value;
                    ActivityLogService.ReportActivity(TargetEntityType.ConfigurationDataItem, item.ID, ActivityAction.Edit);
                }
            }
            else
            {
                if (value == null)
                {
                    // Requested to delete a non-existing configuration item
                    return;
                }
                var configurationDataItem = new ConfigurationDataItem {Identifier = identifier, Value = value};
                DbManager.Db.ConfigurationDataItemsDbSet.Add(configurationDataItem);
                DbManager.SaveDefaultDbChanges();

                ActivityLogService.ReportActivity(TargetEntityType.ConfigurationDataItem, configurationDataItem.ID,
                    ActivityAction.Create);
            }
            ApplicationSettings.Reload();
        }

        #endregion
    }
}