using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace b2b.Models
{
    public class Authentication
    {
        public static bool ValidateToken(string apiKey, string token)
        {
            bool result = false;
            try
            {

                string key = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                string[] parts = key.Split(new char[] { ':' });

                if (parts.FirstOrDefault(a => a == "12345") != null && parts.FirstOrDefault(c => c == GetHashedPassword("12345")) != null && parts.FirstOrDefault(d => d == GetIpAddr().Replace(":", "")) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }

        }


        private const string _alg = "HmacSHA256";
        private const string _salt = "rz8LuOtFBXphj9WQfvFh";
        public static string GenerateToken(string username, string password)
        {

            string hash = string.Join(":", new string[] { username.ToLower() });
            string hashLeft = "";
            string hashRight = "";
            DateTime date = DateTime.ParseExact("04/30/2013 23:00", "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                hmac.Key = Encoding.UTF8.GetBytes(GetHashedPassword(password));
                hmac.ComputeHash(Encoding.UTF8.GetBytes(hash));
                hashLeft = Convert.ToBase64String(hmac.Hash);
                //hashRight = string.Join(":", new string[] { username, GetHashedPassword(password), GetIpAddr(), Convert.ToDateTime(date).ToString("dd/mm/yy hhmmss") });
                hashRight = string.Join(":", new string[] { username, GetHashedPassword(password), GetIpAddr(), date.ToString("dd/mm/yy hhmm") });

            }
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Join(":", hashLeft, hashRight)));
        }
        public static string GetHashedPassword(string password)
        {
            string key = string.Join(":", new string[] { password, _salt });
            using (HMAC hmac = HMACSHA256.Create(_alg))
            {
                // Hash the key.
                hmac.Key = Encoding.UTF8.GetBytes(_salt);
                hmac.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hmac.Hash);
            }
        }
        public static string GetIpAddr()
        {
            //return (from nic in NetworkInterface.GetAllNetworkInterfaces() where nic.OperationalStatus == OperationalStatus.Up select nic.GetPhysicalAddress().ToString()).FirstOrDefault().ToString();
            return HttpContext.Current.Request.UserHostAddress;

        }
        public static class JohnEncryptDecrypt
        {
            public static string JohnEncrypt(string Data)
            {
                try
                {

                    string passPhrase = "laveta207";
                    string saltValue = "nickiminaj";
                    string hashAlgorithm = "MD5";
                    int passwordIterations = 1;
                    string initVector = "koxskfruvdslbsxu";
                    int keySize = 128;

                    byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                    byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                    byte[] plainTextBytes = Encoding.UTF8.GetBytes(Data);

                    PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
                    byte[] keyBytes = password.GetBytes(keySize / 8);

                    RijndaelManaged symmetricKey = new RijndaelManaged();
                    symmetricKey.Mode = CipherMode.CBC;

                    ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();

                    byte[] cipherTextBytes = memoryStream.ToArray();

                    memoryStream.Close();
                    cryptoStream.Close();

                    string cipherText = Convert.ToBase64String(cipherTextBytes);

                    return cipherText;

                }
                catch { }

                return "";
            }

            public static string JohnDecrypt(string Data)
            {
                try
                {

                    string passPhrase = "laveta207";
                    string saltValue = "nickiminaj";
                    string hashAlgorithm = "MD5";
                    int passwordIterations = 1;
                    string initVector = "koxskfruvdslbsxu";
                    int keySize = 128;

                    byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                    byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                    byte[] cipherTextBytes = Convert.FromBase64String(Data);

                    PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
                    byte[] keyBytes = password.GetBytes(keySize / 8);

                    RijndaelManaged symmetricKey = new RijndaelManaged();
                    symmetricKey.Mode = CipherMode.CBC;

                    ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                    MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                    memoryStream.Close();
                    cryptoStream.Close();

                    string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                    return plainText;

                }
                catch { }

                return "";
            }
        }
    }
}