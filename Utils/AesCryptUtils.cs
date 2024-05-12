using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AzureBlobManager.Utils
{
    public class AesCryptUtils
    {
        private const string defaultKey = "76AA6E93-AB72-4B33-B382-ABF77FF64C83"; // Must be at least 16 characters
        private static readonly string defaultIv = "55DCB9FA-32DB-4E93-A7BE-BF4EDBE29C42".Substring(0,16); // Must be 16 characters

        public static string EncryptString(string plainText)
        {
            // Convert plain text into a byte array
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Create a new Aes object with key and IV
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(defaultKey);
                aes.IV = Encoding.UTF8.GetBytes(defaultIv);
                aes.Mode = CipherMode.CBC; // Set the cipher mode (CBC is common)

                // Create an encryptor
                using (var encryptor = aes.CreateEncryptor())
                {
                    // Create a memory stream to store the encrypted data
                    using (var memoryStream = new MemoryStream())
                    {
                        // Create a crypto stream using the encryptor
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            // Write the plain text bytes to the crypto stream
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        }
                        // Convert the encrypted data from memory stream to a base64 string
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        public static string DecryptString(string encryptedText)
        {
            // Convert the base64 string to a byte array
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            // Create a new Aes object with the same key and IV
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(defaultKey);
                aes.IV = Encoding.UTF8.GetBytes(defaultIv);
                aes.Mode = CipherMode.CBC; // Set the same cipher mode

                // Create a decryptor
                using (var decryptor = aes.CreateDecryptor())
                {
                    // Create a memory stream to store the decrypted data
                    using (var memoryStream = new MemoryStream(encryptedBytes))
                    {
                        // Create a crypto stream using the decryptor
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            // Create a byte array to hold the decrypted data
                            var decryptedBytes = new byte[encryptedBytes.Length];
                            // Read the decrypted data from the crypto stream
                            int decryptedCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                            // Convert the decrypted byte array to a string
                            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedCount);
                        }
                    }
                }
            }
        }

        public static string EncryptString2(string plainText, string key, string salt)
        {
            if (string.IsNullOrWhiteSpace(plainText)) throw new ArgumentNullException("plainText");

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentNullException("Key and salt must not be empty.");
            }

            if (salt.Length < 16) throw new ArgumentException("Salt must be at least 16 characters.");

            if (key.Length != 16) throw new ArgumentException("Key must be 16 characters long.");


            // Convert plain text into a byte array
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Create a new Aes object with key and IV
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(salt);
                aes.Mode = CipherMode.CBC; // Set the cipher mode (CBC is common)

                // Create an encryptor
                using (var encryptor = aes.CreateEncryptor())
                {
                    // Create a memory stream to store the encrypted data
                    using (var memoryStream = new MemoryStream())
                    {
                        // Create a crypto stream using the encryptor
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            // Write the plain text bytes to the crypto stream
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        }
                        // Convert the encrypted data from memory stream to a base64 string
                        return Convert.ToBase64String(memoryStream.ToArray());
                    }
                }
            }
        }

        public static string DecryptString2(string encryptedText, string key, string salt)
        {
            if (string.IsNullOrWhiteSpace(encryptedText)) throw new ArgumentNullException("encryptedText");

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(salt))
            {
                throw new ArgumentNullException("Key and salt must not be empty.");
            }

            if (salt.Length < 16) throw new ArgumentException("Salt must be at least 16 characters.");

            if (key.Length != 16) throw new ArgumentException("Key must be 16 characters long.");

            // Convert the base64 string to a byte array
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            // Create a new Aes object with the same key and IV
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(defaultIv);
                aes.Mode = CipherMode.CBC; // Set the same cipher mode

                // Create a decryptor
                using (var decryptor = aes.CreateDecryptor())
                {
                    // Create a memory stream to store the decrypted data
                    using (var memoryStream = new MemoryStream(encryptedBytes))
                    {
                        // Create a crypto stream using the decryptor
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            // Create a byte array to hold the decrypted data
                            var decryptedBytes = new byte[encryptedBytes.Length];
                            // Read the decrypted data from the crypto stream
                            int decryptedCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                            // Convert the decrypted byte array to a string
                            return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedCount);
                        }
                    }
                }
            }
        }
    }
}

