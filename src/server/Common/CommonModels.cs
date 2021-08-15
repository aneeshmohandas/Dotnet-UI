using System.Net;

namespace DotnetUI.Common
{
    public class AppResponse
    {
        public string RequestId { get; set; }
        public HttpStatusCode Status { get; set; }
        public object Data { get; set; }
    }
    public class AppRequest
    {
        public string RequestId { get; set; }
        public object Data { get; set; }
    }
    public class SocketResponse : AppResponse
    {

    }
}