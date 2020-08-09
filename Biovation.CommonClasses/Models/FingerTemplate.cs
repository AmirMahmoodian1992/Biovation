using System;
using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class FingerTemplate
    {
        [Id]
        public int Id { get; set; }
        public long UserId { get; set; }
        public int TemplateIndex { get; set; }
        [OneToOne]
        public Lookup FingerIndex { get; set; }
        public byte[] Template { get; set; }
        public int Index { get; set; }
        public bool Duress { get; set; }
        public int CheckSum { get; set; }
        public int SecurityLevel { get; set; }
        public int EnrollQuality { get; set; }
        public int Size { get; set; }
        
        public string Image { get; set; }
        [OneToOne]
        public Lookup FingerTemplateType { get; set; }
        public DateTime? CreateAt { get; set; }
        public long CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public long UpdateBy { get; set; }
    }
}
