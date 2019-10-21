using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core.Entity;
using DRD.Domain;
using System.Configuration;
using Geocoding.Google;
using Geocoding;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using System.Based.Core;

namespace DRD.Core
{
    public class CompanyService
    {
        private readonly string _connString;

        public CompanyService(string connString)
        {
            _connString = connString;
        }

        public CompanyService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public DtoCompany GetById(long id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Companies
                     where c.Id == id
                     select new DtoCompany
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Email = c.Email,
                         Contact = c.Contact,
                         Phone = c.Phone,
                         Address = c.Address,
                         PointLocation = c.PointLocation,
                         Latitude = c.Latitude,
                         Longitude = c.Longitude,
                         CountryCode = c.CountryCode,
                         CountryName = c.CountryName,
                         AdminArea = c.AdminArea,
                         SubAdminArea = c.SubAdminArea,
                         Locality = c.Locality,
                         SubLocality = c.SubLocality,
                         Thoroughfare = c.Thoroughfare,
                         SubThoroughfare = c.SubThoroughfare,
                         PostalCode = c.PostalCode,
                         IsActive = c.IsActive,
                         Descr = c.Descr,
                         BackColorBar = c.BackColorBar,
                         BackColorPage = c.BackColorPage,
                         Image1 = c.Image1,
                         Image2 = c.Image2,
                         ImageCard = c.ImageCard,
                         CompanyType = c.CompanyType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,

                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoCompany GetByCode(string code)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Companies
                     where c.Code == code
                     select new DtoCompany
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Email = c.Email,
                         Contact = c.Contact,
                         Phone = c.Phone,
                         Address = c.Address,
                         PointLocation = c.PointLocation,
                         Latitude = c.Latitude,
                         Longitude = c.Longitude,
                         CountryCode = c.CountryCode,
                         CountryName = c.CountryName,
                         AdminArea = c.AdminArea,
                         SubAdminArea = c.SubAdminArea,
                         Locality = c.Locality,
                         SubLocality = c.SubLocality,
                         Thoroughfare = c.Thoroughfare,
                         SubThoroughfare = c.SubThoroughfare,
                         PostalCode = c.PostalCode,
                         IsActive = c.IsActive,
                         Descr = c.Descr,
                         BackColorBar = c.BackColorBar,
                         BackColorPage = c.BackColorPage,
                         Image1 = c.Image1,
                         Image2 = c.Image2,
                         ImageCard = c.ImageCard,
                         CompanyType = c.CompanyType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,

                     }).FirstOrDefault();

