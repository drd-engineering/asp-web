using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("Symbols", Schema = "public")]
    public class Symbol
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 50)
        public string Name { get; set; } // Name (length: 100)
        public string Description { get; set; } // Descr
        public string Image { get; set; } // Image (length: 100)
        public int SymbolType { get; set; } // SymbolType
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public Symbol()
        {
            SymbolType = 0;
        }
    }
}
