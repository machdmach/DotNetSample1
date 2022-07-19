namespace System
{
    //https://docs.microsoft.com/en-us/dotnet/api/nunit.framework.assert?view=xamarin-ios-sdk-12
    //https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert?view=mstest-net-1.2.0

    public static class MAssert
    {
    }

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

        public static void IsTrue(bool flag, string mesg = null)
        {
            ThrowIfFalse(flag, flag, true);
        }
        public static void NotNullable(object obj, string mesg = null)
        {
            AssertX.IsTrue(obj != null, mesg);
        }

        public static void IsPositive(long val, string mesg = null)
        {
            ThrowIfFalse(val > 0, val, "GT zero");
        }
    }
}