                return result;
            }
        }

        public IEnumerable<JsonGenericData> GetAllGeneric(int type)
        {
            using (var db = new DrdContext(_connString))
            {

                var result =
                    (from c in db.Companies
                     where (type == -1 || c.CompanyType == type)
                     select new JsonGenericData
                     {
                         Id = c.Id,
                         Descr = c.Name,
                     }).ToList();

                return result;

            }
        }

        public IEnumerable<DtoCompanyLite> GetLiteAll(int type)
        {
            using (var db = new DrdContext(_connString))
            {

                var result =
                    (from c in db.Companies
                     where (type == -1 || c.CompanyType == type)
                     select new DtoCompanyLite
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Email = c.Email,
                         Name = c.Name,
                         Contact = c.Contact,
                         Phone = c.Phone,
                         DateCreated = c.DateCreated,
                         SubAdminArea = c.SubAdminArea,
                         CompanyType = c.CompanyType,
                         IsActive = c.IsActive,

                     }).ToList();

                return result;

            }
        }

        public IEnumerable<DtoCompanyLite> GetLiteAll(string topCriteria, int page, int pageSize)
        {
            return GetLiteAll(topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<DtoCompanyLite> GetLiteAll(string topCriteria, int page, int pageSize, string order)
        {
            return GetLiteAll(topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<DtoCompanyLite> GetLiteAll(string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "DateCreated desc";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Companies
                     where /*c.IsActive &&*/ (topCriteria == null
                            || tops.All(x => (c.Code + " " + c.Name + " " + c.Contact + " " + c.SubAdminArea + " " + c.Phone).Contains(x)))
                     select new DtoCompanyLite
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Email = c.Email,
                         Name = c.Name,
                         Contact = c.Contact,
                         Phone = c.Phone,
                         DateCreated = c.DateCreated,
                         SubAdminArea = c.SubAdminArea,
                         CompanyType = c.CompanyType,
                         IsActive = c.IsActive,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public long GetLiteAllCount(string topCriteria)
        {
            return GetLiteAllCount(topCriteria, null);
        }

        public long GetLiteAllCount(string topCriteria, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Companies
                     where /*c.IsActive &&*/ (topCriteria == null
                            || tops.All(x => (c.Code + " " + c.Name + " " + c.Contact + " " + c.SubAdminArea + " " + c.Phone).Contains(x)))
                     select new DtoCompanyLite
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        /// 

        public IEnumerable<JsonPermissionCompany> GetLitePermissionAll(string topCriteria, int page, int pageSize)
        {
            return GetLitePermissionAll(topCriteria, page, pageSize, null, null);
        }

        public IEnumerable<JsonPermissionCompany> GetLitePermissionAll(string topCriteria, int page, int pageSize, string order)
        {
            return GetLitePermissionAll(topCriteria, page, pageSize, order, null);
        }

        public IEnumerable<JsonPermissionCompany> GetLitePermissionAll(string topCriteria, int page, int pageSize, string order, string criteria)
        {
            int skip = pageSize * (page - 1);
            string ordering = "Name";

            if (!string.IsNullOrEmpty(order))
                ordering = order;

            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Companies
                     where c.IsActive && (topCriteria == null
                            || tops.All(x => (c.Code + " " + c.Name + " " + c.Contact + " " + c.SubAdminArea + " " + c.Phone).Contains(x)))
                     select new JsonPermissionCompany
                     {
                         Id = c.Id,
                         CompanyId = c.Id,
                         Name = c.Name,
                         Projects =
                            (from x in c.Projects
                             select new JsonPermissionProject
                             {
                                 Id = x.Id,
                                 ProjectId = x.Id,
                                 Name = x.Name,
                                 Workflows =
                                    (from y in x.Workflows
                                     select new JsonPermissionWorkflow
                                     {
                                         Id = y.Id,
                                         WorkflowId = y.Id,
                                         Name = y.Name,
                                         Rotations =
                                            (from z in y.Rotations
                                             select new JsonPermissionRotation
                                             {
                                                 Id = z.Id,
                                                 RotationId = z.Id,
                                                 Subject = z.Subject,
                                             }).ToList(),
                                     }).ToList(),
                             }).ToList(),
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();

                return result;

            }
        }

        public long GetLitePermissionAllCount(string topCriteria)
        {
            return GetLitePermissionAllCount(topCriteria, null);
        }

        public long GetLitePermissionAllCount(string topCriteria, string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                criteria = "1=1";

            // top criteria
            string[] tops = new string[] { };
            if (!string.IsNullOrEmpty(topCriteria))
                tops = topCriteria.Split(' ');
            else
                topCriteria = null;

            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Companies
                     where /*c.IsActive &&*/ (topCriteria == null
                            || tops.All(x => (c.Code + " " + c.Name + " " + c.Contact + " " + c.SubAdminArea + " " + c.Phone).Contains(x)))
                     select new JsonPermissionCompany
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }

        public string Save(DtoCompany prod)
        {
            LocationService locsvr = new LocationService();
            JsonAddressComponent addr = locsvr.GetAddressComponent((double)prod.Latitude, (double)prod.Longitude);

            Company product;
            ApplConfigService asvr = new ApplConfigService();
            ImageService isvr = new ImageService();
            QrBarcodeTools qrtool = new QrBarcodeTools();
            JsonDataImage dimage = new JsonDataImage();
            int result = 0;
            bool isgenerateqe = true;
            using (var db = new DrdContext(_connString))
            {
                if (prod.Id != 0)
                {
                    product = db.Companies.FirstOrDefault(c => c.Id == prod.Id);
                    prod.Code = product.Code;
                    prod.ImageQrCode = product.ImageQrCode;

                    if (string.IsNullOrEmpty(prod.ImageQrCode))
                    {
                        prod.ImageQrCode = "DRDQR-" + prod.Code + "-" + DateTime.Now.ToString("fffff") + ".png";
                        isgenerateqe = true;
                    }
                    else
                        isgenerateqe = false;
                }
                else {
                    product = new Company();
                    var emailcx = db.Companies.Count(c => c.Email == prod.Email);
                    if (emailcx != 0)
                        return "DBLEMAIL";
                }

                for (int i = 0; i < ConfigConstant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        if (prod.Id == 0)
                        {
                            product.Code = asvr.GenerateNumber<Company>("COMP_CODE", "Company", "Code", db.Companies);
                            product.ImageQrCode = "DRDQR-" + product.Code + "-" + DateTime.Now.ToString("ffff") + ".png";
                        }
                        product.Code = prod.Code;
                        product.Name = prod.Name;
                        product.Contact = prod.Contact;
                        product.Phone = prod.Phone;
                        product.Email = prod.Email;
                        product.CompanyType = prod.CompanyType;
                        product.Address = prod.Address;
                        product.PointLocation = addr.PointLocation();
                        product.Latitude = prod.Latitude;
                        product.Longitude = prod.Longitude;
                        product.CountryCode = addr.CountryCode();
                        product.CountryName = addr.CountryName();
                        product.AdminArea = addr.AdminArea();
                        product.SubAdminArea = addr.SubAdminArea();
                        product.Locality = addr.Locality();
                        product.SubLocality = addr.SubLocality();
                        product.Thoroughfare = addr.Thoroughfare();
                        product.SubThoroughfare = addr.SubThoroughfare();
                        product.PostalCode = addr.PostalCode();
                        product.Image1 = prod.Image1;
                        product.Image2 = prod.Image2;
                        product.ImageCard = prod.ImageCard;
                        product.BackColorBar = prod.BackColorBar;
                        product.BackColorPage = prod.BackColorPage;
                        product.Descr = prod.Descr;
                        product.ImageQrCode = prod.ImageQrCode;
                        product.IsActive = prod.IsActive;
                        product.UserId = prod.UserId;
                        if (prod.Id == 0)
                        {
                            product.DateCreated = DateTime.Now;
                            db.Companies.Add(product);
                        }
                        else
                            product.DateUpdated = DateTime.Now;

                        result = db.SaveChanges();

                        if (prod.Id == 0 || isgenerateqe)
                        {
                            dimage.TargetFolder = "/Images/company/qrbarcode/";
                            dimage.FileName = product.ImageQrCode;
                            dimage.Data = qrtool.Generate(product.Code);
                            isvr.UploadFoto(dimage);
                        }

                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > ConfigConstant.TEST_DUPLICATION_COUNT)
                            throw new Exception(x.Message);
                    }
                }

                return product.Code;

            }

        }

        public long Save(Company comp)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            LocationService locsvr = new LocationService();
            JsonAddressComponent addr = locsvr.GetAddressComponent((double)comp.Latitude, (double)comp.Longitude);

            ApplConfigService asvr = new ApplConfigService();
            ImageService isvr = new ImageService();
            QrBarcodeTools qrtool = new QrBarcodeTools();
            JsonDataImage dimage = new JsonDataImage();
            int result = 0;
            using (var db = new DrdContext(_connString))
            {
                for (int i = 0; i < ConfigConstant.TEST_DUPLICATION_COUNT; i++)
                {
                    try
                    {
                        comp.Code = asvr.GenerateNumber<Company>("COMP_CODE", "Company", "Code", db.Companies);
                        comp.ImageQrCode = "DRDQR-" + comp.Code + "-" + DateTime.Now.ToString("ffff") + ".png";
                        comp.PointLocation = addr.PointLocation();
                        comp.CountryCode = addr.CountryCode();
                        comp.CountryName = addr.CountryName();
                        comp.AdminArea = addr.AdminArea();
                        comp.SubAdminArea = addr.SubAdminArea();
                        comp.Locality = addr.Locality();
                        comp.SubLocality = addr.SubLocality();
                        comp.Thoroughfare = addr.Thoroughfare();
                        comp.SubThoroughfare = addr.SubThoroughfare();
                        comp.PostalCode = addr.PostalCode();
                        comp.UserId = "REG";
                        comp.DateCreated = DateTime.Now;
                        db.Companies.Add(comp);
                        result = db.SaveChanges();

                        dimage.TargetFolder = "/Images/company/qrbarcode/";
                        dimage.FileName = comp.ImageQrCode;
                        dimage.Data = qrtool.Generate(comp.Code);
                        isvr.UploadFoto(dimage);

                        break;
                    }
                    catch (DbUpdateException x)
                    {
                        if (i > ConfigConstant.TEST_DUPLICATION_COUNT)
                            throw new Exception(x.Message);
                    }
                }

                return comp.Id;
            }
        }

        public JsonSubscriptionRegistryResult SaveRegistration(JsonSubscriptionRegistry registry)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            var regResult = new JsonSubscriptionRegistryResult();

            string pwd = DateTime.Now.ToString("ffff");

            var db = new DrdContext(_connString);
            var result = db.Members.Where(c => c.Email.Equals(registry.MemberEmail)).ToList();
            if (result.Count != 0)
            {
                regResult.UserNumber = "DBLEMAIL";
                return regResult;
            }

            var result2 = db.Companies.Where(c => c.Email.Equals(registry.CompanyEmail)).ToList();
            if (result2.Count != 0)
            {
                regResult.SubscriptionId = "DBLEMAIL";
                return regResult;
            }

            ApplConfigService appsvr = new ApplConfigService();
            var topaz = appsvr.GetValue("APPL_NAME");
            var admName = appsvr.GetValue("EMAILUSERDISPLAY");
            var groupuser = appsvr.GetValue("DEF_GROUP_USER");

            CompanyService csvr = new CompanyService();
            Company comp = new Company();
            comp.Name = registry.CompanyName;
            comp.Contact = registry.MemberName;
            comp.Phone = registry.CompanyPhone;
            comp.Email = registry.CompanyEmail;
            comp.Address = registry.CompanyAddress;
            comp.PointLocation = registry.CompanyPointLocation;
            comp.Latitude = registry.Latitude;
            comp.Longitude = registry.Longitude;
            comp.CompanyType = 0;
            comp.Image1 = "default_panel.jpg";
            comp.Image2 = "default_logo.png";
            comp.BackColorBar = "#42a5f5";
            comp.BackColorPage = "#eceff1";
            long compId = csvr.Save(comp);

            MemberService svr = new MemberService();
            Member mem = new Member();
            mem.Email = registry.MemberEmail;
            mem.MemberTitleId = registry.MemberTitleId;
            mem.Name = registry.MemberName.Trim();
            mem.Phone = registry.MemberPhone;
            mem.Password =  XEncryptionHelper.Encrypt(pwd);
            mem.MemberType = 1;
            mem.CompanyId = compId;
            mem.UserGroup = groupuser;
            long memId = svr.Save(mem);

            MemberPlanService plan = new MemberPlanService();
            plan.Save(memId, registry.SubscriptTypeId);

            JsonMemberRegister reg = new JsonMemberRegister();
            reg.Name = mem.Name;
            reg.Number = mem.Number;
            reg.Email = mem.Email;
            reg.Password = pwd;
            svr.sendEmailRegistration(reg);

            regResult.SubscriptionId = comp.Code;
            regResult.UserNumber = mem.Number;
            regResult.Email = registry.MemberEmail;
            return regResult;
        }

    }
}
