namespace System
{
    public class TimedCounter
    {
        public string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiredDT { get; set; }
        public TimeSpan Duration { get; set; }
        public int MaxValue { get; set; }
        public int CurrentCount { get; set; }
        public int TotalThrowCount { get; set; } = 0;

        //===================================================================================================
        public TimedCounter(string name, int startVal, TimeSpan duration)
        {
            Name = name;
            CreatedOn = DateTime.Now;
            MaxValue = startVal;
            Duration = duration;
            Reset();
        }

        public string ToHtml(string mesg = null)
        {
            var timeLeft = ExpiredDT - DateTime.Now;
            string ret = $"{Name} CurrenCount/MaxValue: ({CurrentCount}/{MaxValue}). Duration/timeLeft: ({Duration}/{timeLeft}";
            if (mesg != null)
            {
                ret = mesg + "<br>\n" + ret;
            }
            return ret;
        }

        //===================================================================================================
        public void Reset()
        {
            CurrentCount = 1;
            ExpiredDT = DateTime.Now.Add(Duration);
        }
        //===================================================================================================
        public void Increment()
        {
            if (ExpiredDT < DateTime.Now) //is in the past
            {
                Reset();
            }
            CurrentCount++;
        }

        //===================================================================================================
        public bool Done
        {
            get
            {
                if (ExpiredDT < DateTime.Now) //is in the past
                {
                    Reset();
                }
                var ret = (CurrentCount > MaxValue);
                //if (ret) Decrement();
                return ret;
            }
        }
        //===================================================================================================
        //public bool Done__postDecrementzz()
        //{
        //    var ret = Done;
        //    if (ret) Decrement();
        //    return ret;
        //}
        //===================================================================================================
        public void ThrowDoS()
        {
            TotalThrowCount++;
            throw new DosException("The system is currently busy. Please try again later.");
        }

        //===================================================================================================
        public void CheckDoS__postIncrement()
        {
            MOutput.WriteLine(JsonHValue.Of(this));

            bool throwDoS = false;
            if (Done)
            {
                throwDoS = true;
            }
            Increment();
            if (throwDoS) ThrowDoS();
        }

    }
}
