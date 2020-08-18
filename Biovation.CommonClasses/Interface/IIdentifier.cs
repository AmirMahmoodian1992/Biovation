using Biovation.Domain;
using System.Collections.Generic;

namespace Biovation.CommonClasses.Interface
{
    public interface IIdentifier
    {
        ResultViewModel Initialize();
        ResultViewModel LoadData(List<User> users);
        ResultViewModel ClearData();
        ResultViewModel<User> Identify(FingerTemplate fingerTemplate);
        ResultViewModel<User> Identify(FingerTemplate fingerTemplate, List<User> users);
        ResultViewModel Authenticate(FingerTemplate fingerTemplate, User user);
    }
}
