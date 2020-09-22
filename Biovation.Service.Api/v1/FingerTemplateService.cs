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
            Lookup fingerTemplateType = default, int from = default, int size = default, int pageNumber = default,
            int pageSize = default)
        {
            return _fingerTemplateRepository.FingerTemplates(userId, templateIndex, fingerTemplateType, from, size,
                pageNumber, pageSize)?.Data?.Data ?? new List<FingerTemplate>();
        }

        public List<Lookup> GetFingerTemplateTypes(string brandId = default)
        {
            return _fingerTemplateRepository.GetFingerTemplateTypes(brandId)?.Data?.Data ?? new List<Lookup>();

        }

        public int GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            return _fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType)?.Data ?? default;
        }

        public ResultViewModel ModifyFingerTemplate(FingerTemplate fingerTemplate = default)
        {
            return _fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate);
        }

        public ResultViewModel DeleteFingerTemplate(int userId = default, int fingerIndex = default)
        {
            return _fingerTemplateRepository.DeleteFingerTemplate(userId, fingerIndex);
        }
    }
}
