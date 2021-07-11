using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Authorize]
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class IrisTemplateController : ControllerBase
    {
        private readonly IrisTemplateRepository _irisTemplateRepository;

        public IrisTemplateController(IrisTemplateRepository irisTemplateRepository)
        {
            _irisTemplateRepository = irisTemplateRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<IrisTemplate>>> IrisTemplates(string irisTemplateTypeCode = default, long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _irisTemplateRepository.GetIrisTemplates(irisTemplateTypeCode, userId, index, pageNumber, pageSize));
        }
    }
}