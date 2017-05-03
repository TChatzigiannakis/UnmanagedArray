using System.Collections.Generic;
using System.Linq;

namespace UnmanagedArray
{
    public static unsafe class Utilities
    {
        public static byte* ToPointer(this Array<byte> source) => (byte*)source.Buffer;
        public static short* ToPointer(this Array<short> source) => (short*)source.Buffer;
        public static int* ToPointer(this Array<int> source) => (int*)source.Buffer;
        public static long* ToPointer(this Array<long> source) => (long*)source.Buffer;
        public static float* ToPointer(this Array<float> source) => (float*)source.Buffer;
        public static double* ToPointer(this Array<double> source) => (double*)source.Buffer;
        public static decimal* ToPointer(this Array<decimal> source) => (decimal*)source.Buffer;
        public static void* ToPointer<T>(this Array<T> source) where T : struct => (void*)source.Buffer;

        public static Array<T> ToEnormousArray<T>(this IEnumerable<T> source)
            where T : struct
        {
            Array<T> target;
            if (source is IList<T> sourceAsList)
            {
                target = new Array<T>(sourceAsList.LongCount());
            }
            else
            {
                target = new Array<T>(4);
            }

            var i = 0;
            foreach(var e in source)
            {
                if(i == target.Length)
                {
                    target.Resize(2 * target.Length);
                }
                target[i] = e;
                i++;
            }

            target.Resize(i);

            return target;
        }
    }
}
