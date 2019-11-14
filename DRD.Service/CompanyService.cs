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
                    listReturn.companies.Append(company);
                }
                return listReturn;
            }
        }
        public Company getCompany(int id)
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.Id == id).ToList();
                if (result.Count == 0)
                {
                    return null;
                }
                else
                {
                    var returnCompany = new Company();
                    foreach(Models.Company x in result){
                        returnCompany.Id = x.Id;
                        returnCompany.Name = x.Name;
                        returnCompany.Code = x.Code;
                    }
                    return returnCompany;
                }
            }
        }
    }
}
