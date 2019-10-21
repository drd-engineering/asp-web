using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Serialization;
using DRD.Domain;
using DRD.Core;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Web.Controllers
{
    public class FaspayApiController : ApiController
    {
        private async Task<Domain.faspay> GetFaspayRequestObjectAsync(HttpRequestMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(Domain.faspay));
                using (var stream = await message.Content.ReadAsStreamAsync())
                {
                    var faspayRequest = (Domain.faspay)xmlSerializer.Deserialize(stream);
                    return faspayRequest;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ValidateFaspayRequest(faspay faspay)
        {
            FaspayData fpdata = new FaspayData();
            FaspayDataService fpdataService = new FaspayDataService();
            fpdata = fpdataService.GetData("Debit");

            try
            {
                if (faspay == null) throw new ArgumentNullException("faspay");
                if (string.IsNullOrWhiteSpace(faspay.trx_id)) return false;
                if (string.IsNullOrWhiteSpace(faspay.bill_no)) return false;
                //if (string.IsNullOrWhiteSpace(faspay.payment_total)) return false;
                //if (string.IsNullOrWhiteSpace(faspay.amount)) return false;
                var merchantId = fpdata.MerchantId;
                if (faspay.merchant_id != merchantId) return false;
                //decimal amount;
                ////var isAmountSuccess = decimal.TryParse(faspay.payment_total, out amount);
                //var isAmountSuccess = decimal.TryParse(faspay.amount, out amount);
                //if (!isAmountSuccess) return false;
                //if (amount <= 0) return false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ValidateSignature(Domain.faspay faspay, string userId, string password)
        {
            if (faspay == null) throw new ArgumentNullException("faspay");

            try
            {
                var bill_no = faspay.bill_no;
                var payment_status_code = faspay.payment_status_code;

                var toHash = userId + password + bill_no + payment_status_code;
                toHash = FaspayUtils.GetMd5HashString2(toHash);
                var finalHash = FaspayUtils.GetSha1HashString2(toHash);

                var signature = faspay.signature.ToLower();
                return signature.Equals(finalHash, StringComparison.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Domain.Response.faspay GetResponse(Domain.faspay faspayRequest)
        {
            if (faspayRequest == null) throw new ArgumentNullException("faspayRequest");
            try
            {
                var response = new Domain.Response.faspay
                {
                    response = "Payment Notification",
                    trx_id = faspayRequest.trx_id,
                    merchant_id = faspayRequest.merchant_id,
                    bill_no = faspayRequest.bill_no,
                    response_code = "00",
                    response_desc = "Sukses",
                    //response_date = DateTime.Now.ToOpDateTime().ToString("yyyy-MM-dd HH:mm:ss")
                    response_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("api/faspay/cbdebit")]
        public async Task<IHttpActionResult> AutocallbackDebit()//Post()
        {
            FaspayData fpdata = new FaspayData();
            FaspayDataService fpdataService = new FaspayDataService();
            fpdata = fpdataService.GetData("Debit");

            try
            {
                var faspayRequest = await GetFaspayRequestObjectAsync(Request);
                var validateSuccess = ValidateFaspayRequest(faspayRequest);
                if (!validateSuccess) return BadRequest("Not a valid faspay request");

                //var userId = fpdata.UserId;
                //var userPwd = fpdata.Password;
                var validateSignature = ValidateSignature(faspayRequest, fpdata.UserId, fpdata.Password);
                if (!validateSignature) return BadRequest("Not a valid signature");

                if (string.IsNullOrWhiteSpace(faspayRequest.bill_no))
                    return BadRequest("Not a valid faspay request - bill_no is empty");

                if (faspayRequest.merchant_id != fpdata.MerchantId)
                    return BadRequest("Not a valid faspay request - invalid merchant_id");

                using (var db = new DrdContext(ConfigConstant.CONSTRING))
                {
                    FaspayDebitStatu entity = new FaspayDebitStatu();

                    entity.Request = faspayRequest.request;
                    entity.TrxId = faspayRequest.trx_id;
                    entity.MerchantId = faspayRequest.merchant_id;
                    entity.Merchant = faspayRequest.merchant;
                    entity.BillNo = faspayRequest.bill_no;
                    entity.PaymentReff = faspayRequest.payment_reff;
                    entity.PaymentDate = faspayRequest.payment_date;
                    entity.PaymentStatusCode = faspayRequest.payment_status_code;
                    entity.PaymentStatusDesc = faspayRequest.payment_status_desc;
                    entity.Signature = faspayRequest.signature;
                    //entity.Amount = faspayRequest.amount;
                    //entity.PaymentTotal = faspayRequest.payment_total;

                    db.FaspayDebitStatus.Add(entity);
                    var result = db.SaveChanges();

                    if (faspayRequest.payment_status_code.Equals("2"))
                    {
                        JsonFaspayPayment faspayment = new JsonFaspayPayment();
                        FaspayPaymentService faspaymentSvr = new FaspayPaymentService();
                        PaymentMethodService methodSvr = new PaymentMethodService();
                        faspayment = faspaymentSvr.GetByPayId(long.Parse(faspayRequest.bill_no));
                        int methodId = methodSvr.GetIdByCode(faspayment.PayType);

                        if (faspayment.TrxType.Contains("TU"))
                        {
                            //MemberTopupService topup = new MemberTopupService(ConfigConstant.CONSTRING);
                            //topup.UpdateStatus(faspayment.TrxId, "02", -methodId, 0);

                            MemberTopupDepositService mtdsvr = new MemberTopupDepositService();
                            DtoMemberTopupDeposit jsontd = mtdsvr.GetById(faspayment.TrxId);
                            BankService bsvr = new BankService();
                            var bank = bsvr.GetByCode(faspayment.PayType.Split('|')[1]);

                            var cb = db.CompanyBanks.FirstOrDefault(c => c.PaymentMethodId == methodId && c.BankId == bank.Id);

                            MemberTopupPaymentService mtpsvr = new MemberTopupPaymentService();
                            DtoMemberTopupPayment jsonpay = new DtoMemberTopupPayment();
                            jsonpay.CompanyBankId = cb.Id;// - methodId;
                            jsonpay.TopupDepositId = faspayment.TrxId;
                            jsonpay.Amount = jsontd.Amount;
                            jsonpay.PaymentStatus = "02";
                            mtpsvr.Save(jsonpay);
                        }
                        //else if (faspayment.TrxType.Equals("RO"))
                        //{
                        //    PaymentOrderService posvr = new PaymentOrderService("VTIAPIKlaxonWebContext");
                        //    posvr.UpdateConfirmed(faspayment.TrxId);
                        //}
                        //else if (faspayment.TrxType.Equals("RQ"))
                        //{
                        //    PaymentOrderService posvr = new PaymentOrderService("VTIAPIKlaxonWebContext");
                        //    posvr.UpdateConfirmed(faspayment.TrxId);
                        //}
                    }



                }

                // baut log
                // end email

                //var isTransLogExistAsync = await IsTransLogExistAsync(faspayRequest.trx_id);
                //if (isTransLogExistAsync)
                //{
                //    return new ResponseMessageResult(Request.CreateResponse((HttpStatusCode)444, "No Response"));
                //}

                //var saveTransLogSuccess = await SavePgTransLogToDbAsync(Request, faspayRequest);
                //if (!saveTransLogSuccess)
                //{
                //    return new ResponseMessageResult(Request.CreateResponse((HttpStatusCode)444, "No Response"));
                //}

                //var saveDepositLogSuccess = await TopupDepositAsync(faspayRequest);
                //if (!saveDepositLogSuccess)
                //{
                //    return new ResponseMessageResult(Request.CreateResponse((HttpStatusCode)444, "No Response"));
                //}

                var response = GetResponse(faspayRequest);
                return Content(HttpStatusCode.OK, response, Configuration.Formatters.XmlFormatter);
            }
            catch (Exception x)
            {
                var i = x;
                return InternalServerError();
            }
        }
    }

}
