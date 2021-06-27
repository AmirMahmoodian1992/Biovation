using System.Runtime.InteropServices;
using UCSAPICOMLib;

namespace Biovation.Brands.Virdi.UniComAPI
{
	public static class UCSAPI
	{
		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_Initialize")]
		public static extern VirdiError Initialize([In] ref Init0 init0);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_ServerStart")]
        public static extern VirdiError ServerStart([In] uint maxTerminal, [MarshalAs(UnmanagedType.U4)] [In] uint port, [In] ref InitFlag flag, [MarshalAs(UnmanagedType.FunctionPtr)] [In] CallbackEventHandler callbackEventFunction);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_ServerStop")]
		public static extern VirdiError ServerStop();

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_AddUserToTerminal")]
		public static extern VirdiError AddUserToTerminal([In] ushort clientID, [In] uint terminalID, [In] bool isOverwrite, [In] ref UserData userData);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_DeleteUserFromTerminal")]
		public static extern VirdiError DeleteUserFromTerminal([In] ushort clientID, [In] uint terminalID, [In] uint userID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_DeleteAllUserFromTerminal")]
		public static extern VirdiError DeleteAllUserFromTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetUserCountFromTerminal")]
		public static extern VirdiError GetUserCountFromTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetUserInfoListFromTerminal")]
		public static extern VirdiError GetUserInfoListFromTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetUserDataFromTerminal")]
		public static extern VirdiError GetUserDataFromTerminal([In] ushort clientID, [In] uint terminalID, [In] uint userID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetAccessLogCountFromTerminal")]
		public static extern VirdiError GetAccessLogCountFromTerminal([In] ushort clientID, [In] uint terminalID, [In] VirdiDeviceLogType logType);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetAccessLogCountFromTerminalEx")]
		public static extern VirdiError GetAccessLogCountFromTerminalEx([In] ushort clientID, [In] uint terminalID, [In] VirdiDeviceLogType logType, [In] ref DatePeriod period);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetAccessLogFromTerminal")]
		public static extern VirdiError GetAccessLogFromTerminal([In] ushort clientID, [In] uint terminalID, [In] VirdiDeviceLogType logType);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetAccessLogFromTerminalEx")]
		public static extern VirdiError GetAccessLogFromTerminalEx([In] ushort clientID, [In] uint terminalID, [In] VirdiDeviceLogType logType, [In] ref DatePeriod period);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendAuthInfoToTerminal")]
		public static extern VirdiError SendAuthInfoToTerminal([In] uint terminalID, [In] ref AuthenticationInfo userAuthType);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendAntipassbackResultToTerminal")]
		public static extern VirdiError SendAntipassbackResultToTerminal([In] uint terminalID, [In] uint userID, [In] bool result);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendAuthResultToTerminal")]
		public static extern VirdiError SendAuthResultToTerminal([In] uint TerminalID, [In] ref AuthenticationResult result);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetTerminalCount")]
		public static extern VirdiError GetTerminalCount([In] ref uint count);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetOptionFromTerminal")]
		public static extern VirdiError GetOptionFromTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetOptionToTerminal")]
		public static extern VirdiError SetOptionToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref TerminalOption pOption);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendPrivateMessageToTerminal")]
		public static extern VirdiError SendPrivateMessageToTerminal([In] ushort clientID, [In] uint terminalID, [In] uint Reserved, [In] ref PrivateMessage message);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendPublicMessageToTerminal")]
		public static extern VirdiError SendPublicMessageToTerminal([In] ushort clientID, [In] uint terminalID, [In] bool show, [In] ref PublicMessage message);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetTerminalInfo")]
		public static extern VirdiError GetTerminalInfo([In] uint terminalID, [In] [Out] ref TerminalInfo terminalInfo);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetFirmwareVersionFromTerminal")]
		public static extern VirdiError GetFirmwareVersionFromTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_UpgradeFirmwareToTerminal")]
		public static extern VirdiError UpgradeFirmwareToTerminal([In] ushort clientID, [In] uint terminalID, [MarshalAs(UnmanagedType.LPStr)] [In] string pFilePath);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_TerminalOptionDialog")]
		public static extern VirdiError TerminalOptionDialog([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_OpenDoorToTerminal")]
		public static extern VirdiError OpenDoorToTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetDoorStatusToTerminal")]
		public static extern VirdiError SetDoorStatusToTerminal([In] ushort clientID, [In] DoorStatus terminalID, [In] uint Status);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_ControlPeripheralDevice")]
		public static extern VirdiError ControlPeripheralDevice([In] ushort clientID, [In] uint terminalID, [In] ref PeripheralDevice peripheralDevice);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetMealConfigFromTerminal")]
		public static extern VirdiError GetMealConfigFromTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetMealConfigToTerminal")]
		public static extern VirdiError SetMealConfigToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref MealConfiguration pMealConfig);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetTAFunctionFromTerminal")]
		public static extern VirdiError GetTAFunctionFromTerminal([In] ushort clientID, [In] uint terminalID);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetTAFunctionToTerminal")]
		public static extern VirdiError SetTAFunctionToTerminal([In] ushort clientID, [In] uint terminalID, [In] uint TAMode);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetTATimeToTerminal")]
		public static extern VirdiError SetTATimeToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref TaTime pTATime);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_EnrollFromTerminal")]
		public static extern VirdiError EnrollFromTerminal([In] ushort clientID, [In] uint terminalID, out ExportData pFingerData);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_FreeExportData")]
		public static extern VirdiError FreeExportData([In] ref ExportData pFingerData);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetAccessControlDataToTerminal")]
		public static extern VirdiError SetAccessControlDataToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref AccessControlData pAccessControlData);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetSkinResource")]
		public static extern VirdiError SetSkinResource([MarshalAs(UnmanagedType.LPStr)] [In] string szSkinPath);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetFingerImageSend")]
		public static extern VirdiError SetFingerImageSend([In] bool isSend);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendUserFileToTerminal")]
		public static extern VirdiError SendUserFileToTerminal([In] ushort clientID, [In] uint terminalID, [In] UserFileType fileType, [MarshalAs(UnmanagedType.LPStr)] [In] string filePath);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetSmartCardLayoutToTerminal")]
		public static extern VirdiError SetSmartCardLayoutToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref SmartCardLayout pCardLayout);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetTerminalTimezone")]
		public static extern VirdiError SetTerminalTimezone([In] uint terminalID, [MarshalAs(UnmanagedType.LPStr)] [In] string pTimezoneName);

		[DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetError")]
		public static extern VirdiError SetError([In] uint terminalID, [In] ErrorType errorType);

        [DllImport("UCSAPI40.dll")]
        public static extern VirdiError UCSAPI_EnrollFromTerminal(short clientID, int terminalID, out ExportData pFingerData);
    }
}
