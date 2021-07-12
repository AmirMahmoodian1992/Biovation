using System;
using System.Linq;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class Camera : Gadget
    {
        public string ConnectionUrl { get; set; }
        public string LiveStreamUrl { get; set; }
        [OneToOne]
        public Lookup Resolution { get; set; }

        public int ImageHeight
        {
            get
            {
                try
                {
                    var heightString = Resolution?.Name?.Split('*').LastOrDefault()?.Trim();
                    if (!string.IsNullOrWhiteSpace(heightString))
                    {
                        var parseResult = int.TryParse(heightString, out var height);
                        if (parseResult)
                            return height;
                    }
                }
                catch (Exception)
                {
                    //igonre
                }

                return 1080;
            }
        }

        public int ImageWidth
        {
            get
            {
                try
                {
                    var widthString = Resolution?.Name?.Split('*').FirstOrDefault()?.Trim();
                    if (!string.IsNullOrWhiteSpace(widthString))
                    {
                        var parseResult = int.TryParse(widthString, out var width);
                        if (parseResult)
                            return width;
                    }
                }
                catch (Exception)
                {
                    //igonre
                }

                return 1920;
            }
        }

        [OneToOne]
        public CameraModel Model { get; set; }
    }
}