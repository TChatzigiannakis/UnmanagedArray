using UnmanagedArray.Allocators;
using System;

namespace UnmanagedArray
{
    public unsafe partial class Array<T>
    {
        public static implicit operator IntPtr(Array<T> e) => e.Buffer;

        /// <summary>
        /// Resizes the array to the specified length. 
        /// </summary>
        /// <param name="newLength"></param>
        public void Resize(long newLength)
        {
            if (newLength < 0) throw new ArgumentException(nameof(newLength));

            var oldBuffer = Buffer;
            var oldLength = Length;

            if(oldLength == newLength)
            {
                return;
            }

            Buffer = default(IntPtr);
            Length = default(long);

            var newBuffer = Allocator.Allocate<T>(newLength, true);
            Allocator.Copy<T>(newBuffer, oldBuffer, oldLength);

            Buffer = newBuffer;
            Length = newLength;
        }

        /// <summary>
        /// Creates a copy of this array.
        /// </summary>
        /// <returns></returns>
        public Array<T> Copy()
        {
            var e = new Array<T>(Length);
            Allocator.Copy<T>(e, this, Length);
            return e;
        }

        /// <summary>
        /// Copies elements from a managed C# array to this one.
        /// </summary>
        /// <param name="source"></param>
        public void CopyFrom(T[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            Allocator.Copy(this, source);
        }

        /// <summary>
        /// Copies elements from this array to a managed C# array.
        /// </summary>
        /// <param name="target"></param>
        public void CopyTo(T[] target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var len = checked((int)Math.Min(Length, target.Length));
            Allocator.Copy(target, this);
        }        
    }
}
