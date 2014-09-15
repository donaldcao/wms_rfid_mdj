
namespace THOK.WCS.REST.Models
{
    public class RestResult
    {
        public bool IsSuccess { get; set; }
        public string Message = string.Empty;
        public SRMTask Data { get; set; }
    }
}
