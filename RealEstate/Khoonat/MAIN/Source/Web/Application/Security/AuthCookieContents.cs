namespace JahanJooy.RealEstate.Web.Application.Security
{
	public class AuthCookieContents
	{
		public long UserID { get; set; }
		public string LoginName { get; set; }

		public bool IsValid
		{
			get
			{
				return UserID > 0 &&
				       !string.IsNullOrWhiteSpace(LoginName);
			}
		}
	}
}