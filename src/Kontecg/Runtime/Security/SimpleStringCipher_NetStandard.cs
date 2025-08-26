using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

//This code is got from http://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp

namespace Kontecg.Runtime.Security
{
    /// <summary>
    ///     Can be used to simply encrypt/decrypt texts.
    /// </summary>
    public class SimpleStringCipher
    {
        /// <summary>
        ///     This constant is used to determine the keysize of the encryption algorithm.
        /// </summary>
        public const int DefaultKeysize = 256;

        /// <summary>
        ///     This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        ///     This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        ///     32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        /// </summary>
        public byte[] InitVectorBytes;

        static SimpleStringCipher()
        {
            DefaultPassPhrase = "gsKnGZ041HLL4IM8";
            DefaultInitVectorBytes = Encoding.ASCII.GetBytes("jkE49230Tf093b42");
            DefaultSalt = Encoding.ASCII.GetBytes("hgt!16kl");
            Iterations = 100;
            HashAlgorithmName = HashAlgorithmName.SHA256;
            Instance = new SimpleStringCipher();
        }

        public SimpleStringCipher()
        {
            InitVectorBytes = DefaultInitVectorBytes;
        }

        public static SimpleStringCipher Instance { get; }

        /// <summary>
        ///     Default password to encrypt/decrypt texts.
        ///     It's recommented to set to another value for security.
        ///     Default value: "gsKnGZ041HLL4IM8"
        /// </summary>
        public static string DefaultPassPhrase { get; set; }

        /// <summary>
        ///     Default value: Encoding.ASCII.GetBytes("jkE49230Tf093b42")
        /// </summary>
        public static byte[] DefaultInitVectorBytes { get; set; }

        /// <summary>
        ///     Default value: Encoding.ASCII.GetBytes("hgt!16kl")
        /// </summary>
        public static byte[] DefaultSalt { get; set; }

        public static int Iterations { get; set; }

        public static HashAlgorithmName HashAlgorithmName { get; set; }

        public string Encrypt(
            string plainText,
            string passPhrase = null,
            byte[] salt = null,
            int? keySize = null,
            byte[] initVectorBytes = null,
            int? iterations = null,
            HashAlgorithmName? hashAlgorithmName = null)
        {
            if (plainText == null)
            {
                return null;
            }

            passPhrase ??= DefaultPassPhrase;

            salt ??= DefaultSalt;

            keySize ??= DefaultKeysize;

            initVectorBytes ??= InitVectorBytes;

            iterations ??= Iterations;

            hashAlgorithmName ??= HashAlgorithmName;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, salt, iterations.Value, hashAlgorithmName.Value))
            {
                byte[] keyBytes = password.GetBytes(keySize.Value / 8);
                using (Aes symmetricKey = Aes.Create())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream =
                                   new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(
            string cipherText,
            string passPhrase = null,
            byte[] salt = null,
            int? keySize = null,
            byte[] initVectorBytes = null,
            int? iterations = null,
            HashAlgorithmName? hashAlgorithmName = null)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }

            passPhrase ??= DefaultPassPhrase;

            salt ??= DefaultSalt;

            keySize ??= DefaultKeysize;

            initVectorBytes ??= InitVectorBytes;

            iterations ??= Iterations;

            hashAlgorithmName ??= HashAlgorithmName;

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, salt, iterations.Value, hashAlgorithmName.Value))
            {
                byte[] keyBytes = password.GetBytes(keySize.Value / 8);
                using (Aes symmetricKey = Aes.Create())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (CryptoStream cryptoStream =
                                   new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                int totalDecryptedByteCount = 0;
                                while (totalDecryptedByteCount < plainTextBytes.Length)
                                {
                                    int decryptedByteCount = cryptoStream.Read(
                                        plainTextBytes,
                                        totalDecryptedByteCount,
                                        plainTextBytes.Length - totalDecryptedByteCount
                                    );

                                    if (decryptedByteCount == 0)
                                    {
                                        break;
                                    }

                                    totalDecryptedByteCount += decryptedByteCount;
                                }

                                return Encoding.UTF8.GetString(plainTextBytes, 0, totalDecryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
    }
}
