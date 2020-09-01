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

        public ResultViewModel<PagingResult<FingerTemplate>> FingerTemplates(int userId, int templateIndex,
            Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default,
            int PageSize = default)
        {
            return _fingerTemplateRepository.FingerTemplates(userId, templateIndex, fingerTemplateType, from, size,
                pageNumber, PageSize);
        }

        public ResultViewModel<PagingResult<Lookup>> GetFingerTemplateTypes(string brandId)
        {
            return _fingerTemplateRepository.GetFingerTemplateTypes(brandId);

        }
}
