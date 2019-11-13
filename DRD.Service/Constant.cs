namespace DRD.Service
{
    public class Constant
    {
        public const string CONSTRING = "ServiceContext";
        public const string ENCRYPT_DECRYPT_SALT = "50b25ef599144db2953b656f11a84139";
        public const string ENCRYPT_DECRYPT_PWD = "drdw1w1rjang";
        public const string ENCRYPT_KEY = "drdtul@l1t3";
        public const string INIT_LOGIN = "_init_login_xbudi_";
        public const int LOOP_TRY_SAVE = 10;
        public const int LOOP_TRY_SAVE_THROW = 6;
        public const int DATA_TEMP3 = 2020;
        public const int DATA_TEMP2 = 2;
        public const int DATA_TEMP1 = 10;
        public const int TEST_DUPLICATION_COUNT = 10;
        public const string CONSTRING_USER = "DrdUserContext";
        public const string API_KEY = "AIzaSyB5y7e2nuBQ4OFE257snefNT8XorZVIGYY";
        
        /// Constructor
        public Constant(){}
        
        public enum EnumContentType
        {
            ARTICLE = 1,
            EVENT = 2,
            PROFILE = 4,
            INFORMATION = 8
        }
        public enum EnumActivityAction
        {
            SUBMIT = 1,
            REJECT = 2,
            REVISI = 4,
            ALTER = 8
        }
        public enum EnumDocumentAction
        {
            SIGN = 1,
            REVISI = 2,
            VIEW = 4,
            PRINT = 8,
            DOWNLOAD = 16,
            PRIVATESTAMP = 32
        }
        public enum EnumDataHit
        {
            NEWS = 0,
            VIDEO = 1,
            PODCAST = 2,
            BANNER = 3,
            PARTNERPROMO = 4
        }
        public enum EnumInvitationStatus
        {
            REJECTEDBYMEMBER = 0,
            REJECTEDBYCOMPANY = 1,
            WAITINGFORMEMBER = 2,
            WAITINGFORCOMPANY = 3,
            ACCEPTEDBYMEMBER = 4,
            ACCEPTEDBYCOMPANY = 5
        }
    }
}