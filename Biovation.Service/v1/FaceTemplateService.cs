using System.Collections.Generic;
using Biovation.Domain;
using Biovation.Repository.SQL.v1;

namespace Biovation.Service.Sql.v1
{
    public class FaceTemplateService
    {
        private readonly FaceTemplateRepository _fingerTemplateRepository;

        public FaceTemplateService(FaceTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }

        public ResultViewModel ModifyFaceTemplate(FaceTemplate fingerTemplate)
        {
            return _fingerTemplateRepository.ModifyFaceTemplate(fingerTemplate);
        }

        public List<FaceTemplate> GetAllFaceTemplates()
        {
            return _fingerTemplateRepository.GetAllFaceTemplates();
        }

        public List<FaceTemplate> GetAllFaceTemplatesByFaceTemplateType(string faceTemplateTypeCode)
        {
            return _fingerTemplateRepository.GetAllFaceTemplatesByFaceTemplateType(faceTemplateTypeCode);
        }

        public List<FaceTemplate> GetFaceTemplateByUserId(long userId)
        {
            return _fingerTemplateRepository.GetFaceTemplateByUserId(userId);
        }

        public List<FaceTemplate> GetFaceTemplateByUserIdAndIndex(long userId, int index)
        {
            return _fingerTemplateRepository.GetFaceTemplateByUserIdAndIndex(userId, index);
        }

        public ResultViewModel DeleteFaceTemplateByUserId(long userId)
        {
            return _fingerTemplateRepository.DeleteFaceTemplateByUserId(userId);
        }

        public ResultViewModel DeleteFaceTemplateByUserIdAndIndex(long userId, int templateIndex)
        {
            return _fingerTemplateRepository.DeleteFaceTemplateByUserIdAndIndex(userId, templateIndex);
        }
    }
}
