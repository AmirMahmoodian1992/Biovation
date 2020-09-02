using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.SQL.v2
{
    public class GenericCodeMappingRepository
    {
        private readonly GenericRepository _repository;

        public GenericCodeMappingRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public ResultViewModel<PagingResult<GenericCodeMapping>> GetGenericCodeMappings(int categoryId = default, string brandCode = default, int manufactureCode = default, int genericCode = default, int pageNumber = default, int pageSize = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@categoryId", categoryId),
                new SqlParameter("@brandCode", brandCode),
                new SqlParameter("@manufactureCode", manufactureCode),
                new SqlParameter("@genericCode", genericCode),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},

            };

            return _repository.ToResultList<PagingResult<GenericCodeMapping>>("SelectGenericCodeMappings", parameters, fetchCompositions: true).FetchFromResultList();
        }
    }
}
