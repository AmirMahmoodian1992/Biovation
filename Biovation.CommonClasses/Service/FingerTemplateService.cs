using System.Collections.Generic;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;

namespace Biovation.CommonClasses.Service
{
    public class FingerTemplateService
    {
        public ResultViewModel ModifyFingerTemplate(FingerTemplate fingerTemplate)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate);
        }

        public List<UserTemplateCount> GetFingerTemplatesCount()
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.GetFingerTemplatesCount();
        }

        public List<FingerTemplate> GetAllFingerTemplates()
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.GetAllFingerTemplates();
        }

        public List<FingerTemplate> GetAllFingerTemplatesByFingerTemplateType(Lookup fingerTemplateType, int from = 0, int size = 0)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.GetAllFingerTemplatesByFingerTemplateType(fingerTemplateType, from, size);
        }
        public int GetAllFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType);
        }

        public List<FingerTemplate> GetFingerTemplateByUserId(long userId)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.GetFingerTemplateByUserId(userId);
        }

        public List<FingerTemplate> GetFingerTemplateByUserIdAndTemplateIndex(int userId, int templateIndex)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.GetFingerTemplateByUserIdAndTemplateIndex(userId, templateIndex);
        }

        public ResultViewModel DeleteFingerTemplateByUserId(int userId)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.DeleteFingerTemplateByUserId(userId);
        }

        public ResultViewModel DeleteFingerTemplateByUserIdAndTemplateIndex(int userId, int templateIndex)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.DeleteFingerTemplateByUserIdAndTemplateIndex(userId, templateIndex);
        }

        public List<Lookup> GetFingerTemplateTypes(string brandId)
        {
            var fingerTemplateRepository = new FingerTemplateRepository();
            return fingerTemplateRepository.GetFingerTemplateTypes(brandId);
        }
    }
}
