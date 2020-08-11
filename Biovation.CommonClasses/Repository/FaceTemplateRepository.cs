using Biovation.CommonClasses.Models;
using DataAccessLayerCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayerCore.Repositories;

namespace Biovation.CommonClasses.Repository
{
    public class FaceTemplateRepository
    {
        private readonly GenericRepository _repository;

        public FaceTemplateRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="faceTemplate"></param>
        /// <returns></returns>
        public ResultViewModel ModifyFaceTemplate(FaceTemplate faceTemplate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", faceTemplate.Id),
                new SqlParameter("@UserId", faceTemplate.UserId),
                new SqlParameter("@Template", faceTemplate.Template),
                new SqlParameter("@CheckSum", faceTemplate.CheckSum),
                new SqlParameter("@SecurityLevel", faceTemplate.SecurityLevel),
                new SqlParameter("@Size", faceTemplate.Size),
                new SqlParameter("@Index", faceTemplate.Index),
                new SqlParameter("@EnrollQuality", faceTemplate.EnrollQuality),
                new SqlParameter("@FaceTemplateType", faceTemplate.FaceTemplateType.Code)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyFaceTemplate", parameters).Data.FirstOrDefault();
        }

        public List<FaceTemplate> GetAllFaceTemplates()
        {
            return _repository.ToResultList<FaceTemplate>("SelectFaceTemplates", fetchCompositions: true).Data;
        }

        public List<FaceTemplate> GetAllFaceTemplatesByFaceTemplateType(string fingerTemplateTypeCode)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@FaceTemplateType", fingerTemplateTypeCode)
            };

            return _repository.ToResultList<FaceTemplate>("SelectFaceTemplatesByFaceTemplateType", parameters, fetchCompositions: true).Data;
        }

        public List<FaceTemplate> GetFaceTemplateByUserId(long userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            return _repository.ToResultList<FaceTemplate>("SelectFaceTemplatesByUserId", parameters, fetchCompositions: true).Data;
        }

        public List<FaceTemplate> GetFaceTemplateByUserIdAndIndex(long userId, int index)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Index", index)
            };

            return _repository.ToResultList<FaceTemplate>("SelectFaceTemplatesByUserIdAndIndex", parameters, fetchCompositions: true).Data;
        }

        public ResultViewModel DeleteFaceTemplateByUserId(long userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteFaceTemplatesByUserId", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteFaceTemplateByUserIdAndIndex(long userId, int index)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Index", index)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteFaceTemplatesByUserIdAndIndex", parameters).Data.FirstOrDefault();
        }
    }
}
