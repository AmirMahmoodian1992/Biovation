using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace DataAccessLayerCore
{
    /// <summary>
    /// DBContext which is responsible for creating command on a connection.
    /// </summary>
    public class DbContext:IDisposable
    {
        private readonly IDbConnection _connection;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<AdoNetUnitOfWork> _uows = new LinkedList<AdoNetUnitOfWork>();

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="connectionFactory">Factory class for creating connection</param>
        public DbContext(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.Create();
        }


        /// <summary>
        /// Create a Transactional Unit of Work.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork CreateUnitOfWork()
        {
            var transaction = _connection.BeginTransaction();
            var uow = new AdoNetUnitOfWork(transaction, RemoveTransaction, RemoveTransaction);

            _rwLock.EnterWriteLock();
            _uows.AddLast(uow);
            _rwLock.ExitWriteLock();

            return uow;
        }

        /// <summary>
        /// Create a IDbCommand.
        /// </summary>
        /// <returns></returns>
        public IDbCommand CreateCommand()
        {
            var cmd = _connection.CreateCommand();
            _rwLock.EnterReadLock();
            if (_uows.Count > 0)
                cmd.Transaction = _uows.First.Value.Transaction;
            _rwLock.ExitReadLock();

            return cmd;
        }
        /// <summary>
        /// Remove a Transaction.
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveTransaction(AdoNetUnitOfWork obj)
        {
            _rwLock.EnterWriteLock();
            _uows.Remove(obj);
            _rwLock.ExitWriteLock();
        }

        /// <summary>
        /// Distroy a DbContext and it's connection.
        /// </summary>
        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
