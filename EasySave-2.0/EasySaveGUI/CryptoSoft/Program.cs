using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSoft
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: cryptosoft.exe <source_file> <destination_file>");
                return;
            }

            string lSourcePath = args[0];
            string lDestinationPath = args[1];

            CXorChiffrement lXorChiffrement = new CXorChiffrement();
            // cm - Init secret key
            byte[] key = System.Text.Encoding.UTF8.GetBytes("secret");

            try
            {
                // cm - Read the source file
                byte[] lSourceData = File.ReadAllBytes(lSourcePath);

                // cm - Encrypt the data
                byte[] lEncryptedData = lXorChiffrement.Encrypt(lSourceData, key);

                // cm - Write the encrypted data to the destination file
                File.WriteAllBytes(lDestinationPath, lEncryptedData);

                Console.WriteLine("Operation completed successfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
