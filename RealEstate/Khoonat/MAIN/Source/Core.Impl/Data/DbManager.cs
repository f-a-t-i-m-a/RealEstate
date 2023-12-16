using System;
using System.Collections.Generic;
using Compositional.Composer;
using log4net;

namespace JahanJooy.RealEstate.Core.Impl.Data
{
	[Contract]
	[Component]
	public class DbManager
	{
	    private static readonly ILog Log = LogManager.GetLogger(typeof (DbManager));

		[ComponentPlug]
		public IComposer Composer { get; set; }

		#region Backing-store for db instances

		[ThreadStatic]
		private static Stack<DbInstances> _threadBoundInstancesStack;

		private DbInstances CurrentInstances
		{
			get
			{
			    var threadBoundInstances = CurrentThreadBoundInstances;
                if (threadBoundInstances != null)
					return threadBoundInstances;

				if (!DbRequestBoundInstances.CanBeUsed)
					throw new InvalidOperationException("There is no request in the context, and no thread-bound Db has been initialized.");

				return Composer.GetComponent<DbRequestBoundInstances>();
			}
		}

        private DbInstances CurrentThreadBoundInstances
        {
            get
            {
                if (_threadBoundInstancesStack == null || _threadBoundInstancesStack.Count <= 0)
                    return null;

                return _threadBoundInstancesStack.Peek();
            }
        }

		#endregion

		#region Default DB methods

		public Db Db
		{
			get
			{
				return CurrentInstances.Db;
			}
		}

		public void SaveDefaultDbChanges()
		{
			CurrentInstances.SaveDefaultDbChanges();
		}

		public void CancelDefaultDbChanges()
		{
			CurrentInstances.CancelDefaultDbChanges();
		}

		#endregion

		#region Explicit DB methods

		public Db GetFreshDb()
		{
			return CurrentInstances.GetFreshDb();
		}

		public void SaveDbChanges(Db db)
		{
			CurrentInstances.SaveDbChanges(db);
		}

		public void CancelDbChanges(Db db)
		{
			CurrentInstances.CancelDbChanges(db);
		}

		#endregion

		public DbInstances StartThreadBoundScope()
		{
			if (_threadBoundInstancesStack != null && _threadBoundInstancesStack.Count > 0)
				throw new InvalidOperationException("There is already a thread-bound DbInstances on this thread.");

		    return PushNewThreadBoundScope();
		}

	    public DbInstances PushNewThreadBoundScope()
	    {
	        if (_threadBoundInstancesStack == null)
                _threadBoundInstancesStack = new Stack<DbInstances>();

	        var result = new ThreadBoundDisposable(this);
	        _threadBoundInstancesStack.Push(result);

	        return result;
	    }

		public void EndThreadBoundScope()
		{
		    var currentThreadBoundInstances = CurrentThreadBoundInstances;
            if (currentThreadBoundInstances == null)
				throw new InvalidOperationException("There is no thread-bound DbInstances on this thread to end.");

            if (!currentThreadBoundInstances.IsDisposed)
				throw new InvalidOperationException("The thread-bound DbInstances is not disposed, and ending the scope will cause a leak.");

			_threadBoundInstancesStack.Pop();
		}

		public void SaveAllChanges()
		{
			CurrentInstances.SaveAllChanges();
		}

		public void CancelAllChanges()
		{
			CurrentInstances.CancelAllChanges();
		}

		public void EndRequest()
		{
			while (_threadBoundInstancesStack != null && _threadBoundInstancesStack.Count > 0)
			{
                Log.Warn("A thread-bound DbInstances class remained active till the end of request, and is not disposed properly. " +
                         "This should not happen if all thread-bound Db usages are placed in using{} blocks.");

				EndThreadBoundScope();
			}

			Composer.GetComponent<DbRequestBoundInstances>().Dispose();
		}

		public bool IsAnyDbInstancesInScope
		{
			get { return (_threadBoundInstancesStack != null && _threadBoundInstancesStack.Count > 0 && !_threadBoundInstancesStack.Peek().IsDisposed) || DbRequestBoundInstances.CanBeUsed; }
		}

        #region Inner classes

        private class ThreadBoundDisposable : DbInstances
        {
            private readonly DbManager _dbManager;

            public ThreadBoundDisposable(DbManager dbManager)
            {
                _dbManager = dbManager;
            }

            public override void Dispose()
            {
                base.Dispose();
                _dbManager.EndThreadBoundScope();
            }
        }

        #endregion
	}
}