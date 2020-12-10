using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class TaskTypes
    {
        public const string SendUsersCode = "11001";
        public const string SendDevicesCode = "11101";
        public const string SendFoodsCode = "11501";
        public const string SendMealsCode = "11502";
        public const string SendRestaurantsCode = "11503";
        public const string SendReservationsCode = "11504";
        public const string GetServeLogsCode = "11505";
        public const string SendMealTimingsCode = "11506";

        public const string UpgradeDeviceFirmwareCode = "11102";
        public const string UnlockDeviceCode = "11103";
        public const string SendTimeZoneToTerminalCode = "11104";
        public const string SendAccessGroupToTerminalCode = "11105";
        public const string RetrieveUserFromTerminalCode = "11106";
        public const string RetrieveAllUsersFromTerminalCode = "11107";
        public const string OpenDoorCode = "11108";
        public const string LockDeviceCode = "11109";
        public const string EnrollFromTerminalCode = "11110";
        public const string DeleteUserFromTerminalCode = "11111";
        public const string EnrollFaceFromTerminalCode = "11112";

        public const string SendBlackListCode = "11601";


        public const string ClearLogCode = "11401";
        public const string GetLogsInPeriodCode = "11402";
        public const string GetLogsCode = "11403";

        public const string DeleteUsersCode = "11002";
        public const string DeleteDevicesCode = "11102";
        public const string DeleteFoodsCode = "11507";
        public const string DeleteMealsCode = "11508";
        public const string DeleteRestaurantsCode = "11509";
        public const string DeleteReservationsCode = "11510";
        public const string DeleteMealTimingsCode = "11511";

        //Tools
        public const string UserAdaptationCode = "11701";

        public TaskTypes(Lookups lookups)
        {
            SendUsers = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendUsersCode));
            SendDevices = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendDevicesCode));
            SendFoods = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendFoodsCode));
            SendMeals = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealsCode));
            SendMealTimings = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealTimingsCode));
            SendRestaurants = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendRestaurantsCode));
            SendReservations = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendReservationsCode));
            GetServeLogs = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetServeLogsCode));
            DeleteUsers = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUsersCode));
            DeleteDevices = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteDevicesCode));
            DeleteFoods = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteFoodsCode));
            DeleteMeals = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealsCode));
            DeleteMealTimings = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealTimingsCode));
            DeleteRestaurants = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteRestaurantsCode));
            DeleteReservations = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteReservationsCode));

            UpgradeDeviceFirmware = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UpgradeDeviceFirmwareCode));
            UnlockDevice = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnlockDeviceCode));
            SendTimeZoneToTerminal = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendTimeZoneToTerminalCode));
            SendAccessGroupToTerminal = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendAccessGroupToTerminalCode));
            RetrieveUserFromTerminal = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveUserFromTerminalCode));

            RetrieveAllUsersFromTerminal = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveAllUsersFromTerminalCode));
            OpenDoor = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, OpenDoorCode));
            LockDevice = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, LockDeviceCode));
            EnrollFromTerminal = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFromTerminalCode));
            DeleteUserFromTerminal = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUserFromTerminalCode));
            EnrollFaceFromTerminal = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFaceFromTerminalCode));

            ClearLog = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, ClearLogCode));
            GetLogsInPeriod = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsInPeriodCode));
            GetLogs = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsCode));


            SendBlackList = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendBlackListCode));

            UserAdaptation = lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UserAdaptationCode));
        }


        public Lookup SendUsers { get; set; }
        public Lookup SendDevices { get; set; }
        public Lookup SendFoods { get; set; }
        public Lookup SendMeals { get; set; }
        public Lookup SendMealTimings { get; set; }
        public Lookup SendRestaurants { get; set; }
        public Lookup SendReservations { get; set; }
        public Lookup GetServeLogs { get; set; }
        public Lookup DeleteUsers { get; set; }
        public Lookup DeleteDevices { get; set; }
        public Lookup DeleteFoods { get; set; }
        public Lookup DeleteMeals { get; set; }
        public Lookup DeleteMealTimings { get; set; }
        public Lookup DeleteRestaurants { get; set; }
        public Lookup DeleteReservations { get; set; }
                                                      
        public Lookup UpgradeDeviceFirmware { get; set; }
        public Lookup UnlockDevice { get; set; }
        public Lookup SendTimeZoneToTerminal { get; set; }
        public Lookup SendAccessGroupToTerminal { get; set; }
        public Lookup RetrieveUserFromTerminal { get; set; }

        public Lookup RetrieveAllUsersFromTerminal { get; set; }
        public Lookup OpenDoor { get; set; }
        public Lookup LockDevice { get; set; }
        public Lookup EnrollFromTerminal { get; set; }
        public Lookup DeleteUserFromTerminal { get; set; }
        public Lookup EnrollFaceFromTerminal { get; set; }

        public Lookup ClearLog { get; set; }
        public Lookup GetLogsInPeriod { get; set; }
        public Lookup GetLogs { get; set; }

        public Lookup SendBlackList { get; set; }

        //Tools
        public Lookup UserAdaptation { get; set; }

    }
}
