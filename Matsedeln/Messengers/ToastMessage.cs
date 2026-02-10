namespace Matsedeln.Messengers
{
    public class ToastMessage
    {
        public string Message { get; set; }
        public int DurationInSeconds { get; set; } = 3;
        public bool IsError { get; set; }
        public ToastMessage(string message, bool isError = false, int durationInSeconds = 3)
        {
            Message = message;
            IsError = isError;
            DurationInSeconds = durationInSeconds;
        }
    }
}
