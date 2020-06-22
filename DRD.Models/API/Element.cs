using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models.API
{
    [NotMapped]
    public class Element
    {
        public string EncryptedUserId { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Foto { get; set; }
    }
}
