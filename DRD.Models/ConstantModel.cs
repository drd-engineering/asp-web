using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DRD.Models
{
    public static class ConstantModel
    {
        public const string CONSTRING = "ServiceContext";
        public const string ENCRYPT_DECRYPT_SALT = "50b25ef599144db2953b656f11a84139";
        public const string ENCRYPT_DECRYPT_PWD = "drdw1w1rjang";
        public const string ENCRYPT_KEY = "drdtul@l1t3";
        public const string INIT_LOGIN = "_init_login_xbudi_";
        public const long MINIMUM_VALUE_ID = 1000000000;
        public const long MAXIMUM_VALUE_ID = 1000000000000000;
        public const int LOOP_TRY_SAVE = 10;
        public const int LOOP_TRY_SAVE_THROW = 6;
        public const int DATA_TEMP3 = 2020;
        public const int DATA_TEMP2 = 2;
        public const int DATA_TEMP1 = 10;
        public const int TEST_DUPLICATION_COUNT = 10;
        public const int ALLOW_EXCEED_LIMIT = -99;
        public const string CONSTRING_USER = "DrdUserContext";
        public const string API_KEY = "AIzaSyB5y7e2nuBQ4OFE257snefNT8XorZVIGYY";
        public static readonly IList<String> RESTRICTED_FOLDER_NAME = new ReadOnlyCollection<string>(new List<String> { ">", "<", ":", "\"", "/", "\\", "|", "?", "*" });

    }
}