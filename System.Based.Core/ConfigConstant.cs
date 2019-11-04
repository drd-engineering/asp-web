using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Based.Core
{
    public class ConfigConstant
    {
        public const string CONSTRING = "DrdContext";
        public const string CONSTRING_USER = "DrdUserContext";
        public const int TEST_DUPLICATION_COUNT = 10;
        public const int DATA_TEMP1 = 10;
        public const int DATA_TEMP2 = 2;
        public const int DATA_TEMP3 = 2020;
        public const string API_KEY = "AIzaSyB5y7e2nuBQ4OFE257snefNT8XorZVIGYY";
        public const int LOOP_TRY_SAVE = 10;
        public const int LOOP_TRY_SAVE_THROW = 6;
        public const string INIT_LOGIN = "_init_login_xbudi_";
        public const string ENCRYPT_KEY = "drdtul@l1t3";

        public const string ENCRYPT_DECRYPT_PWD = "drdw1w1rjang";
        public const string ENCRYPT_DECRYPT_SALT = "50b25ef599144db2953b656f11a84139";


        public enum EnumActivityAction
        {
            SUBMIT = 1, REJECT = 2, REVISI = 4, ALTER = 8
        }

        public enum EnumDocumentAction
        {
            SIGN = 1, REVISI = 2, VIEW = 4, PRINT = 8, DOWNLOAD = 16, PRIVATESTAMP = 32
        }
        public enum enumContentType
        {
            ARTICLE = 1, EVENT = 2, PROFILE = 4, INFORMATION = 8
        }
        public enum enumDataHit
        {
            NEWS, VIDEO, PODCAST, BANNER, PARTNERPROMO
        }

    }
}
