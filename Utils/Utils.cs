using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace NET_API.Utils;

public class Utils
{
    public static byte[] DeriveKey(string originalKey, int keySizeInBits = 256)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] originalKeyBytes = Encoding.UTF8.GetBytes(originalKey);
            byte[] hashedKey = sha256.ComputeHash(originalKeyBytes);

            // If you need a key of specific size, you might truncate or pad the hash
            byte[] derivedKey = new byte[keySizeInBits / 8];
            Buffer.BlockCopy(hashedKey, 0, derivedKey, 0, derivedKey.Length);

            return derivedKey;
        }
    }
    
    public static string EncryptAES(string plainText, string key)
    {
        string result;
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = DeriveKey(key);
            aesAlg.IV = new byte[16];

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                result = BitConverter.ToString(msEncrypt.ToArray()).Replace("-", "");
            }
        }
        return result;
    }

    public static string DecryptAES(string cipherText, string key)
    {
        byte[] encryptedBytes = Enumerable.Range(0, cipherText.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(cipherText.Substring(x, 2), 16))
            .ToArray();
        string result;
        using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = DeriveKey(key);
                aesAlg.IV = new byte[16];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        result = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return result;
    }
    
    public static T Map<T>(IDataRecord record) where T : new()
    {
        T obj = new T();
        foreach (var property in typeof(T).GetProperties())
        {
            if (!record.IsDBNull(record.GetOrdinal(property.Name)))
            {
                property.SetValue(obj, record[property.Name]);
            }
        }
        return obj;
    }
}