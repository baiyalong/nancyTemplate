using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MDM.Helpers
{
    public class CryptoHelper
    {
        /// <summary>
        /// MD5多次哈希算法
        /// </summary>
        /// <param name="count"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] MD5Hash(int count, string key)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] original = Encoding.UTF8.GetBytes(key);
            List<byte> result = new List<byte>();
            for (int ii = 1; ii <= count; ii++)
            {
                List<byte> param = new List<byte>();
                param.AddRange(result.ToArray());
                param.AddRange(original);
                byte[] hash = md5.ComputeHash(param.ToArray());
                result.AddRange(hash);
            }
            return result.ToArray();
        }

        #region Decrypt

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt)
        {
            string strKey = ConfigHelper.GetValue<string>("ssoKey", string.Empty);
            string strIv = ConfigHelper.GetValue<string>("ssoIV", string.Empty);
            return Decrypt(strKey, strIv, toDecrypt);
        }

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strIv"></param>
        /// <param name="toDecrypt"></param>
        /// <returns></returns>
        public static string Decrypt(string strKey, string strIv, string toDecrypt)
        {
            string result = string.Empty;
            try
            {
                byte[] key = Encoding.UTF8.GetBytes(strKey);
                byte[] iv = Encoding.UTF8.GetBytes(strIv);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Mode = CipherMode.CBC;
                des.IV = iv;
                des.Key = key;
                ICryptoTransform decryptor = des.CreateDecryptor();
                string plaintext = string.Empty;
                byte[] buffers = ToBase16Bytes(toDecrypt);
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(buffers, 0, buffers.Length);
                    }
                    result = Encoding.UTF8.GetString(msDecrypt.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(typeof(CryptoHelper), "token解密失败：" + ex);
                result = string.Empty;
            }
            return result;
        }

        #endregion

        #region Encrypt

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="toDecrypt"></param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt)
        {
            string strKey = ConfigHelper.GetValue<string>("ssoKey", string.Empty);
            string strIv = ConfigHelper.GetValue<string>("ssoIV", string.Empty);
            return Encrypt(strKey, strIv, toEncrypt);
        }


        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strIv"></param>
        /// <param name="toEncrypt"></param>
        /// <returns></returns>
        public static string Encrypt(string strKey, string strIv, string toEncrypt)
        {
            string result = string.Empty;
            try
            {
                byte[] key = Encoding.UTF8.GetBytes(strKey);
                byte[] iv = Encoding.UTF8.GetBytes(strIv);
                TripleDESCryptoServiceProvider enc = new TripleDESCryptoServiceProvider();
                enc.Mode = CipherMode.CBC;
                enc.IV = iv;
                enc.Key = key;
                ICryptoTransform encryptor = enc.CreateEncryptor(key, iv);
                byte[] buffers = Encoding.UTF8.GetBytes(toEncrypt);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(buffers, 0, buffers.Length);
                    }
                    result = ToBase16String(msEncrypt.ToArray());
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(typeof(CryptoHelper), "token加密失败：" + ex);
                result = string.Empty; ;
            }
            return result;
        }

        /// <summary>
        /// 将byte数组转换为base16的字符串
        /// </summary>
        /// <param name="data">待转换的byte数组</param>
        /// <returns>转换好的16进制字符串</returns>
        public static string ToBase16String(byte[] data)
        {
            StringBuilder strB = new StringBuilder();
            if (data != null)
            {
                for (int i = 0; i < data.Length; i++)
                    strB.AppendFormat("{0:x2}", data[i]);
            }
            return strB.ToString();
        }

        /// <summary>
        /// 将保存好的16进制字符串转换为byte数组
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static byte[] ToBase16Bytes(string strData)
        {
            byte[] data = null;
            if (strData == null) data = new byte[0];
            else
            {
                data = new Byte[strData.Length / 2];
                for (int i = 0; i < strData.Length / 2; i++)
                    data[i] = Convert.ToByte(strData.Substring(i * 2, 2), 16);
            }
            return data;
        }
        #endregion
    }
}