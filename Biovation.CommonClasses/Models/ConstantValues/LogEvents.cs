using System.Linq;
using Biovation.CommonClasses.Manager;

namespace Biovation.CommonClasses.Models.ConstantValues
{
    public class LogEvents
    {
        public const string ConnectCode = "16001";//88
        public const string DisconnectCode = "16002";//87


        public const string AuthorizedCode = "16003";//55;
        public const string UnAuthorizedCode = "16004";//56;

        public const string AddUserToDeviceCode = "16005";//44;
        public const string RemoveUserFromDeviceCode ="16006"; //45;

        public const string DeviceEnabledCode = "16007";//12;
        public const string  DeviceDisabledCode = "16008";//13;
        

        public const string EnrollSuccessCode = "16009";//23;
        public const string IdentifyFaceSuccessCode = "16010";//61suprema;

        public static Lookup Connect = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, ConnectCode));
        public static Lookup Disconnect= Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DisconnectCode));
        public static Lookup Authorized = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, AuthorizedCode));
        public static Lookup UnAuthorized = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, UnAuthorizedCode));
        public static Lookup AddUserToDevice = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, AddUserToDeviceCode));
        public static Lookup RemoveUserFromDevie = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, RemoveUserFromDeviceCode));
        public static Lookup DeviceEnabled = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceEnabledCode));
        public static Lookup DeviceDisabled = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceDisabledCode));
        public static Lookup EnrollSuccess = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollSuccessCode));
        public static Lookup IdentifySuccessFace = Lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, IdentifyFaceSuccessCode));


    }
}


