using System;
using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class FaceTemplate
    {
        [Id]
        public int Id { get; set; }
        public byte[] Template { get; set; }
        public int Index { get; set; }
        public long UserId { get; set; }
        public int EnrollQuality { get; set; }
        public int CheckSum { get; set; }
        public int SecurityLevel { get; set; }
        public int Size { get; set; }
        [OneToOne]
        public Lookup FaceTemplateType { get; set; }
        public DateTime? CreateAt { get; set; }
        public long CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public long UpdateBy { get; set; }
    }
}
