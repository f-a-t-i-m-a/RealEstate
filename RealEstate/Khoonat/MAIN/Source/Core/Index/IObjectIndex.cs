using System.Collections.Generic;
using Compositional.Composer;

namespace JahanJooy.RealEstate.Core.Index
{
	[Contract]
	public interface IObjectIndex
	{
		string IndexID { get; }
	}

	[Contract]
	public interface IObjectIndex<in TObject> : IObjectIndex where TObject : class
	{
		void AddOrReplace(IEnumerable<TObject> objs);
		void Delete(IEnumerable<TObject> objs);
	}

}