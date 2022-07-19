using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace System
{
    public class UserDataValidator
    {
        //DataValidator d;

        public StringBuilder buf = new StringBuilder();
        private readonly string requiredFormat = "<strong>{0}</strong> is required.<br>\n";
        private readonly string errorFormat2 = "<strong>{0}</strong> {1}.<br>\n";
        //string errorFormat2

        //===================================================================================================
        public String ValidateFieldLength(string val, string fieldLabel, int min = 0, int max = 100)
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
        public void ValidateRequiredField(string val, string fieldLabel)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                buf.AppendFormat(requiredFormat, fieldLabel);
            }
        }

        //===================================================================================================
        public void ValidateRange(int? val, int min, int max, string fieldLabel)
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
        public void ValidateRange(decimal val, decimal min, decimal max, string fieldLabel)
        {
            if (val < min || max < val)
            {
                buf.AppendFormat(errorFormat2, fieldLabel, "must be between " + min + " and " + max);
            }
        }

        //===================================================================================================
        public void ValidateRange(DateTime? val, DateTime min, DateTime max, string fieldLabel)
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
        public void ValidateFutureDateTime(DateTime? val, DateTime max, string fieldLabel)
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
        public string ValidateFieldsSameValue(string email, string email2, string fieldLabel)
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
        public string ValidateEmailAddress(string email, string fieldLabel)
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

    }
}