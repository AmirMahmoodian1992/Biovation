using System.Collections.Generic;
using System.Data.SqlClient;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Components;

namespace Biovation.Data.Queries.Controllers.v2
{

    [Route("biovation/api/queries/v2/[controller]")]
    public class GenericCodeMappingRepository
    {
        private readonly GenericRepository _repository;

        public GenericCodeMappingRepository(GenericRepository repository)
        {
            _repository = repository;
        }

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
