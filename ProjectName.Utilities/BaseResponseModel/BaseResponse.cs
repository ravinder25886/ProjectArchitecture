using ProjectName.Utilities.BaseResponseModel.Error;

namespace ProjectName.Utilities.BaseResponseModel;
public class BaseResponse<TResponse> : IErrorResponse
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public BaseResponse()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        Errors = new List<string>();
    }

    public TResponse Data { get; set; }
    //Error Response
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; } = 200;
    public List<string> Errors { get; set; }
    public Exception OriginalException { get; set; }
    public string CustomErrorMessage { get; set; }
    public string Message { get; set; }
    public int ErrorID { get; set; }

}
