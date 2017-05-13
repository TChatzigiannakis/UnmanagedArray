using System.Collections;
using System.Collections.Generic;

namespace UnmanagedArray
{
    public partial class Array<T> : IEnumerable<T>
    {
        /// <summary>
        /// Creates an enumerator for this array. The calling code is responsible for making sure that the array and its backing storage are not modified while the enumerator is still being used.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for(var i = 0; i < Length; i++)
            {
                yield return UnsafeGet(i);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
