namespace System
{
    public class TimeParser
    {

        public bool IsValidTime;
        public string ErrorMessage;

        //public TimeSpan Time = new TimeSpan();
        public int hh;
        public int mm;
        public TimeSpan TimeSpanResult;
        public string OrigTimeStr;

        public TimeParser() { Clear(); }
        public void Clear()
        {
            IsValidTime = true;
            ErrorMessage = null;
            hh = 0;
            mm = 0;
        }
        //===================================================================================================
        public static void Test1()
        {
            var p = new TimeParser();

            p.TryParseTime("190");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("1:09");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("109");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("0109");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("1090");
            MOutput.WriteLine(p.ToStr());


            p.TryParseTime("19:0");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("1");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("23");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("25");
            MOutput.WriteLine(p.ToStr());

            p.TryParseTime("-2");
            MOutput.WriteLine(p.ToStr());
        }
        //===================================================================================================
        public string ToStr()
        {
            var buf = new StringBuilder();
            buf.Append(OrigTimeStr + ": ");

            if (!IsValidTime)
            {
                buf.AppendFormat("Error: {0}", ErrorMessage);
            }
            else
            {
                buf.AppendFormat("Parse OK,  HH={0}, MM={1}", hh, mm);
            }
            return buf.ToString();
        }
        //===================================================================================================   
        private bool ParseHHMM(string hhStr, string mmStr)
        {
            Clear();
            if (ParseHH(hhStr))
            {
                ParseMM(mmStr);
            }
            return IsValidTime;
        }
        //===================================================================================================   
        private bool ParseHH(string hhStr)
        {
            if (!Int32.TryParse(hhStr, out hh))
            {
                ErrorMessage = "Invalid value for hour: " + hhStr;
                IsValidTime = false;
            }
            if (hh < 0 || 23 < hh)
            {
                ErrorMessage = "Hour value out of range: " + hhStr;
                IsValidTime = false;
            }
            return IsValidTime;
        }
        //===================================================================================================   
        private bool ParseMM(string mmStr)
        {
            if (!Int32.TryParse(mmStr, out mm))
            {
                ErrorMessage = "Invalid value for minutes: " + mmStr;
                IsValidTime = false;
            }
            if (mm < 0 || 59 < mm)
            {
                ErrorMessage = "Minute value out of range: " + mmStr;
                IsValidTime = false;
            }
            return IsValidTime;
        }

        //===================================================================================================   
        public bool TryParseTime(string s)
        {
            var ret = TryParseTime(s, out TimeSpan ts, out ErrorMessage);
            TimeSpanResult = ts;
            hh = ts.Hours;
            mm = ts.Minutes;
            return ret;
        }
        //===================================================================================================   
        public bool TryParseTime2(string s)
        {
            OrigTimeStr = s;
            Clear();

            if (string.IsNullOrWhiteSpace(s))
            {
                ErrorMessage = "Value must not be null or blank";
                IsValidTime = false;
            }
            else
            {
                s = s.Trim();
                if (s.Contains(":"))
                {
                    var sArr = s.Split(':');
                    string hhStr = sArr[0];
                    string mmStr = sArr[1];

                    ParseHHMM(hhStr, mmStr);
                }
                else if (s.Length == 0)
                {
                    ErrorMessage = "Value must not be blank";
                    IsValidTime = false;
                }
                else if (s.Length > 4)
                {
                    ErrorMessage = "Value is too long: " + s;
                    IsValidTime = false;
                }
                else if (s.Length == 4)
                {
                    string hhStr = s.Substring(0, 2);
                    string mmStr = s.Substring(2);

                    ParseHHMM(hhStr, mmStr);
                }
                else if (s.Length == 3)
                {
                    string hhStr = s.Substring(0, 1); //140 = 1:40,  105 = 1:05
                    string mmStr = s.Substring(1);
                    ParseHHMM(hhStr, mmStr);
                    if (mm > 59)
                    {
                        hhStr = s.Substring(0, 2); //190 = 19:00
                        mmStr = s.Substring(2);
                        ParseHHMM(hhStr, mmStr);
                    }
                }
                else //len=1 or len=2
                {
                    string hhStr = s;
                    ParseHHMM(hhStr, "0");
                }

                if (IsValidTime)
                {
                    var ts = new TimeSpan(hh, mm, 0);
                }
                else
                {
                    if (ErrorMessage == null)
                    {
                        ErrorMessage = "Invalid Time: " + s;
                    }
                }
            }
            //MOutput.WriteLine(this.ToStr());
            return IsValidTime;
        }

        //===================================================================================================
        public static bool TryParseTime(string time, out TimeSpan ts, out string reason)
        {
            reason = "";
            ts = new TimeSpan();
            if (time == null)
            {
                reason = "arg null";
                return false;
                //time = "0000";
            }
            time = time.Trim();
            time = time.ToUpper();
            if (time == "NOW")
            {
                var now = DateTime.Now;
                ts = new TimeSpan(now.Hour, now.Minute, now.Second);
                return true;
            }

            bool isPM = false;
            if (time.EndsWith("AM"))
            {
                time = time.Substring(0, time.Length - 2);
            }
            else if (time.EndsWith("PM"))
            {
                time = time.Substring(0, time.Length - 2);
                isPM = true;
            }
            time = time.Trim();

            int mm = 0;
            string hhStr = null;
            string mmStr = null;
            int colonIdx = time.IndexOf(':');

            if (colonIdx >= 0)
            {
                hhStr = time.Substring(0, colonIdx);
                mmStr = time.Substring(colonIdx + 1);
                //if (TimeSpan.TryParse(time, out ts))
                //{
                //    MOutput.WriteLineFormat("TimeSpan parse {0} result: {1}", time, ts);
                //    return true;
                //}
            }
            else
            {
                if (time.Length == 0)
                {
                    reason = "time is blank";
                    return false;
                    //time = "0000"; 
                }
                else if (time.Length == 1)
                {
                    hhStr = time; //2 becomes 0200
                }
                else if (time.Length == 2)
                {
                    hhStr = time; //12 becomes 1200
                }
                else if (time.Length == 3)
                {
                    hhStr = time.Substring(0, 1);
                    mmStr = time.Substring(1);  //530 becomes 0530                    
                }
                else if (time.Length == 4)
                {
                    hhStr = time.Substring(0, 2);
                    mmStr = time.Substring(2);  //530 becomes 0530                    
                }
                else
                {
                    reason = "len GT 4";
                    return false;
                }
            }

            if (!Int32.TryParse(hhStr, out int hh))
            {
                reason = "hh is not an integer: " + hhStr;
                return false;
            }
            if (isPM)
            {
                if (hh < 12)
                {
                    hh += 12;
                }
            }
            if (hh < 0 || 23 < hh)
            {
                reason = "hh is out of range 0-23: " + hh;
                return false;
            }
            if (mmStr != null)
            {
                if (!Int32.TryParse(mmStr, out mm))
                {
                    reason = "mm is not an integer: " + mmStr;
                    return false;
                }
                if (mm < 0 || 59 < mm)
                {
                    reason = "mm is out of range 0-59: " + mm;
                    return false;
                }
            }
            ts = new TimeSpan(hh, mm, 0);
            return true;
        }
    }
}