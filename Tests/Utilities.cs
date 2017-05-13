using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal static class Utilities
    {
        public static (long time, T result) Measure<T>(this Stopwatch stopwatch, Func<T> act)
        {
            stopwatch.Restart();
            var result = act();
            stopwatch.Stop();
            return (stopwatch.ElapsedMilliseconds, result);
        }
    }
}
