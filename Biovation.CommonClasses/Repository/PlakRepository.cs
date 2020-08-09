using Biovation.CommonClasses.Models;
using DataAccessLayer.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Biovation.CommonClasses.Repository
{
    class PlakRepository
    {/*
        private readonly GenericRepository _repository;

        /// <summary>
        /// برای اتصال به دیتابیس
        /// </summary>
        public PlakRepository()
        {
            _repository = new GenericRepository();
        }

        /*
        public List<PlakLogViewModel> GetLogPlak(DateTime sdate, DateTime edate)
        {
            //using (var command = _context.CreateCommand())
            //{
            var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Sdate", sdate),
                    new SqlParameter("@Edate", edate),
                };

            return _repository.ToResultList<PlakLogViewModel>("SelectPlakLogs", parameters: parameters, connectionName: "PlakConnection").Data;
            //}
        }*/
    }
}