namespace ProjectName.Utilities.Constants;
public static class MainMessages
{
    public static string WebAppName = "Chalk Talk: ";
    public static string Error = "Somthing went wrong. Please try later or report to the support team!";
    public static string OrganizationDataBaseNoExist = "Please setup organization 1st, then try again!";
    private const string CREATED_SUCCESSFULLY = "%s has been created successfully.";
    private const string UPDATEDED_SUCCESSFULLY = "%s has been updated successfully.";
    private const string DELETE_SUCCESSFULLY = "%s deleted successfully.";
    private const string REQUIRED_VALUE_MISSING = "%s is required value.";
    private const string NOT_FOUND = "%s not found.";
    private const string ALREADY_EXISTS = "%s is already exists, please try another value.";
    public static string USERACCESS_FOR_ROLE_UPDATEDED_SUCCESSFULLY = "User Access has been updated successfully for %s role.";
    public static string Record_Saved = "record saved successfully";
    public static string WRONG_Owner_Data = "You cannot  modify another company's data!";
    public static string WRONG_USER = "You have entered an invalid username or password.";
    public static string WRONG_Password = "Please provide valid current password.";
    public static string WRONG_Code = "Please provide valid code.";
    public static string ACCOUNT_DEACTIVATED = "Account deactivated, please contact your support team.";
    public static string LOGIN_SUCCESSFULLY = "You are successfully logged in.";
    public static string UserNameExist = "Username is already exists. Please use a different email address.";
    public static string Reset_Password_Subject = "Verification Code ";
    public static string TokenAlreadyUsed = "This verification code has been already used or expired! please try agian";
    private const string ResetPasswordSuccessfully = "%s your password has been changed successfully, you need to re-login now.";
    public static string Forbidden = "Permission denied, please contact your administrator.";
    public static string InvalidUserNameOrEmail = "You have entered an invalid username or Email.";
    public static string ForgotPasswordEmailLinkSent = "Forgot password request link has been sent successfully on your email.";
    public static string EmailSent = "e-mail has been sent successfully.";
    public static string VerificationCode = "verification code has been sent successfully on your email address.";
    public static string InvitationSent = "Invitation(S) has been sent successfully.";
    public static string WelcomeUser = "Thanks, your account has been created!";
    public static string TeamInvitation = "Invitation has been sent!";
    public static string TeamInvitation_AlreayInTheTeam = "This team memember is already in your team!";
    public static string TeamInvitation_TimeLimit = "You have sent recently an invitation to this user, so you have to wait some time, please try soon!";
    public static string ResetUser2FA = "Microsoft 2 factor authentication has been reset!";
    public static string NoRights = "No Rights !";

    public static string SubjectCreatedSuccess(string data)
    {
        return CREATED_SUCCESSFULLY.Replace("%s", data);
    }
    public static string SubjectUpdatedSuccess(string data)
    {
        return UPDATEDED_SUCCESSFULLY.Replace("%s", data);
    }
    public static string SubjectDeletedSuccess(string data)
    {
        return DELETE_SUCCESSFULLY.Replace("%s", data);
    }
    public static string SubjectNotFound(string data)
    {
        return NOT_FOUND.Replace("%s", data);
    }
    public static string SubjectAlreadyExists(string data)
    {
        return ALREADY_EXISTS.Replace("%s", data);
    }
    public static string UserAccessForRoleSuccess(string data)
    {
        return USERACCESS_FOR_ROLE_UPDATEDED_SUCCESSFULLY.Replace("%s", data);
    }

    public static string ResetPasswordSuccess(string data)
    {
        return ResetPasswordSuccessfully.Replace("%s", data);
    }
    public static string SubjectIsRequired(string data)
    {
        return REQUIRED_VALUE_MISSING.Replace("%s", data);
    }
    public static decimal percentageOf(decimal num, decimal per)
    {
        try
        {
            return (num / 100) * per;
        }
        catch (System.Exception)
        {

            throw;
        }
    }
}
