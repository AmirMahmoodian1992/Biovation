using Biovation.Brands.Virdi.Model.Unis;
using Biovation.Brands.Virdi.Repository;
using Biovation.CommonClasses.Models;
using DataAccessLayerCore.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Brands.Virdi.Service
{
    public class VirdiUnisService
    {
        private readonly VirdiUnisRepository _unisRepository;

        public VirdiUnisService(string serverAddress, string databaseName, string username, string password)
        {
            var connectionInfo = new DatabaseConnectionInfo
            {
                DataSource = serverAddress,
                InitialCatalog = databaseName,
                UserId = username,
                Password = password,
                ProviderName = "System.Data.SqlClient"
            };

            _unisRepository = new VirdiUnisRepository(connectionInfo);
        }

        public Task<List<UnisUser>> GetAllUser()
        {
            return Task.Run(() => _unisRepository.GetAllUser());
        }

        public Task<List<UnisFingerTemplate>> GetAllFingerTemplatesFromUnis()
        {
            return Task.Run(() => _unisRepository.GetAllFingerTemplates());
        }

        public Task<List<UnisFaceTemplate>> GetAllFaceTemplates()
        {
            return Task.Run(() => _unisRepository.GetAllFaceTemplates());
        }

        public Task<List<UserCard>> GetAllUserCards()
        {
            return Task.Run(() => _unisRepository.GetAllUserCards());
        }
    }
}
