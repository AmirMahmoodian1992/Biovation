using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

namespace Biovation.Service.Api.v2
{
    public class IrisTemplateService
    {
        private readonly IrisTemplateRepository _irisTemplateRepository;

        public IrisTemplateService(IrisTemplateRepository irisTemplateRepository)
        {
            _irisTemplateRepository = irisTemplateRepository;
        }

        public List<IrisTemplate> IrisTemplates(string fingerTemplateTypeCode = default,
            long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return _irisTemplateRepository.IrisTemplates(fingerTemplateTypeCode, userId, index, pageNumber, pageSize)?.Data?.Data ?? new List<IrisTemplate>();
        }

        public ResultViewModel ModifyIrisTemplate(IrisTemplate irisTemplate, string token = default)
        {
            return _irisTemplateRepository.ModifyIrisTemplate(irisTemplate, token);
        }

        public ResultViewModel DeleteIrisTemplate(long userId = 0, int index = 0, string token = default)
        {
            return _irisTemplateRepository.DeleteIrisTemplate(userId, index, token);
        }
    }
}