using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core;


namespace DRD.Service
{
    public class MenuService
    {

        public string Encrypt(string data)
        {
            var result = XEncryptionHelper.Encrypt(data);
            var first = reverse(result.Substring(0, result.Length / 2));
            result = first + result.Substring(result.Length / 2);
            return result;
        }

        public string EncryptData(long id)
        {
            return Encrypt(DateTime.Now.ToString("yyyyMMddHHmmssfff") + "," + id.ToString());
        }

        public string Decrypt(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            var first = reverse(data.Substring(0, data.Length / 2));
            var result = first + data.Substring(data.Length / 2);
            try
            {
                result = XEncryptionHelper.Decrypt(result);
            }
            catch (Exception x)
            {
                return null;
            }

            return result;
        }

        private string reverse(string data)
        {
            string result = "";
            for (int i = data.Length - 1; i >= 0; i--)
            {
                result += data[i];
            }
            return result;
        }
    }
}
