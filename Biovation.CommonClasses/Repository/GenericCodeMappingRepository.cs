using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Biovation.CommonClasses.Repository
{
    public class GenericCodeMappingRepository
    {
        private readonly GenericRepository _repository = new GenericRepository();

        public List<GenericCodeMapping> GetGenericCodeMappings(int categoryId = default, string brandCode = default, int manufactureCode = default, int genericCode = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@categoryId", categoryId),
                new SqlParameter("@brandCode", brandCode),
                new SqlParameter("@manufactureCode", manufactureCode),
                new SqlParameter("@genericCode", genericCode)
            };

            return _repository.ToResultList<GenericCodeMapping>("SelectGenericCodeMappings", parameters, fetchCompositions: true).Data;
        }
    }
}
