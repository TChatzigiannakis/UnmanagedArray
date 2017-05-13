using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnmanagedArray;

namespace Tests
{
    [TestFixture]
    public class Performance : Common
    {
        [Test]
        public void Value([Values(16 * MB, 32 * MB, 64 * MB, 128 * MB, 256 * MB)] long size)
        {
            Require64();

            var sw = new Stopwatch();
            (var managedWriteTime, var managed) = sw.Measure(() =>
            {
                var arr = new int[size];
                for (var i = 0; i < arr.Length; i++)
                {
                    arr[i] = 1;
                }
                return arr;
            });
            (var managedReadTime, var managedSum) = sw.Measure(() => managed.Sum());
            (var unmanagedWriteTime, var unmanaged) = sw.Measure(() => new Array<int>(size, 1));
            (var unmanagedReadTime, var unmanagedSum) = sw.Measure(() => unmanaged.Sum());

            var writeRatio = unmanagedWriteTime / (decimal)managedWriteTime;
            Console.WriteLine($"Managed array write time: {managedWriteTime}ms");
            Console.WriteLine($"Unmanaged array write time: {unmanagedWriteTime}ms");
            Console.WriteLine($"Ratio: {writeRatio.ToString("0.##")}");

            var readRatio = unmanagedReadTime / (decimal)managedReadTime;
            Console.WriteLine($"Managed array read time: {managedReadTime}ms");
            Console.WriteLine($"Unmanaged array read time: {unmanagedReadTime}ms");
            Console.WriteLine($"Ratio: {readRatio.ToString("0.##")}");

            Assert.AreEqual(managedSum, unmanagedSum);
        }

        [Test]
        public void Reference([Values(16 * MB, 32 * MB, 64 * MB, 128 * MB, 256 * MB)] long size)
        {
            Require64();

            var sw = new Stopwatch();
            (var managedWriteTime, var managed) = sw.Measure(() =>
            {
                var arr = new string[size];
                for (var i = 0; i < arr.Length; i++)
                {
                    arr[i] = 1.ToString();
                }
                return arr;
            });
            (var managedReadTime, var managedSum) = sw.Measure(() => managed.Select(int.Parse).Sum());
            (var unmanagedWriteTime, var unmanaged) = sw.Measure(() => new Array<string>(size, 1.ToString()));
            (var unmanagedReadTime, var unmanagedSum) = sw.Measure(() => unmanaged.Select(int.Parse).Sum());

            var writeRatio = unmanagedWriteTime / (decimal)managedWriteTime;
            Console.WriteLine($"Managed array write time: {managedWriteTime}ms");
            Console.WriteLine($"Unmanaged array write time: {unmanagedWriteTime}ms");
            Console.WriteLine($"Ratio: {writeRatio.ToString("0.##")}");

            var readRatio = unmanagedReadTime / (decimal)managedReadTime;
            Console.WriteLine($"Managed array read time: {managedReadTime}ms");
            Console.WriteLine($"Unmanaged array read time: {unmanagedReadTime}ms");
            Console.WriteLine($"Ratio: {readRatio.ToString("0.##")}");

            Assert.AreEqual(managedSum, unmanagedSum);
        }
    }
}
