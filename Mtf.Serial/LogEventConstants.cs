namespace Mtf.Serial
{
    public static class LogEventConstants
    {
        public const int SerialErrorReceivedEventId = 1;

        public const int SerialDebugReceivedEventId = 2;

        public const string SerialErrorReceivedFormatMessage = "Error occurred in a serial device: {Device}, {EventDetails}";

        public const string SerialDebugReceivedFormatMessage = "Debug message on serial device: {Device}, {Message}";
    }
}
