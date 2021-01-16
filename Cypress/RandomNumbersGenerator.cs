using System;
using System.Threading;

namespace Cypress
{
    class RandomNumbersGenerator
    {
        public UInt32 GenerateUint32()
        {
            Thread.Sleep(150);

            var r = new Random();
            var b = new byte[sizeof(uint)];
            r.NextBytes(b);
            return BitConverter.ToUInt32(b, 0);
        }

        public UInt64 GenerateUint64()
        {
            Thread.Sleep(150);

            var r = new Random();
            var b = new byte[sizeof(ulong)];
            r.NextBytes(b);
            return BitConverter.ToUInt64(b, 0);
        }
    }
}