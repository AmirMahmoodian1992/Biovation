using System;
using System.Data;

namespace DataAccessLayerCore
{
   /// <summary>
   /// این کلاس کانکشن و فعالیت های روی ای دی او را مدیریت می کند
   /// </summary>
    public class AdoNetUnitOfWork : IUnitOfWork
    {
        private IDbTransaction _transaction;
        private readonly Action<AdoNetUnitOfWork> _rolledBack;
        private readonly Action<AdoNetUnitOfWork> _committed;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="rolledBack"></param>
        /// <param name="committed"></param>
        public AdoNetUnitOfWork(IDbTransaction transaction, Action<AdoNetUnitOfWork> rolledBack, Action<AdoNetUnitOfWork> committed)
        {
            Transaction = transaction;
            _transaction = transaction;
            _rolledBack = rolledBack;
            _committed = committed;
        }

        public IDbTransaction Transaction { get; private set; }

        public void Dispose()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback();
            _transaction.Dispose();
            _rolledBack(this);
            _transaction = null;
        }

        public void SaveChanges()
        {
            if (_transaction == null)
                throw new InvalidOperationException("May not call save changes twice.");

            _transaction.Commit();
            _committed(this);
            _transaction = null;
        }
    }
}
