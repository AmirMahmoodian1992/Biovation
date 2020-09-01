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

        public ResultViewModel<PagingResult<AccessGroup>> GetAccessGroups(int userId = default, int adminUserId = default,
            int userGroupId = default, int id = default, int deviceId = default, int deviceGroupId = default, int pageNumber = default, int pageSize = default)
        {
            return _accessGroupRepository.GetAccessGroups(userId, adminUserId, userGroupId, id, deviceId, deviceGroupId,
                pageNumber, pageSize);
        }

        public ResultViewModel<AccessGroup> GetAccessGroup(int id = default, int nestingDepthLevel = default)
        {
            return _accessGroupRepository.GetAccessGroup(id, nestingDepthLevel);
        }




    }
}
