using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    public class RegisterResponse
    {
        
        public string Id { get; set; }
        public string Email { get; set; }

        public RegisterResponse(string Id, string email)
        {
            
            this.Id = Id;
            this.Email = email;
        }
        public RegisterResponse()
        {
           
            this.Id = "";
            this.Email = "";
        }
    }
}
