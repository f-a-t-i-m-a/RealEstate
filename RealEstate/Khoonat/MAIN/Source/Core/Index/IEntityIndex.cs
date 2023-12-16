using System;
using Compositional.Composer;
using JahanJooy.Common.Util.DomainModel;

namespace JahanJooy.RealEstate.Core.Index
{
	[Contract]
	public interface IEntityIndex : IObjectIndex
	{
		Type EntityType { get; }
	}

	[Contract]
	public interface IEntityIndex<in TEntity> : IObjectIndex<TEntity>, IEntityIndex where TEntity : class, IIndexedEntity
	{
	}
}
