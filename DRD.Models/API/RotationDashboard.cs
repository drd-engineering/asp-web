namespace DRD.Models.API
{
    public class RotationDashboard
    {
        public long Id { get; set; } // Id (Primary key)
        public long InboxId { get; set; } // Id (Primary key)
        public string Name { get; set; } // Subject (length: 100)
        public int Status { get; set; } // Status (length: 2)
        public System.DateTime? CreatedAt { get; set; } // DateCreated
        public System.DateTime? UpdatedAt { get; set; } // DateUpdated
        public System.DateTime? StartedAt { get; set; } // DateStarted
        // Reverse navigation
        public virtual System.Collections.Generic.ICollection<string> Tags { get; set; }
        public virtual System.Collections.Generic.ICollection<UserDashboard> RotationUsers { get; set; } // RotationMember.FK_RotationMember_Rotation
        public virtual UserDashboard Creator { get; set; }
        public virtual WorkflowDashboard Workflow { get; set; }

        public class WorkflowDashboard
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
        public class UserDashboard
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public string EncryptedId { get; set; }
            public string ProfileImageFileName { get; set; }

            public System.DateTime? InboxTimeStamp { get; set; } // DateCreated
            public int InboxStatus { get; set; } // string
            public bool IsHere { get; set; }
        }
    }
}
