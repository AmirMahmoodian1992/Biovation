using System.Linq;
using Biovation.Domain;

namespace Biovation.Constants
{
    public class LogEvents
    {
        public const string ConnectCode = "16001";//88
        public const string DisconnectCode = "16002";//87


        public const string AuthorizedCode = "16003";//55;
        public const string UnAuthorizedCode = "16004";//56;

        public const string AddUserToDeviceCode = "16005";//44;
        public const string RemoveUserFromDeviceCode = "16006"; //45;

        public const string DeviceEnabledCode = "16007";//12;
        public const string DeviceDisabledCode = "16008";//13;


        public const string EnrollSuccessCode = "16009";//23;
        public const string IdentifyFaceSuccessCode = "16010";//61suprema;


        public LogEvents()
        {
            Connect = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, ConnectCode));
            Disconnect = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DisconnectCode));
            Authorized = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, AuthorizedCode));
            UnAuthorized = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, UnAuthorizedCode));
            AddUserToDevice = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, AddUserToDeviceCode));
            RemoveUserFromDevie = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, RemoveUserFromDeviceCode));
            DeviceEnabled = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceEnabledCode));
            DeviceDisabled = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceDisabledCode));
            EnrollSuccess = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollSuccessCode));
            IdentifySuccessFace = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, IdentifyFaceSuccessCode));
        }

        public static Lookup Connect { get; set; }
        public static Lookup Disconnect { get; set; }
        public static Lookup Authorized { get; set; }
        public static Lookup UnAuthorized { get; set; }
        public static Lookup AddUserToDevice { get; set; }
        public static Lookup RemoveUserFromDevie { get; set; }
        public static Lookup DeviceEnabled { get; set; }
        public static Lookup DeviceDisabled { get; set; }
        public static Lookup EnrollSuccess { get; set; }
        public static Lookup IdentifySuccessFace { get; set; }
    }
}


