namespace DRD.Models.API
{
    public class DocUploadResult
    {
        public long Id { get; set; };
        public string Status { get; set; };
        public string FileUrl { get; set; };
        public string FileName { get; set; };
        public string FileExtension { get; set; };
    }
}
