using System.Runtime.Serialization;

namespace DRD.Domain
{
    /* from faspay staf
    <faspay>
    <request>Payment Notification</request>
    <trx_id>111222</trx_id>
    <merchant_id>111111</merchant_id>
    <merchant>ABC, PT</merchant>
    <bill_no>12345678</bill_no>
    <payment_reff>1234567</payment_reff>
    <payment_date>2012-01-01 15:17:30</payment_date>
    <payment_status_code></payment_status_code>
    <payment_status_desc>Description status</payment_status_desc>
    <signature>Description status</signature>
    <amount>100000</amount>
    </faspay>

     responya:
    <faspay>
    <response>Payment Notification</response>
    <trx_id>111222</trx_id>
    <merchant_id>111111</merchant_id>
    <bill_no></bill_no>
    <response_code>00</response_code>
    <response_desc>Sukses</response_desc>
    <response_date>2012-01-01 15:17:30</response_date>
    </faspay>
    */
    public class faspay
    {
        public string request { get; set; }
        public string trx_id { get; set; }
        public string merchant_id { get; set; }
        public string merchant { get; set; }
        public string bill_no { get; set; }
        public string payment_reff { get; set; }
        public string payment_date { get; set; }
        public string payment_status_code { get; set; }
        public string payment_status_desc { get; set; }
        public string signature { get; set; }
        public string amount { get; set; }
        //public string payment_total { get; set; }
    }
}

namespace DRD.Domain.Response
{
    [DataContract]
    public class faspay
    {
        [DataMember]
        public string bill_no { get; set; }

        [DataMember]
        public string merchant_id { get; set; }

        [DataMember]
        public string response { get; set; }

        [DataMember]
        public string response_code { get; set; }

        [DataMember]
        public string response_date { get; set; }

        [DataMember]
        public string response_desc { get; set; }

        [DataMember]
        public string trx_id { get; set; }
    }
}
