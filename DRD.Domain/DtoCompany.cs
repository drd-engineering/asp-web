using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoCompany
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
        public string Contact { get; set; } // Contact (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 100)
        public string Descr { get; set; } // Descr
        public string Address { get; set; } // Address (length: 1000)
        public string PointLocation { get; set; } // PointLocation (length: 1000)
        public decimal Latitude { get; set; } // Latitude
        public decimal Longitude { get; set; } // Longitude
        public string CountryCode { get; set; } // CountryCode (length: 10)
        public string CountryName { get; set; } // CountryName (length: 50)
        public string AdminArea { get; set; } // AdminArea (length: 50)
        public string SubAdminArea { get; set; } // SubAdminArea (length: 50)
        public string Locality { get; set; } // Locality (length: 50)
        public string SubLocality { get; set; } // SubLocality (length: 50)
        public string Thoroughfare { get; set; } // Thoroughfare (length: 50)
        public string SubThoroughfare { get; set; } // SubThoroughfare (length: 10)
        public string PostalCode { get; set; } // PostalCode (length: 5)
        public string Image1 { get; set; } // Image1 (length: 100)
        public string Image2 { get; set; } // Image2 (length: 100)
        public string ImageCard { get; set; } // ImageCard (length: 100)
        public string BackColorBar { get; set; } // BackColorBar (length: 20)
        public string BackColorPage { get; set; } // BackColorPage (length: 20)
        public int CompanyType { get; set; } // CompanyType
        public string ImageQrCode { get; set; } // ImageQrCode (length: 50)
        public bool IsActive { get; set; } // IsActive
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<DtoDocument> Documents { get; set; } // Document.FK_Document_Company
        public virtual System.Collections.Generic.ICollection<DtoMember> Members { get; set; } // Member.FK_Member_Company
        //public virtual System.Collections.Generic.ICollection<DtoMemberSubscribe> MemberSubscribes { get; set; } // MemberSubscribe.FK_MemberSubscribe_Company
        public virtual System.Collections.Generic.ICollection<DtoProject> Projects { get; set; } // Project.FK_Project_Company

        // Foreign keys
        public virtual DtoSubscriptType SubscriptType { get; set; } // FK_Company_SubscriptType

        public DtoCompany()
        {
            Latitude = 0m;
            Longitude = 0m;
            CompanyType = 0;
            IsActive = true;
            UserId = "SYST";
            DateCreated = System.DateTime.Now;
            Members = new System.Collections.Generic.List<DtoMember>();
            Projects = new System.Collections.Generic.List<DtoProject>();
        }
    }
}
