using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cypress
{
    public static class Extensions
    {
        public static uint[] XOR(this uint[] arr1, uint[] arr2)
        {
            var result = new uint[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                result[i] = arr1[i] ^ arr2[i];
            }

            return result;
        }

        public static ulong[] XOR(this ulong[] arr1, ulong[] arr2)
        {
            var result = new ulong[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                result[i] = arr1[i] ^ arr2[i];
            }

            return result;
        }

        public static uint[] ADD(this uint[] arr1, uint[] arr2)
        {
            var result = new uint[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                unchecked
                {
                    result[i] = arr1[i] + arr2[i];
                }
            }

            return result;
        }
        
        public static ulong[] ADD(this ulong[] arr1, ulong[] arr2)
        {
            var result = new ulong[arr1.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                unchecked
                {
                    result[i] = arr1[i] + arr2[i];
                }
            }

            return result;
        }

        public static uint[] GetBytesFromText32(this string path)
        {
            var bytes = File.ReadAllBytes(path).ToList();
            while (bytes.Count % (sizeof(uint) * 8) != 0)
            {
                bytes.Add(0);
            }

            var paddedBytes = bytes.ToArray();

            var obtainedUints = new List<UInt32>();
            for (int i = 0; i < paddedBytes.Length / 4; i++)
            {
                var parsedNumber = BitConverter.ToUInt32(paddedBytes, i);
                obtainedUints.Add(parsedNumber);
            }

            return obtainedUints.ToArray();
        }

        public static ulong[] GetBytesFromText64(this string path)
        {
            var bytes = File.ReadAllBytes(path).ToList();
            while (bytes.Count % (sizeof(ulong) * 8) != 0)
            {
                bytes.Add(0);
            }

            var paddedBytes = bytes.ToArray();

            var obtainedUints = new List<ulong>();
            for (int i = 0; i < paddedBytes.Length / 8; i++)
            {
                var parsedNumber = BitConverter.ToUInt64(paddedBytes, i);
                obtainedUints.Add(parsedNumber);
            }

            return obtainedUints.ToArray();
        }

        public static void GetTextFromUintArr(this uint[] array, string path)
        {
            var obtainedBytes = array.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            string result = System.Text.Encoding.UTF8.GetString(obtainedBytes);
            File.WriteAllText(path, result);
        }

        public static void GetTextFromUlongArr(this ulong[] array, string path)
        {
            var obtainedBytes = array.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            string result = System.Text.Encoding.UTF8.GetString(obtainedBytes);
            File.WriteAllText(path, result);
        }
    }
}