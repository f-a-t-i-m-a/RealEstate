namespace JahanJooy.RealEstate.Core.Services.Dto
{
	public class EntityUpdateResult
	{
		public static readonly EntityUpdateResult Default = new EntityUpdateResult { CancelUpdate = false }; 
		public static readonly EntityUpdateResult Cancel = new EntityUpdateResult { CancelUpdate = true }; 

		public bool CancelUpdate { get; set; } 
	}
}