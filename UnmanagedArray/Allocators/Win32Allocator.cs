using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnmanagedArray.Allocators
{
    public abstract class Win32Allocator : IAllocator
    {
        protected const string KernelLib = "kernel32.dll";

        [DllImport(KernelLib)] protected static extern void CopyMemory(IntPtr dest, IntPtr src, Native len);

        protected Win32Allocator() { }

        public abstract IntPtr Allocate<T>(long size, bool zeroInit);
        public abstract void Free(IntPtr buffer);

        public void Copy<T>(IntPtr target, IntPtr source, long count)
        {
            CopyMemory(target, source, count * Unsafe.SizeOf<T>());
        }
    }
}
