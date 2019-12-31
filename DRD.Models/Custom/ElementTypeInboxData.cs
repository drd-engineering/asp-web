using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
   
    public class ElementTypeInboxData
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 20)
        public string Description { get; set; } // Descr (length: 500)
    }
}
