using System;
using System.Collections.Generic;
using System.Linq;
using DRD.Models.API;
using System.IO;
using System.Reflection;

namespace DRD.Service
{
    public class MenuService
    {
        public List<Menu> GetMenus(int activeId)
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"Menu.csv");
            List <Menu> values = File.ReadAllLines(path)
                                           .Select(v => Menu.FromCsv(v))
                                           .ToList();
            return values;
        }
        public List<Menu> GetMenus(string path, int activeId)
        {
            var menuPath = Path.Combine(path,"..\\Menu.csv");
            List<Menu> values = File.ReadAllLines(menuPath)
                                           .Skip(1)
                                           .Select(v => Menu.FromCsv(v))
                                           .ToList();
            return values;
        }

        public string Encrypt(string data)
        {
            var result = Utilities.Encrypt(data);
            var first = Reverse(result.Substring(0, result.Length / 2));
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
            var first = (data.Substring(0, data.Length / 2));
            var result = first + data.Substring(data.Length / 2);
            try
            {
                result = Utilities.Decrypt(result);
            }
            catch (Exception x)
            {
                return null;
            }

            return result;
        }
        private string Reverse(string data)
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


