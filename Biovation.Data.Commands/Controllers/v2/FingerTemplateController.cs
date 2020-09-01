using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;

namespace Biovation.Data.Commands.Controllers.v2
{
   /* public class FingerTemplateRepository
    {
        private readonly GenericRepository _repository;

        public FingerTemplateRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="fingerTemplate"></param>
        /// <returns></returns>
        public ResultViewModel ModifyFingerTemplate(FingerTemplate fingerTemplate)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", fingerTemplate.Id),
                new SqlParameter("@UserId", fingerTemplate.UserId),
                new SqlParameter("@Template", fingerTemplate.Template),
                new SqlParameter("@FingerIndexCode", fingerTemplate.FingerIndex.Code),
                new SqlParameter("@TemplateIndex", fingerTemplate.TemplateIndex),
                new SqlParameter("@SecurityLevel", fingerTemplate.SecurityLevel),
                new SqlParameter("@Size", fingerTemplate.Size),
                new SqlParameter("@Index", fingerTemplate.Index),
                new SqlParameter("@EnrollQuality", fingerTemplate.EnrollQuality),
                new SqlParameter("@Duress", fingerTemplate.Duress),
                new SqlParameter("@CheckSum", fingerTemplate.CheckSum),
                new SqlParameter("@FingerTemplateType", fingerTemplate.FingerTemplateType.Code),
                new SqlParameter("@CreateBy", fingerTemplate.CreateBy),
                new SqlParameter("@UpdateBy", fingerTemplate.UpdateBy)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyFingerTemplate", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteFingerTemplateByUserId(int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteFingerTemplatesByUserId", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteFingerTemplateByUserIdAndTemplateIndex(int userId, int templateIndex)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@TemplateIndex", templateIndex)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteFingerTemplatesByUserIdAndTemplateIndex", parameters).Data.FirstOrDefault();
        }

     
    }*/
}
