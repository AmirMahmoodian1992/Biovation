namespace Biovation.CommonClasses.Interface
{
    /// <summary>
    /// It is starting point of Command Pattern. For more information visit https://sourcemaking.com/design_patterns/command
    /// </summary>
    public interface ICommand
    {
        object Execute();
        void Rollback();
        string GetTitle();
        string GetDescription();
    }
}
