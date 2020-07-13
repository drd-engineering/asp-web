using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DRD.Models
{
    public class Constant
    {

        //environment setup
        public const string CONSTRING = "dev"; //dev, staging, prod

        //Encryption related
        public const string ENCRYPT_DECRYPT_SALT = "50b25ef599144db2953b656f11a84139";
        public const string ENCRYPT_DECRYPT_PWD = "0a4cee1548314672507d3d11ea59557d";
        public const string ENCRYPT_KEY = "d80ced56acac85dde3c9bd6b5b357caf";
        
        public const long MINIMUM_VALUE_ID = 1000000000;
        public const long MAXIMUM_VALUE_ID = 1000000000000000;
        public const long ID_NOT_FOUND = 0;
        public const int ALLOW_EXCEED_LIMIT = -99;
        public const string API_KEY = "AIzaSyB5y7e2nuBQ4OFE257snefNT8XorZVIGYY";
        public static readonly IList<String> RESTRICTED_FOLDER_NAME = new ReadOnlyCollection<string>(new List<String> { ">", "<", ":", "\"", "/", "\\", "|", "?", "*" });

        /// Constructor
        public Constant() { }

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
        public enum GENERAL
        {
            OK = 200,
            NOT_FOUND = 404,
            BAD_REQUEST = 500
        }

        public enum InivitationStatus
        {
            Connected = 0,
            Pending = 1,
            Inactive = 2,
            ERROR_NOT_FOUND = 404
        }

        public enum WorkflowStatus
        {
            OK = 200,
            NOT_FOUND = 404,
            USED_IN_ROTATION = 300
        }
        public enum RotationStatus
        {
            OK = 200,
            NOT_FOUND = 404,
            ERROR_WORKFLOW_START_NODE_NOT_FOUND = -5,
            ERROR_ROTATION_ALREADY_STARTED = -1,
            Open = 0,
            In_Progress = 1,
            Pending = 2,
            Signed = 3,
            Revision = 5,
            Altered = 6,
            Completed = 90,
            Declined = 98,
            Canceled = 99,
            Waiting_For_Response = 10,
            Accepted = 11,
            Expired = 97
        }

        public enum BusinessUsageStatus
        {
            OK = 200,
            ADMINISTRATOR_EXCEED_LIMIT = 2,
            MEMBER_EXCEED_LIMIT = 3,
            ROTATION_STARTED_EXCEED_LIMIT = 4,
            NO_ACTIVE_PLAN = 5,
            NOT_AUTHORIZED = 6,
            EXPIRED = 7,
            STORAGE_EXCEED_LIMIT = 99
        }

        public enum DocumentUploadStatus
        {
            OK = 1,
            NOT_FOUND = 2,
            SERVER_ERROR = 3
        }

        public enum DocumentPrintOrDownloadStatus
        {
            OK = 1,
            NOT_FOUND = 2,
            USER_HAS_NO_ACCESS = 3,
            EXCEED_LIMIT = 4
        }

        public enum AuditTrail
        {
            Setting = 1,
            User = 2,
            Rotation = 4,
            Workflow = 8,
            Company = 16,
            Document = 32
        }

    }
}