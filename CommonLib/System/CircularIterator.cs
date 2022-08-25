using System.Collections;

namespace Libx
{
    public class CircularIterator<T>
    {
        public int lastIndex = -1;

        //Queue<T> queue; //= new Queue<String>(MaxSize);
        private readonly T[] values;
        public CircularIterator(params T[] values)
        {
            //this.MaxSize = maxSize;
            //queue = new Queue<T>(maxSize);
            this.values = values;
        }

        private string lastFlag = "unitx33";
        public T Next(string flag)
        {
            T ret;
            if (flag == lastFlag)
            {
                ret = values[lastIndex];
            }
            else
            {
                lastFlag = flag;
                lastIndex++;
                if (lastIndex >= values.Length)
                {
                    lastIndex = 0;
                }
                ret = values[lastIndex];
            }
            return ret;
        }
        //T Previous()
        //{
        //    T ret = values[curIndex];
        //    return ret;
        //}

        //===================================================================================================
        public string ToHtml()
        {
            string rval = IEnumerableX.ToHtml(values, "Values", "<pre>{0}</pre");
            return rval;
        }

    }
}
