using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnmanagedArray.Allocators
{
    public class Win32HeapAllocator : Win32Allocator
    {
        [DllImport(KernelLib)] private static extern IntPtr GetProcessHeap();
        [DllImport(KernelLib)] private static extern IntPtr HeapAlloc(IntPtr handle, uint dwFlags, Native dwBytes);
        [DllImport(KernelLib)] private static extern IntPtr HeapFree(IntPtr handle, uint dwFlags, IntPtr mem);

        public override IntPtr Allocate<T>(long count, bool zeroInit)
        {
            return HeapAlloc(GetProcessHeap(), zeroInit ? 0x8u : 0x0u, Unsafe.SizeOf<T>() * count);
        }
        
        public override void Free(IntPtr buffer)
        {
            HeapFree(GetProcessHeap(), 0x0, buffer);
        }
    }
}
