using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AzureBlobManager.Utils
{
    public class AesCryptUtils
    {
        private const string defaultKey = "76AA6E93-AB72-4B33-B382-ABF77FF64C83"; // Must be at least 16 characters
        private static readonly string defaultIv = "55DCB9FA-32DB-4E"; // Must be 16 characters
        private const string KeyAndSaltMustNotBeEmpty = "Key and salt must not be empty.";
        private const string SaltMustBeAtLeast16Characters = "Salt must be at least 16 characters.";
        private const string KeyMustBe16CharactersLong = "Key must be 16 characters long.";
        private const string EncryptedTextMustNotBeBlank = "encryptedText must not be blank.";
        private const string PlainTextMustNotBeBlank = "plainText must not be blank.";

        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentNullException(PlainTextMustNotBeBlank);
            }

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(defaultKey);
                aes.IV = Encoding.UTF8.GetBytes(defaultIv);
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

        public static string DecryptString(string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentNullException(EncryptedTextMustNotBeBlank);
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(defaultKey);
                aes.IV = Encoding.UTF8.GetBytes(defaultIv);
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
                aes.IV = Encoding.UTF8.GetBytes(defaultIv);
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

