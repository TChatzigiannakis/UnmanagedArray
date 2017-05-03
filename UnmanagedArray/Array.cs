using UnmanagedArray.Allocators;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnmanagedArray
{
    [DebuggerDisplay("Length = {Length}")]
    public unsafe partial class Array<T> : IDisposable
        where T : struct
    {
        private static bool IsZeroBit(T element) => Equals(element, default(T));

        /// <summary>
        /// Gets the total number of elements in the array.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[long index]
        {
            get => UnsafeGet(EnsureInRange(index));
            set
            {
                UnsafeSet(EnsureInRange(index), value);
            }
        }

        internal IntPtr Buffer;

        private IAllocator Allocator = new Win32GlobalAllocator();

        /// <summary>
        /// Creates an array with the specified size initialized to the default value of the element type.
        /// </summary>
        /// <param name="count"></param>
        public Array(long count)
            : this(count, true)
        {
        }

        /// <summary>
        /// Creates an array with the specified size initialized to the default value of the element type, using the provided allocator.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="allocator"></param>
        public Array(long count, IAllocator allocator)
        {
            if (count < 0) throw new ArgumentException(nameof(count));
            Allocator = allocator ?? throw new ArgumentNullException(nameof(allocator));
            Length = count;
            Buffer = Allocator.Allocate<T>(count, true);
        }

        /// <summary>
        /// Creates an array with the specified size initialized using the specified generator function.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="initializer"></param>
        public Array(long count, Func<long, T> initializer)
            : this(count, false)
        {
            if (initializer == null) throw new ArgumentNullException(nameof(initializer));
            for(var i = 0L; i < Length; i++)
            {
                UnsafeSet(i, initializer(i));
            }
        }

        /// <summary>
        /// Creates an array with the specified size initialized using the specified generator function.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="initializer"></param>
        public Array(long count, Func<T> initializer)
            : this(count, x => initializer())
        {
        }

        /// <summary>
        /// Creates an array with the specified size initialized using the specified value.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="initialValue"></param>
        public Array(long count, T initialValue)
            : this(count, IsZeroBit(initialValue))
        {
            if (!IsZeroBit(initialValue))
            {
                for (var i = 0L; i < Length; i++)
                {
                    UnsafeSet(i, initialValue);
                }
            }
        }

        /// <summary>
        /// Creates an array with the specified size with the option to initialize using the default value or leave the initial values unspecified.
        /// </summary>
        public Array(long count, bool clear)
        {
            if (count < 0) throw new ArgumentException(nameof(count));
            Length = count;
            Buffer = Allocator.Allocate<T>(count, clear);
        }

        private IntPtr AddressOf(long index) => new IntPtr((byte*)Buffer + Marshal.SizeOf<T>() * index);

        private T UnsafeGet(long index) => (T)Marshal.PtrToStructure(AddressOf(index), typeof(T));

        private void UnsafeSet(long index, T value)
        {
            Marshal.StructureToPtr(value, AddressOf(index), false);
        }
        
        private long EnsureInRange(long index)
        {
            if (0 <= index && index < Length)
            {
                return index;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Frees the backing memory used by the array and sets the size to zero.
        /// </summary>
        public void Dispose()
        {
            Allocator.Free(Buffer);
            Buffer = default(IntPtr);
            Length = default(long);
        }
    }
}
