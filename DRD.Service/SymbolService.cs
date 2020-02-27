using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DRD.Models;
using DRD.Models.API;
using DRD.Service.Context;
using DRD.Models.View;

namespace DRD.Service
{
    public class SymbolService
    {
        public List<Symbol> getSymbolList()
        {
            var root = System.Web.HttpContext.Current.Server.MapPath("~");
            var path = Path.Combine(root, @"Symbols.csv");
            List<Symbol> values = File.ReadAllLines(path)
                                           .Select(v => Symbol.FromCsv(v))
                                           .ToList();
            return values;
        }

        public Symbol getSymbol(String symbol)
        {
            return getSymbolList().Find(x => x.Code == symbol);
        }

        public int getSymbolId(String symbol)
        {
            return getSymbol(symbol).Id;
        }
    }
}
