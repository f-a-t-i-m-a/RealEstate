using Compositional.Composer;
using JahanJooy.RealEstate.Domain.Audit;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IActivityLogService
	{
		void ReportActivity(TargetEntityType targetEntity, long? targetEntityID, ActivityAction action, 
			string actionDetails = null, bool? succeeded = null,
			TargetEntityType? parentEntity = null, long? parentEntityID = null,
			DetailEntityType? detailEntity = null, long? detailEntityID = null,
			AuditEntityType? auditEntity = null, long? auditEntityID = null);

		void MarkTaskComplete(TargetEntityType targetEntityType, long targetEntityID, ActivityAction action);
	}
}