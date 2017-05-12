﻿using System.Runtime.CompilerServices;

namespace UnmanagedArray
{
    public unsafe partial class Array<T>
    {
        private readonly static int Size = Unsafe.SizeOf<T>();

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
