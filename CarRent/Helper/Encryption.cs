using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarRent.Helper
{
    class Encryption
    {
        private const string SALT_TEXT = "RandomSalt...";

        public static string GenerateSaltedHash(string text)
        {
            byte[] plainText = Encoding.ASCII.GetBytes(text);
            byte[] salt = Encoding.ASCII.GetBytes(SALT_TEXT);

            HashAlgorithm algorithm = new SHA512Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }
            return Convert.ToBase64String(algorithm.ComputeHash(plainTextWithSaltBytes));
        }
    }
}
