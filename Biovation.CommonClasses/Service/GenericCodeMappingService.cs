using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Service
{
    public class GenericCodeMappingService
    {
        private readonly GenericCodeMappingRepository _genericCodeMappingRepository = new GenericCodeMappingRepository();

        public Task<List<GenericCodeMapping>> GetGenericCodeMappings(int categoryId = default, string brandCode = default, int manufactureCode = default, int genericCode = default)
        {
            return Task.Run(() => _genericCodeMappingRepository.GetGenericCodeMappings(categoryId, brandCode, manufactureCode, genericCode));
        }
    }
}
