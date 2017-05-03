using System;
using System.Runtime.InteropServices;

namespace UnmanagedArray.Allocators
{
    public class Win32GlobalAllocator : IAllocator
    {
        const string KernelLib = "kernel32.dll";

        [DllImport(KernelLib)] private static extern IntPtr GlobalAlloc(uint dwFlags, Native dwBytes);
        [DllImport(KernelLib)] private static extern IntPtr GlobalFree(IntPtr ptr);
        [DllImport(KernelLib)] private static extern void CopyMemory(IntPtr dest, IntPtr src, Native len);

        public IntPtr Allocate<T>(long count, bool zeroInit)
        {
            var ptr = GlobalAlloc(zeroInit ? 0x0040u : 0x0000u, Marshal.SizeOf<T>() * count);
            if(ptr == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }
            return ptr;
        }

        public void Free(IntPtr buffer)
        {
            GlobalFree(buffer);
        }

        public void Copy<T>(IntPtr target, IntPtr source, long count)
        {
            CopyMemory(target, source, count * Marshal.SizeOf<T>());
        }
    }
}
