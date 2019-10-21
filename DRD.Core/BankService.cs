using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Based.Core.Entity;
using System.Based.Core;

namespace DRD.Core
{
    public class BankService
    {
        private readonly string _connString;

        public BankService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public BankService(string connString)
        {
            _connString = connString;
        }

        public IEnumerable<DtoBank> GetAll()
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Banks
                     orderby c.Name
                     select new DtoBank
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         BankType = c.BankType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).ToList();

                return result;
            }
        }

        public DtoBank GetById(int id)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Banks
                     where c.Id == id
                     select new DtoBank
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         BankType = c.BankType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).FirstOrDefault();

                return result;
            }
        }

        public DtoBank GetByCode(string code)
        {
            using (var db = new DrdContext(_connString))
            {
                var result =
                    (from c in db.Banks
                     where c.Code == code
                     select new DtoBank
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         BankType = c.BankType,
                         UserId = c.UserId,
                         DateCreated = c.DateCreated,
                         DateUpdated = c.DateUpdated,
                     }).FirstOrDefault();

                return result;
            }
        }
        public IEnumerable<DtoBank> GetLiteAll(string topCriteria, int page, int pageSize, string order, string criteria)
        {
            ApplConfigService.GenerateUniqueKeyLong();

            int skip = pageSize * (page - 1);
            string ordering = "Code";

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
                    (from c in db.Banks
                     where (c.BankType & 1) == 1 && (topCriteria == null || tops.All(x => (c.Name).Contains(x)))
                     select new DtoBank
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         Logo = c.Logo,
                         BankType = c.BankType,
                     }).Where(criteria).OrderBy(ordering).Skip(skip).Take(pageSize).ToList();


                return result;

            }
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
                    (from c in db.Banks
                     where (c.BankType & 1) == 1 && (topCriteria == null || tops.All(x => (c.Name).Contains(x)))
                     select new DtoBank
                     {
                         Id = c.Id,
                     }).Where(criteria).Count();

                return result;

            }
        }


    }
    /*
    <? php
      $tranid = date("YmdGis");
      $signaturecc=sha1('##'.strtoupper('mandiri_advance').'##'.strtoupper('nwdvj').'##'.$tranid.'##50000.00##'.'0'.'##');
      //$signaturecc1= '##'.strtoupper(mandiri_lavendermart).'##'.strtoupper(ehhvq).'##'.$tranid.'##50000.00##'.'0'.'##';
	  $post = array(
                  "LANG" 					=> '',
                  "MERCHANTID"              => 'mandiri_advance',    // MERCHANT ID                            	
                  "PAYMENT_METHOD"			=> '1',
				  "TXN_PASSWORD" 			=> 'nwdvj', //Transaction password 
                  "MERCHANT_TRANID"			=> $tranid,
                  "CURRENCYCODE"			=> 'IDR',
                  "AMOUNT"					=> '50000.00',
                  "CUSTNAME"				=> 'TAS',
                  "CUSTEMAIL"				=> 'hanna.panjaitan@faspay.co.id',
                  "DESCRIPTION"             => 'tas',
                  "RETURN_URL"              => 'http://localhost/sample/CreditCard/merchant_return_page.php',
                  "SIGNATURE" 			 	=> $signaturecc,
                  "BILLING_ADDRESS"			=> '',
                  "BILLING_ADDRESS_CITY"			 => '',
                  "BILLING_ADDRESS_REGION"		=> '',
                  "BILLING_ADDRESS_STATE"		=> '',
                  "BILLING_ADDRESS_POSCODE"		=> '',
                  "BILLING_ADDRESS_COUNTRY_CODE"    => '',
                  "RECEIVER_NAME_FOR_SHIPPING"		=> '',
                  "SHIPPING_ADDRESS" 			=> '',
                  "SHIPPING_ADDRESS_CITY" 		=> '',
                  "SHIPPING_ADDRESS_REGION"		=> '',
                  "SHIPPING_ADDRESS_STATE"		=> '',
                  "SHIPPING_ADDRESS_POSCODE"		=> '',
                  "SHIPPING_ADDRESS_COUNTRY_CODE"	=> '',
                  "SHIPPINGCOST"			=> '0.00',
                  "PHONE_NO" 				=> '',
                  "MREF1"				=> '',
                  "MREF2" 				=> '',
                  "MREF3"				=> '',
                  "MREF4"				=> '',
                  "MREF5"				=> '',
                  "MREF6"				=> '',
                  "MREF7"				=> '',
                  "MREF8"				=> '',
                  "MREF9"				=> '',
                  "MREF10"				=> '',
                  "MPARAM1" 				=> 'tes',
                  "MPARAM2" 				=> 'test lagi aja',
                  "CUSTOMER_REF"	 		=> '',
                  "PYMT_IND"    		=> '',
				  "PYMT_CRITERIA"   	=> '',
                  "FRISK1"				=> '',
                  "FRISK2"				=> '',
                  "DOMICILE_ADDRESS"			=> '',
                  "DOMICILE_ADDRESS_CITY"		=> '',
                  "DOMICILE_ADDRESS_REGION"		=> '',
                  "DOMICILE_ADDRESS_STATE"		=> '',
                  "DOMICILE_ADDRESS_POSCODE" 		=> '',
                  "DOMICILE_ADDRESS_COUNTRY_CODE"	=> '',
                  "DOMICILE_PHONE_NO"	 		=> '',
                  "handshake_url"			=> '',
                  "handshake_param"			=> '',
    );*/

    /* $signaturenohash= '##'.strtoupper('cimb_itc').'##'.strtoupper('ibnyw').'##'.$tranid.'##50000.00##'.'0'.'##';	
	echo "$signaturenohash";
	var_dump($post);exit; */
    /*
	$string = '<form method="post" name="form" action="https://ucdev.faspay.co.id/payment/PaymentWindow.jsp">';  // yang diubah URLnya ke prod apa dev
	if ($post != null) {
        foreach ($post as $name=>$value) {
			$string .= '<input type="hidden" name="'.$name.'" value="'.$value.'">';
		}
	}

	$string .= '</form>';
	$string .= '<script> document.form.submit();</script>';

	echo $string;
	exit;


    ?>*/
}
