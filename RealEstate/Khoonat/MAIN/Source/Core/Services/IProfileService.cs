using Compositional.Composer;
using JahanJooy.RealEstate.Domain;

namespace JahanJooy.RealEstate.Core.Services
{
	[Contract]
	public interface IProfileService
	{
		void EditProfile(User user);
		 
	}
}