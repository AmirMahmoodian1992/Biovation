
namespace Biovation.Domain
{
    public static class CommandType
    {
        public const int PersonnelEvent = 1;
        public const int GuestEvent = 2;
        public const int ServerEventLogForceUpdate = 3;
        public const int SendAccessGroupToDevice = 4;
        public const int SendTimeZoneToDevice = 5;
        public const int ForceUpdateForSpecificDevice = 6;
        public const int SendUserToDevice = 7;
        public const int SyncAllUsers = 8;
        public const int SetTime = 9;
        public const int RetrieveAllLogsOfDevice = 10;
        public const int RetrieveLogsOfDeviceInPeriod = 11;
        public const int GetAllLogsOfDevice = 12;
        public const int GetLogsOfDeviceInPeriod = 13;
        public const int LockDevice = 14;
        public const int UnlockDevice = 15;
        public const int EnrollFromTerminal = 16;
        public const int AddUserToTerminal = 17;
        public const int DeleteUserFromTerminal = 18;
        public const int RetrieveUserFromDevice = 19;
        public const int RetrieveUsersListFromDevice = 20;
        public const int EnrollFaceFromDevice = 21;
        public const int GetUsersOfDevice = 101;
        public const int GetOnlineDevices = 102;
        public const int OpenDoor = 103;
        public const int GetLogsOfAllDevicesInPeriod = 501;
        public const int DeviceConnectedCallback = 601;
        public const int GetAllLogsOfClient = 603;
        public const int GetNewUserFromClient = 604;
        public const int UserChangedCallback = 605;
        public const int ServerSideIdentification = 606;
        public const int EditFingerTemplate = 607;
        public const int GetUser = 608;
        public const int GetDeviceAdditionalData = 609;
        public const int ClearLogOfDevice = 610;
        public const int GetServerTime = 800;

        public const int UpgradeFirmware = 11102;
        public const int DownloadUserPhotos = 11103;
        public const int UploadUserPhotos = 11104;


        public const int SendBlackList = 11601;

        #region Tools
        public const int UserAdaptation = 11701;
        #endregion

        #region Restaurant
        public const int SendUsers = 12001;
        public const int RetrieveDevices = 12101;
        public const int RetrieveFoods = 12501;
        public const int RetrieveMeals = 12502;
        public const int RetrieveRestaurants = 12503;
        public const int RetrieveReservations = 12504;
        public const int SendServeLogs = 12505;
        public const int RetrieveMealTimings = 12506;

        public const int DeleteUsers = 12002;
        public const int DeleteDevices = 12102;
        public const int DeleteFoods = 12507;
        public const int DeleteMeals = 12508;
        public const int DeleteRestaurants = 12509;
        public const int DeleteReservations = 12510;
        public const int DeleteMealTimings = 12511;
        #endregion
    }
}
