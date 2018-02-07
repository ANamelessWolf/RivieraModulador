using NamelessOld.Libraries.Yggdrasil.Exceptions;
using NamelessOld.Libraries.Yggdrasil.Lilith;
using NamelessOld.Libraries.Yggdrasil.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NamelessOld.Libraries.Yggdrasil.Alice
{
    /// <summary>
    /// Define a new caterpillar from encrypt and decrypt
    /// </summary>
    public class Caterpillar : NamelessObject
    {
        #region Vars
        /// <summary>
        /// The key of the encription
        /// </summary>
        byte[] Key;
        /// <summary>
        /// The vector for the encription
        /// </summary>
        byte[] IV;
        #endregion
        #region Constructor
        /// <summary>
        /// Create a new Caterpillar Encryptor and decryptor.
        /// </summary>
        /// <param name="key">The key to encrypt or decrypt.</param>
        /// <param name="vector">The vector to decrypt or encrypt.</param>
        public Caterpillar(byte[] key, byte[] vector)
        {
            //1: Guardamos las propiedades del Caterpillar
            Key = key;
            IV = vector;
        }
        #endregion
        #region Caterpillar detalles.
        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="originalString">The string to be encrypted</param>
        /// <returns>The string encrypted.</returns>
        public string Encrypt(string originalString)
        {
            try
            {
                if (originalString != null && originalString != String.Empty)
                {
                    DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                    cryptoProvider.Mode = CipherMode.ECB;
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream,
                        cryptoProvider.CreateEncryptor(Key, IV), CryptoStreamMode.Write);
                    StreamWriter writer = new StreamWriter(cryptoStream);
                    writer.Write(originalString);
                    writer.Flush();
                    cryptoStream.FlushFinalBlock();
                    writer.Flush();
                    return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                }
                else
                    throw (new RedQueenException(Errors.EmptyStringCrypto));
            }
            catch (CryptographicException exc)
            {
                throw (new RedQueenException(Errors.BadEncrypting, exc));
            }
            catch (System.Exception exc)
            {
                throw (new RedQueenException(Errors.BadEncrypting, exc));
            }
        }
        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="cryptedString">The string to be decrypted</param>
        /// <returns>The original string.</returns>
        public string Decrypt(string cryptedString)
        {
            try
            {
                if (cryptedString != null && cryptedString != String.Empty)
                {
                    DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                    cryptoProvider.Mode = CipherMode.ECB;
                    MemoryStream memoryStream = new MemoryStream
                            (Convert.FromBase64String(cryptedString));
                    CryptoStream cryptoStream = new CryptoStream(memoryStream,
                        cryptoProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read);
                    StreamReader reader = new StreamReader(cryptoStream);
                    return reader.ReadToEnd();
                }
                else
                    throw (new RedQueenException(Errors.EmptyStringCrypto));
            }
            catch (CryptographicException exc)
            {
                throw (new RedQueenException(Errors.BadEncrypting, exc));
            }
            catch (System.Exception exc)
            {
                throw (new RedQueenException(Errors.BadEncrypting, exc));
            }
        }
        #endregion
    }
}
