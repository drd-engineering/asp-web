namespace DRD.Models.View
{
    public class ContactItem
    {
        public long Id { get; set; }
        public string EncryptedId { get; set; } // Id (Primary key)
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ImageProfile { get; set; }
    }
}
