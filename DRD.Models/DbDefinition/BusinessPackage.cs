﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models
{
    [Table("BusinessPackages", Schema = "public")]
    public class BusinessPackage
    {
        [Key]
        public long Id { get; set; } // Id (Primary key)

        public bool IsActive { get; set; } // IsDefault
        public bool IsExceedLimitAllowed { get; set; } = false; // IsDefault
        public bool IsExpirationDateExtendedAutomatically { get; set; } = false;// IsDefault
        public bool IsPublic { get; set; } = true;// IsDefault
        public int Administrator { get; set; } = -99;
        public int Duration { get; set; } = -99;
        public int Rotation { get; set; } = -99;
        public int RotationStarted { get; set; }
        public int User { get; set; } = -99;
        public int Workflow { get; set; } = -99;
        public long Storage { get; set; } = -99;
        public string Name { get; set; } // MemberId
        public System.DateTime? CreatedAt { get; set; } // ValidPackage

        public BusinessPackage()
        {
            IsActive = true;
        }
    }
}