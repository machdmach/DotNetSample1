using System.Linq;
using System.Net;

namespace System
{
    public class SecLibXzz
    {
        public static string adminUsers = null;
        //===================================================================================================
        public static bool IsAdminUser(string currentUser)
        {
            if (adminUsers == null)
            {
                adminUsers = AppSettingsX.GetString("admin.users", false);
                //if (adminUsers == null)
                //{
                //    adminUsers = AppSettingsX.GetString(AppSettingsX.AppName+ ".admin.users", false);
                //}
                if (adminUsers == null)
                {
                    return false;
                }

                adminUsers = adminUsers.Trim();
                adminUsers = adminUsers.Replace(" ", "");

                adminUsers = "," + adminUsers.ToLower() + ",";
            }

            bool ret = false;
            if (adminUsers.Contains("*"))
            {
                ret = true;
            }
            else if (currentUser != null)
            {
                currentUser = "," + currentUser.ToLower() + ",";
                //if (adminUsers.IndexOf(currentUser, StringComparison.InvariantCultureIgnoreCase) >= 0)
                if (adminUsers.IndexOf(currentUser) >= 0)
                {
                    ret = true;
                }
            }
            return ret;
        }
        //===================================================================================================
        public static bool IsUserInWebConfig(string currentUser, string webConfigAppSettingKey)
        {
            bool rval = false;
            string users = "" + AppSettingsX.GetString(webConfigAppSettingKey);
            users = users.Trim();
            users = "," + users + ",";

            if (users.Contains(",*,"))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                currentUser = "," + currentUser + ",";
                if (users.IndexOf(currentUser, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    //throw new Exception("");
                    rval = true;
                }
            }
            MOutput.WriteLine("CurrentUser: " + currentUser + ", access=" + rval);
            return rval;
        }

        //===================================================================================================
        public static bool IsSuperUser(string currentUser)
        {
            bool rval = false;
            string users = AppSettingsX.GetString("superUsers", false);

            users = "," + users + ",";
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                currentUser = "," + currentUser + ",";
                if (users.IndexOf(currentUser, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    //throw new Exception("");
                    rval = true;
                }
            }
            return rval;
        }

        //===================================================================================================
        public static bool IsInRange(string startIpAddr, string endIpAddr, string address)
        {
            long ipStart = BitConverter.ToInt32(IPAddress.Parse(startIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ipEnd = BitConverter.ToInt32(IPAddress.Parse(endIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ip = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes().Reverse().ToArray(), 0);

            return ip >= ipStart && ip <= ipEnd; //edited
                                                 //Console.WriteLine(IsInRange("100.0.0.1", "110.0.0.255", "102.0.0.4"));//true
        }
    }
}
