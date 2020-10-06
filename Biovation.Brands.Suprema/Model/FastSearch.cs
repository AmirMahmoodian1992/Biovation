
using Biovation.CommonClasses.Models;
using System.Collections.Generic;

namespace Biovation.Brands.Suprema.Model
{
    public class FastSearch
    {
        public int[] TemplateSize { get; set; }
        public byte[][] TemplateByte { get; set; }
        public List<FingerTemplate> FingerTemplateList { get; set; }
    }
}
