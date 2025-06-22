using System.Text.Json.Serialization;

namespace MyToDo.Shared
{
    public class ApiResponse
    {
        public ApiResponse(bool status, object result)
        {
            this.Status = status;
            this.Result = result;
            if (this.Status)
                this.Message = "OK";
        }

        public ApiResponse(string message, bool status = false)
        {
            this.Status = status;
            this.Message = message;
        }

        public ApiResponse() { }

        public string Message { get; set; }

        public bool Status { get; set; }

        public object Result { get; set; }
    }

    public class ApiResponse<T>
    {
        public ApiResponse(bool status, T result)
        {
            this.Status = status;
            this.Result = result;
        }

        public ApiResponse(string message, bool status = false)
        {
            this.Status = status;
            this.Message = message;
        }

        public ApiResponse() { }

        public string Message { get; set; }

        public bool Status { get; set; }

        public T Result { get; set; }
    }
}
