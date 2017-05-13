using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public abstract class Common
    {
        protected const long KB = 1024;
        protected const long MB = 1024 * KB;
        protected const long GB = 1024 * MB;

        protected void Require64()
        {
            if (!Environment.Is64BitProcess)
            {
                Assert.Inconclusive();
            }
        }
    }
}
