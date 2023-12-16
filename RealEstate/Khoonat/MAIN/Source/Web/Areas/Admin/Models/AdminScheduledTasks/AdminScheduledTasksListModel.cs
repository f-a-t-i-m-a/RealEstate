using System.Collections.Generic;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Web.Areas.Admin.Models.AdminScheduledTasks
{
    public class AdminScheduledTasksListModel
    {
        public List<ScheduledTask> ScheduledTasks { get; set; }
    }
}