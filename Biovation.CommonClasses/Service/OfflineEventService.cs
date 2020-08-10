using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository;
using System.Collections.Generic;

namespace Biovation.CommonClasses.Service
{
    public class OfflineEventService
    {
        public List<OfflineEvent> GetOfflineEvents(int deviceId = 0)
        {
            var userRepository = new OfflineEventsRepository();
            return userRepository.GetOfflineEvents(deviceId);
        }

        public int AddOfflineEvent(OfflineEvent offlineEvent)
        {
            var userRepository = new OfflineEventsRepository();
            return userRepository.AddOfflineEvent(offlineEvent);
        }

        public bool DeleteOfflineEvent(int offlineEventId)
        {
            var userRepository = new OfflineEventsRepository();
            return userRepository.DeleteOfflineEvent(offlineEventId);
        }

        public bool DeleteOfflineEvent(OfflineEvent offlineEvent)
        {
            var userRepository = new OfflineEventsRepository();
            return userRepository.DeleteOfflineEvent(offlineEvent);
        }
    }
}
