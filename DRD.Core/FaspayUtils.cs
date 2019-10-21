using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DRD.Core
{
    public class FaspayUtils
    {
        public static string GetSha1HashString2(string toHash)
        {
            var bytesToHash = Encoding.UTF8.GetBytes(toHash);
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var bytesSha1Hash = sha1.ComputeHash(bytesToHash);
                var strSha1Hash = BitConverter.ToString(bytesSha1Hash);
                return strSha1Hash.Replace("-", "").ToLower();
            }
        }

        public static string GetMd5HashString2(string toHash)
        {
            var bytesToHash = Encoding.UTF8.GetBytes(toHash);
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bytesMd5Hash = md5.ComputeHash(bytesToHash);
                var strMd5Hash = BitConverter.ToString(bytesMd5Hash);
                return strMd5Hash.Replace("-", "").ToLower();
            }
        }

        public static string GetSha1HashString1(string toHash)
        {
            var bytesToHash = Encoding.UTF8.GetBytes(toHash);
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var bytesMd5Hash = sha1.ComputeHash(bytesToHash);
                var strBuilder = new StringBuilder(bytesMd5Hash.Length * 2);
                for (var i = 0; i < bytesMd5Hash.Length; i++)
                {
                    strBuilder.Append(bytesMd5Hash[i].ToString("x2"));
                }

                return strBuilder.ToString();
            }
        }

        public static string GetMd5HashString1(string toHash)
        {
            var bytesToHash = Encoding.UTF8.GetBytes(toHash);
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bytesMd5Hash = md5.ComputeHash(bytesToHash);
                var strBuilder = new StringBuilder(bytesMd5Hash.Length * 2);
                for (var i = 0; i < bytesMd5Hash.Length; i++)
                {
                    strBuilder.Append(bytesMd5Hash[i].ToString("x2"));
                }

                return strBuilder.ToString();
            }
        }
    }
}
