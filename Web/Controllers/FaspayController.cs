using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DRD.Core;
using DRD.Domain;
using System.Security.Cryptography;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

using System.Based.Core;
using System.Based.Core.Entity;

namespace DRD.Web.Controllers
{
    public class FaspayController : Controller
    {
        public ActionResult TestPayment()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CreditStatus(
            string PAYMENT_METHOD, string ERR_CODE, string ERR_DESC, string USR_CODE, string USR_MSG,
            string CUSTNAME, string DESCRIPTION, string CURRENCYCODE, string CUSTEMAIL,
            string AMOUNT, string MERCHANT_TRANID, string BANK_RES_CODE, string TXN_STATUS)
        {

            ViewBag.PAYMENT_METHOD = PAYMENT_METHOD;
            ViewBag.ERR_CODE = ERR_CODE;
            ViewBag.ERR_DESC = ERR_DESC;
            ViewBag.USR_CODE = USR_CODE;
            ViewBag.USR_MSG = USR_MSG;
            ViewBag.AMOUNT = AMOUNT;
            ViewBag.MERCHANT_TRANID = MERCHANT_TRANID;
            ViewBag.CUSTNAME = CUSTNAME;
            ViewBag.DESCRIPTION = DESCRIPTION;
            ViewBag.CURRENCYCODE = CURRENCYCODE;
            ViewBag.BANK_RES_CODE = BANK_RES_CODE;
            ViewBag.TXN_STATUS = TXN_STATUS;

            //var sts = 90;
            //if (ERR_CODE.Equals("0"))
            //{
            //    //mail.SendMail("xbudim@gmail.com", emailTo, "GOLDEN RAMA TICKET: Success", "Success");
            //}
            //else
            //{
            //    sts = 99;
            //    //mail.SendMail("xbudim@gmail.com", emailTo, "GOLDEN RAMA TICKET: Success", "Success");
            //}

            cbcredit(PAYMENT_METHOD, ERR_CODE, ERR_DESC, USR_CODE, USR_MSG,
                 CUSTNAME, DESCRIPTION, CURRENCYCODE, CUSTEMAIL,
                 AMOUNT, MERCHANT_TRANID, BANK_RES_CODE, TXN_STATUS);

            return View();
        }

