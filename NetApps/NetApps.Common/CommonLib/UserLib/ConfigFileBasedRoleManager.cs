namespace Libx;
public class ConfigFileBasedRoleManager
{
    public List<String> GetUserRoles(string UserName)
    {
        var ret = new List<string>();
        if (IsUserInRole(UserName, UserInfo.SuperuserRoleName))
        {
            ret.Add(UserInfo.SuperuserRoleName);
        }
        if (IsUserInRole(UserName, UserInfo.AdminUserRoleName))
        {
            ret.Add(UserInfo.AdminUserRoleName);
        }
        if (IsUserInRole(UserName, UserInfo.ReadonlyUserRoleName))
        {
            ret.Add(UserInfo.ReadonlyUserRoleName);
        }
        //if (IsSuperUser())
        //{
        //    ret.Add("superuser");
        //}
        //if (IsAdminUser())
        //{
        //    ret.Add("admin");
        //}
        //if (ret.Count == 0 && IsReadOlyUser())
        //{
        //    ret.Add("readonly");
        //}
        //if (ret.Count == 0)
        //{
        //    ret.Add("NoRolesFound");
        //}
        return ret;
    }
    //===================================================================================================
    private bool IsUserInRole(string UserName, string roleName)
    {
        //if (RunThis) return true;

        if (string.IsNullOrEmpty(UserName))
        {
            return false; //----------------------------
        }

        bool rval = false;
        string key;
        string users = AppSettingsX.GetString(key = "Common."+ roleName);
        //MOutput.WriteLine("AppSettingsX.GetString for key: " + key + " = " + users);

        string appUsers = AppSettingsX.GetString(key = "App." + roleName, null);
        //MOutput.WriteLine("AppSettingsX.GetString for key: " + key + " = " + appUsers);

        users += ", App:," + appUsers;

        if (users == null)
        {
            return false; //---------------------------------
        }
        users = users.Replace(" ", "");
        users = users.Trim();
        users = "," + users + ",";

        if (users.Contains(",*,"))
        {
            rval = true;
        }
        else
        {
            var userNameMod = "," + UserName + ",";
            if (users.IndexOf(userNameMod, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                rval = true;
            }
        }
        MOutput.WriteLineFormat("User: {0} is in role: {1}, result: {2}", UserName, roleName, rval);
        return rval;
    }
}
