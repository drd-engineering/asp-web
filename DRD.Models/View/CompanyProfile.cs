﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    public class CompanyProfile
    {
        public ICollection<CompanyProfileItem> companyProfiles;

        public CompanyProfile()
        {
            companyProfiles = new List<CompanyProfileItem>();
        }

    }
    public class CompanyProfileItem
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long OwnerId { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string PointLocation { get; set; }
        public bool isVerified { get; set; }
        public BusinessSubscription businessSubscription { get; set; }
        public virtual ICollection<DRD.Models.Member> Members { get; set; }
        public virtual ICollection<DRD.Models.Member> Administrators { get; set; }

        public CompanyProfileItem()
        {
            isVerified = false;
            Members = new List<DRD.Models.Member>();
            Administrators = new List<DRD.Models.Member>();
        }

    }
}
