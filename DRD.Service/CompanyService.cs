using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.API.Register;
using DRD.Service.Context;
namespace DRD.Service
{
    public class CompanyService
    {
        public CompanyList GetAllCompany()
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.IsActive == true).ToList();
                var listReturn = new CompanyList();
                foreach (Models.Company x in result)
                {
                    var company = new Company();
                    company.Id = x.Id;
                    company.Code = x.Code;
                    company.Name = x.Name;
                }
                return listReturn;
            }
        }
    }
}