        public int cbcredit(
            string PAYMENT_METHOD, string ERR_CODE, string ERR_DESC, string USR_CODE, string USR_MSG,
            string CUSTNAME, string DESCRIPTION, string CURRENCYCODE, string CUSTEMAIL,
            string AMOUNT, string MERCHANT_TRANID, string BANK_RES_CODE, string TXN_STATUS)
        {

            try
            {
                using (var db = new DrdContext(ConfigConstant.CONSTRING))
                {
                    FaspayCreditStatu entity = new FaspayCreditStatu();
                    entity.PaymentMethod = PAYMENT_METHOD;
                    entity.MerchantTranid = MERCHANT_TRANID;
                    entity.ErrCode = ERR_CODE;
                    entity.ErrDesc = ERR_DESC;
                    entity.UsrCode = USR_CODE;
                    entity.UsrMsg = USR_MSG;
                    entity.TxnStatus = TXN_STATUS;
                    entity.Custname = CUSTNAME;
                    entity.Description = DESCRIPTION;
                    entity.Currencycode = CURRENCYCODE;
                    entity.Amount = decimal.Parse(AMOUNT);
                    entity.BankResCode = BANK_RES_CODE;
                    db.FaspayCreditStatus.Add(entity);
                    var result = db.SaveChanges();

                    FaspayData fpdata = new FaspayData();
                    FaspayDataService fpdataService = new FaspayDataService();
                    fpdata = fpdataService.GetData("Credit");

                    if (string.IsNullOrWhiteSpace(MERCHANT_TRANID)) return 0;
                    //if (AMOUNT == null || AMOUNT < 0) return 0;
                    var merchantId = fpdata.MerchantId;
                    //if (MERCHANTID != merchantId) return 0;

                    if (ERR_CODE.Equals("0") && BANK_RES_CODE.Equals("0") && TXN_STATUS.Equals("S"))
                    {
                        JsonFaspayPayment faspayment = new JsonFaspayPayment();
                        FaspayPaymentService faspaymentSvr = new FaspayPaymentService();
                        PaymentMethodService methodSvr = new PaymentMethodService();
                        faspayment = faspaymentSvr.GetByPayId(long.Parse(MERCHANT_TRANID));
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

                    return result;
                }
            }
            catch (ArgumentNullException e)
            {
                return 0;
            }
        }


        // tidak di pakai   
        public int cbcreditx(
            string PAYMENT_METHOD, string MERCHANTID, string MERCHANT_TRANID, string ERR_CODE,
            string ERR_DESC, string USR_CODE, string USR_MSG, string TXN_STATUS, string CUSTNAME, string DESCRIPTION,
            string CURRENCYCODE, decimal? AMOUNT, string SIGNATURE, string EUI, int? TRANSACTIONID,
            string TRANSACTIONTYPE, string MPARAM1, string MPARAM2, string ACQUIRER_ID, string TRANDATE,
            string IS_BLACKLISTED, int? FRAUDRISKLEVEL, decimal? FRAUDRISKSCORE, int? POINT_USED,
            decimal? POINT_AMOUNT, decimal? PAYMENT_AMOUNT, decimal? POINT_BALANCE, string EXCEED_HIGH_RISK,
            string CARDTYPE, string CARD_NO_PARTIAL, string CARDNAME, string is_on_us, string ACQUIRER_BANK,
            string WHITELIST_CARD, string BANK_RES_CODE, string BANK_RES_MSG, string AUTH_ID, string BANK_REFERENCE,
            string INSTALLMENT_CODE, string INSTALLMENT_TERM, decimal? INSTALLMENT_MONTHLY, decimal? INSTALLMENT_LAST)
        {

            try
            {
                using (var db = new DrdContext(ConfigConstant.CONSTRING))
                {
                    FaspayCreditStatu entity = new FaspayCreditStatu();
                    entity.PaymentMethod = PAYMENT_METHOD;
                    entity.Merchantid = MERCHANTID;
                    entity.MerchantTranid = MERCHANT_TRANID;
                    entity.ErrCode = ERR_CODE;
                    entity.ErrDesc = ERR_DESC;
                    entity.UsrCode = USR_CODE;
                    entity.UsrMsg = USR_MSG;
                    entity.TxnStatus = TXN_STATUS;
                    entity.Custname = CUSTNAME;
                    entity.Description = DESCRIPTION;
                    entity.Currencycode = CURRENCYCODE;
                    entity.Amount = AMOUNT;
                    entity.Signature = SIGNATURE;
                    entity.Eui = EUI;
                    entity.Transactionid = TRANSACTIONID;
                    entity.Transactiontype = TRANSACTIONTYPE;
                    entity.Mparam1 = MPARAM1;
                    entity.Mparam2 = MPARAM2;
                    entity.AcquirerId = ACQUIRER_ID;
                    entity.Trandate = TRANDATE;
                    entity.IsBlacklisted = IS_BLACKLISTED;
                    entity.Fraudrisklevel = FRAUDRISKLEVEL;
                    entity.Fraudriskscore = FRAUDRISKSCORE;
                    entity.PointUsed = POINT_USED;
                    entity.PointAmount = POINT_AMOUNT;
                    entity.PaymentAmount = PAYMENT_AMOUNT;
                    entity.PointBalance = POINT_BALANCE;
                    entity.ExceedHighRisk = EXCEED_HIGH_RISK;
                    entity.Cardtype = CARDTYPE;
                    entity.CardNoPartial = CARD_NO_PARTIAL;
                    entity.Cardname = CARDNAME;
                    entity.IsOnUs = is_on_us;
                    entity.AcquirerBank = ACQUIRER_BANK;
                    entity.WhitelistCard = WHITELIST_CARD;
                    entity.BankResCode = BANK_RES_CODE;
                    entity.BankResMsg = BANK_RES_MSG;
                    entity.AuthId = AUTH_ID;
                    entity.BankReference = BANK_REFERENCE;
                    entity.InstallmentCode = INSTALLMENT_CODE;
                    entity.InstallmentTerm = INSTALLMENT_TERM;
                    entity.InstallmentMonthly = INSTALLMENT_MONTHLY;
                    entity.InstallmentLast = INSTALLMENT_LAST;
                    db.FaspayCreditStatus.Add(entity);
                    var result = db.SaveChanges();

                    FaspayData fpdata = new FaspayData();
                    FaspayDataService fpdataService = new FaspayDataService();
                    fpdata = fpdataService.GetData("Credit");

                    if (string.IsNullOrWhiteSpace(MERCHANT_TRANID)) return 0;
                    if (AMOUNT == null || AMOUNT < 0) return 0;
                    var merchantId = fpdata.MerchantId;
                    if (MERCHANTID != merchantId) return 0;

                    if (ERR_CODE.Equals("0") && BANK_RES_CODE.Equals("0") && TXN_STATUS.Equals("S"))
                    {
                        JsonFaspayPayment faspayment = new JsonFaspayPayment();
                        FaspayPaymentService faspaymentSvr = new FaspayPaymentService();
                        PaymentMethodService methodSvr = new PaymentMethodService();
                        faspayment = faspaymentSvr.GetByPayId(long.Parse(MERCHANT_TRANID));
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

                    return result;
                }
            }
            catch (ArgumentNullException e)
            {
                return 0;
            }
        }


        public ActionResult DebitStatus(string merchant_id, string bill_no, string trx_id,
            string bill_reff, string bill_total, string payment_reff, string payment_date,
            string bank_user_name, string Status, string bill_amount)
        {

            ViewBag.merchant_id = merchant_id;
            ViewBag.bill_no = bill_no;
            ViewBag.trx_id = trx_id;
            ViewBag.bill_reff = bill_reff;
            ViewBag.bill_total = bill_total;
            ViewBag.payment_reff = payment_reff;
            ViewBag.payment_date = payment_date;
            ViewBag.bank_user_name = bank_user_name;
            ViewBag.Status = Status;
            ViewBag.bill_amount = bill_amount;

            return View();
        }

        public async Task Payment(string key)// long trxId, string trxType, long companyBankId)
        // decimal invoiceId, string invoiceNo, string trxType, string type, decimal amount, string descr, string bank, string name, string email)
        {
            //http://localhost:50590/Faspay/Payment?invoiceId=123&invoiceNo=RO9389900&type=CC&amount=54000&descr=Bayar Rental Mobil&bank=BCA&name=Budi Mulyana&email=xbudi@yahoo.com

            string[] keys = key.Split(',');

            string trxId = keys[0];
            string trxType = keys[1];
            string companyBankId = keys[2];

            long invoiceId = 0;
            string invoiceNo = "";
            string type = "";
            decimal amount = 0;
            string descr = "";
            string accNo = "";
            string bank = "";
            string custno = "";
            string phone = "";
            string name = "";
            string email = "";
            if (trxType.Equals("MTU"))
            {
                MemberTopupDepositService mtdsvr = new MemberTopupDepositService();
                var topup = mtdsvr.GetById(trxId);
                invoiceId = topup.Id;
                invoiceNo = topup.TopupNo;
                amount = Math.Round(topup.Amount, 2, MidpointRounding.ToEven);
                descr = "Topup deposit";
                custno = topup.Member.Number;
                phone = topup.Member.Phone;
                name = topup.Member.Name;
                email = topup.Member.Email;
                CompanyBankService cbsvr = new CompanyBankService();
                var cb = cbsvr.GetById(companyBankId);
                type = cb.PaymentMethod.Code;
                accNo = cb.AccountNo;
                bank = cb.Bank.Code;
            }


            JsonFaspayPayment faspayment = new JsonFaspayPayment();
            FaspayPaymentService faspaymentSvr = new FaspayPaymentService();
            faspayment.PayType = type + "|" + bank;
            faspayment.TrxId = invoiceId;
            faspayment.TrxNo = invoiceNo;
            faspayment.TrxType = trxType;
            faspayment = faspaymentSvr.Save(faspayment);

            PaymentParam pparam = new PaymentParam();
            //pparam.OrderNo = "RO" + DateTime.Now.ToString("yyyyMMddHHmmss");// orderno;
            pparam.InvoiceId = invoiceId;
            pparam.InvoiceNo = faspayment.PayId.ToString();// invoiceNo + "-TX" + trxType + invoiceId + "-" + type + "-" + DateTime.Now.ToString("HHmmssfff");
            pparam.Type = type;
            pparam.Amount = amount;
            pparam.AccountNo = accNo;
            pparam.Bank = bank;
            pparam.CustNo = custno;
            pparam.FullName = name;
            pparam.EMail = email;
            pparam.Phone = phone;
            pparam.Descr = descr;
            pparam.TrxType = trxType;

            if (type == "CC")
                CreditCard(pparam);
            else if (type == "VA")
                await VirtualAccount(pparam);
            //else if (type == "BCAKP")
            //    await BCAKlikPay(pparam);
            //else if (type == "KBCA")
            //    await KlikBca(pparam);
            //else if (type == "MDRCP")
            //    await MandiriClickPay(pparam);
            //else if (type == "MDRATM")
            //    await ATMMandiri(pparam);
        }

        public void CreditCard(PaymentParam pparam)
        {
            FaspayData fpdata = new FaspayData();
            FaspayDataService fpdataService = new FaspayDataService();
            fpdata = fpdataService.GetData("Credit");
            var merchid = fpdata.MerchantId;
            var pwd = fpdata.Password;
            var amount = pparam.Amount;
            var tranid = pparam.InvoiceNo;

            var signaturecc1 = "##" + merchid.ToUpper() + "##" + pwd.ToUpper() + "##" + tranid + "##" + pparam.Amount.ToString("##0.00").Replace(",", ".") + "##0##";
            var signaturecc = fpdataService.Hash(signaturecc1);

            var parms = new Dictionary<string, string>();

            parms["LANG"] = "";
            parms["MERCHANTID"] = merchid;
            parms["PAYMENT_METHOD"] = "1";
            parms["TXN_PASSWORD"] = pwd;
            parms["MERCHANT_TRANID"] = tranid;
            parms["CURRENCYCODE"] = "IDR";
            parms["AMOUNT"] = pparam.Amount.ToString("##0.00").Replace(",", ".");
            parms["CUSTNAME"] = pparam.FullName;
            parms["CUSTEMAIL"] = pparam.EMail;
            parms["DESCRIPTION"] = pparam.Descr;
            parms["RETURN_URL"] = Request.Url.OriginalString.Replace(Request.Url.PathAndQuery, "") + "/Faspay/CreditStatus";
            parms["SIGNATURE"] = signaturecc;
            parms["BILLING_ADDRESS"] = "";
            parms["BILLING_ADDRESS_CITY"] = "";
            parms["BILLING_ADDRESS_REGION"] = "";
            parms["BILLING_ADDRESS_STATE"] = "";
            parms["BILLING_ADDRESS_POSCODE"] = "";
            parms["BILLING_ADDRESS_COUNTRY_CODE"] = "";
            parms["RECEIVER_NAME_FOR_SHIPPING"] = "";
            parms["SHIPPING_ADDRESS"] = "";
            parms["SHIPPING_ADDRESS_CITY"] = "";
            parms["SHIPPING_ADDRESS_REGION"] = "";
            parms["SHIPPING_ADDRESS_STATE"] = "";
            parms["SHIPPING_ADDRESS_POSCODE"] = "";
            parms["SHIPPING_ADDRESS_COUNTRY_CODE"] = "0.00";
            parms["SHIPPINGCOST"] = "";
            parms["PHONE_NO"] = "";
            parms["MREF1"] = "";
            parms["MREF2"] = "";
            parms["MREF3"] = "";
            parms["MREF4"] = "";
            parms["MREF5"] = "";
            parms["MREF6"] = "";
            parms["MREF7"] = "";
            parms["MREF8"] = "";
            parms["MREF9"] = "";
            parms["MREF10"] = "";
            parms["MPARAM1"] = "";
            parms["MPARAM2"] = "";
            parms["CUSTOMER_REF"] = "";

            // for development
            String PYMT_IND = "";
            String PYMT_CRITERIA = "";
            if (fpdata.IsProduction)
            {
                // for production
                PYMT_IND = "card_range_ind";
                PYMT_CRITERIA = "card_bin_3D_Indo";
            }

            parms["PYMT_IND"] = PYMT_IND;
            parms["PYMT_CRITERIA"] = PYMT_CRITERIA;

            parms["FRISK1"] = "";
            parms["FRISK2"] = "";
            parms["DOMICILE_ADDRESS"] = "";
            parms["DOMICILE_ADDRESS_CITY"] = "";
            parms["DOMICILE_ADDRESS_REGION"] = "";
            parms["DOMICILE_ADDRESS_STATE"] = "";
            parms["DOMICILE_ADDRESS_POSCODE"] = "";
            parms["DOMICILE_ADDRESS_COUNTRY_CODE"] = "";
            parms["DOMICILE_PHONE_NO"] = "";
            parms["handshake_url"] = "";
            parms["handshake_param"] = "";

            // for development
            string url = "https://fpgdev.faspay.co.id/payment";//"https://ucdev.faspay.co.id/payment/PaymentWindow.jsp";// "https://fpgdev.faspay.co.id/payment";// yang lama "https://ucdev.faspay.co.id/payment/PaymentWindow.jsp";
            if (fpdata.IsProduction)
            {
                // for production
                url = "https://fpg.faspay.co.id/payment";// yang lama "https://uc.faspay.co.id/payment/PaymentWindow.jsp";
            }

            Response.Write("<form method=\"post\" name=\"form\" action=\"" + url + "\">");
            foreach (var prm in parms)
            {
                Response.Write("<input type=\"hidden\" name=\"" + prm.Key + "\" value=\"" + prm.Value + "\">");
            }
            Response.Write("</form><script> document.form.submit();</script>");
        }

        public async Task<string> VirtualAccount(PaymentParam pparam)
        {
            FaspayData fpdata = new FaspayData();
            FaspayDataService fpdataService = new FaspayDataService();
            MD5 md5Hash = MD5.Create();
            fpdata = fpdataService.GetData("Debit");

            var date = DateTime.Now;// DateTimeHelpers.ToOpDateTime(DateTime.UtcNow);
            var merchid = fpdata.MerchantId;
            var userid = fpdata.UserId;
            var pwd = fpdata.Password;
            var tranid = pparam.InvoiceNo;

            var signaturecc1 = userid + pwd + tranid;
            signaturecc1 = FaspayUtils.GetMd5HashString2(signaturecc1);
            var signaturecc = FaspayUtils.GetSha1HashString2(signaturecc1);

            string obj = "";

            using (StreamReader sr = new StreamReader(Server.MapPath("~/assets/xml/faspay_debit.xml")))
            {
                obj = sr.ReadToEnd();
                obj = obj.Replace("_MERCHANT_ID_", merchid);
                obj = obj.Replace("_BILL_NO_", tranid);
                obj = obj.Replace("_SIGNATURE_", signaturecc);
                obj = obj.Replace("_PAYMENT_CHANNEL_", pparam.AccountNo);
                obj = obj.Replace("_BILL_DATE_", date.ToString("yyyy-MM-dd HH:mm:ss"));
                obj = obj.Replace("_BILL_EXP_", date.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"));

                //------------------------------ harus tambah 00
                obj = obj.Replace("_AMOUNT_", pparam.Amount.ToString("##0") + "00");
                //------------------------------ /harus tambah 00

                obj = obj.Replace("_DESCR_", pparam.Descr);
                obj = obj.Replace("_PHONE_", pparam.Phone);
                obj = obj.Replace("_CUST_NO_", pparam.CustNo);
                obj = obj.Replace("_CUST_NAME_", pparam.FullName);
                obj = obj.Replace("_EMAIL_", pparam.EMail);
            }

            using (var client = new HttpClient())
            {
                // development
                string baseUri = "https://dev.faspay.co.id";// "http://faspaydev.mediaindonusa.com/";
                if (fpdata.IsProduction)
                {
                    // production
                    baseUri = "https://web.faspay.co.id";// "https://faspay.mediaindonusa.com/";
                }
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

                var request = new HttpRequestMessage();
                request.Content = new StringContent(obj, Encoding.UTF8, "text/xml");

                // development
                string url = "pws/300011/183xx00010100000";// "pws/300002/183xx00010100000";
                if (fpdata.IsProduction)
                {
                    // production
                    url = "pws/300011/383xx00010100000";// "pws/300002/383xx00010100000";
                }
                var postResponse = await client.PostAsync(url, request.Content);
                postResponse.EnsureSuccessStatusCode();
                var responseAsString = await postResponse.Content.ReadAsStringAsync();


                var xml = new XmlDocument();
                xml.LoadXml(responseAsString);

                var trx_id = xml.GetElementsByTagName("trx_id");
                if (trx_id.Count == 0)
                {
                    Response.Write(responseAsString);
                    return "";
                }

                // development
                string redirect_url = "https://dev.faspay.co.id/pws/100003/0830000010100000/";// "http://faspaydev.mediaindonusa.com/pws/100003/0830000010100000/";
                if (fpdata.IsProduction)
                {
                    // production
                    redirect_url = "https://web.faspay.co.id/pws/100003/2830000010100000/";// "https://faspay.mediaindonusa.com/pws/100003/2830000010100000/";
                }
                redirect_url += signaturecc + "?trx_id=" + trx_id[0].InnerXml + "&merchant_id=" + merchid + "&bill_no=" + tranid; ;
                Response.Redirect(redirect_url);


                return responseAsString;
            }
        }
    }
}