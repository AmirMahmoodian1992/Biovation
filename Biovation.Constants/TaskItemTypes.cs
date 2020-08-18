using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class TaskItemTypes
    {
        public const string SendUserCode = "12001";
        public const string SendDeviceCode = "12101";
        public const string SendFoodCode = "12501";
        public const string SendMealCode = "12502";
        public const string SendRestaurantCode = "12503";
        public const string SendReservationCode = "12504";
        public const string GetServeLogsCode = "12505";
        public const string SendMealTimingsCode = "12506";
        public const string GetServeLogsInPeriodCode = "12507";
        public const string UpgradeDeviceFirmwareCode = "12102";
        public const string UnlockDeviceCode = "12103";
        public const string SendTimeZoneToTerminalCode = "12104";
        public const string SendAccessGroupToTerminalCode = "12105";
        public const string RetrieveUserFromTerminalCode = "12106";
        public const string RetrieveAllUsersFromTerminalCode = "12107";
        public const string OpenDoorCode = "12108";
        public const string LockDeviceCode = "12109";
        public const string EnrollFromTerminalCode = "12110";
        public const string DeleteUserFromTerminalCode = "12111";
        public const string EnrollFaceFromTerminalCode = "12112";



        public const string DeleteUserCode = "12002";
        public const string DeleteDeviceCode = "12102";
        public const string DeleteFoodCode = "12507";
        public const string DeleteMealCode = "12508";
        public const string DeleteRestaurantCode = "12509";
        public const string DeleteReservationCode = "12510";
        public const string DeleteMealTimingsCode = "12511";

        public const string SendBlackListCode= "12601";

        public const string ClearLogCode = "12401";
        public const string GetLogsInPeriodCode = "12402";
        public const string GetLogsCode = "12403";


        public static Lookup SendUser = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendUserCode));
        public static Lookup SendDevice = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendDeviceCode));
        public static Lookup SendFood = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendFoodCode));
        public static Lookup SendMeal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealCode));
        public static Lookup SendMealTimings = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealTimingsCode));
        public static Lookup SendRestaurant = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendRestaurantCode));
        public static Lookup SendReservation = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendReservationCode));
        public static Lookup GetServeLogs = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetServeLogsCode));
        public static Lookup GetServeLogsInPeriod = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetServeLogsInPeriodCode));
        public static Lookup UpgradeDeviceFirmware = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UpgradeDeviceFirmwareCode));
        public static Lookup UnlockDevice = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnlockDeviceCode));
        public static Lookup SendTimeZoneToTerminal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendTimeZoneToTerminalCode));
        public static Lookup SendAccessGroupToTerminal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendAccessGroupToTerminalCode));
        public static Lookup RetrieveUserFromTerminal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveUserFromTerminalCode));
        public static Lookup RetrieveAllUsersFromTerminal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveAllUsersFromTerminalCode));
        public static Lookup OpenDoor = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, OpenDoorCode));
        public static Lookup LockDevice = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, LockDeviceCode));
        public static Lookup EnrollFromTerminal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFromTerminalCode));
        public static Lookup DeleteUserFromTerminal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUserFromTerminalCode));
        public static Lookup EnrollFaceFromTerminal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFaceFromTerminalCode));
        public static Lookup SendBlackList = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendBlackListCode));





        public static Lookup DeleteUser = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUserCode));
        public static Lookup DeleteDevice = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteDeviceCode));
        public static Lookup DeleteFood = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteFoodCode));
        public static Lookup DeleteMeal = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealCode));
        public static Lookup DeleteMealTimings = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealTimingsCode));
        public static Lookup DeleteRestaurant = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteRestaurantCode));
        public static Lookup DeleteReservation = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteReservationCode));

        public static Lookup ClearLog = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, ClearLogCode));
        public static Lookup GetLogsInPeriod = Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsInPeriodCode));
        public static Lookup GetLogs= Lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsCode));

    }
}
