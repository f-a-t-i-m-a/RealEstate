using Compositional.Composer;
using JahanJooy.RealEstate.Core.Impl.Data;
using JahanJooy.RealEstate.Core.Services;

namespace JahanJooy.RealEstate.Core.Impl.Services
{
	[Component]
	public class RequestService : IRequestService
	{
		[ComponentPlug]
		public DbManager DbManager { get; set; }


	}
}