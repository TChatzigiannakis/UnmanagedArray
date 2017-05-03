using System.Collections;
using System.Collections.Generic;

namespace UnmanagedArray
{
    public partial class Array<T> : IEnumerable<T>
    {
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
