using Biovation.Domain;
using System.Linq;

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


        public LogEvents(Lookups lookups)
        {
            Connect =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, ConnectCode));
            Disconnect =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DisconnectCode));
            Authorized =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, AuthorizedCode));
            UnAuthorized =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, UnAuthorizedCode));
            AddUserToDevice =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, AddUserToDeviceCode));
            RemoveUserFromDevice =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, RemoveUserFromDeviceCode));
            DeviceEnabled =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceEnabledCode));
            DeviceDisabled =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, DeviceDisabledCode));
            EnrollSuccess =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, EnrollSuccessCode));
            IdentifySuccessFace =
                lookups.LogEvents.FirstOrDefault(lookup => string.Equals(lookup.Code, IdentifyFaceSuccessCode));
        }

        public Lookup Connect { get; set; }
        public Lookup Disconnect { get; set; }
        public Lookup Authorized { get; set; }
        public Lookup UnAuthorized { get; set; }
        public Lookup AddUserToDevice { get; set; }
        public Lookup RemoveUserFromDevice { get; set; }
        public Lookup DeviceEnabled { get; set; }
        public Lookup DeviceDisabled { get; set; }
        public Lookup EnrollSuccess { get; set; }
        public Lookup IdentifySuccessFace { get; set; }
    }
}


