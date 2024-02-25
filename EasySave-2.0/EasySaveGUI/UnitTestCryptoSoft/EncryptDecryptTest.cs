using CryptoSoft;
using System.Text;

namespace UnitTestCryptoSoft
{
    public class EncryptDecryptTest
    {
        [Fact]
        public void XorTest()
        {
            CXorChiffrement lXorChiffrement = new CXorChiffrement();
            string lOriginalText = "Hello, World!";
            byte[] lOriginalData = Encoding.UTF8.GetBytes(lOriginalText);
            byte[] lKey = Encoding.UTF8.GetBytes("secret");

            byte[] lEncryptedData = lXorChiffrement.Encrypt(lOriginalData, lKey);
            byte[] lDecryptedData = lXorChiffrement.Decrypt(lEncryptedData, lKey);
            string lDecryptedText = Encoding.UTF8.GetString(lDecryptedData);

            Assert.Equal(lOriginalText, lDecryptedText);
        }
    }
}