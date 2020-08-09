using Biovation.CommonClasses.Models;
using DataAccessLayer.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Biovation.CommonClasses.Repository
{
    public class LookupRepository
    {
        private readonly GenericRepository _repository = new GenericRepository();

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
