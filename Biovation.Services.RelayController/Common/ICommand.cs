namespace Biovation.Services.RelayController.Common
{
    public interface ICommand
    {
        public IRelay Relay { get; set; }

        public object Execute();
    }
}