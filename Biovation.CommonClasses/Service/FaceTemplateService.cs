using System.Collections.Generic;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;

namespace Biovation.CommonClasses.Service
{
    public class FaceTemplateService
    {
        public ResultViewModel ModifyFaceTemplate(FaceTemplate fingerTemplate)
        {
            var fingerTemplateRepository = new FaceTemplateRepository();
            return fingerTemplateRepository.ModifyFaceTemplate(fingerTemplate);
        }

        public List<FaceTemplate> GetAllFaceTemplates()
        {
            var fingerTemplateRepository = new FaceTemplateRepository();
            return fingerTemplateRepository.GetAllFaceTemplates();
        }

        public List<FaceTemplate> GetAllFaceTemplatesByFaceTemplateType(string faceTemplateTypeCode)
        {
            var fingerTemplateRepository = new FaceTemplateRepository();
            return fingerTemplateRepository.GetAllFaceTemplatesByFaceTemplateType(faceTemplateTypeCode);
        }

        public List<FaceTemplate> GetFaceTemplateByUserId(long userId)
        {
            var fingerTemplateRepository = new FaceTemplateRepository();
            return fingerTemplateRepository.GetFaceTemplateByUserId(userId);
        }

        public List<FaceTemplate> GetFaceTemplateByUserIdAndIndex(long userId, int index)
        {
            var fingerTemplateRepository = new FaceTemplateRepository();
            return fingerTemplateRepository.GetFaceTemplateByUserIdAndIndex(userId, index);
        }

        public ResultViewModel DeleteFaceTemplateByUserId(long userId)
        {
            var fingerTemplateRepository = new FaceTemplateRepository();
            return fingerTemplateRepository.DeleteFaceTemplateByUserId(userId);
        }

        public ResultViewModel DeleteFaceTemplateByUserIdAndIndex(long userId, int templateIndex)
        {
            var fingerTemplateRepository = new FaceTemplateRepository();
            return fingerTemplateRepository.DeleteFaceTemplateByUserIdAndIndex(userId, templateIndex);
        }
    }
}
