using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DRD.Models
{
    public class ConstantModel
    {
        public const string CONSTRING = "ServiceContext";
        public const string ENCRYPT_DECRYPT_SALT = "50b25ef599144db2953b656f11a84139";
        public const string ENCRYPT_DECRYPT_PWD = "drdw1w1rjang";
        public const string ENCRYPT_KEY = "drdtul@l1t3";
        public const string INIT_LOGIN = "_init_login_xbudi_";
        public const long MINIMUM_VALUE_ID = 1000000000;
        public const long MAXIMUM_VALUE_ID = 1000000000000000;
        public const long ID_NOT_FOUND = 0;
        public const int LOOP_TRY_SAVE = 10;
        public const int LOOP_TRY_SAVE_THROW = 6;
        public const int DATA_TEMP3 = 2020;
        public const int DATA_TEMP2 = 2;
        public const int DATA_TEMP1 = 10;
        public const int ALLOW_EXCEED_LIMIT = -99;
        public const string CONSTRING_USER = "DrdUserContext";
        public const string API_KEY = "AIzaSyB5y7e2nuBQ4OFE257snefNT8XorZVIGYY";
        public static readonly IList<String> RESTRICTED_FOLDER_NAME = new ReadOnlyCollection<string>(new List<String> { ">", "<", ":", "\"", "/", "\\", "|", "?", "*" });

        /// Constructor
        public ConstantModel() { }

        public enum MemberRole
        {
            Not_Member = 0,
            Member = 1,
            Administrator = 2,
            Owner = 3
        }

        public enum AccessType
        {
            //restricted, cannot access at all
            noAccess = 0,
            //read only access
            readOnly = 1,
            //can access page
            responsible = 2,
            //fully access granted if there are multiple feature access in a pages
            fullAccess = 3
        }
        public enum DocumentActionPermissionType
        {
            //restricted, cannot access at all
            NoAccess = 0,
            //depends on rotation User to allow 
            DependsOnRotationUser = 1,
            //fully access granted if there are multiple feature access in a pages
            FullAccess = 2
        }
        public enum BusinessPackageItem
        {
            Administrator = 0,
            Member = 1,
            Rotation_Started = 4,
            Storage = 99
        }

        public enum SubscriptionType : byte
        {
            PERSONAL = 1,
            BUSINESS = 0
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
            REVISI = 2, // Tidak digunakan
            VIEW = 4,
            PRINT = 8,
            DOWNLOAD = 16,
            PRIVATESTAMP = 32,
            REMOVE = 64,
        }
        public enum EnumElementTypeId
        {
            PEN = 1,
            HIGHLIGHTER = 2,
            TEXT = 3,
            SIGNATURE = 4,
            INITIAL = 5,
            STAMP = 6,
            PRIVATESTAMP = 7
        }

    }
}