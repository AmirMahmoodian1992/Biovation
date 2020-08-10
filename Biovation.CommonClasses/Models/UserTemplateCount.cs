namespace Biovation.CommonClasses.Models
{
    public class UserTemplateCount
    {
        public long UserId { get; set; }
        public int FingerTemplateCount { get; set; }
        public int FaceTemplateCount { get; set; }
        public int CardCount { get; set; }
    }
}
