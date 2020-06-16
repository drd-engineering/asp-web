using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    public class AddMemberResponse
    {
        public string email { get; set; }
        public int status { get; set; }
        public string companyName { get; set; }
        public AddMemberResponse(string email, int status, string companyName)
        {
            this.email = email;
            this.status = status;
            this.companyName = companyName;
        }
    }
}
