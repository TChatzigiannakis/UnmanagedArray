using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnmanagedArray
{
    public unsafe partial class Array<T>
    {
        private readonly static int Size = Unsafe.SizeOf<T>();
        private readonly static bool IsReferenceType = typeof(T).IsClass;
        private readonly static bool IsValueType = typeof(T).IsValueType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void* AddressOf(long index) => (byte*)Buffer + Size * index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T UnsafeGet(long index)
        {
            var addr = AddressOf(index);
            if (IsValueType)
            {
                return Unsafe.Read<T>(addr);
            }
            else
            {                
                var handle = Unsafe.Read<GCHandle>(addr);
                if (handle.IsAllocated)
                {
                    var obj = handle.Target;
                    return (T)obj;
                }
                else
                {
                    return default(T);
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnsafeSet(long index, T value)
        {
            var addr = AddressOf(index);
            if (IsValueType)
            {
                Unsafe.Write(addr, value);
            }
            else
            {
                var oldHandle = Unsafe.Read<GCHandle>(addr);
                if (oldHandle.IsAllocated)
                {
                    oldHandle.Free();
                }
                var newHandle = GCHandle.Alloc(value, GCHandleType.Pinned);
                Unsafe.Write(addr, newHandle);
            }
        }
    }
}
