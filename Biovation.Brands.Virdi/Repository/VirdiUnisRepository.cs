using Biovation.Brands.Virdi.Model.Unis;
using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Domain;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using CommandType = System.Data.CommandType;

namespace Biovation.Brands.Virdi.Repository
{
    public class VirdiUnisRepository
    {
        private readonly GenericRepository _repository;

        public VirdiUnisRepository(DatabaseConnectionInfo connectionInfo)
        {
            _repository = new GenericRepository(connectionInfo);
        }

        public List<UnisUser> GetAllUser()
        {
            const string query = @"SELECT [L_ID],[C_Name]
                                            FROM [dbo].[tUser]";

            return _repository.ToResultList<UnisUser>(query, commandType: CommandType.Text).Data;
        }

        public List<UnisFingerTemplate> GetAllFingerTemplates()
        {
            const string query = @"SELECT   [L_UID] AS UserId
                                           ,[L_IsWideChar] AS IsWideChar
                                           ,[B_TextFIR] AS Template
                                        FROM [dbo].[iUserFinger]";
            return _repository.ToResultList<UnisFingerTemplate>(query, commandType: CommandType.Text).Data;
        }

        public List<UnisFaceTemplate> GetAllFaceTemplates()
        {
            const string query = @"SELECT  [L_UID] AS UserId
                                          ,[B_Face] AS Template
                                      FROM [dbo].[iUserFace]";
            return _repository.ToResultList<UnisFaceTemplate>(query, commandType: CommandType.Text).Data;
        }

        public List<UserCard> GetAllUserCards()
        {
            const string query = @"SELECT  [C_CardNum] AS CardNum
                                          ,[L_UID] AS UserId
                                          ,[L_DataCheck] AS DataCheck 
                                          ,1 AS IsActive
                                      FROM [dbo].[iUserCard]";
            return _repository.ToResultList<UserCard>(query, commandType: CommandType.Text).Data;
        }
    }
}
