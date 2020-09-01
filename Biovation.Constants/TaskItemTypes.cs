﻿using Biovation.Domain;
using System.Linq;

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

        public const string SendBlackListCode = "12601";

        public const string ClearLogCode = "12401";
        public const string GetLogsInPeriodCode = "12402";
        public const string GetLogsCode = "12403";


        public TaskItemTypes(Lookups lookups)
        {
            SendUser = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendUserCode));
            SendDevice = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendDeviceCode));
            SendFood = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendFoodCode));
            SendMeal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealCode));
            SendMealTimings = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendMealTimingsCode));
            SendRestaurant = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendRestaurantCode));
            SendReservation = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendReservationCode));
            GetServeLogs = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetServeLogsCode));
            GetServeLogsInPeriod = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetServeLogsInPeriodCode));
            UpgradeDeviceFirmware = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UpgradeDeviceFirmwareCode));
            UnlockDevice = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, UnlockDeviceCode));
            SendTimeZoneToTerminal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendTimeZoneToTerminalCode));
            SendAccessGroupToTerminal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendAccessGroupToTerminalCode));
            RetrieveUserFromTerminal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveUserFromTerminalCode));
            RetrieveAllUsersFromTerminal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, RetrieveAllUsersFromTerminalCode));
            OpenDoor = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, OpenDoorCode));
            LockDevice = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, LockDeviceCode));
            EnrollFromTerminal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFromTerminalCode));
            DeleteUserFromTerminal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUserFromTerminalCode));
            EnrollFaceFromTerminal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollFaceFromTerminalCode));
            SendBlackList = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, SendBlackListCode));

            DeleteUser = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteUserCode));
            DeleteDevice = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteDeviceCode));
            DeleteFood = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteFoodCode));
            DeleteMeal = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealCode));
            DeleteMealTimings = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteMealTimingsCode));
            DeleteRestaurant = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteRestaurantCode));
            DeleteReservation = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, DeleteReservationCode));

            GetLogs = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsCode));
            ClearLog = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, ClearLogCode));
            GetLogsInPeriod = lookups.TaskItemTypes.FirstOrDefault(lookup => string.Equals(lookup.Code, GetLogsInPeriodCode));
        }

        public static Lookup SendUser;
        public static Lookup SendDevice;
        public static Lookup SendFood;
        public static Lookup SendMeal;
        public static Lookup SendMealTimings;
        public static Lookup SendRestaurant;
        public static Lookup SendReservation;
        public static Lookup GetServeLogs;
        public static Lookup GetServeLogsInPeriod;
        public static Lookup UpgradeDeviceFirmware;
        public static Lookup UnlockDevice;
        public static Lookup SendTimeZoneToTerminal;
        public static Lookup SendAccessGroupToTerminal;
        public static Lookup RetrieveUserFromTerminal;
        public static Lookup RetrieveAllUsersFromTerminal;
        public static Lookup OpenDoor;
        public static Lookup LockDevice;
        public static Lookup EnrollFromTerminal;
        public static Lookup DeleteUserFromTerminal;
        public static Lookup EnrollFaceFromTerminal;
        public static Lookup SendBlackList;

        public static Lookup DeleteUser;
        public static Lookup DeleteDevice;
        public static Lookup DeleteFood;
        public static Lookup DeleteMeal;
        public static Lookup DeleteMealTimings;
        public static Lookup DeleteRestaurant;
        public static Lookup DeleteReservation;

        public static Lookup GetLogs;
        public static Lookup ClearLog;
        public static Lookup GetLogsInPeriod;

    }
}
