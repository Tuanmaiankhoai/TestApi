namespace BaiTestPost.Payload.Responses
{
    public class ResponseObject<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public ResponseObject() { }
        public ResponseObject(int status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
        public ResponseObject<T> ResponseAccess(string message, T data)
        {
            return new ResponseObject<T>(StatusCodes.Status200OK, message, data);
        }
        public ResponseObject<T> responseError (int status, string message, T data)
        {
            return new ResponseObject<T>(status, message, data);
        }
    }
}
