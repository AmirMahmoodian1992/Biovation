﻿using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v1
{
    public class LookupService
    {
        private readonly LookupRepository _lookupRepository;

        public LookupService(LookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }

        public async Task<List<Lookup>> GetLookups(string code = default, string name = default,
            int lookupCategoryId = default, string codePrefix = default, int pageNumber = default,
            int pageSize = default)
        {
            return (await _lookupRepository.GetLookups(code, name, lookupCategoryId, codePrefix, pageNumber, pageSize))?.Data?.Data ?? new List<Lookup>();
        }
    }
}
