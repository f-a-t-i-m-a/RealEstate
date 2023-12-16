using System.Data.Entity.Spatial;
using JahanJooy.Common.Util.Spatial;
using ServiceStack.Text;

namespace JahanJooy.Common.Util.Serialization
{
	public static class JsSerializationConfigUtil
	{
		public static void InitializeDefaultConfiguration()
		{
			JsConfig.TreatEnumAsInteger = true;
			JsConfig.DateHandler = DateHandler.ISO8601;

			JsConfig<DbGeography>.SerializeFn = geography => geography.ToWkt();
			JsConfig<DbGeography>.DeSerializeFn = s => s.IfNotNull(DbGeography.FromText);
		}
	}
}