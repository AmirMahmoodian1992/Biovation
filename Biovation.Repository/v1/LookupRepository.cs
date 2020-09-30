using System.Collections.Generic;
using System.Data.SqlClient;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v1
{
    public class LookupRepository
    {
        private readonly GenericRepository _repository;

        public LookupRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public List<Lookup> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@code", code),
                new SqlParameter("@name", name),
                new SqlParameter("@lookupCategoryId", lookupCategoryId),
                new SqlParameter("@codePrefix", codePrefix)
            };

            return _repository.ToResultList<Lookup>("SelectLookups", parameters).Data;
        }
    }
}
