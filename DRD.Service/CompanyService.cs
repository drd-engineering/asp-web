using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models;
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
                    var company = new CompanyItem();
                    company.Id = x.Id;
                    company.Code = x.Code;
                    company.Name = x.Name;
                    listReturn.companies.Append(company);
                }
                return listReturn;
            }
        }
        public CompanyItem GetCompany(int id)
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
                    var returnCompany = new CompanyItem();
                    foreach(Company x in result){
                        returnCompany.Id = x.Id;
                        returnCompany.Name = x.Name;
                        returnCompany.Code = x.Code;
                    }
                    return returnCompany;
                }
            }
        }

        public Company GetCompany(long id)
        {
            using (var db = new ServiceContext())
            {
                return db.Companies.Where(companyItem => companyItem.Id == id).FirstOrDefault();
            }
        }

        public void AddCompany(Company company)
        {
            using (var db = new ServiceContext())
            {
                var result = db.Companies.Where(companyItem => companyItem.Name.Equals(company.Name)).ToList();
                if (result.Count != 0)
                {
                    return;
                }
                Company newCompany = new Company();
                newCompany.Name = company.Name;
                newCompany.OwnerId = company.OwnerId;
                newCompany.Address = company.Address;
                newCompany.Descr = company.Descr;
                newCompany.Email = company.Email;
                newCompany.Phone = company.Phone;
                newCompany.PointLocation = company.PointLocation;
                newCompany.PostalCode = company.PostalCode;

                CompanyService companyService = new CompanyService();
                long companyId = companyService.Save(company);

            }
        }
        public long Save(Company company)
        {

            int result = 0;
            using (var db = new ServiceContext())
            {
                for (int i = 0; i < Constant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        company.Id = Utilities.RandomLongGenerator(minimumValue: 1000000000, maximumValue: 10000000000);

                        //TODO: remove these lines when production
                        System.Diagnostics.Debug.WriteLine("[[USERSERVICE]]User ID expected when saving : " + company.Id);

                        
                        db.Companies.Add(company);
                        result = db.SaveChanges();
                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > Constant.TEST_DUPLICATION_COUNT)
                            throw new Exception(x.Message);
                    }
                }

                return company.Id;
            }
        }

        public long changeOwner(long companyId, long newOwnerUserId)
        {
            using (var db = new ServiceContext())
            {
                Company company = GetCompany(companyId);
                company.OwnerId = newOwnerUserId;

                long result = db.Companies.Add(company).OwnerId;
                db.SaveChanges();

                return result;
            }
        }

        public long chooseSubscription(long subscriptionId, long companyId)
        {
            using (var db = new ServiceContext())
            {
                Company company = GetCompany(companyId);
                company.BusinessSubscriptionId = subscriptionId;

                long result = db.Companies.Add(company).OwnerId;
                db.SaveChanges();

                return result;
            }
        }

        public long verifyCompany(long companyId)
        {
            using (var db = new ServiceContext())
            {
                Company company = GetCompany(companyId);
                company.IsVerified = true;

                long result = db.Companies.Add(company).OwnerId;
                db.SaveChanges();

                return result;
            }
        }
    }
}
