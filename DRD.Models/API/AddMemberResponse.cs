namespace DRD.Models.API
{
    public class AddMemberResponse
    {
        public string Email { get; set; }
        public long MemberId { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }
        public string CompanyName { get; set; }
        public AddMemberResponse(string email, long memberId, string userName, int status, string companyName)
        {
            this.Email = email;
            this.MemberId = memberId;
            this.UserName = userName;
            this.Status = status;
            this.CompanyName = companyName;
        }
    }
}
