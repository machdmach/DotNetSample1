using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace System
{
    public class EmailAddressLib
    {
        //===================================================================================================
        private static readonly Regex ValidEmailRegex = CreateValidEmailRegex();
        private static Regex CreateValidEmailRegex()
        {
            //http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }
        //#validateEmail
        public static bool IsValidEmail(string emailAddress)
        {
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);
            return isValid;
        }

        //===================================================================================================
        public static bool ValidateEmailAddress(string email, bool throwsOnInvalid = true)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                if (throwsOnInvalid)
                {
                    throw new Exception(email + " is not a valid email address");
                }
                return false;
            }
        }

        //===================================================================================================
        public record ExtractOptions(int min, int max, bool throwOnMaxExceeded, bool ignoreDuplicates)
        {
            public ExtractOptions() : this(0, 100, false, true) { }
        }

        public static string ExtractExactlyOneEmailAddress(string s, ExtractOptions options = null)
        {
            options ??= new() { min = 1, max = 1, throwOnMaxExceeded = true };
            return ExtractEmailAddresses(s, options).Single();
        }
        public static List<string> ExtractEmailAddresses(string s, ExtractOptions options = null)
        {
            options ??= new();
            MOutput.WriteLine(options.ToString());

            var ret = new List<string>();

            int k = 0;
            foreach (string emailx in s.Split())
            {
                var email = emailx.Trim();
                if (!email.Contains('@'))
                {
                    continue;
                }

                //email is not valid: <derek.germann@mainelectricsupply.com>:
                int iLT = email.IndexOf('<');
                if (iLT >=0)
                {
                    email = email.Substring(iLT + 1);
                }
                int iGT = email.IndexOf('>');
                if (iGT >= 0)
                {
                    email = email.Substring(0, iGT);
                }
                //email = email.Substring(email.Length); //OK, not exceeded length

                if (!EmailAddressLib.IsValidEmail(email))
                {
                    MOutput.WriteHtmlEncodedPre("email is not valid: " + email);
                    continue;
                }
                if (ret.Exists(e => e.ToLower() == email.ToLower()))
                {
                    continue;
                    //ignore
                }
                //err = string.Format(errorFormat2, fieldLabel, email + " is not an valid email address");

                MOutput.WriteHtmlEncodedPre("email: " + email);

                //MOutput.Write("email2: " + email);

                if (++k > options.max)
                {
                    if (options.throwOnMaxExceeded)
                    {
                        throw new Exception($"Too many emails found, exceeded Max: {options.max}, " + String.Join(", ", ret) + "/ " + email);
                    }
                    break;
                }
                ret.Add(email);
            }
            if (ret.Count < options.min)
            {
                if (options.min == 1)
                {
                    throw new Exception("No email address found");
                }
                else
                {
                    throw new Exception($"Min email address count expected is {options.min}, but actual is {ret.Count}.  found=" + String.Join(", ", ret));
                }
            }
            return ret;
        }

    }
}
