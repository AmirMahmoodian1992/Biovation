using DataAccessLayerCore.Attributes;
using System.Globalization;

namespace Biovation.Domain
{
    public class Color
    {
        [Id] public string Code { get; set; }
        public string Name { get; set; }
        public int OrderIndex { get; set; }
        public string Description { get; set; }
        public string HexCode
        {
            get => hHexCode;
            set
            {
                hHexCode = value;
                if (HexCode == default) return;
                if (HexCode.IndexOf('#') != -1)
                    hHexCode = HexCode.Replace("#", "");
                rR = int.Parse(hHexCode.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                gG = int.Parse(hHexCode.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                bB = int.Parse(hHexCode.Substring(4, 2), NumberStyles.AllowHexSpecifier);

            }
        }
        private string hHexCode { get; set; }

        public int R => rR;
        private int rR { get; set; }
        public int G => gG;
        private int gG { get; set; }
        public int B => bB;
        private int bB { get; set; }
    }
}