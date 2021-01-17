using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;


namespace Cypress
{
    class Program
    {
        static string textPath = @"C:\Users\junge\source\repos\Cypress\Cypress\1984_en.txt";
        static string encryptedPath = @"C:\Users\junge\source\repos\Cypress\Cypress\Result.txt";

        static void Main(string[] args)
        {
            var text32 = textPath.GetBytesFromText32();
            var text64 = textPath.GetBytesFromText64();
            
            var generator = new RandomNumbersGenerator();

            var key32 = Enumerable.Range(0, 8).Select(x => generator.GenerateUint32()).ToArray();
            var key64 = Enumerable.Range(0, 8).Select(x => generator.GenerateUint64()).ToArray();



            Console.WriteLine();
            Console.WriteLine("Start");


            var cypr = new Cypress256(true);
            cypr.Key = key32;
            cypr.RoundKeysExpansion();

            var cypar512 = new Cypress512(false);
            cypar512.Key = key64;
            cypar512.RoundKeysExpansion();
            var watch = System.Diagnostics.Stopwatch.StartNew();


            var encrypted = cypr.FullEncrypt(text32.ToList());
            encrypted.GetTextFromUintArr(encryptedPath);
            //var encrypted = cypar512.FullEncrypt(text64.ToList());

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine();
            Console.WriteLine("SEC: ");
            Console.WriteLine(elapsedMs);

            Console.WriteLine("Encrypted:");
            //cypherText1.ToList().ForEach(x => Console.Write(x + " "));

            Console.WriteLine();


            //using (Aes myAes = Aes.Create())
            //{
            //    byte[] encryptedAes = EncryptStringToBytes_Aes(File.ReadAllText(textPath), myAes.Key, myAes.IV);
            //}
            Console.ReadKey();
        }


        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

    }
}
