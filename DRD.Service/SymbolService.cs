using DRD.Models.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
