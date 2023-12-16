using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Compositional.Composer;
using JahanJooy.Common.Util.ApiModel.Pagination;
using JahanJooy.RealEstateAgency.Api.Web.Base;
using JahanJooy.RealEstateAgency.Domain.Audit;
using JahanJooy.RealEstateAgency.Domain.Enums.User;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using JahanJooy.RealEstateAgency.Util.Identity;
using JahanJooy.RealEstateAgency.Util.Models.Base;
using JahanJooy.RealEstateAgency.Util.Models.UserActivities;
using JahanJooy.RealEstateAgency.Util.UserActivityLog;
using MongoDB.Driver;

namespace JahanJooy.RealEstateAgency.Api.Web.Controllers
{
    [Contract]
    [Component]
    [ComponentCache(null)]
    [RoutePrefix("useractivity")]
    public class UseractivityController : ExtendedApiController
    {
        #region Injected dependencies

        [ComponentPlug]
        public DbManager DbManager { get; set; }

        private int MaxPageSize = 10000;
        private int DefaultPageSize = 10;

        #endregion

        #region Action methods

        [HttpGet, Route("get/{correlationid}")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        public UserActivityDetails Get(string correlationid)
        {
            var filter = Builders<UserActivity>.Filter.Eq("CorrelationID", correlationid);
            var activityList = DbManager.UserActivity.Find(filter).ToListAsync().Result;

            var mainActivity = activityList.SingleOrDefault(a => a.IsMainActivity);
            if (mainActivity == null)
                return new UserActivityDetails();

            var result = Mapper.Map<UserActivityDetails>(mainActivity);
            result.RelativeActivities =
                Mapper.Map<List<UserActivitySummary>>(activityList.Where(a => a.ID != mainActivity.ID).ToList());

            return result;
        }

        [HttpPost, Route("search")]
        [ApplicationAuthorize(BuiltInRoles = new[] { BuiltInRole.Administrator })]
        [SkipUserActivityLogging]
        public SearchOutput Search(SearchInput input)
        {
            if (input.PageSize == 0)
                input.PageSize = DefaultPageSize;
            else if (input.PageSize < 0 || input.PageSize > MaxPageSize)
                input.PageSize = MaxPageSize;

            var filterBuilder = Builders<UserActivity>.Filter;
            var filter = FilterDefinition<UserActivity>.Empty;

            if (!input.AllActivity.HasValue || !input.AllActivity.Value)
            {
                filter = filterBuilder.Where(u => u.IsMainActivity);
            }

            if (input.ApplicationType.HasValue)
                filter &= filterBuilder.Where(u => u.ApplicationType.Equals(input.ApplicationType.Value));
            if (!string.IsNullOrEmpty(input.Controller))
                filter &= filterBuilder.Where(u => u.ControllerName.Contains(input.Controller));
            if (!string.IsNullOrEmpty(input.ActionName))
                filter &= filterBuilder.Where(u => u.ActionName.Contains(input.ActionName));
            if (!string.IsNullOrEmpty(input.CorrelationId))
                filter &= filterBuilder.Where(u => u.CorrelationID.Contains(input.CorrelationId));
            if (!string.IsNullOrEmpty(input.TargetState))
                filter &= filterBuilder.Where(u => u.TargetState.Contains(input.TargetState));
            if (!string.IsNullOrEmpty(input.ActivitySubType))
                filter &= filterBuilder.Where(u => u.ActivitySubType.Contains(input.ActivitySubType));
            if (input.HasTargetState.HasValue && input.HasTargetState.Value)
                filter &= filterBuilder.Where(u => u.TargetState != null);
            if (input.HasComment.HasValue && input.HasComment.Value)
                filter &= filterBuilder.Where(u => u.Comment.Length != 0);
            if (input.UserId.HasValue)
                filter &= filterBuilder.Where(u => u.User.Id == input.UserId.Value.ToString());
            if (input.TargetType.HasValue)
                filter &= filterBuilder.Where(u => u.TargetType.Equals(input.TargetType.Value));
            if (input.TargetId.HasValue)
                filter &= filterBuilder.Where(u => u.TargetID.Equals(input.TargetId.Value));
            if (input.Succeeded.HasValue)
                filter &= filterBuilder.Where(u => u.Succeeded.Equals(input.Succeeded.Value));
            if (input.ActivityType.HasValue)
                filter &= filterBuilder.Where(u => u.ActivityType.Equals(input.ActivityType.Value));
            if (input.ParentType.HasValue)
                filter &= filterBuilder.Where(u => u.ParentType.Equals(input.ParentType.Value));
            if (input.ParentId.HasValue)
                filter &= filterBuilder.Where(u => u.ParentID.Equals(input.ParentId.Value));
            if (input.DetailType.HasValue)
                filter &= filterBuilder.Where(u => u.DetailType.Equals(input.DetailType.Value));
            if (input.DetailId.HasValue)
                filter &= filterBuilder.Where(u => u.DetailID.Equals(input.DetailId.Value));
            if (input.FromActivityTime.HasValue)
                filter &= filterBuilder.Where(u => u.ActivityTime >= input.FromActivityTime.Value);
            if (input.FromActivityTimeHour.HasValue)
                filter &= filterBuilder.Where(u => u.ActivityTime.Hour >= input.FromActivityTimeHour.Value);
            if (input.FromActivityTimeMinute.HasValue)
                filter &= filterBuilder.Where(u => u.ActivityTime.Minute >= input.FromActivityTimeMinute.Value);
            if (input.ToActivityTime.HasValue)
                filter &= filterBuilder.Where(u => u.ActivityTime <= input.ToActivityTime.Value);
            if (input.ToActivityTimeHour.HasValue)
                filter &= filterBuilder.Where(u => u.ActivityTime.Hour <= input.ToActivityTimeHour.Value);
            if (input.ToActivityTimeMinute.HasValue)
                filter &= filterBuilder.Where(u => u.ActivityTime.Minute <= input.ToActivityTimeMinute.Value);

            SortDefinition<UserActivity> sort = null;
            if (input.SortColumn.HasValue && input.SortDirection.HasValue)
            {
                var sortBuilder = Builders<UserActivity>.Sort;
                switch (input.SortColumn.Value)
                {
                    case UserActivitySortColumn.ActivityTime:
                        sort = input.SortDirection.Value.Equals(SortDirectionType.Asc)
                            ? sortBuilder.Ascending(pie => pie.ActivityTime)
                            : sortBuilder.Descending(pie => pie.ActivityTime);
                        break;
                }
            }
            else if (!input.SortColumn.HasValue && input.SortDirection.HasValue)
            {
                sort = input.SortDirection.Value.Equals(SortDirectionType.Asc)
                    ? Builders<UserActivity>.Sort.Ascending(pie => pie.ActivityTime)
                    : Builders<UserActivity>.Sort.Descending(pie => pie.ActivityTime);
            }

            List<UserActivitySummary> activities = DbManager.UserActivity.Find(filter)
                .Sort(sort)
                .Skip(input.StartIndex)
                .Limit(input.PageSize)
                .Project(p => Mapper.Map<UserActivitySummary>(p))
                .ToListAsync().Result;

            var totalSize = DbManager.UserActivity.Find(filter)
                .CountAsync().Result;

            var result = new PagedListOutput<UserActivitySummary>
            {
                PageItems = activities,
                PageNumber = (input.StartIndex/input.PageSize) + 1,
                TotalNumberOfItems = (int) totalSize,
                TotalNumberOfPages = (int) Math.Ceiling((decimal) totalSize/input.PageSize)
            };

            return new SearchOutput
            {
                UserActivities = result
            };
        }

        #endregion

        #region Private helper methods

        #endregion
    }
}