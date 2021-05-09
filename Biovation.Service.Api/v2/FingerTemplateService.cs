using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class FingerTemplateService
    {
        private readonly FingerTemplateRepository _fingerTemplateRepository;

        public FingerTemplateService(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }

        public ResultViewModel<PagingResult<UserTemplateCount>> GetTemplateCount(string token = default)
        {
            return _fingerTemplateRepository.GetTemplateCount(token);
        }

        public async Task<ResultViewModel<PagingResult<FingerTemplate>>> FingerTemplates(int userId = default, int templateIndex = default,
            string fingerTemplateType = default, int pageNumber = default, int pageSize = default, string token = default)
        {
            return await _fingerTemplateRepository.FingerTemplates(userId, templateIndex, fingerTemplateType, pageNumber, pageSize, token);
        }

        public ResultViewModel<PagingResult<Lookup>> GetFingerTemplateTypes(string brandId = default, string token = default)
        {
            return _fingerTemplateRepository.GetFingerTemplateTypes(brandId, token);

        }

        public ResultViewModel<int> GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType, string token = default)
        {
            return _fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType, token);
        }

        public async Task<ResultViewModel> ModifyFingerTemplate(FingerTemplate fingerTemplate = default, string token = default)
        {
            return await _fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate, token);
        }

        public ResultViewModel DeleteFingerTemplate(int userId = default, int fingerIndex = default, string token = default)
        {
            return _fingerTemplateRepository.DeleteFingerTemplate(userId, fingerIndex, token);
        }
    }
}
