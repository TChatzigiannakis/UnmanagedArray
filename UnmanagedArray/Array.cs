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
        public long Length { get; private set; }
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

        public Array(long count)
            : this(count, true)
        {
        }

        public Array(long count, IAllocator allocator)
        {
            Allocator = allocator;
            Length = count;
            Buffer = Allocator.Allocate<T>(count, true);
        }

        public Array(long count, Func<long, T> initializer)
            : this(count, false)
        {
            for(var i = 0L; i < Length; i++)
            {
                UnsafeSet(i, initializer(i));
            }
        }

        public Array(long count, Func<T> initializer)
            : this(count, x => initializer())
        {
        }

        public Array(long count, T initialValue)
            : this(count, false)
        {
            for (var i = 0L; i < Length; i++)
            {
                UnsafeSet(i, initialValue);
            }
        }

        public Array(long count, bool clear)
        {
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

        public void Dispose()
        {
            Allocator.Free(Buffer);
            Buffer = default(IntPtr);
            Length = default(long);
        }
    }
}
