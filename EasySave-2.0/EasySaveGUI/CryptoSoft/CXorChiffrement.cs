using System;
using System.Collections.Generic;
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
        public override byte[] Encrypt(byte[] data, byte[] key)
        {
            return Xor(data, key);
        }

        public override byte[] Decrypt(byte[] data, byte[] key)
        {
            // cm - For XOR, encryption and decryption are the same operation.
            return Xor(data, key);
        }

        private byte[] Xor(byte[] data, byte[] key)
        {
            byte[] output = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                // cm - XOR each byte of the data with the key, wrapping around the key if necessary
                output[i] = (byte)(data[i] ^ key[i % key.Length]);
            }

            return output;
        }
    }
}
