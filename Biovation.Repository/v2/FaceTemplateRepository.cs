using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2
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

        public ResultViewModel<PagingResult<FaceTemplate>> GetFaceTemplates(string faceTemplateType, long userId,
            int index, int pageNumber, int pageSize, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Index", index),
                new SqlParameter("@FaceTemplateType", faceTemplateType),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},

            };
            return _repository.ToResultList<PagingResult<FaceTemplate>>("SelectFaceTemplatesByFilter", parameters,
                    fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel)
                .FetchFromResultList();
        }




        public ResultViewModel DeleteFaceTemplate(long userId, int index)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Index", index)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteFaceTemplates", parameters).Data.FirstOrDefault();
        }
    }
}
