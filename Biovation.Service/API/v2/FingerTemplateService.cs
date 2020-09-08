using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class FingerTemplateService
    {
        private readonly FingerTemplateRepository _fingerTemplateRepository;

        public FingerTemplateService(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }

        public ResultViewModel<PagingResult<UserTemplateCount>> GetTemplateCount()
        {
            return _fingerTemplateRepository.GetTemplateCount();
        }

        public ResultViewModel<PagingResult<FingerTemplate>> FingerTemplates(int userId = default, int templateIndex = default,
            Lookup fingerTemplateType = default, int from = default, int size = default, int pageNumber = default,
            int pageSize = default)
        {
            return _fingerTemplateRepository.FingerTemplates(userId, templateIndex, fingerTemplateType, from, size,
                pageNumber, pageSize);
        }

        public ResultViewModel<PagingResult<Lookup>> GetFingerTemplateTypes(string brandId = default)
        {
            return _fingerTemplateRepository.GetFingerTemplateTypes(brandId);

        }

        public ResultViewModel<int> GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            return _fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType);
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
