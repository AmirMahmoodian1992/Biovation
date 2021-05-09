using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

namespace Biovation.Service.Api.v1
{
    public class FingerTemplateService
    {
        private readonly FingerTemplateRepository _fingerTemplateRepository;

        public FingerTemplateService(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }

        public List<UserTemplateCount> GetTemplateCount()
        {
            return _fingerTemplateRepository.GetTemplateCount()?.Data?.Data ?? new List<UserTemplateCount>();
        }

        public List<FingerTemplate> FingerTemplates(int userId = default, int templateIndex = default,
            string fingerTemplateType = default, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return _fingerTemplateRepository.FingerTemplates(userId, templateIndex, fingerTemplateType,
                pageNumber, pageSize, token).Result?.Data?.Data ?? new List<FingerTemplate>();
        }

        public List<Lookup> GetFingerTemplateTypes(string brandId = default, string token = default)
        {
            return _fingerTemplateRepository.GetFingerTemplateTypes(brandId, token)?.Data?.Data ?? new List<Lookup>();

        }

        public int GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType, string token = default)
        {
            return _fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType, token)?.Data ?? default;
        }

        public ResultViewModel ModifyFingerTemplate(FingerTemplate fingerTemplate = default, string token = default)
        {
            return _fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate, token).Result;
        }

        public ResultViewModel DeleteFingerTemplate(int userId = default, int fingerIndex = default, string token = default)
        {
            return _fingerTemplateRepository.DeleteFingerTemplate(userId, fingerIndex, token).Result;
        }
    }
}
