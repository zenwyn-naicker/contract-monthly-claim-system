/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Nikunj Satasiya
 * Date: 2025
 * Title: Encrypting and Decrypting Files using .NET Core
 * Type: [Source Code]
 * Available at: <https://www.c-sharpcorner.com/article/encrypting-and-decrypting-files-using-net-core/> [Accessed 17 October 2025]
 **************************************************************************************************************/

using System.Security.Cryptography;

namespace ContractMonthlyClaimSystem.Helpers
{
    public class EncryptionHelper
    {
        // These are 256-bit keys for AES encryption
        private static readonly byte[] Key = ReadEncryptionSetting("CMCS_ENCRYPTION_KEY", 32);
        private static readonly byte[] IV = ReadEncryptionSetting("CMCS_ENCRYPTION_IV", 16);

        private static byte[] ReadEncryptionSetting(string name, int requiredLength)
        {
            var encoded = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(encoded))
                throw new InvalidOperationException($"Set {name} to a Base64-encoded local secret.");

            var bytes = Convert.FromBase64String(encoded);
            if (bytes.Length != requiredLength)
                throw new InvalidOperationException($"{name} must decode to {requiredLength} bytes.");

            return bytes;
        }

        //_______________________________ In-Text Referencing _____________________________//
        //The following layout was taken from Nikunj Satasiya (2025) Encrypting and Decrypting Files using .NET Core
        //Link: https://www.c-sharpcorner.com/article/encrypting-and-decrypting-files-using-net-core/

        // Encrypts a file using AES encryption
        public static void EncryptFile(string inputFile, string outputFile)
        {
            try
            {
                using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (CryptoStream cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write))
                    using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                    {
                        inputStream.CopyTo(cryptoStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error encrypting file: {ex.Message}", ex);
            }
        }

        // Decrypts an encrypted file using AES decryption
        public static void DecryptFile(string inputFile, string outputFile)
        {
            try
            {
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read))
                    using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                    {
                        cryptoStream.CopyTo(outputStream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error decrypting file: {ex.Message}", ex);
            }
        }

        //_______________________________________ End _____________________________________//

        // Generates a new random key for AES encryption (256-bit)
        public static string GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return Convert.ToBase64String(aes.Key);
            }
        }

        // Generates a new random IV for AES encryption (128-bit)
        public static string GenerateIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return Convert.ToBase64String(aes.IV);
            }
        }
    }
}