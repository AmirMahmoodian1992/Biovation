using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Biovation.Repository.Sql.v2
{
    public class IrisTemplateRepository
    {
        private readonly GenericRepository _repository;

        public IrisTemplateRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="irisTemplate"></param>
        /// <returns></returns>
        public ResultViewModel ModifyIrisTemplate(IrisTemplate irisTemplate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", irisTemplate.Id),
                new SqlParameter("@UserId", irisTemplate.UserId),
                new SqlParameter("@Template", irisTemplate.Template),
                new SqlParameter("@CheckSum", irisTemplate.CheckSum),
                new SqlParameter("@SecurityLevel", irisTemplate.SecurityLevel),
                new SqlParameter("@Size", irisTemplate.Size),
                new SqlParameter("@Index", irisTemplate.Index),
                new SqlParameter("@EnrollQuality", irisTemplate.EnrollQuality),
                new SqlParameter("@IrisTemplateType", irisTemplate.IrisTemplateType.Code)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyIrisTemplate", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel<PagingResult<IrisTemplate>> GetIrisTemplates(string irisTemplateType, long userId,
            int index, int pageNumber, int pageSize, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Index", index),
                new SqlParameter("@IrisTemplateType", irisTemplateType),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},

            };
            return _repository.ToResultList<PagingResult<IrisTemplate>>("SelectIrisTemplatesByFilter", parameters,
                    fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel)
                .FetchFromResultList();
        }




        public ResultViewModel DeleteIrisTemplate(long userId, int index)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Index", index)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteIrisTemplates", parameters).Data.FirstOrDefault();
        }
    }
}
