using System;
using System.Runtime.CompilerServices;

namespace UnmanagedArray
{
    public unsafe partial class Array<T>
    {
        private readonly static int Size = Unsafe.SizeOf<T>();

        private void* RequireNotNull(void* ptr) => ptr == (void*)0 ? throw new OutOfMemoryException() : ptr;
        private IntPtr RequireNotNull(IntPtr ptr) => ptr == IntPtr.Zero ? throw new OutOfMemoryException() : ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* AddressOf(long index) => (byte*)Buffer + Size * index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T UnsafeGet(long index) => Unsafe.Read<T>(AddressOf(index));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsafeSet(long index, T value)
        {
            Unsafe.Write(AddressOf(index), value);
        }
    }
}
