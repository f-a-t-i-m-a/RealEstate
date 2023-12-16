using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace JahanJooy.Common.Util.EF
{
	public class EntityChangeExtractor
	{
		public static List<EntityChange> ExtractChanges(DbContext context, IEnumerable<EntityChangeSource> sources)
		{
			var result = new List<EntityChange>();

			foreach (var source in sources)
			{
				if (source == null || source.Entity == null)
					continue;

				DbEntityEntry dbEntityEntry = context.Entry(source.Entity);
				if (dbEntityEntry.State != EntityState.Modified)
					continue;

				string prefix = source.Prefix ?? "";

				foreach (string propName in dbEntityEntry.CurrentValues.PropertyNames)
				{
					DbPropertyEntry dbPropertyEntry = dbEntityEntry.Property(propName);

					if (dbPropertyEntry.IsModified)
						result.Add(new EntityChange {
							PropertyPath = prefix + propName,
							OriginalValue = dbPropertyEntry.OriginalValue,
							ChangedValue = dbPropertyEntry.CurrentValue
						});
				}
			}

			return result;
		}
	}
}