using NUnit.Framework;
using UnmanagedArray;
using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualBasic.Devices;

namespace Tests
{
    [TestFixture]
    public class Main
    {
        private const long KB = 1024;
        private const long MB = 1024 * KB;
        private const long GB = 1024 * MB;

        private void Require64()
        {
            if (!Environment.Is64BitProcess)
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void ZeroInit()
        {
            using (var arr = new Array<int>(1000))
            {
                Assert.IsTrue(arr.All(x => x == 0));
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(4, true)]
        [TestCase(8, true)]
        [TestCase(16, true)]
        public void VeryLargeAllocation(long gigaBytes, bool optional = false)
        {
            Require64();
            try
            {
                using (var arr = new Array<byte>(gigaBytes * GB))
                {
                    arr[0] = 255;
                    arr[arr.Length - 1] = 255;
                }
            }
            catch (OutOfMemoryException)
            {
                if(optional)
                {
                    Assert.Inconclusive();
                }
                else
                {
                    throw;
                }
            }
        }

        [Test]
        public void SetAndGet()
        {
            Func<long, long> f = i => i * i;
            using (var arr = new Array<long>(5, f))
            {
                for (var i = 0L; i < arr.Length; i++)
                {
                    Assert.AreEqual(f(i), arr[i]);
                }
            }
        }

        [Test]
        public void Enumerable()
        {
            using (var arr = new Array<int>(5, 15))
            {
                foreach (var e in arr)
                {
                    Assert.AreEqual(e, 15);
                }
            }
        }

        [Test]
        public void Linq()
        {
            using(var arr = new Array<long>(13, i => i * i))
            {
                Assert.AreEqual(0, arr.First());
                Assert.AreEqual(1, arr.Skip(1).First());
                Assert.AreEqual(144, arr.Last());
                Assert.AreEqual(144, arr.Reverse().First());
            }
        }

        [Test]
        [TestCase(5, 10)]
        [TestCase(4096, 8192)]
        [TestCase(1024 * 1024, 2L * 1024 * 1024 * 1024, false)]
        public void Resize(long initialLength, long newLength, bool verifyBeyondOldBoundary = true)
        {
            Require64();

            try
            {
                Func<long, long> f = i => i * i;
                using (var arr = new Array<long>(initialLength, f))
                {
                    Assert.AreEqual(initialLength, arr.Length);

                    arr.Resize(newLength);
                    Assert.AreEqual(newLength, arr.Length);

                    var end = verifyBeyondOldBoundary ? newLength : initialLength;
                    for (var i = 0L; i < end; i++)
                    {
                        if (i < initialLength)
                        {
                            Assert.AreEqual(f(i), arr[i]);
                        }
                        else
                        {
                            Assert.AreEqual(0, arr[i]);
                        }
                    }
                }
            }
            catch (OutOfMemoryException)
            {
                Assert.Inconclusive();
            }
        }

        [Test]
        public void BreakEnumerator()
        {
            for(var i = 0; i < 100000; i++)
            {
                using (var arr = new Array<long>(5, 1))
                {
                    var sum = arr.Sum(x =>
                    {
                        arr.Resize(arr.Length - 1);
                        return x;
                    });
                    Assert.AreEqual(3, sum);
                }
            }
        }

        [Test]
        public void Performance([Values(16 * MB, 32 * MB, 64 * MB, 128 * MB, 256 * MB)] long size)
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
            unmanaged.Dispose();

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
