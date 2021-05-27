using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class IrisTemplateController : ControllerBase
    {
        private readonly IrisTemplateRepository _irisTemplateRepository;

        public IrisTemplateController(IrisTemplateRepository irisTemplateRepository)
        {
            _irisTemplateRepository = irisTemplateRepository;
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyIrisTemplate([FromBody] IrisTemplate irisTemplate)
        {
            return Task.Run(() => _irisTemplateRepository.ModifyIrisTemplate(irisTemplate));
        }

        [HttpDelete]
        [Authorize]
        public Task<ResultViewModel> DeleteIrisTemplate(long userId = 0, int index = 0)
        {
            return Task.Run(() => _irisTemplateRepository.DeleteIrisTemplate(userId, index));
        }
    }
}
