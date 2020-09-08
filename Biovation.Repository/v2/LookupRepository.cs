using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.SQL.v2
{
    public class LookupRepository
    {
        private readonly GenericRepository _repository;

        public LookupRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public ResultViewModel<PagingResult<Lookup>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default, int pageNumber = default,
            int pageSize = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@code", code),
                new SqlParameter("@name", name),
                new SqlParameter("@lookupCategoryId", lookupCategoryId),
                new SqlParameter("@codePrefix", codePrefix),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},

            };

            return _repository.ToResultList<PagingResult<Lookup>>("SelectLookups", parameters).FetchFromResultList();
        }
    }
}
