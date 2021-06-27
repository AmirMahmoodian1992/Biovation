using System.Runtime.InteropServices;
using Biovation.Brands.Virdi.UniComAPI;
using Biovation.CommonClasses;

namespace Biovation.Brands.Virdi
{
    //public static class WinAPI
    //{
    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_ServerStart(int maxTerminal, int port, int reserved, _DIUCSAPIEvents_EventTerminalConnectedEventHandler callbackEventFunction); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_ServerStop();

    //    [DllImport("UCSAPI40.dll", EntryPoint = "UCSAPI_AddUserToTerminal", CallingConvention = CallingConvention.StdCall)]
    //    public static extern uint UCSAPI_AddUserToTerminal(short clientID, int terminalID, bool isOverwrite, ucsapi_user_data pUserData); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_DeleteUserFromTerminal(short clientID, int terminalID, int userID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetTerminalInfo(int terminalID, out UCSAPI_TERMINAL_INFO tInfo);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_DeleteAllUserFromTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetUserCountFromTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetUserInfoListFromTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetUserDataFromTerminal(short clientID, int terminalID, int userID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetAccessLogCountFromTerminal(short clientID, int terminalID, int logType); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetAccessLogFromTerminal(short clientID, int terminalID, int logType); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SendAuthInfoToTerminal(int terminalID, byte pUserAuthType); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SendAuthResultToTerminal(int terminalID, byte pResult); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetTerminalCount(int pCount);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetFirmwareVersionFromTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_UpgradeFirmwareToTerminal(short clientID, int terminalID, string pFilePath); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetOptionFromTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SetOptionToTerminal(short clientID, int terminalID, out ucsapi_terminal_option pOption); //

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_TerminalOptionDialog(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_OpenDoorToTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_ControlPeripheralDevice(short clientID, int terminalID, byte peripheralDevice);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetMealConfigFromTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SetMealConfigToTerminal(short clientID, int terminalID, byte pMealConfig);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_GetTAFunctionFromTerminal(short clientID, int terminalID);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SetTAFunctionToTerminal(short clientID, int terminalID, int taMode);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SetTATimeToTerminal(short clientID, int terminalID, byte pTATime);

    //    //[DllImport("UCSAPI40.dll")]
    //    //public static extern uint UCSAPI_EnrollFromTerminal(short clientID, int terminalID, out UNIONCOMM.SDK.UCBioBSP.UCBioAPI.Export.EXPORT_DATA pFingerData);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_FreeExportData(byte pFingerData);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SetAccessControlDataToTerminal(short clientID, int terminalID, byte pAccessControlData);

    //    [DllImport("UCSAPI40.dll")]
    //    public static extern uint UCSAPI_SetSkinResource(string szSkinPath);

    //    public struct ucsapi_terminal_option
    //    {
    //        public ucsapi_terminal_option_flag Flsgs;

    //        public ucsapi_security_level SecurityLevel;
    //        public short InputIDLength;	// 4 - 8
    //        public short AutoEnterKey;	// 0/1
    //        public short Sound;			// 0/1
    //        public short Authentication;	// N/S, S/N, NO, SO
    //        public short Applicaction;	// Access Control, Time Attendance, Meal Management
    //        public short Antipassback;
    //        public ucsapi_network_info Network;
    //        public ucsapi_server_info Server;
    //        public short InputIDType;
    //        public short AccessLevel;
    //        public short PrintName;
    //        public ucsapi_terminal_schedule Schedule;
    //    }
    //    public class ucsapi_security_level
    //    {
    //        public short Verify = 4;
    //        public short Identify = 4;
    //    }

    //    public class ucsapi_terminal_option_flag
    //    {
    //        public short SecurityLevel;
    //        public short InputIDLength;
    //        public short AutoEnterKey;
    //        public short Sound;
    //        public short Authentication;
    //        public short Applicaction;
    //        public short Antipassback;
    //        public short Network;
    //        public short Server;
    //        public short InputIDType;
    //        public short AccessLevel;
    //        public short PrintText;
    //        public short Schedule;
    //    }

    //    public struct ucsapi_network_info
    //    {
    //        public short NetworkType;
    //        public short IP;
    //        public short Subnet;
    //        public short Gateway;
    //    }

    //    public struct ucsapi_server_info
    //    {
    //        public short IP;
    //        public short Port;
    //        public short Reserved;
    //    }

    //    public struct ucsapi_terminal_schedule
    //    {
    //        public ucsapi_terminal_day_schedule Sun;
    //        public ucsapi_terminal_day_schedule Mon;
    //        public ucsapi_terminal_day_schedule Tue;
    //        public ucsapi_terminal_day_schedule Wed;
    //        public ucsapi_terminal_day_schedule Thu;
    //        public ucsapi_terminal_day_schedule Fri;
    //        public ucsapi_terminal_day_schedule Sat;
    //        public ucsapi_terminal_day_schedule Holiday1;
    //        public ucsapi_terminal_day_schedule Holiday2;
    //        public ucsapi_terminal_day_schedule Holiday3;
    //        public ucsapi_terminal_holiday_info Holidays;

    //    }
    //    public struct ucsapi_terminal_day_schedule
    //    {
    //        public ucsapi_terminal_timezone Lock1;
    //        public ucsapi_terminal_timezone Lock2;
    //        public ucsapi_terminal_timezone Lock3;
    //        public ucsapi_terminal_timezone Open1;
    //        public ucsapi_terminal_timezone Open2;
    //        public ucsapi_terminal_timezone Open3;

    //    }
    //    public struct ucsapi_terminal_timezone
    //    {
    //        public short IsUsed;
    //        public short StartHour;
    //        public short StartMin;
    //        public short EndHour;
    //        public short EndMin;
    //    }
    //    public struct ucsapi_terminal_holiday_info
    //    {
    //        public short Month;
    //        public short Day;
    //        public UCSAPI_HOLIDAY_TYPE HolidayType; // 1 ~ 3 digit
    //    }

    //    public class UCSAPI_HOLIDAY_TYPE
    //    {
    //        public const short UCSAPI_HOLIDAY_TYPE_1 = 1;
    //        public const short UCSAPI_HOLIDAY_TYPE_2 = 2;
    //        public const short UCSAPI_HOLIDAY_TYPE_3 = 3;
    //    }

    //    /////////////////////////////////////////////////////////////////
    //    public struct ucsapi_user_data
    //    {
    //        public ucsapi_user_info UserInfo;
    //        public ucsapi_auth_data AuthData;
    //        public ucsapi_picture_data PictureData;
    //    }

    //    public struct ucsapi_user_info
    //    {
    //        public uint UserID;
    //        public ucsapi_data UserName;
    //        public ucsapi_data UniqueID;
    //        public ucsapi_user_property Property;
    //        public byte Reserved;
    //        public ucsapi_access_authority AccessAuthority;

    //    }

    //    public struct ucsapi_data
    //    {
    //        public uint Length;				/* just length of Data (not sizeof structure) */
    //        public object Data;

    //    }
    //    public class ucsapi_user_property
    //    {
    //        public byte Finger;
    //        public byte FPCard;
    //        public byte Password;
    //        public byte Card;
    //        public byte CardID;
    //        public byte Operation;
    //        public byte Identify;
    //        public byte Admin;
    //    }

    //    public struct ucsapi_access_authority
    //    {
    //        public ucsapi_data AccessGroup;
    //        public UCSAPI_ACCESS_DATE_TYPE AccessDateType;
    //        public ucsapi_access_date AccessDate;
    //    }

    //    //public enum UCSAPI_HOLIDAY_TYPE : int
    //    //{
    //    //    UCSAPI_DATE_TYPE_NOT_USE = 0,
    //    //    UCSAPI_DATE_TYPE_ALLOWED = 1,
    //    //    UCSAPI_DATE_TYPE_RESTRICTION = 2

    //    //}

    //    public struct ucsapi_access_date
    //    {
    //        public ucsapi_date_yyyy_mm_dd StartDate;
    //        public ucsapi_date_yyyy_mm_dd EndDate;
    //    }

    //    public struct ucsapi_date_yyyy_mm_dd
    //    {
    //        public ushort Year;
    //        public byte Month;
    //        public byte Day;
    //    }

    //    public struct ucsapi_auth_data
    //    {
    //        public ucsapi_data Password;
    //        public ucsapi_card_data Card;
    //        public ucsapi_finger_data Finger;
    //    }



    //    public struct ucsapi_card_data
    //    {
    //        public int CardNum;
    //        public ucsapi_data RFID;
    //    }

    //    public struct ucsapi_finger_data
    //    {
    //        public int SecurityLevel;
    //        public bool IsCheckSimilarFinger;
    //        public ucbioapi_export_data ExportData;
    //    }

    //    public struct ucbioapi_export_data
    //    {
    //        public int Length;           /* sizeof of structure */
    //        public UCBioAPI_TEMPLATE_TYPE TemplateType;     /* Template type */
    //        public byte FingerNum;
    //        public UCBioAPI_FINGER_ID DefaultFingerID;  /* UCBioAPI_FINGER_ID */
    //        public byte SamplesPerFinger;
    //        public byte Reserved;
    //        public ucbioapi_finger_block FingerInfo;       /* Finger information */
    //    }
    //    public enum UCSAPI_ACCESS_DATE_TYPE : int
    //    {
    //        UCSAPI_DATE_TYPE_NOT_USE = 0,
    //        UCSAPI_DATE_TYPE_ALLOWED = 1,
    //        UCSAPI_DATE_TYPE_RESTRICTION = 2
    //    }

    //    public class UCBioAPI_TEMPLATE_TYPE
    //    {
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE400 = 400;
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE500 = 500;
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE600 = 600;
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE800 = 800;
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE1000 = 1000;
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE1200 = 1200;
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE320 = 320;
    //        public const int UCBioAPI_TEMPLATE_TYPE_SIZE256 = 256;
    //        public const int UCBioAPI_TEMPLATE_TYPE_FMR = 1;
    //        public const int UCBioAPI_TEMPLATE_TYPE_ANSI = 2;
    //    }

    //    public class UCBioAPI_FINGER_ID
    //    {
    //        public const byte UCBioAPI_FINGER_ID_UNKNOWN = 0; /* for verify */
    //        public const byte UCBioAPI_FINGER_ID_RIGHT_THUMB = 1;
    //        public const byte UCBioAPI_FINGER_ID_RIGHT_INDEX = 2;
    //        public const byte UCBioAPI_FINGER_ID_RIGHT_MIDDLE = 3;
    //        public const byte UCBioAPI_FINGER_ID_RIGHT_RING = 4;
    //        public const byte UCBioAPI_FINGER_ID_RIGHT_LITTLE = 5;
    //        public const byte UCBioAPI_FINGER_ID_LEFT_THUMB = 6;
    //        public const byte UCBioAPI_FINGER_ID_LEFT_INDEX = 7;
    //        public const byte UCBioAPI_FINGER_ID_LEFT_MIDDLE = 8;
    //        public const byte UCBioAPI_FINGER_ID_LEFT_RING = 9;
    //        public const byte UCBioAPI_FINGER_ID_LEFT_LITTLE = 10;
    //        public const byte UCBioAPI_FINGER_ID_MAX = 11;
    //    }

    //    public struct ucbioapi_finger_block
    //    {
    //        public int Length;        /* sizeof of structure */
    //        public UCBioAPI_FINGER_ID FingerID;      /* UCBioAPI_FINGER_ID */
    //        public ucbioapi_template_block TemplateInfo;  /* TemplateInfo = UCBioAPI_TEMPLATE_INFO * SamplesPerFinger */
    //    }

    //    public struct ucbioapi_template_block
    //    {
    //        public int Length;           /* just length of Data (not sizeof structure) */
    //        public byte Data;             /* variable length of data */
    //    }

    //    public struct ucsapi_picture_data
    //    {
    //        public ucsapi_picture_header Header;
    //        public byte Data;
    //    }

    //    public struct ucsapi_picture_header
    //    {
    //        public byte Format;		/* must be "JPG" */
    //        public int Length;			/* max Length is 7 Kbytes */
    //    }

    //    public class UCSAPI_DATA_SIZE
    //    {
    //        public const int UCSAPI_DATA_SIZE_PASSWORD = 8;
    //        public const int UCSAPI_DATA_SIZE_RFID = 24;
    //        public const int UCSAPI_DATA_SIZE_USER_NAME = 16;
    //        public const int UCSAPI_DATA_SIZE_UNIQUE_ID = 20;
    //        public const int UCSAPI_DATA_SIZE_CODE4 = 4;
    //        public const int UCSAPI_DATA_SIZE_PICTURE = 7168;
    //    }

    //    public struct ucsapi_callback_param_0
    //    {
    //        public uint ClientID;
    //        public uint ErrorCode;
    //        public ucsapi_progress_info Progress;

    //    }
    //    public struct ucsapi_progress_info
    //    {
    //        public uint CurrentIndex;
    //        public uint TotalNumber;
    //    }

    //    //public struct ucsapi_callback_param_1
    //    //{
    //    //    public UCSAPI_CALLBACK_DATA_TYPE DataType;
    //    //    public ucsapi_user_info UserInfo;
    //    //    public ucsapi_user_data UserData;
    //    //    public ucsapi_access_log_data AccessLog;
    //    //    public ucsapi_callback_param_1(UCSAPI_CALLBACK_DATA_TYPE type, ucsapi_user_info info, ucsapi_user_data data, ucsapi_access_log_data log)
    //    //    {
    //    //        DataType = type;
    //    //        UserInfo = info;
    //    //        UserData = data;
    //    //        AccessLog = log;
    //    //    }
    //    //}

    //    [StructLayout(LayoutKind.Sequential)]
    //    public struct ucsapi_callback_param_1
    //    {
    //        public UCSAPI_CALLBACK_DATA_TYPE DataType;

    //        [StructLayout(LayoutKind.Explicit)]
    //        public struct Data
    //        {
    //            //[MarshalAs(UnmanagedType.FunctionPtr)]
    //            [FieldOffset(0)]
    //            public uint UserInfo;

    //            //[MarshalAs(UnmanagedType.AsAny)]
    //            [FieldOffset(0)]
    //            public uint UserData;

    //            //[MarshalAs(UnmanagedType.AsAny)]
    //            [FieldOffset(0)]
    //            public uint AccessLog;
    //        }
    //        public Data data;
    //    }


    //    public enum UCSAPI_CALLBACK_EVENT_TYPE : uint
    //    {
    //        UCSAPI_CALLBACK_EVENT = 1600,
    //        UCSAPI_CALLBACK_EVENT_CONNECTED = UCSAPI_CALLBACK_EVENT + 1,
    //        UCSAPI_CALLBACK_EVENT_DISCONNECTED = UCSAPI_CALLBACK_EVENT + 2,
    //        UCSAPI_CALLBACK_EVENT_REALTIME_ACCESS_LOG = UCSAPI_CALLBACK_EVENT + 3,

    //        UCSAPI_CALLBACK_EVENT_GET_ACCESS_LOG = UCSAPI_CALLBACK_EVENT + 4,
    //        UCSAPI_CALLBACK_EVENT_GET_ACCESS_LOG_COUNT = UCSAPI_CALLBACK_EVENT + 5,

    //        UCSAPI_CALLBACK_EVENT_ADD_USER = UCSAPI_CALLBACK_EVENT + 10,
    //        UCSAPI_CALLBACK_EVENT_DELETE_USER = UCSAPI_CALLBACK_EVENT + 11,
    //        UCSAPI_CALLBACK_EVENT_DELETE_ALL_USER = UCSAPI_CALLBACK_EVENT + 12,
    //        UCSAPI_CALLBACK_EVENT_GET_USER_COUNT = UCSAPI_CALLBACK_EVENT + 13,
    //        UCSAPI_CALLBACK_EVENT_GET_USER_INFO_LIST = UCSAPI_CALLBACK_EVENT + 14,
    //        UCSAPI_CALLBACK_EVENT_GET_USER_DATA = UCSAPI_CALLBACK_EVENT + 15,

    //        UCSAPI_CALLBACK_EVENT_VERIFY_USER_AUTH_INFO = UCSAPI_CALLBACK_EVENT + 20,
    //        UCSAPI_CALLBACK_EVENT_VERIFY_FINGER_1_TO_N = UCSAPI_CALLBACK_EVENT + 21,
    //        UCSAPI_CALLBACK_EVENT_VERIFY_FINGER_1_TO_1 = UCSAPI_CALLBACK_EVENT + 22,
    //        UCSAPI_CALLBACK_EVENT_VERIFY_CARD = UCSAPI_CALLBACK_EVENT + 23,
    //        UCSAPI_CALLBACK_EVENT_VERIFY_PASSWORD = UCSAPI_CALLBACK_EVENT + 24,

    //        UCSAPI_CALLBACK_EVENT_GET_TERMINAL_OPTION = UCSAPI_CALLBACK_EVENT + 30,
    //        UCSAPI_CALLBACK_EVENT_SET_TERMINAL_OPTION = UCSAPI_CALLBACK_EVENT + 31,

    //        UCSAPI_CALLBACK_EVENT_FW_UPGRADING = UCSAPI_CALLBACK_EVENT + 80,
    //        UCSAPI_CALLBACK_EVENT_FW_UPGRADED = UCSAPI_CALLBACK_EVENT + 81,
    //        UCSAPI_CALLBACK_EVENT_FW_VERSION = UCSAPI_CALLBACK_EVENT + 82,

    //        UCSAPI_CALLBACK_EVENT_OPEN_DOOR = UCSAPI_CALLBACK_EVENT + 90,
    //        UCSAPI_CALLBACK_EVENT_TERMINAL_STATUS = UCSAPI_CALLBACK_EVENT + 91,

    //        UCSAPI_CALLBACK_EVENT_PICTURE_LOG = UCSAPI_CALLBACK_EVENT + 100,
    //        UCSAPI_CALLBACK_EVENT_ANTIPASSBACK = UCSAPI_CALLBACK_EVENT + 101,
    //        UCSAPI_CALLBACK_EVENT_SET_ACCESS_CONTROL_DATA = UCSAPI_CALLBACK_EVENT + 102,

    //        UCSAPI_CALLBACK_EVENT_CONTROL_PERIPHERAL_DEVICE = UCSAPI_CALLBACK_EVENT + 110,
    //        UCSAPI_CALLBACK_EVENT_GET_TA_FUNCTION = UCSAPI_CALLBACK_EVENT + 120,
    //        UCSAPI_CALLBACK_EVENT_SET_TA_FUNCTION = UCSAPI_CALLBACK_EVENT + 121,
    //        UCSAPI_CALLBACK_EVENT_SET_TA_TIME = UCSAPI_CALLBACK_EVENT + 122,

    //        UCSAPI_CALLBACK_EVENT_GET_TERMINAL_MEAL_CONFIG = UCSAPI_CALLBACK_EVENT + 130,
    //        UCSAPI_CALLBACK_EVENT_SET_TERMINAL_MEAL_CONFIG = UCSAPI_CALLBACK_EVENT + 131
    //    }

    //    //public class UCSAPI_CALLBACK_EVENT_TYPE
    //    //{
    //    //    public const uint UCSAPI_CALLBACK_EVENT = 1600;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_CONNECTED = UCSAPI_CALLBACK_EVENT + 1;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_DISCONNECTED = UCSAPI_CALLBACK_EVENT + 2;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_REALTIME_ACCESS_LOG = UCSAPI_CALLBACK_EVENT + 3;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_ACCESS_LOG = UCSAPI_CALLBACK_EVENT + 4;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_ACCESS_LOG_COUNT = UCSAPI_CALLBACK_EVENT + 5;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_ADD_USER = UCSAPI_CALLBACK_EVENT + 10;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_DELETE_USER = UCSAPI_CALLBACK_EVENT + 11;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_DELETE_ALL_USER = UCSAPI_CALLBACK_EVENT + 12;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_USER_COUNT = UCSAPI_CALLBACK_EVENT + 13;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_USER_INFO_LIST = UCSAPI_CALLBACK_EVENT + 14;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_USER_DATA = UCSAPI_CALLBACK_EVENT + 15;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_VERIFY_USER_AUTH_INFO = UCSAPI_CALLBACK_EVENT + 20;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_VERIFY_FINGER_1_TO_N = UCSAPI_CALLBACK_EVENT + 21;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_VERIFY_FINGER_1_TO_1 = UCSAPI_CALLBACK_EVENT + 22;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_VERIFY_CARD = UCSAPI_CALLBACK_EVENT + 23;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_VERIFY_PASSWORD = UCSAPI_CALLBACK_EVENT + 24;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_TERMINAL_OPTION = UCSAPI_CALLBACK_EVENT + 30;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_SET_TERMINAL_OPTION = UCSAPI_CALLBACK_EVENT + 31;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_FW_UPGRADING = UCSAPI_CALLBACK_EVENT + 80;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_FW_UPGRADED = UCSAPI_CALLBACK_EVENT + 81;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_FW_VERSION = UCSAPI_CALLBACK_EVENT + 82;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_OPEN_DOOR = UCSAPI_CALLBACK_EVENT + 90;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_TERMINAL_STATUS = UCSAPI_CALLBACK_EVENT + 91;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_PICTURE_LOG = UCSAPI_CALLBACK_EVENT + 100;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_ANTIPASSBACK = UCSAPI_CALLBACK_EVENT + 101;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_SET_ACCESS_CONTROL_DATA = UCSAPI_CALLBACK_EVENT + 102;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_CONTROL_PERIPHERAL_DEVICE = UCSAPI_CALLBACK_EVENT + 110;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_TA_FUNCTION = UCSAPI_CALLBACK_EVENT + 120;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_SET_TA_FUNCTION = UCSAPI_CALLBACK_EVENT + 121;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_SET_TA_TIME = UCSAPI_CALLBACK_EVENT + 122;

    //    //    public const uint UCSAPI_CALLBACK_EVENT_GET_TERMINAL_MEAL_CONFIG = UCSAPI_CALLBACK_EVENT + 130;
    //    //    public const uint UCSAPI_CALLBACK_EVENT_SET_TERMINAL_MEAL_CONFIG = UCSAPI_CALLBACK_EVENT + 131;
    //    //}

    //    public enum UCSAPI_CALLBACK_DATA_TYPE : uint
    //    {
    //        UCSAPI_CALLBACK_DATA_TYPE_USER_INFO = 0,
    //        UCSAPI_CALLBACK_DATA_TYPE_USER_DATA = 1,
    //        UCSAPI_CALLBACK_DATA_TYPE_ACCESS_LOG = 2
    //    }

    //    public struct ucsapi_access_log_data
    //    {
    //        public uint UserID;
    //        public ucsapi_date_time_info DateTime;
    //        public ushort AuthMode;
    //        public ushort AuthType;
    //        public ushort[] Reserved;
    //        public bool IsAuthorized;
    //        public ucsapi_data RFID;
    //        public UCSAPI_AUTH_ERROR ErrorCode;
    //        public ucsapi_picture_data PictureData;

    //    }

    //    public class UCSAPI_AUTH_ERROR
    //    {
    //        public uint UCSAPI_AUTH_AUTHORIZED = 0; // This value is success and other value is fail.
    //        public uint UCSAPI_AUTH_INVALID_USER = 1;
    //        public uint UCSAPI_AUTH_MATCHING = 2;
    //        public uint UCSAPI_AUTH_PERMISSION = 3;
    //        public uint UCSAPI_AUTH_CAPTURE = 4;
    //        public uint UCSAPI_AUTH_DUPLICATED_AUTHENTICATION = 5;
    //        public uint UCSAPI_AUTH_ANTIPASSBACK = 6;
    //        public uint UCSAPI_AUTH_NETWORK = 7;
    //        public uint UCSAPI_AUTH_SERVER_BUSY = 8;
    //        public uint UCSAPI_AUTH_FACE_DETECTION = 9;
    //    }

    //    public struct UCSAPI_TERMINAL_INFO
    //    {
    //        public uint TerminalID;
    //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //        public byte[] TerminalIP;
    //        public byte TerminalStatus;
    //        public byte DoorStatus;
    //        public byte CoverStatus;
    //        public byte LockStatus;
    //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //        public byte[] ExtSignal;
    //        public UCSVersion Firmware;
    //        public UCSVersion Protocol;
    //        public UCSVersion CardReader;
    //        public ushort ModelNo;
    //        public byte TerminalType;
    //        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    //        public byte[] MacAddr;
    //    }


    //    public struct ucsapi_date_time_info
    //    {
    //        public byte Year;
    //        public ushort Month;
    //        public ushort Day;
    //        public ushort Hour;
    //        public ushort Min;
    //        public ushort Sec;
    //        public ushort Reserved;
    //    }

    //    public struct UCSVersion
    //    {
    //        public byte Major;

    //        public byte Minor;
    //    }
    //}

    public static class WinApi
    {
        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_Initialize")]
        public static extern VirdiError Initialize([In] ref Init0 init0);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_ServerStart")]
        public static extern VirdiError ServerStart([In] uint maxTerminal, /*[MarshalAs(UnmanagedType.U4)]*/ [In] uint port, [In] ref InitFlag flag, [MarshalAs(UnmanagedType.FunctionPtr)] [In] CallbackEventHandler callbackEventFunction);

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
        public static extern VirdiError GetAccessLogCountFromTerminal([In] ushort clientID, [In] uint terminalID, [In] LogType logType);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetAccessLogCountFromTerminalEx")]
        public static extern VirdiError GetAccessLogCountFromTerminalEx([In] ushort clientID, [In] uint terminalID, [In] VirdiDeviceLogType logType, [In] ref DatePeriod period);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetAccessLogFromTerminal")]
        public static extern VirdiError GetAccessLogFromTerminal([In] ushort clientID, [In] uint terminalID, [In] LogType logType);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetAccessLogFromTerminalEx")]
        public static extern VirdiError GetAccessLogFromTerminalEx([In] ushort clientID, [In] uint terminalID, [In] LogType logType, [In] ref DatePeriod period);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendAuthInfoToTerminal")]
        public static extern VirdiError SendAuthInfoToTerminal([In] uint terminalID, [In] ref AuthenticationInfo userAuthType);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendAntipassbackResultToTerminal")]
        public static extern VirdiError SendAntipassbackResultToTerminal([In] uint terminalID, [In] uint userID, [In] bool result);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendAuthResultToTerminal")]
        public static extern VirdiError SendAuthResultToTerminal([In] uint terminalID, [In] ref AuthenticationResult result);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetTerminalCount")]
        public static extern VirdiError GetTerminalCount([In] ref uint count);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetOptionFromTerminal")]
        public static extern VirdiError GetOptionFromTerminal([In] ushort clientID, [In] uint terminalID);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetOptionToTerminal")]
        public static extern VirdiError SetOptionToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref TerminalOption pOption);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SendPrivateMessageToTerminal")]
        public static extern VirdiError SendPrivateMessageToTerminal([In] ushort clientID, [In] uint terminalID, [In] uint reserved, [In] ref PrivateMessage message);

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
        public static extern VirdiError SetDoorStatusToTerminal([In] ushort clientID, [In] DoorStatus terminalID, [In] uint status);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_ControlPeripheralDevice")]
        public static extern VirdiError ControlPeripheralDevice([In] ushort clientID, [In] uint terminalID, [In] ref PeripheralDevice peripheralDevice);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetMealConfigFromTerminal")]
        public static extern VirdiError GetMealConfigFromTerminal([In] ushort clientID, [In] uint terminalID);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetMealConfigToTerminal")]
        public static extern VirdiError SetMealConfigToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref MealConfiguration pMealConfig);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_GetTAFunctionFromTerminal")]
        public static extern VirdiError GetTAFunctionFromTerminal([In] ushort clientID, [In] uint terminalID);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetTAFunctionToTerminal")]
        public static extern VirdiError SetTAFunctionToTerminal([In] ushort clientID, [In] uint terminalID, [In] uint taMode);

        [DllImport("UCSAPI40.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "UCSAPI_SetTATimeToTerminal")]
        public static extern VirdiError SetTATimeToTerminal([In] ushort clientID, [In] uint terminalID, [In] ref TaTime pTaTime);

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
    }
}
