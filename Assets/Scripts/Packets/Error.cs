namespace Packets
{
    public class Error
    {
        public const byte CRITICAL = 2;
        public const byte ERROR = 1;
        public const byte INFO = 0;

        public byte Level
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }
    }
}