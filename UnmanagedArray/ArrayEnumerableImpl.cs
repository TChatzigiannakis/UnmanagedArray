using System.Collections;
using System.Collections.Generic;

namespace UnmanagedArray
{
    public partial class Array<T> : IEnumerable<T>
    {
        /// <summary>
        /// Creates an enumerator for this array. You must make sure that the array and its backing storage are not modified in any way while an enumerator is still being used.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0L; i < Length; i++)
            {
                yield return this[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
