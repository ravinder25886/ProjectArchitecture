namespace RS.Utilities.BaseResponseModel.Error;
public class ErrorResponse : IErrorResponse
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public ErrorResponse()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        //APIErrors = new List<string>();
        Errors = new List<string>();
    }

    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; } = 200;
    //public List<string> APIErrors { get; set; }
    public List<string> Errors { get; set; }
    public Exception OriginalException { get; set; }
    public string CustomErrorMessage { get; set; }
    public int ErrorID { get; set; }
}
