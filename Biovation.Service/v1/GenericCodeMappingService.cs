using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository;

namespace Biovation.Service.Sql.v1
{
    public class GenericCodeMappingService
    {
        private readonly GenericCodeMappingRepository _genericCodeMappingRepository;

        public GenericCodeMappingService(GenericCodeMappingRepository genericCodeMappingRepository)
        {
            _genericCodeMappingRepository = genericCodeMappingRepository;
        }

        public Task<List<GenericCodeMapping>> GetGenericCodeMappings(int categoryId = default, string brandCode = default, int manufactureCode = default, int genericCode = default)
        {
            return Task.Run(() => _genericCodeMappingRepository.GetGenericCodeMappings(categoryId, brandCode, manufactureCode, genericCode));
        }
    }
}
