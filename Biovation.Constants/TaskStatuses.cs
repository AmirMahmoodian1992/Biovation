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
        public const string IncompleteCode = "10007";
        public const string RecurringCode = "10008";

        public TaskStatuses(Lookups lookups)
        {
            Queued = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, QueuedCode));
            Done = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, DoneCode));
            Failed = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, FailedCode));
            InProgress = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, InProgressCode));
            Scheduled = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, ScheduledCode));
            DeviceDisconnected = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceDisconnectedCode));
            Incomplete = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, IncompleteCode));
            Recurring = lookups.TaskStatuses.FirstOrDefault(lookup => string.Equals(lookup.Code, RecurringCode));
        }

        public Lookup Queued;
        public Lookup Done;
        public Lookup Failed;
        public Lookup InProgress;
        public Lookup Scheduled;
        public Lookup DeviceDisconnected;
        public Lookup Incomplete;
        public Lookup Recurring;


        public Lookup GetTaskStatusByCode(string statusCode)
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

                case IncompleteCode:
                    return Incomplete;

                case RecurringCode:
                    return Recurring;

                default:
                    return null;
            }
        }
    }
}
