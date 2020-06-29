namespace DRD.Models.API
{
    public class RegistrationResponse
    {

        public string Id { get; set; }
        public string Email { get; set; }

        public RegistrationResponse(string Id, string email)
        {

            this.Id = Id;
            this.Email = email;
        }
        public RegistrationResponse()
        {

            this.Id = "";
            this.Email = "";
        }
    }
}
