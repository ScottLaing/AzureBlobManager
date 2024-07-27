using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static AzureBlobManager.Constants;

#pragma warning disable SYSLIB0022 // Type or member is obsolete

namespace AzureBlobManager.Utils
{
    /// <summary>
    /// Provides methods for encrypting and decrypting strings using RijndaelManaged.
    /// </summary>
    public class CryptUtils
    {
        //While an app specific salt is not the best practice for
        //password based encryption, it's probably safe enough as long as
        //it is truly uncommon. Also too much work to alter this answer otherwise.
        public static byte[] _salt = Encoding.ASCII.GetBytes(Constants.Salt);

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static string EncryptString(string plainText, string salt = Constants.Salt, string? sharedSecret = null)
        {
            if (sharedSecret == null)
            {
                sharedSecret = Constants.EncryptionKey;
            }

            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(PlainText);

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
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for encryption.</param>
        public static string EncryptString2(string plainText, string? sharedSecret, string? salt)
        {
            if (plainText?.Trim() == string.Empty)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException(PlainText);
            }
            
            if (string.IsNullOrEmpty(sharedSecret))
            {
                sharedSecret = Constants.EncryptionKey;
            }
            
            if (string.IsNullOrEmpty(salt))
            {
                salt = Constants.Salt;
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
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
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

        public static string DecryptString(string cipherText, string salt)
        {
            if (salt == null)
            {
                salt = Constants.Salt;
            }
            return DecryptString(cipherText, salt, Constants.EncryptionKey);
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="salt">The salt used to generate a key for decryption.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptString(string cipherText, string salt, string sharedSecret)
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

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            plaintext = RemovePadding(plaintext);

            return plaintext;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        /// <param name="sharedSecret">A password used to generate a key for decryption.</param>
        public static string DecryptString2(string cipherText, string? sharedSecret, string? salt)
        {
            if (cipherText?.Trim() == string.Empty)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException(CipherText);
            }

            if (string.IsNullOrEmpty(sharedSecret))
            {
                sharedSecret = Constants.EncryptionKey;
            }

            if (string.IsNullOrEmpty(salt))
            {
                salt = Constants.Salt;
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

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
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
    }
}
#pragma warning restore SYSLIB0022 // Type or member is obsolete