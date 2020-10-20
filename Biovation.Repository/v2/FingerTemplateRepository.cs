using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2
{
    public class FingerTemplateRepository
    {
        private readonly GenericRepository _repository;

        public FingerTemplateRepository(GenericRepository repository)
        {
            _repository = repository;
        }

    
        public ResultViewModel AddFingerTemplate(FingerTemplate fingerTemplate)
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

        public ResultViewModel<PagingResult<UserTemplateCount>> GetFingerTemplatesCount(int nestingDepthLevel = 4)
        {
            return _repository.ToResultList<PagingResult<UserTemplateCount>>("SelectTemplatesCount", fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }

        public ResultViewModel<PagingResult<FingerTemplate>> FingerTemplates(int userId, int templateIndex, Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default,
        int pageSize = default, int nestingDepthLevel = 4)
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
                    new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},
                };

                return _repository.ToResultList<PagingResult<FingerTemplate>>("SelectFingerTemplateByFilter", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public ResultViewModel<int> GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType, int nestingDepthLevel = 4)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FingerTemplateType", fingerTemplateType.Code ==null ? "0":fingerTemplateType.Code)
                };

                return _repository.ToResultList<int>("SelectFingerTemplatesCountByFingerTemplateType", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        public ResultViewModel DeleteFingerTemplate(int userId, int fingerIndex)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@FingerIndex", fingerIndex)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteFingerTemplates", parameters).Data.FirstOrDefault();
        }
        public ResultViewModel<PagingResult<Lookup>> GetFingerTemplateTypes(string brandId, int pageNumber = default,
        int pageSize = default, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@brandId", brandId),
                new SqlParameter("@PageNumber", SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@PageSize", SqlDbType.Int) {Value = pageSize},
            };

            return _repository.ToResultList<PagingResult<Lookup>>("SelectFingerTemplateTypes", parameters, fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel).FetchFromResultList();
        }
    }
}
