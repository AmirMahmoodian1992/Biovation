using Biovation.Domain;
using Biovation.Repository.API.v2;

namespace Biovation.Service.API.v2
{
    public class AccessGroupService
    {
        private readonly AccessGroupRepository _accessGroupRepository;

        public AccessGroupService(AccessGroupRepository accessGroupRepository)
        {
            _accessGroupRepository = accessGroupRepository;
        }
        


        
    }
}
