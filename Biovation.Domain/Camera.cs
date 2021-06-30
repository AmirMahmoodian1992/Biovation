using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class Camera : Gadget
    {
        public string ConnectionUrl { get; set; }
        public string LiveStreamUrl { get; set; }

        [OneToOne]
        public CameraModel Model { get; set; }
    }
}