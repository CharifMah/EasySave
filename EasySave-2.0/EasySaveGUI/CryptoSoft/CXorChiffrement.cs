using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSoft
{
    /// <summary>
    /// Classe concrete de chiffrement XOR
    /// </summary>
    public class CXorChiffrement : BaseChiffrement
    {
        public override byte[] Encrypt(byte[] pData, byte[] pKey)
        {
            Stopwatch lStopwatch = new Stopwatch();
            lStopwatch.Start();
 
            byte[] lResult = Xor(pData, pKey);

            lStopwatch.Stop();
            this.EncryptTime = lStopwatch.Elapsed;

            return lResult;
        }

        public override byte[] Decrypt(byte[] pData, byte[] pKey)
        {

            Stopwatch lStopwatch = new Stopwatch();
            lStopwatch.Start();

            // cm - For XOR, encryption and decryption are the same operation.
            byte[] lResult = Xor(pData, pKey);

            lStopwatch.Stop();
            this.DecryptTime = lStopwatch.Elapsed;

            return lResult;
        }

        private byte[] Xor(byte[] pData, byte[] pKey)
        {
            byte[] lOutput = new byte[pData.Length];

            for (int i = 0; i < pData.Length; i++)
            {
                // cm - XOR each byte of the data with the key, wrapping around the key if necessary
                lOutput[i] = (byte)(pData[i] ^ pKey[i % pKey.Length]);
            }

            return lOutput;
        }
    }
}
