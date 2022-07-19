using System.Runtime.Serialization;

namespace System;

[DataContract]
public class UserInfo
{
    ///<seealso cref="SecLibXzz"/>
    public const string ReadonlyUserRoleName = "readonlyUsers";
    public const string AdminUserRoleName = "adminUsers";
    public const string SuperuserRoleName = "superusers";

    public UserInfo() { } //for serialization
    public UserInfo(string username, string appName = null)
    {
        AppName = appName;
        UserName = username;
    }
    [DataMember]
    public string UserName { get; set; }
    [DataMember]
    public string AppName { get; set; }
    [DataMember]
    public int UserID { get; set; }

    public List<String> RoleList { get; set; }

    [DataMember]
    public string FullName { get; set; }

    [DataMember]
    public string JobTitle { get; set; }

    [DataMember]
    public string DepartmentName { get; set; }

    [DataMember]
    public string UserType { get; set; }
    [DataMember]
    public string UserSkin { get; set; } //Gets or sets the user interface theme the user is using.  

    //===================================================================================================
    public void SetIsAdminUser(bool flag) { _isAdminUser = flag; }
    private bool? _isAdminUser;
    public bool IsAdminUser()
    {
        if (!_isAdminUser.HasValue)
        {
            _isAdminUser = IsAdminUser(UserName);
        }
        return _isAdminUser.Value;
    }
    //===================================================================================================
    public bool IsAdminUser(string username)
    {
        bool rval = HasRole(AdminUserRoleName);
        return rval;
    }
    public bool IsAdminUserOrAbove()
    {
        return IsAdminUser() || IsSuperUser();
    }

    //===================================================================================================
    public bool IsSuperUser()
    {
        bool rval = HasRole(SuperuserRoleName);
        return rval;
    }

    //===================================================================================================
    public bool IsReadOlyUserOrAbove()
    {
        return IsReadOlyUser() || IsAdminUser() || IsSuperUser();
    }
    public bool IsReadOlyUser()
    {
        bool rval = HasRole(ReadonlyUserRoleName);
        return rval;
    }

    //===================================================================================================
    public bool HasRole(string roleName)
    {
        return RoleList.Contains(roleName);
    }

    //===================================================================================================
    protected UserInfo usr => this;
    public void CheckUserForSuperuserRole(string unauthorizedMesg = null)
    {
        if (!usr.IsSuperUser())
        {
            unauthorizedMesg ??= "you need to have Superuser role to access this area";
            string errMesg = string.Format("{0}, {1}.", usr.UserName, unauthorizedMesg);
            throw new UserException(errMesg);
        }
    }

    //===================================================================================================
    public void CheckUserForAdminOrAboveRole(string unauthorizedMesg = null)
    {
        if (!usr.IsAdminUserOrAbove())
        {
            unauthorizedMesg ??= "you need to have Admin role or above to access this area";
            string errMesg = string.Format("{0}, {1}.", usr.UserName, unauthorizedMesg);
            throw new UserException(errMesg);
        }
    }
    //===================================================================================================
    public void CheckUserForAnyRole(string unauthorizedMesg = null)
    {
        if (!usr.IsReadOlyUserOrAbove())
        {
            unauthorizedMesg ??= "you are not authorized to access this application";
            string errMesg = string.Format("{0}, {1}.", usr.UserName, unauthorizedMesg);
            throw new UserException(errMesg);
        }
    }

    //===================================================================================================
    public bool CheckAdminAccessByIP(string configKey = "admin.IPs")
    {
        //#todo:
        //MOutput.WriteLine("Checking Admin Access");

        //string ip = Request.QueryString["ip"];
        //if (ip == null)
        //{
        //    ip = reqData.UserHostAddress;
        //}

        //string adminIPs = AppSettingsX.GetString(configKey, true);
        //adminIPs = "," + adminIPs + ",";

        //bool accessOK = adminIPs.Contains("," + ip + ",");

        //if (!accessOK)
        //{
        //    // Request.Url.Host + ", " + Request.RawUrl + ", Unauthorized user: " + reqData.CurrentUsername
        //    //throw new UnauthorizedAccessException(Request.Url.Host + ", " + Request.RawUrl + ", Unauthorized user: " + reqData.CurrentUsername);
        //    throw new Exception(Request.Url.Host + ", " + Request.RawUrl + ", Unauthorized user IP: " + ip + ", on: " + DateTime.Now);
        //}
        //MOutput.WriteLine("Admin Access ok");
        return true;
    }
}
