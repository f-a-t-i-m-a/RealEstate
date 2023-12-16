namespace JahanJooy.Common.Util.DomainModel
{
	public interface IEntityContentContainer<TContent> where TContent : class, IEntityContent
	{
		string ContentString { get; set; }
		TContent Content { get; set; }
	}
}