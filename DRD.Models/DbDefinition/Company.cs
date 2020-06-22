﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DRD.Models
{
    [Table("Companies", Schema = "public")]
    public class Company
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
        public long OwnerId { get; set; } // OwnerId (ForeignKey to user)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 100)
        public string Descr { get; set; } // Descr
        public string Address { get; set; } // Address (length: 1000)
        public string PointLocation { get; set; } // PointLocation (length: 1000)
        public string PostalCode { get; set; } // PostalCode (length: 5)
        public string Image1 { get; set; } // Image1 (length: 100)
        public string Image2 { get; set; } // Image2 (length: 100)
        public string ImageCard { get; set; } // ImageCard (length: 100)
        public bool IsActive { get; set; } // IsActive
        public bool IsVerified { get; set; } // IsVerified


        // public string SubscriptionName nanti samain atribut2 yang ada di bussinesssubscription.

        public DateTime CreatedAt { get; set; } // DateCreated

        // Reverse navigation

        // public virtual ICollection<Document> Documents { get; set; } // Document.FK_Document_Company
        public virtual ICollection<Member> Members { get; set; } // Member.FK_Member_Company
        //public virtual System.Collections.Generic.ICollection<DtoMemberSubscribe> MemberSubscribes { get; set; } // MemberSubscribe.FK_MemberSubscribe_Company
        /*public virtual ICollection<Tag> Tags { get; set; } // Project.FK_Project_Company*/

        // Foreign keys
        //public virtual SubscriptType subscript_type { get; set; } // FK_Company_SubscriptType

        public Company()
        {
            IsActive = true;
            IsVerified = false;
            CreatedAt = DateTime.Now;
            Members = new List<Member>();
        }
    }
}