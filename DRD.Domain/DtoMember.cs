using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class DtoMember
    {
        public long Id { get; set; } // Id (Primary key)
        public string Number { get; set; } // Number (length: 20)
        public int MemberTitleId { get; set; } // MemberTitleId
        public string Name { get; set; } // Name (length: 50)
        public string Phone { get; set; } // Phone (length: 20)
        public string Email { get; set; } // Email (length: 50)
        public int MemberType { get; set; } // MemberType
        public string KtpNo { get; set; } // KtpNo (length: 50)
        public string ImageProfile { get; set; } // ImageProfile (length: 50)
        public string ImageQrCode { get; set; } // ImageQrCode (length: 50)
        public System.DateTime? LastLogin { get; set; } // LastLogin
        public System.DateTime? LastLogout { get; set; } // LastLogout
        public long? ActivationKeyId { get; set; } // ActivationKeyId
        public string Password { get; set; } // Password (length: 50)
        public long? CompanyId { get; set; } // CompanyId
        public string UserGroup { get; set; } // UserGroup (length: 20)
        public string ImageSignature { get; set; } // ImageSignature (length: 100)
        public string ImageInitials { get; set; } // ImageInitials (length: 100)
        public string ImageStamp { get; set; } // ImageStamp (length: 100)
        public string ImageKtp1 { get; set; } // ImageKtp1 (length: 100)
        public string ImageKtp2 { get; set; } // ImageKtp2 (length: 100)
        public bool IsActive { get; set; } // IsActive
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<DtoDocumentMember> DocumentMembers { get; set; } // DocumentMember.FK_DocumentMember_Member
        public virtual System.Collections.Generic.ICollection<DtoRotation> Rotations { get; set; } // Rotation.FK_Rotation_Member
        public virtual System.Collections.Generic.ICollection<DtoRotationNode> RotationNodes { get; set; } // RotationNode.FK_RotationNode_Member
        public virtual System.Collections.Generic.ICollection<DtoWorkflowNode> WorkflowNodes { get; set; } // WorkflowNode.FK_WorkflowNode_Member

        // Foreign keys
        public virtual DtoCompany Company { get; set; } // FK_Member_Company
        public virtual DtoMemberTitle MemberTitle { get; set; } // FK_Member_MemberTitle

        public DtoMemberMaster Master { get; set; }
        public long LoginId { get; set; }

        public DtoMember()
        {
            LoginId = 0;
            MemberType = 1;
            IsActive = true;
            DocumentMembers = new System.Collections.Generic.List<DtoDocumentMember>();
            Rotations = new System.Collections.Generic.List<DtoRotation>();
            RotationNodes = new System.Collections.Generic.List<DtoRotationNode>();
            WorkflowNodes = new System.Collections.Generic.List<DtoWorkflowNode>();

            Master = new DtoMemberMaster();
        }
    }
}
