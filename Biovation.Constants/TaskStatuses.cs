using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class TaskStatuses
    {
        public const string QueuedCode = "10001";
        public const string DoneCode = "10002";
        public const string FailedCode = "10003";
        public const string InProgressCode = "10004";
        public const string ScheduledCode = "10005";
        public const string DeviceDisconnectedCode = "10006";

        public TaskStatuses(Lookups lookups)
        {
            Queued = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, QueuedCode));
            Done = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, DoneCode));
            Failed = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, FailedCode));
            InProgress = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, InProgressCode));
            Scheduled = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, ScheduledCode));
            DeviceDisconnected = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceDisconnectedCode));
        }

        public static Lookup Queued;
        public static Lookup Done;
        public static Lookup Failed;
        public static Lookup InProgress;
        public static Lookup Scheduled;
        public static Lookup DeviceDisconnected;


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
