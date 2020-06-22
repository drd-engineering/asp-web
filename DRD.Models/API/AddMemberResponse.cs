namespace DRD.Models.API
{
    public class AddMemberResponse
    {
        public string email { get; set; }
        public long memberId { get; set; }
        public string userName { get; set; }
        public int status { get; set; }
        public string companyName { get; set; }
        public AddMemberResponse(string email, long memberId, string userName, int status, string companyName)
        {
            this.email = email;
            this.memberId = memberId;
            this.userName = userName;
            this.status = status;
            this.companyName = companyName;
        }
    }
}
