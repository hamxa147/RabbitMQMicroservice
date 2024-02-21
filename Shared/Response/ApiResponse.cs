
namespace SharedMessages.Response
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = "";
        public object Result { get; set; }
    }
}
