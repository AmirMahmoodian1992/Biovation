using Biovation.CommonClasses.Manager;
using System.Linq;

namespace Biovation.CommonClasses.Models.ConstantValues
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

        public TaskTypes()
        {
            SendUsers = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendUsersCode));
            SendDevices = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendDevicesCode));
            SendFoods = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendFoodsCode));
            SendMeals = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealsCode));
            SendMealTimings = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealTimingsCode));
            SendRestaurants = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendRestaurantsCode));
            SendReservations = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendReservationsCode));
            GetServeLogs = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetServeLogsCode));
            DeleteUsers = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUsersCode));
            DeleteDevices = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteDevicesCode));
            DeleteFoods = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteFoodsCode));
            DeleteMeals = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealsCode));
            DeleteMealTimings = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealTimingsCode));
            DeleteRestaurants = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteRestaurantsCode));
            DeleteReservations = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteReservationsCode));

            UpgradeDeviceFirmware = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UpgradeDeviceFirmwareCode));
            UnlockDevice = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnlockDeviceCode));
            SendTimeZoneToTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendTimeZoneToTerminalCode));
            SendAccessGroupToTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendAccessGroupToTerminalCode));
            RetrieveUserFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveUserFromTerminalCode));

            RetrieveAllUsersFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveAllUsersFromTerminalCode));
            OpenDoor = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, OpenDoorCode));
            LockDevice = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, LockDeviceCode));
            EnrollFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFromTerminalCode));
            DeleteUserFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUserFromTerminalCode));
            EnrollFaceFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFaceFromTerminalCode));

            ClearLog = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, ClearLogCode));
            GetLogsInPeriod = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsInPeriodCode));
            GetLogs = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsCode));


            SendBlackList = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendBlackListCode));
        }


        public static Lookup SendUsers { get; set; }
        public static Lookup SendDevices { get; set; }
        public static Lookup SendFoods { get; set; }
        public static Lookup SendMeals { get; set; }
        public static Lookup SendMealTimings { get; set; }
        public static Lookup SendRestaurants { get; set; }
        public static Lookup SendReservations { get; set; }
        public static Lookup GetServeLogs { get; set; }
        public static Lookup DeleteUsers { get; set; }
        public static Lookup DeleteDevices { get; set; }
        public static Lookup DeleteFoods { get; set; }
        public static Lookup DeleteMeals { get; set; }
        public static Lookup DeleteMealTimings { get; set; }
        public static Lookup DeleteRestaurants { get; set; }
        public static Lookup DeleteReservations { get; set; }
                                                             
        public static Lookup UpgradeDeviceFirmware { get; set; }
        public static Lookup UnlockDevice { get; set; }
        public static Lookup SendTimeZoneToTerminal { get; set; }
        public static Lookup SendAccessGroupToTerminal { get; set; }
        public static Lookup RetrieveUserFromTerminal { get; set; }

        public static Lookup RetrieveAllUsersFromTerminal { get; set; }
        public static Lookup OpenDoor { get; set; }
        public static Lookup LockDevice { get; set; }
        public static Lookup EnrollFromTerminal { get; set; }
        public static Lookup DeleteUserFromTerminal { get; set; }
        public static Lookup EnrollFaceFromTerminal { get; set; }

        public static Lookup ClearLog { get; set; }
        public static Lookup GetLogsInPeriod { get; set; }
        public static Lookup GetLogs { get; set; }

        public static Lookup SendBlackList { get; set; }

    }
}
