using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Cypress
{
    class Cypress256
    {
        public UInt32[] OpenText { get; set; }
        public UInt32[] Key { get; set; }
        public List<UInt32[]> _roundKeys;

        private readonly int _mod; //s
        private readonly int _iterationsAmount; //t
        private readonly int[] _circularShiftValues; //r0, r1, r2, r3


        private int r0, r1, r2, r3;


        public Cypress256(bool isCypress256)
        {
            _mod = isCypress256 ? 32 : 64;
            _iterationsAmount = isCypress256 ? 10 : 14;
            _circularShiftValues = isCypress256 ?
                new[] { 16, 12, 8, 7 } : new[] { 32, 24, 16, 15 };
            _roundKeys = new List<uint[]>();

            Init();
        }


        public void Arx(uint[] block)
        {
            var p0 = block[0];
            var p1 = block[1];
            var p2 = block[2];
            var p3 = block[3];

            unchecked
            {
                p0 += p1; p3 ^= p0; p3 = (p3 << r0 | p3 >> (_mod - r0));
                p2 += p3; p1 ^= p2; p1 = (p1 << r1 | p1 >> (_mod - r1));
                p0 += p1; p3 ^= p0; p3 = (p3 << r2 | p3 >> (_mod - r2));
                p2 += p3; p1 ^= p2; p1 = (p1 << r3 | p1 >> (_mod - r3));

                p0 += p1; p3 ^= p0; p3 = (p3 << r0 | p3 >> (_mod - r0));
                p2 += p3; p1 ^= p2; p1 = (p1 << r1 | p1 >> (_mod - r1));
                p0 += p1; p3 ^= p0; p3 = (p3 << r2 | p3 >> (_mod - r2));
                p2 += p3; p1 ^= p2; p1 = (p1 << r3 | p1 >> (_mod - r3));
            }


            block[0] = p0;
            block[1] = p1;
            block[2] = p2;
            block[3] = p3;
        }

        public void RoundKeysExpansion()
        {
            uint[] one = { 1, 1, 1, 1 };
            uint[] tmv = { 0x000F000F, 0x000F000F, 0x000F000F, 0x000F000F };

            for (int i = 0; i < _iterationsAmount / 2; i++)
            {
                var K_l = Key.Take(4).ToArray();
                var K_r = Key.Skip(4).Take(4).ToArray();

                var st = K_l.XOR(one);
                Arx(st);
                st = st.ADD(K_r);
                Arx(st);
                st = st.XOR(K_l);
                var K_sigma = st;

                st = K_l;
                var K_t = K_sigma.ADD(tmv);

                st = st.ADD(K_t);
                Arx(st);

                st = st.XOR(K_t);
                Arx(st);

                st = st.ADD(K_t);
                _roundKeys.Add(st); //RK_2i

                for (int j = 0; j < tmv.Length; j++)
                {
                    tmv[j] = tmv[j] << 0x1;
                }

                st = K_r;
                K_t = K_sigma.ADD(tmv);
                st = st.ADD(K_t);
                Arx(st);
                st = st.XOR(K_t);
                Arx(st);
                st = st.ADD(K_t);
                _roundKeys.Add(st); //RK_2i+1

                for (int j = 0; j < tmv.Length; j++)
                {
                    tmv[j] = tmv[j] << 0x1;
                }

                for (int j = 0; j < Key.Length; j++)
                {
                    Key[j] = (Key[j] << 1) | (Key[j] >> (_mod - 1));
                }
            }

            Console.WriteLine();
            _roundKeys.ForEach(x =>
            {
                Console.Write("KEY: ");
                x.ToList().ForEach(z => Console.Write("\t" + z));
                Console.WriteLine();
            });

        }

        public uint[] EncryptBlock(uint[] plainText)
        {
            var currentFullBlock = plainText;
            for (int i = 0; i < _iterationsAmount; i++)
            {
                var leftPart = currentFullBlock.Take(4).ToArray();
                var righPart = currentFullBlock.Skip(4).Take(4).ToArray();

                var leftXorRK = leftPart.XOR(_roundKeys[i]);
                Arx(leftXorRK);
                var arxResultXorRigth = leftXorRK.XOR(righPart).ToList();

                arxResultXorRigth.AddRange(leftPart);
                currentFullBlock = arxResultXorRigth.ToArray();
            }

            return currentFullBlock;
        }

        public uint[] FullEncrypt(List<UInt32> message)
        {
            var encryptedMessage = new List<uint>();

            for (int i = 0; i < message.Count/8; i++)
            {
                var plainTextPart = message.GetRange(i, 8).ToArray();
                var encryptedPart = EncryptBlock(plainTextPart);
                encryptedMessage.AddRange(encryptedPart);
            }

            return encryptedMessage.ToArray();
        }

        private void Init()
        {
            r0 = _circularShiftValues[0];
            r1 = _circularShiftValues[1];
            r2 = _circularShiftValues[2];
            r3 = _circularShiftValues[3];
        }
    }
}