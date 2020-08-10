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

        public static Lookup SendUsers = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendUsersCode));
        public static Lookup SendDevices = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendDevicesCode));
        public static Lookup SendFoods = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendFoodsCode));
        public static Lookup SendMeals = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealsCode));
        public static Lookup SendMealTimings = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealTimingsCode));
        public static Lookup SendRestaurants = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendRestaurantsCode));
        public static Lookup SendReservations = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendReservationsCode));
        public static Lookup GetServeLogs = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetServeLogsCode));
        public static Lookup DeleteUsers = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUsersCode));
        public static Lookup DeleteDevices = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteDevicesCode));
        public static Lookup DeleteFoods = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteFoodsCode));
        public static Lookup DeleteMeals = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealsCode));
        public static Lookup DeleteMealTimings = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealTimingsCode));
        public static Lookup DeleteRestaurants = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteRestaurantsCode));
        public static Lookup DeleteReservations = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteReservationsCode));

        public static Lookup UpgradeDeviceFirmware = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UpgradeDeviceFirmwareCode));
        public static Lookup UnlockDevice = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnlockDeviceCode));
        public static Lookup SendTimeZoneToTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendTimeZoneToTerminalCode));
        public static Lookup SendAccessGroupToTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendAccessGroupToTerminalCode));
        public static Lookup RetrieveUserFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveUserFromTerminalCode));

        public static Lookup RetrieveAllUsersFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveAllUsersFromTerminalCode));
        public static Lookup OpenDoor = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, OpenDoorCode));
        public static Lookup LockDevice = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, LockDeviceCode));
        public static Lookup EnrollFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFromTerminalCode));
        public static Lookup DeleteUserFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUserFromTerminalCode));
        public static Lookup EnrollFaceFromTerminal = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFaceFromTerminalCode));

        public static Lookup ClearLog = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, ClearLogCode));
        public static Lookup GetLogsInPeriod = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsInPeriodCode));
        public static Lookup GetLogs= Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsCode));


        public static Lookup SendBlackList = Lookups.TaskTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendBlackListCode));

    }
}
