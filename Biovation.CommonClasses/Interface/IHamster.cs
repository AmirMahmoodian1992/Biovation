using Biovation.CommonClasses.Models;

namespace Biovation.CommonClasses.Interface
{
    public interface IHamster
    {
        ResultViewModel Initialize();
        ResultViewModel<FingerTemplate> Capture(int securityLevel = 4, int minimumQuality = 6);
        ResultViewModel<FingerTemplate> Enroll(int fingerIndex, int fingerTemplateIndex, int templateIndex, int captureCount, int minimumQuality, int securityLevel = 3);
    }
}
