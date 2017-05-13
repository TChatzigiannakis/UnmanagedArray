using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnmanagedArray.Allocators
{
    public unsafe class Win32GlobalAllocator : Win32Allocator
    {
        [DllImport(KernelLib)] private static extern IntPtr GlobalAlloc(uint dwFlags, Native dwBytes);
        [DllImport(KernelLib)] private static extern IntPtr GlobalFree(IntPtr ptr);
        
        public override IntPtr Allocate<T>(long count, bool zeroInit)
        {
            return GlobalAlloc(zeroInit ? 0x0040u : 0x0000u, Unsafe.SizeOf<T>() * count);
        }

        public override void Free(IntPtr buffer)
        {
            GlobalFree(buffer);
        }
    }
}
