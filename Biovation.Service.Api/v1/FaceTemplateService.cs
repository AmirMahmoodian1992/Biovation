using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Collections.Generic;

namespace Biovation.Service.Api.v1
{
    public class FaceTemplateService
    {
        private readonly FaceTemplateRepository _faceTemplateRepository;

        public FaceTemplateService(FaceTemplateRepository faceTemplateRepository)
        {
            _faceTemplateRepository = faceTemplateRepository;
        }

        public List<FaceTemplate> FaceTemplates(string fingerTemplateTypeCode = default,
            long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default, string token = default)
        {
            return _faceTemplateRepository.FaceTemplates(fingerTemplateTypeCode, userId, index, pageNumber, pageSize)?.Data?.Data ?? new List<FaceTemplate>();
        }

        public ResultViewModel ModifyFaceTemplate(FaceTemplate faceTemplate, string token = default)
        {
            return _faceTemplateRepository.ModifyFaceTemplate(faceTemplate, token);
        }

        public ResultViewModel DeleteFaceTemplate(long userId = 0, int index = 0, string token = default)
        {
            return _faceTemplateRepository.DeleteFaceTemplate(userId, index, token);
        }
    }
}