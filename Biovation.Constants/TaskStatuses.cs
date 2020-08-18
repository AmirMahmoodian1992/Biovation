using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public static class TaskStatuses
    {
        public const string QueuedCode = "10001";
        public const string DoneCode = "10002";
        public const string FailedCode = "10003";
        public const string InProgressCode = "10004";
        public const string ScheduledCode = "10005";
        public const string DeviceDisconnectedCode = "10006";


        public static Lookup Queued = Lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, QueuedCode));
        public static Lookup Done = Lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, DoneCode));
        public static Lookup Failed = Lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, FailedCode));
        public static Lookup InProgress = Lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, InProgressCode));
        public static Lookup Scheduled = Lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, ScheduledCode));
        public static Lookup DeviceDisconnected = Lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceDisconnectedCode));


        public static Lookup GetTaskStatusByCode(string statusCode)
        {
            switch (statusCode)
            {
                case QueuedCode:
                    return Queued;

                case DoneCode:
                    return Done;

                case FailedCode:
                    return Failed;

                case InProgressCode:
                    return InProgress;

                case ScheduledCode:
                    return Scheduled;

                case DeviceDisconnectedCode:
                    return DeviceDisconnected;

                default:
                    return null;
            }
        }
    }
}
