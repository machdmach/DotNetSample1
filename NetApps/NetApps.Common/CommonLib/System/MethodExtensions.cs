namespace System
{
    public static class MethodExtensions
    {
        public static string ToFriendlyString(this Nullable<Boolean> b)
        {
            return b.HasValue ? (b.Value ? "Yes" : "No") : "";
        }
    }
}