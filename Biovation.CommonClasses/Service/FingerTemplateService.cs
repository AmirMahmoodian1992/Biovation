using System.Collections.Generic;
using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;

namespace Biovation.CommonClasses.Service
{
    public class FingerTemplateService
    {
        private readonly FingerTemplateRepository _fingerTemplateRepository;

        public FingerTemplateService(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }

        public ResultViewModel ModifyFingerTemplate(FingerTemplate fingerTemplate)
        {
            return _fingerTemplateRepository.ModifyFingerTemplate(fingerTemplate);
        }

        public List<UserTemplateCount> GetFingerTemplatesCount()
        {
            return _fingerTemplateRepository.GetFingerTemplatesCount();
        }

        public List<FingerTemplate> GetAllFingerTemplates()
        {
            return _fingerTemplateRepository.GetAllFingerTemplates();
        }

        public List<FingerTemplate> GetAllFingerTemplatesByFingerTemplateType(Lookup fingerTemplateType, int from = 0, int size = 0)
        {
            return _fingerTemplateRepository.GetAllFingerTemplatesByFingerTemplateType(fingerTemplateType, from, size);
        }
        public int GetAllFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            return _fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType);
        }

        public List<FingerTemplate> GetFingerTemplateByUserId(long userId)
        {
            return _fingerTemplateRepository.GetFingerTemplateByUserId(userId);
        }

        public List<FingerTemplate> GetFingerTemplateByUserIdAndTemplateIndex(int userId, int templateIndex)
        {
            return _fingerTemplateRepository.GetFingerTemplateByUserIdAndTemplateIndex(userId, templateIndex);
        }

        public ResultViewModel DeleteFingerTemplateByUserId(int userId)
        {
            return _fingerTemplateRepository.DeleteFingerTemplateByUserId(userId);
        }

        public ResultViewModel DeleteFingerTemplateByUserIdAndTemplateIndex(int userId, int templateIndex)
        {
            return _fingerTemplateRepository.DeleteFingerTemplateByUserIdAndTemplateIndex(userId, templateIndex);
        }

        public List<Lookup> GetFingerTemplateTypes(string brandId)
        {
            return _fingerTemplateRepository.GetFingerTemplateTypes(brandId);
        }
    }
}
