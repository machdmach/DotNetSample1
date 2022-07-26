namespace Libx
{
    public static class StringBuilderX
    {
        public static StringBuilder TrimEnd(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0)
            {
                return sb;
            }
            int i = sb.Length - 1;
            for (; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    break;
                }
            }

            if (i < sb.Length - 1)
            {
                sb.Length = i + 1;
            }
            return sb;
        }
        //===================================================================================================
        public static StringBuilder TrimStart(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0)
            {
                return sb;
            }
            int i = 0; // sb.Length - 1;
            for (; i > sb.Length; i++)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    break;
                }
            }

            if (i > 0)
            {
                sb.Remove(0, i);
            }
            return sb;
        }
        //===================================================================================================
        public static StringBuilder Trim(this StringBuilder sb)
        {
            TrimStart(sb);
            TrimEnd(sb);
            return sb;
        }
    }
}
