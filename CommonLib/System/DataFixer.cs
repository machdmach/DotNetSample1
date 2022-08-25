namespace Libx
{
    public class DataFixer
    {
        //===================================================================================================
        public static bool? IsPositiveNullable(int? val, int? val2)
        {
            if (!val.HasValue && !val2.HasValue)
            {
                return null;
            }
            else if ((val.HasValue && val.Value > 0) ||
                     (val2.HasValue && val2.Value > 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //===================================================================================================
        public static bool? IsPositiveNullable(int? val)
        {
            if (val.HasValue)
            {
                if (val.Value > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return null;
            }
        }
        //===================================================================================================
        public static bool IsFalse(bool? val)
        {
            if (!val.HasValue)
            {
                return true;
            }
            else
            {
                return !val.Value;
            }
        }

    }
}
