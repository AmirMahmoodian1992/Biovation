using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    public class FingerTemplateController : Controller
    {
        private readonly FingerTemplateRepository _fingerTemplateRepository;

        public FingerTemplateController(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }

        [HttpPatch]
        public Task<ResultViewModel> ModifyFingerTemplate([FromBody]FingerTemplate fingerTemplate)
        {
            return Task.Run(() => _fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate));
        }
        [HttpDelete]
        [Route("{userId}")]
        public Task<ResultViewModel> DeleteFingerTemplate(int userId, int fingerIndex)
        {
            return Task.Run(() => _fingerTemplateRepository.DeleteFingerTemplate(userId, fingerIndex));
        }
    }
}
