namespace Libx
{
    public static class AssertX
    {
        public static void AreEqual()
        {

        }
        public static void ThrowIfFalse(bool flag, object actual, object expected)
        {
            if (!flag)
            {
                throw new Exception(string.Format("Actual={0}, Expected={1}", actual, expected));
            }
        }

        public static void True(bool flag, string mesg = null)
        {
            if (!flag)
            {
                if (mesg == null)
                {
                    mesg = "not true";
                }
                //throw new Exception(string.Format("Actual={0}, Expected={1}", actual, expected));
                throw new Exception(mesg);
            }
        }
        public static void NotNull(object obj, string mesg = "expected not null")
        {
            AssertX.True(obj != null, mesg);
        }

        public static void IsPositive(long val, string mesg = null)
        {
            ThrowIfFalse(val > 0, val, "GT zero");
        }
    }
}