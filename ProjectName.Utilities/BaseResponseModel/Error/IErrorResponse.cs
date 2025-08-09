namespace RS.Utilities.BaseResponseModel.Error;
public interface IErrorResponse
{   //Errors that we need to show to the user

    public int StatusCode { get; set; }

    List<string> Errors { get; set; }
    //Application specific errors
    //public List<string> APIErrors { get; set; }
    int ErrorID { get; set; }
    Exception OriginalException { get; set; }
    bool IsSuccess { get; set; }
    public string CustomErrorMessage { get; set; }
}
