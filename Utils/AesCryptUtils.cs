using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static SimpleBlobUtility.Constants;

namespace AzureBlobManager.Utils
{
    public class AesCryptUtils
    {
        /// <summary>
        /// Encrypts a string using AES encryption with default key and IV.
        /// </summary>
        /// <param name="plainText">The plain text to be encrypted.</param>
        /// <returns>The encrypted string.</returns>
        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentNullException(PlainTextMustNotBeBlank);
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AesDefaultKey);
                aes.IV = Encoding.UTF8.GetBytes(AesDefaultIv);
                aes.Mode = CipherMode.CBC; // Set the cipher mode (CBC is common)

                using (var encryptor = aes.CreateEncryptor())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        }
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts a string using AES encryption with a custom key and salt.
        /// </summary>
        /// <param name="plainText">The plain text to be encrypted.</param>
        /// <param name="key">The encryption key.</param>
        /// <param name="salt">The salt value.</param>
        /// <returns>The encrypted string.</returns>
        public static string EncryptString(string plainText, string key, string salt)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentNullException(PlainTextMustNotBeBlank);
            }

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentNullException(KeyAndSaltMustNotBeEmpty);
            }

            if (salt.Length < 16)
            {
                throw new ArgumentException(SaltMustBeAtLeast16Characters);
            }

            if (key.Length != 16)
            {
                throw new ArgumentException(KeyMustBe16CharactersLong);
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(salt);
                aes.Mode = CipherMode.CBC; // Set the cipher mode (CBC is common)

                using (var encryptor = aes.CreateEncryptor())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        }
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts an encrypted string using AES encryption with default key and IV.
        /// </summary>
        /// <param name="encryptedText">The encrypted string to be decrypted.</param>
        /// <returns>The decrypted string.</returns>
        public static string DecryptString(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentNullException(EncryptedTextMustNotBeBlank);
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(AesDefaultKey);
                aes.IV = Encoding.UTF8.GetBytes(AesDefaultIv);
                aes.Mode = CipherMode.CBC; // Set the same cipher mode

                using (var decryptor = aes.CreateDecryptor())
                {
                    using (var memoryStream = new MemoryStream(encryptedBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var decryptedBytes = new byte[encryptedBytes.Length];
                            int decryptedCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedCount);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts an encrypted string using AES encryption with a custom key and salt.
        /// </summary>
        /// <param name="encryptedText">The encrypted string to be decrypted.</param>
        /// <param name="key">The encryption key.</param>
        /// <param name="salt">The salt value.</param>
        /// <returns>The decrypted string.</returns>
        public static string DecryptString(string encryptedText, string key, string salt)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentNullException(EncryptedTextMustNotBeBlank);
            }

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentNullException(KeyAndSaltMustNotBeEmpty);
            }

            if (salt.Length < 16)
            {
                throw new ArgumentException(SaltMustBeAtLeast16Characters);
            }

            if (key.Length != 16)
            {
                throw new ArgumentException(KeyMustBe16CharactersLong);
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(AesDefaultIv);
                aes.Mode = CipherMode.CBC; // Set the same cipher mode

                using (var decryptor = aes.CreateDecryptor())
                {
                    using (var memoryStream = new MemoryStream(encryptedBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var decryptedBytes = new byte[encryptedBytes.Length];
                            int decryptedCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedCount);
                        }
                    }
                }
            }
        }
    }
}

