namespace Biovation.Services.RelayController.Domain
{
    public class CommandType
    {
        public const int Contact = 1;
        public const int TurnOn = 2;
        public const int TurnOff = 3;
        public const int FlashOn = 4;
        public const int FlashOff = 5;
        public const int GetData = 6;
        public const int GetStatus = 7;
    }
}