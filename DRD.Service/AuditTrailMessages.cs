
using DRD.Models;
using DRD.Models.API;
using DRD.Models.Custom;
using DRD.Models.View;
using DRD.Service.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DRD.Service
{
    public class AuditTrailMessages
    {
        private static string GetFormattedRotationIdentity(long rotationId)
        {
            using var db = new Connection();
            return db.Rotations.Find(rotationId).Name +$" ({rotationId})";
        }
        private static string GetFormattedCompanyIdentity(long companyId)
        {
            using var db = new Connection();
            return db.Companies.Find(companyId).Name + $" ({companyId})";
        }
        private static string GetFormattedSubscriptionIdentity(long subscriptionId)
        {
            using var db = new Connection();
            return db.BusinessPackages.Find(subscriptionId).Name + $" ({subscriptionId})";
        }
        private static string GetFormattedWorkflowIdentity(long workflowId)
        {
            using var db = new Connection();
            return db.Workflows.Find(workflowId).Name + $" ({workflowId})";
        }
        private static string GetFormattedDocumentIdentity(long documentId)
        {
            using var db = new Connection();
            return db.Documents.Find(documentId).FileName + $" ({documentId})";
        }
        private static string GetFormattedUserIdentity(long userId)
        {
            using var db = new Connection();
            User user = db.Users.Find(userId);
            return user.Name + $"({userId}-{user.Email})";
        }
        public static string AddRotation(long rotationId)
        {
            return $"add { GetFormattedRotationIdentity(rotationId)}";
        }

        public static string StartRotation(long rotationId, long companyId, long subscriptionId)
        {
            return $"start { GetFormattedRotationIdentity(rotationId) }  using { GetFormattedSubscriptionIdentity(subscriptionId) } subsciption - { GetFormattedCompanyIdentity(companyId)}";
        }

        public static string DeleteRotation(long rotationId)
        {
            return $"delete { GetFormattedRotationIdentity(rotationId)}";
        }

        public static string AddWorkflow(long workflowId)
        {
            return $"add { GetFormattedWorkflowIdentity(workflowId)}";
        }

        public static string DeleteWorkflow(long workflowId)
        {
            return $"delete { GetFormattedWorkflowIdentity(workflowId)}";
        }

        public static string ReadInbox(long rotationId)
        {
            return $"open inbox from rotation { GetFormattedRotationIdentity(rotationId)}";
        }
        public static string SubmitInbox(long rotationId)
        {
            return $"submit an inbox from rotation { GetFormattedRotationIdentity(rotationId)}";
        }
        
        public static string ReviseInbox(long rotationId)
        {
            return $"revise an activity in inbox from rotation{ GetFormattedRotationIdentity(rotationId)}";
        }
        
        public static string RejectInbox(long rotationId)
        {
            return $"reject the rotation { GetFormattedRotationIdentity(rotationId)}";
        }

        public static string UploadDocument(long documentId, long rotationId)
        {
            return $"upload a pdf document { GetFormattedDocumentIdentity(documentId) } in { GetFormattedRotationIdentity(rotationId)}";
        }

        public static string UploadProfilePicture()
        {
            return "upload new profile picture";
        }

        public static string ChangeName(string oldName, string newName)
        {
            return $"change the profile name from {oldName} to { newName}";
        }

        public static string UploadKTP()
        {
            return "upload KTP";
        }
        public static string UploadSignature()
        {
            return "upload Signature";
        }
        public static string UploadInitial()
        {
            return "upload Initial";
        }
        public static string UploadStamp()
        {
            return "upload Stamp";
        }
        public static string AcceptMember(long userId, long companyId)
        {
            return $"accept { GetFormattedUserIdentity(userId) } as { GetFormattedCompanyIdentity(companyId) }'s member";
        }
        public static string RejectMember(long userId, long companyId)
        {
            return $"reject { GetFormattedUserIdentity(userId) } from { GetFormattedCompanyIdentity(companyId) }s pending member request";
        }
        public static string InviteMember(long userId, long companyId)
        {
            return $"invite { GetFormattedUserIdentity(userId) } to be { GetFormattedCompanyIdentity(companyId) }'s member";
        }

    }
}
