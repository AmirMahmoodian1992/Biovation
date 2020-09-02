using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.SQL.v2
{
    public class FingerTemplateRepository
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

        public ResultViewModel<PagingResult<UserTemplateCount>> GetFingerTemplatesCount()
        {
            return _repository.ToResultList<PagingResult<UserTemplateCount>>("SelectTemplatesCount").FetchFromResultList();
        }

        public ResultViewModel<PagingResult<FingerTemplate>> FingerTemplates(int userId, int templateIndex, Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default,
        int PageSize = default)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@TemplateIndex", templateIndex),
                    new SqlParameter("@FingerTemplateType", fingerTemplateType.Code),
                    new SqlParameter("@from", from),
                    new SqlParameter("@size", size),
                    new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                    new SqlParameter("@PageSize", SqlDbType.Int) {Value = PageSize},
                };

                return _repository.ToResultList<PagingResult<FingerTemplate>>("SelectFingerTemplateByFilter", fetchCompositions: true).FetchFromResultList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public int GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FingerTemplateType", fingerTemplateType.Code)
                };

                return _repository.ToResultList<int>("SelectFingerTemplatesCountByFingerTemplateType", parameters).Data.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        public ResultViewModel DeleteFingerTemplate(int userId, int templateIndex)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@TemplateIndex", templateIndex)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteFingerTemplates", parameters).Data.FirstOrDefault();
        }
        public ResultViewModel<PagingResult<Lookup>> GetFingerTemplateTypes(string brandId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@brandId", brandId)
            };

            return _repository.ToResultList<PagingResult<Lookup>>("SelectFingerTemplateTypes", parameters, fetchCompositions: true).FetchFromResultList();
        }
    }
}
