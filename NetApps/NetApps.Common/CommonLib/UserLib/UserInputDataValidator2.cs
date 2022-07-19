using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace System
{
    public class UserDataValidator2
    {
        public static Guid ValidateGuid(string guid)
        {
            try
            {
                var ret = new Guid(guid); //.ToString();
                return ret;
            }
            catch (Exception ex)
            {
                //System.FormatException: Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxx...).
                throw new UserInputDataException("Bad Guid: " + guid + ", " + ex.Message);
            }
        }

        //DataValidator d;

        public StringBuilder buf = new StringBuilder();
        private readonly string requiredFormat = "<strong>{0}</strong> is required.<br>\n";
        private readonly string errorFormat2 = "<strong>{0}</strong> {1}.<br>\n";
        //string errorFormat2

        //===================================================================================================
        public String CheckFieldLength(string val, string fieldLabel, int min = 0, int max = 100)
        {
            int len = 0;
            if (val != null)
            {
                len = val.Length;
            }
            string err = null;
            if (len < min)
            {
                err = string.Format(errorFormat2, fieldLabel, "must be longer than " + min + " characters");
            }
            if (len > min)
            {
                err = string.Format(errorFormat2, fieldLabel, "must be less than " + min + " characters");
            }
            buf.Append(err);
            return err;
        }


        //===================================================================================================
        public void CheckRequiredField(string val, string fieldLabel)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                buf.AppendFormat(requiredFormat, fieldLabel);
            }
        }

        //===================================================================================================
        public void CheckRange(int? val, int min, int max, string fieldLabel)
        {
            if (val.HasValue)
            {
                if (val.Value < min || max < val.Value)
                {
                    buf.AppendFormat(errorFormat2, fieldLabel, "must be between " + min + " and " + max);
                }
            }
        }
        //===================================================================================================
        public void CheckRange(decimal val, decimal min, decimal max, string fieldLabel)
        {
            if (val < min || max < val)
            {
                buf.AppendFormat(errorFormat2, fieldLabel, "must be between " + min + " and " + max);
            }
        }

        //===================================================================================================
        public void CheckRange(DateTime? val, DateTime min, DateTime max, string fieldLabel)
        {
            if (val.HasValue)
            {
                if (val.Value < min || max < val.Value)
                {
                    buf.AppendFormat(errorFormat2, fieldLabel, "must be between " + min + " and " + max);
                }
            }
        }
        //===================================================================================================
        public void CheckFutureDateTime(DateTime? val, DateTime max, string fieldLabel)
        {
            if (val.HasValue)
            {
                var dt = DateTime.Now;
                DateTime min = new DateTime(dt.Year, dt.Month, dt.Day);

                if (val.Value < min)
                {
                    buf.AppendFormat(errorFormat2, fieldLabel, "must be a future date");
                }
                else if (max < val.Value)
                {
                    buf.AppendFormat(errorFormat2, fieldLabel, "must be earlier than " + max.ToShortDateString());
                }
            }
        }

        //===================================================================================================
        public string CheckFieldsSameValue(string email, string email2, string fieldLabel)
        {
            string err = null;
            string emailLower = (email + "").ToLower();
            string email2Lower = (email2 + "").ToLower();

            if (emailLower != email2Lower)
            {
                string mesg1 = string.Format("\"{0}\" and \"{1}\" do not match", email, email2);
                err = string.Format(errorFormat2, fieldLabel, mesg1);
            }
            buf.Append(err);
            return err;
        }

        //===================================================================================================
        public string CheckEmailAddress(string email, string fieldLabel)
        {
            string err = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                bool emailOK = false;
                try
                {
                    var mailAddress = new System.Net.Mail.MailAddress(email);
                    emailOK = (mailAddress.Address == email);
                }
                catch { }

                if (!emailOK)
                {
                    err = string.Format(errorFormat2, fieldLabel, email + " is not an valid email address");
                }

                var validator = new EmailAddressAttribute();
                if (!validator.IsValid(email))
                {
                    err = string.Format(errorFormat2, fieldLabel, email + " is not an valid email address");
                }
            }
            buf.Append(err);
            return err;
        }

        //===================================================================================================

        private static readonly Regex ValidEmailRegex = CreateValidEmailRegex();
        /// <summary>
        /// Taken from http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx
        /// </summary>
        /// <returns></returns>
        private static Regex CreateValidEmailRegex()
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
        }

        public bool EmailIsValid(string emailAddress)
        {
            bool isValid = ValidEmailRegex.IsMatch(emailAddress);

            return isValid;
        }
        //===================================================================================================
        public static bool IsASCII(char c)
        {
            return ((c & (~0x7f)) == 0);
        }
        //===================================================================================================
        public void CheckComments(string val, string label)
        {
            for (int i = 0; i < val.Length; i++)
            {
                var c = val[i];
                if (Char.IsLetterOrDigit(c) ||
                    Char.IsPunctuation(c) ||
                    c == ' ' || //space
                    c == '-' ||
                    c == '\'' ||
                    c == '(' ||
                    c == ')' ||
                    c == '/' ||
                    c == '.')
                {
                    //ok
                }
                else
                {
                    reportError(val, label, c, i);
                }
            }
        }

        //===================================================================================================
        public void CheckPersonName(string val, string label)
        {
            for (int i = 0; i < val.Length; i++)
            {
                var c = val[i];
                if (Char.IsLetterOrDigit(c) ||
                    c == ' ' || //space
                    c == '-' ||
                    c == '\'' ||
                    c == '(' ||
                    c == ')' ||
                    c == '/' ||
                    c == '.')
                {
                    //ok
                }
                else
                {
                    reportError(val, label, c, i);
                }
            }
        }

        //===================================================================================================
        private void reportError(string val, string label, char badChar, int badCharIndex)
        {
            string err;

            if (Char.IsLetterOrDigit(badChar) || Char.IsPunctuation(badChar))
            {
                err = string.Format(label + ", Invalid Character: {0}, at position: {2}", badChar, badCharIndex);
            }
            else
            {
                err = string.Format(label + ", Invalid Character hex: {0}, at position: {1}", badChar, badCharIndex);
            }

            buf.Append(err);
        }
    }
}