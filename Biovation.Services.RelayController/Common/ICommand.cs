using Biovation.Domain;

namespace Biovation.Services.RelayController.Common
{
    public interface ICommand
    {
        public IRelay Relay { get; set; }
        public Lookup _priority { get; set; }

        public ResultViewModel Execute();
    }
}