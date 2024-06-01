namespace SpeakerManagement.Helper
{
    public class MethodResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public MethodResponse(bool success, string message, T data)
        {
            IsSuccess = success;
            Message = message;
            Data = data;
        }

        public static MethodResponse<T> Success(T data, string message = "Success")
        {
            return new MethodResponse<T>(true, message, data);
        }

        public static MethodResponse<T> Fail(string message = "Failure")
        {
            return new MethodResponse<T>(false, message, default(T));
        }
    }
}
