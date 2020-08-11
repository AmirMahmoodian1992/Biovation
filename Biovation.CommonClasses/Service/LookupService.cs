using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.CommonClasses.Service
{
    public class LookupService
    {
        private readonly LookupRepository _lookupRepository;

        public LookupService(LookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public Task<List<Lookup>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default)
        {
            return Task.Run(() => _lookupRepository.GetLookups(code, name, lookupCategoryId, codePrefix));
        }
    }
}
