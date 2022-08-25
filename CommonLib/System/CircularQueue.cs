using System.Collections;

namespace Libx
{
    public class CircularQueue<T> : IEnumerable<T>
    {
        public int MaxSize;
        private readonly Queue<T> queue; //= new Queue<String>(MaxSize);

        //private static readonly ICollection iCollection;
        //private static readonly IEnumerable iEnumerable;
        //private static readonly ICloneable iCloneable;

        public CircularQueue(int maxSize)
        {
            MaxSize = maxSize;
            queue = new Queue<T>(maxSize);
        }
        //===================================================================================================
        public void Enqueue(T obj)
        {
            if (queue.Count >= MaxSize)
            {
                queue.Dequeue();
            }
            //System.ArgumentException: Source array was not long enough. Check srcIndex and length, and the array's lower bounds.
            //at System.Array.Copy(Array sourceArray, Int32 sourceIndex, Array destinationArray, Int32 destinationIndex, Int32 length, Boolean reliable)
            //at System.Collections.Generic.Queue`1.SetCapacity(Int32 capacity)
            //at System.Collections.Generic.Queue`1.Enqueue(T item)
            queue.Enqueue(obj);
        }
        //===================================================================================================
        private readonly object enqueueLock = new object();
        public void EnqueueThreadSafe(T obj) //TS/NTS NonThreadSafe
        {
            lock (enqueueLock)
            {
                if (queue.Count >= MaxSize)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(obj);
            }
        }

        //===================================================================================================
        public void Clear()
        {
            queue.Clear();
        }
        //===================================================================================================
        //public string ToHtml(ToHtmlOptions options)
        //{
        //    //T[] arr = queue.ToArray();            
        //    string rval = queue.ToHtml(options);
        //    return rval;
        //}

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)queue).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)queue).GetEnumerator();
        }
    }
}
