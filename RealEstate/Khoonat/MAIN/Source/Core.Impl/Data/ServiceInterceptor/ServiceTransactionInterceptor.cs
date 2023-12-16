using Compositional.Composer;
using Compositional.Composer.Interceptor;

namespace JahanJooy.RealEstate.Core.Impl.Data.ServiceInterceptor
{
	[Contract]
	[Component]
	public class ServiceTransactionInterceptor : ICallInterceptor
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }

		public void BeforeCall(CallInfo callInfo)
		{
			try
			{
				if (ServiceContext.TxRecursionLevel > 0)
					return;

				// TODO: Begin Transaction
			}
			finally
			{
				ServiceContext.TxRecursionLevel++;
			}
		}

		public void AfterCall(CallInfo callInfo)
		{
			ServiceContext.TxRecursionLevel--;

			if (ServiceContext.TxRecursionLevel > 0)
				return;

			if (callInfo.ThrownException != null)
			{
				if (DbManager.IsAnyDbInstancesInScope)
				{
					// TODO: Rollback Transaction
					DbManager.CancelAllChanges();
				}
			}
			else
			{
				if (DbManager.IsAnyDbInstancesInScope)
				{
					// TODO: Commit Transaction
					DbManager.SaveAllChanges();
				}
			}
		}
	}
}