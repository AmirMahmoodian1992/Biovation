using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DataAccessLayerCore.Repositories;


namespace Biovation.Data.Commands.Controllers.v2
{
    [Route("biovation/api/commands/v2/[controller]")]
    public class FingerTemplateController : Controller
    {
        private readonly FingerTemplateRepository _fingerTemplateRepository;

        public FingerTemplateController(FingerTemplateRepository fingerTemplateRepository)
        {
           _fingerTemplateRepository = fingerTemplateRepository;
        }

        [HttpPatch]
        [Route("ModifyFingerTemplate")]
        public Task<ResultViewModel> ModifyFingerTemplate(FingerTemplate fingerTemplate)
        {
            return Task.Run(() => _fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate));
        }
        [HttpDelete]
        [Route("{userId}")]
        public Task<ResultViewModel> DeleteFingerTemplate(int userId, int templateIndex)
        {
            return Task.Run(() => _fingerTemplateRepository.DeleteFingerTemplate(userId,templateIndex));
        }
    }
}
