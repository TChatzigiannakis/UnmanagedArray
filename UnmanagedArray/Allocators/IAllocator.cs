using System;
using System.Runtime.InteropServices;

namespace UnmanagedArray.Allocators
{
    /// <summary>
    /// Represents an object that can allocate, release, and copy memory.
    /// </summary>
    public interface IAllocator
    {
        IntPtr Allocate<T>(long size, bool zeroInit);
        void Free(IntPtr buffer);
        void Copy<T>(IntPtr destination, IntPtr source, long count);
    }

    internal static class Allocator
    {
        public static void Copy<T>(this IAllocator allocator, IntPtr target, T[] source)
        {
            var handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(source, 0);
            allocator.Copy<T>(target, ptr, source.Length);
            handle.Free();
        }

        public static void Copy<T>(this IAllocator allocator, T[] target, IntPtr source)
        {
            var handle = GCHandle.Alloc(target, GCHandleType.Pinned);
            var ptr = Marshal.UnsafeAddrOfPinnedArrayElement(target, 0);
            allocator.Copy<T>(ptr, source, target.Length);
            handle.Free();
        }
    }
}
