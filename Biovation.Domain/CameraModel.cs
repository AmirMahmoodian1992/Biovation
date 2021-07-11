using System.Collections.Generic;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class CameraModel : GadgetModel
    {
        [OneToMany] public List<ProtocolRemainAddresses> ProtocolRemainAddresses { get; set; }
        public int DefaultPortNumber { get; set; }
        public string DefaultUserName { get; set; }
        public string DefaultPassword { get; set; }

    }

}