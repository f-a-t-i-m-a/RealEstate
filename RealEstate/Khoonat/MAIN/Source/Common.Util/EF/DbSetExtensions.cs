using System.Collections.Generic;
using System.Data.Entity;

namespace JahanJooy.Common.Util.EF
{
    public static class DbSetExtensions
    {
        public static void AddAll<TEntity>(this IDbSet<TEntity> dbSet, IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach (var entity in entities)
            {
                dbSet.Add(entity);
            }
        }
    }
}