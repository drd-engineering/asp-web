using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int UserType { get; set; }
        public string CompanyCode { get; set; }
        public DtoCompany Company { get; set; }
    }
}
