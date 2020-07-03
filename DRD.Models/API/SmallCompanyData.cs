﻿namespace DRD.Models.API
{
    public class SmallCompanyData
    {
        public long Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 10)
        public string Name { get; set; } // Name (length: 50)
        public SmallCompanyData() { }
        public SmallCompanyData(Company companyDb)
        {
            Id = companyDb.Id;
            Code = companyDb.Code;
            Name = companyDb.Name;
        }
    }
}