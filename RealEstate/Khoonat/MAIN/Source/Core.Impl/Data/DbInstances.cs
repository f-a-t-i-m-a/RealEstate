using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Data
{
	public class DbInstances : IDisposable
	{
	    private static readonly ILog Log = LogManager.GetLogger(typeof (DbInstances));

		private List<Db> _nonDefaultDbs;
		private Db _defaultDb;
		private bool _disposed;

		public bool IsDisposed { get { return _disposed; } }

		public DbInstances()
		{
			_nonDefaultDbs = null;
			_defaultDb = null;
			_disposed = false;
		}

		public virtual void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
			DisposeDefaultDb();
			DisposeNonDefaultDbs();
		}

		public void SaveAllChanges()
		{
			if (_disposed)
				throw new InvalidOperationException("Already disposed.");

		    try
		    {
		        if (_defaultDb != null)
		            ((DbContext)_defaultDb).SaveChanges();

		        if (_nonDefaultDbs != null)
		            foreach (var nonDefaultDb in _nonDefaultDbs)
		                ((DbContext)nonDefaultDb).SaveChanges();
		    }
		    catch (DbEntityValidationException e)
		    {
		        LogEntityValidationErrors(e);
		        throw;
		    }
		}

		public void CancelAllChanges()
		{
			if (_disposed)
				throw new InvalidOperationException("Already disposed.");

			DisposeDefaultDb();
			DisposeNonDefaultDbs();
		}

		public Db Db
		{
			get
			{
			    if (_disposed)
					throw new InvalidOperationException("Already disposed.");

			    return _defaultDb ?? (_defaultDb = new Db());
			}
		}

		public void SaveDefaultDbChanges()
		{
			if (_disposed)
				throw new InvalidOperationException("Already disposed.");

			if (_defaultDb == null)
				return;

		    try
		    {
		        ((DbContext)_defaultDb).SaveChanges();
		    }
		    catch (DbEntityValidationException e)
		    {
		        LogEntityValidationErrors(e);
		        throw;
		    }
		}

	    public void CancelDefaultDbChanges()
		{
			if (_disposed)
				throw new InvalidOperationException("Already disposed.");
			if (_defaultDb == null)
				return;

			try
			{
				_defaultDb.Dispose();
			}
			catch (Exception e)
			{
				Log.Error("Exception while disposing default Db instance associated with the thread or request", e);
			}

			_defaultDb = null;
		}

		public Db GetFreshDb()
		{
			var result = new Db();

			if (_nonDefaultDbs == null)
				_nonDefaultDbs = new List<Db>();

			_nonDefaultDbs.Add(result);
			return result;
		}

		public void SaveDbChanges(Db db)
		{
			if (_disposed)
				throw new InvalidOperationException("Already disposed.");
			if (_nonDefaultDbs == null || !_nonDefaultDbs.Contains(db))
				throw new ArgumentException("Passed Db instance is not tracked in this thread or request.");

		    try
		    {
		        ((DbContext)db).SaveChanges();
		    }
		    catch (DbEntityValidationException e)
		    {
		        LogEntityValidationErrors(e);
		        throw;
		    }
		}

		public void CancelDbChanges(Db db)
		{
			if (_disposed)
				throw new InvalidOperationException("Already disposed.");
			if (_nonDefaultDbs == null || !_nonDefaultDbs.Contains(db))
				throw new ArgumentException("Passed Db instance is not tracked in this thread or request.");

			try
			{
				db.Dispose();
			}
			catch (Exception e)
			{
				Log.Error("Exception while disposing specific Db instance associated with the thread or request", e);
			}

			_nonDefaultDbs.Remove(db);
		}

		private void DisposeDefaultDb()
		{
			if (_defaultDb == null) 
				return;

			try
			{
				_defaultDb.Dispose();
			}
			catch (Exception e)
			{
				Log.Error("Exception while disposing default Db instance associated with the thread or request", e);
			}

			_defaultDb = null;
		}

		private void DisposeNonDefaultDbs()
		{
			if (_nonDefaultDbs == null) 
				return;

			foreach (var nonDefaultDb in _nonDefaultDbs)
			{
				try
				{
					nonDefaultDb.Dispose();
				}
				catch (Exception e)
				{
					Log.Error("Exception while disposing specific Db instance associated with the thread or request", e);
				}
			}

			_nonDefaultDbs = null;
		}

        private static void LogEntityValidationErrors(DbEntityValidationException e)
        {
            Log.Error("Entity validation errors occured while saving changes to the database...");
            if (e.EntityValidationErrors != null)
            {
                foreach (var error in e.EntityValidationErrors)
                {
                    var entityType = (error.Entry != null && error.Entry.Entity != null) ? error.Entry.Entity.GetType().Name : "UnknownType";
                    Log.ErrorFormat("    - In entity type {0}, errors: {1}", entityType,
                        string.Join("; ", error.ValidationErrors.Select(propertyError => propertyError.PropertyName + ":" + propertyError.ErrorMessage)));
                }
            }
        }
    }
}