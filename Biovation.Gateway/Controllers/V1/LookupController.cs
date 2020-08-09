using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Service;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Biovation.WebService.APIControllers
{
    public class LookupController : ApiController
    {
        private readonly LookupService _lookupService = new LookupService();

        [HttpGet]
        public Task<List<Lookup>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default)
        {
            return Task.Run(() => _lookupService.GetLookups(code, name, lookupCategoryId, codePrefix));
        }
    }
}
