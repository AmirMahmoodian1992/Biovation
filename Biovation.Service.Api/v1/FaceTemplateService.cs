using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.Api.v1
{
    public class FaceTemplateService
    {
        private readonly FaceTemplateRepository _faceTemplateRepository;

        public FaceTemplateService(FaceTemplateRepository faceTemplateRepository)
        {
            _faceTemplateRepository = faceTemplateRepository;
        }

        public ResultViewModel<PagingResult<FaceTemplate>> FaceTemplates(string fingerTemplateTypeCode = default,
            long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default)
        {
            return _faceTemplateRepository.FaceTemplates(fingerTemplateTypeCode, userId, index, pageNumber, pageSize);
        }

        public ResultViewModel ModifyFaceTemplate(FaceTemplate faceTemplate)
        {
            return _faceTemplateRepository.ModifyFaceTemplate(faceTemplate);
        }

        public ResultViewModel DeleteFaceTemplate(long userId = 0, int index = 0)
        {
            return _faceTemplateRepository.DeleteFaceTemplate(userId, index);
        }
    }
}
