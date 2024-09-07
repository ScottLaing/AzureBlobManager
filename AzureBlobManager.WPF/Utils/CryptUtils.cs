using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static AzureBlobManager.Constants;

#pragma warning disable SYSLIB0022 // Type or member is obsolete warning, disable for now

namespace AzureBlobManager.Utils
{
    /// <summary>
    /// Provides methods for encrypting and decrypting strings using RijndaelManaged.
    /// </summary>
    public class CryptUtils
    {
        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="salt">The salt used to generate a key for encryption.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        /// <returns>The encrypted text.</returns>
        /// <remarks>Async for speed.</remarks>
        public static async Task<string> EncryptStringAsync(string plainText, string salt = Constants.Salt, string? sharedSecret = null)
        {
            if (sharedSecret == null)
            {
                sharedSecret = Constants.EncryptionKey;
            }

            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException(PlainText);
            }

            plainText = AddPadding(plainText);

            string? outStr = null;                       // Encrypted string to return
            RijndaelManaged? aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                byte[] bSalt = Encoding.ASCII.GetBytes(salt);
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, bSalt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            var writingTask = swEncrypt.WriteAsync(plainText);
                            await writingTask;
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="salt">The salt used to generate a key for decryption.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        /// <returns>The decrypted text.</returns>
        /// <remarks>Async for speed.</remarks>
        public static async Task<string> DecryptStringAsync(string cipherText, string salt, string sharedSecret)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException(CipherText);
            }

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged? aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string? plaintext = null;

            try
            {
                byte[] bSalt = Encoding.ASCII.GetBytes(salt);
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, bSalt);
                    
                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            var readerTask = srDecrypt.ReadToEndAsync();
                            plaintext = await readerTask;
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                {
                    aesAlg.Clear();
                }
            }

            plaintext = RemovePadding(plaintext);

            return plaintext;
        }


        /// <summary>
        /// Removes padding from the decrypted text.
        /// </summary>
        /// <param name="plaintext">The decrypted text from which padding will be removed.</param>
        /// <returns>The text after removing padding.</returns>
        private static string RemovePadding(string plaintext)
        {
            plaintext = plaintext.Substring(Constants.GuidLength);
            var len = plaintext.Length;
            plaintext = plaintext.Substring(0, len - Constants.GuidLength);
            return plaintext;
        }


        /// <summary>
        /// Adds padding to the plain text for encryption.
        /// </summary>
        /// <param name="plainText">The text to which padding will be added.</param>
        /// <returns>The padded text.</returns>
        private static string AddPadding(string plainText)
        {
            plainText = Guid.NewGuid().ToString() + plainText + Guid.NewGuid().ToString();
            return plainText;
        }

        /// <summary>
        /// Reads a byte array from the given stream.
        /// </summary>
        /// <param name="s">The stream from which the byte array will be read.</param>
        /// <returns>The byte array read from the stream.</returns>
        /// <exception cref="SystemException">Thrown when the stream does not contain a properly formatted byte array or the byte array could not be read properly.</exception>
        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException(StringDidNotContainProperlyFormattedByteArray);
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException(DidNotReadByteArrayProperly);
            }

            return buffer;
        }

        /// <summary>
        /// Encrypts a string using AES encryption with a custom key and salt.
        /// </summary>
        /// <param name="plainText">The plain text to be encrypted.</param>
        /// <param name="key">The encryption key.</param>
        /// <param name="salt">The salt value.</param>
        /// <returns>The encrypted string.</returns>
        public static string AesEncryptString(string plainText, string key = AesDefaultKey, string salt = AesDefaultIv)
        {
            if (string.IsNullOrWhiteSpace(plainText))
            {
                throw new ArgumentNullException(PlainTextMustNotBeBlank);
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
        /// Decrypts an encrypted string using AES encryption with a custom key and salt.
        /// </summary>
        /// <param name="encryptedText">The encrypted string to be decrypted.</param>
        /// <param name="key">The encryption key.</param>
        /// <param name="salt">The salt value.</param>
        /// <returns>The decrypted string.</returns>
        public static string AesDecryptString(string encryptedText, string key = AesDefaultKey, string salt = AesDefaultIv)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
            {
                throw new ArgumentNullException(EncryptedTextMustNotBeBlank);
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
                aes.IV = Encoding.UTF8.GetBytes(salt);
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
#pragma warning restore SYSLIB0022 // Type or member is obsolete