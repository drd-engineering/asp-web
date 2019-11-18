﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.View;
using System.IO;

namespace DRD.Service
{
    public class MenuService
    {
        public List<Menu> GetMenus(int activeId)
        {
            List<Menu> values = File.ReadAllLines("Menu.csv")
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


