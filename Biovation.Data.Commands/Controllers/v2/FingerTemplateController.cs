﻿using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;


namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class FingerTemplateController : ControllerBase
    {
        private readonly FingerTemplateRepository _fingerTemplateRepository;

        public FingerTemplateController(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }


        [HttpPost]
        [Authorize]
        public Task<ResultViewModel> AddFingerTemplate([FromBody]FingerTemplate fingerTemplate)
        {
            return Task.Run(() => _fingerTemplateRepository.AddFingerTemplate(fingerTemplate));
        }

        [HttpPatch]
        [Authorize]
        public Task<ResultViewModel> ModifyFingerTemplate([FromBody]FingerTemplate fingerTemplate)
        {
            return Task.Run(() => _fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate));
        }

        [HttpDelete]
        [Route("{userId}")]
        [Authorize]
        public Task<ResultViewModel> DeleteFingerTemplate([FromRoute] int userId, int fingerIndex)
        {
            return Task.Run(() => _fingerTemplateRepository.DeleteFingerTemplate(userId, fingerIndex));
        }
    }
}
